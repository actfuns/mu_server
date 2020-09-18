using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	
	public class JieRiGiveKingActivity : Activity
	{
		
		
		private int RANK_LVL_CNT
		{
			get
			{
				return this.allAwardDict.Count;
			}
		}

		
		public void OnGive(GameClient client, int goods, int cnt)
		{
			if (this.InActivityTime())
			{
				if (client != null)
				{
					lock (this._allMemberMutex)
					{
						bool bLoadFromDb;
						JieriGiveKingItemData detail = this.GetClientGiveKingInfo(client, out bLoadFromDb);
						if (detail == null)
						{
							return;
						}
						if (!bLoadFromDb)
						{
							detail.TotalGive += cnt;
						}
						bool bExist = this.orderedGiveList.Any((JieriGiveKingItemData detail1) => detail1.RoleID == client.ClientData.RoleID);
						bool bAdd = false;
						if (!bExist && (this.orderedGiveList.Count < this.RANK_LVL_CNT || this.orderedGiveList[this.RANK_LVL_CNT - 1].TotalGive < detail.TotalGive))
						{
							this.orderedGiveList.Add(detail);
							bAdd = true;
						}
						if (bExist || bAdd)
						{
							this.buildRankingList(this.orderedGiveList);
						}
					}
					if (client._IconStateMgr.CheckJieriGiveKing(client))
					{
						client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		
		private void buildRankingList(List<JieriGiveKingItemData> rankingList)
		{
			rankingList.Sort(delegate(JieriGiveKingItemData left, JieriGiveKingItemData right)
			{
				int result;
				if (left.TotalGive > right.TotalGive)
				{
					result = -1;
				}
				else if (left.TotalGive == right.TotalGive)
				{
					result = left.RoleID - right.RoleID;
				}
				else
				{
					result = 1;
				}
				return result;
			});
			int procListIdx = 0;
			int i = 1;
			while (i <= this.RANK_LVL_CNT && procListIdx < rankingList.Count)
			{
				AwardItem award = null;
				if (this.allAwardDict.TryGetValue(i, out award))
				{
					JieriGiveKingItemData kingItem = rankingList[procListIdx];
					if (kingItem.TotalGive >= award.MinAwardCondionValue)
					{
						kingItem.Rank = i;
						procListIdx++;
					}
				}
				i++;
			}
			this.RoleCountInList = procListIdx;
			for (i = rankingList.Count - 1; i >= procListIdx; i--)
			{
				rankingList[i].Rank = -1;
				if (i >= this.RANK_LVL_CNT)
				{
					rankingList.RemoveAt(i);
				}
			}
		}

		
		public void LoadRankFromDB()
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				string req = string.Format("{0}:{1}:{2}", this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'), this.RANK_LVL_CNT);
				List<JieriGiveKingItemData> items = Global.sendToDB<List<JieriGiveKingItemData>, string>(13203, req, 0);
				lock (this._allMemberMutex)
				{
					this.giveDict.Clear();
					this.orderedGiveList.Clear();
					if (items != null && items.Count != 0)
					{
						foreach (JieriGiveKingItemData item in items)
						{
							this.giveDict[item.RoleID] = item;
							this.orderedGiveList.Add(item);
						}
						this.buildRankingList(this.orderedGiveList);
					}
				}
			}
		}

		
		public byte[] QueryActivityInfo(GameClient client)
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				lock (this._allMemberMutex)
				{
					return DataHelper.ObjectToBytes<JieriGiveKingData>(new JieriGiveKingData
					{
						MyData = this.GetClientGiveKingInfo(client),
						RankingList = this.orderedGiveList.GetRange(0, this.RoleCountInList)
					});
				}
			}
			return null;
		}

		
		public string ProcRoleGetAward(GameClient client, int awardid)
		{
			JieriGiveErrorCode ec = JieriGiveErrorCode.Success;
			if (!this.InAwardTime())
			{
				ec = JieriGiveErrorCode.NotAwardTime;
			}
			else if (!this.HasEnoughBagSpaceForAwardGoods(client, awardid))
			{
				ec = JieriGiveErrorCode.NoBagSpace;
			}
			else
			{
				AwardItem allItem = null;
				AwardItem occItem = null;
				AwardEffectTimeItem timeItem = null;
				if (!this.allAwardDict.TryGetValue(awardid, out allItem) || !this.occAwardDict.TryGetValue(awardid, out occItem) || !this.timeAwardDict.TryGetValue(awardid, out timeItem))
				{
					ec = JieriGiveErrorCode.ConfigError;
				}
				else
				{
					lock (this._allMemberMutex)
					{
						JieriGiveKingItemData myData = this.GetClientGiveKingInfo(client);
						if (myData == null || myData.TotalGive < allItem.MinAwardCondionValue || myData.GetAwardTimes > 0 || myData.Rank != awardid)
						{
							ec = JieriGiveErrorCode.NotMeetAwardCond;
							goto IL_1CB;
						}
						string dbReq = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
						string[] dbRsp = Global.ExecuteDBCmd(13205, dbReq, client.ServerId);
						if (dbRsp == null || dbRsp.Length != 1 || Convert.ToInt32(dbRsp[0]) <= 0)
						{
							ec = JieriGiveErrorCode.DBFailed;
							goto IL_1CB;
						}
						myData.GetAwardTimes = 1;
					}
					if (!base.GiveAward(client, allItem) || !base.GiveAward(client, occItem) || !base.GiveEffectiveTimeAward(client, timeItem.ToAwardItem()))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("发送节日赠送王奖励的时候，发送失败,但是设置领奖成功，roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, awardid), null, true);
					}
					ec = JieriGiveErrorCode.Success;
				}
			}
			IL_1CB:
			if (ec == JieriGiveErrorCode.Success)
			{
				if (client._IconStateMgr.CheckJieriGiveKing(client))
				{
					client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
			return string.Format("{0}:{1}", (int)ec, awardid);
		}

		
		private JieriGiveKingItemData GetClientGiveKingInfo(GameClient client)
		{
			bool _bLoadFromDb;
			return this.GetClientGiveKingInfo(client, out _bLoadFromDb);
		}

		
		private JieriGiveKingItemData GetClientGiveKingInfo(GameClient client, out bool bLoadFromDb)
		{
			bLoadFromDb = false;
			JieriGiveKingItemData item = null;
			if (!this.giveDict.TryGetValue(client.ClientData.RoleID, out item))
			{
				string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				item = Global.sendToDB<JieriGiveKingItemData, string>(13204, cmd, client.ServerId);
				if (item == null)
				{
					item = new JieriGiveKingItemData();
					item.RoleID = client.ClientData.RoleID;
					item.Rolename = client.ClientData.RoleName;
					item.TotalGive = 0;
					item.Rank = -1;
					item.GetAwardTimes = 0;
				}
				else
				{
					bLoadFromDb = true;
				}
				this.giveDict[client.ClientData.RoleID] = item;
			}
			return item;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int id)
		{
			AwardItem allItem = null;
			AwardItem occItem = null;
			AwardEffectTimeItem timeItem = null;
			this.allAwardDict.TryGetValue(id, out allItem);
			this.occAwardDict.TryGetValue(id, out allItem);
			this.timeAwardDict.TryGetValue(id, out timeItem);
			int awardCnt = 0;
			if (allItem != null && allItem.GoodsDataList != null)
			{
				awardCnt += allItem.GoodsDataList.Count;
			}
			if (occItem != null && occItem.GoodsDataList != null)
			{
				awardCnt += occItem.GoodsDataList.Count((GoodsData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
			}
			if (timeItem != null)
			{
				awardCnt += timeItem.GoodsCnt();
			}
			return Global.CanAddGoodsNum(client, awardCnt);
		}

		
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(this.CfgFile));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(this.CfgFile));
				if (null == xml)
				{
					return false;
				}
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					this.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							AwardItem myAwardItem2 = new AwardItem();
							AwardEffectTimeItem timeAwardItem = new AwardEffectTimeItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日赠送王活动配置");
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (!string.IsNullOrEmpty(goodsIDs))
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
								}
								else
								{
									myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日赠送王活动配置");
								}
							}
							string timeGoods = Global.GetSafeAttributeStr(xmlItem, "GoodsThr");
							string timeList = Global.GetSafeAttributeStr(xmlItem, "EffectiveTime");
							timeAwardItem.Init(timeGoods, timeList, "大型节日赠送王时效性物品活动配置");
							string rankings = Global.GetSafeAttributeStr(xmlItem, "Ranking");
							string[] paiHangs = rankings.Split(new char[]
							{
								'-'
							});
							if (paiHangs.Length > 0)
							{
								int min = Global.SafeConvertToInt32(paiHangs[0]);
								int max = Global.SafeConvertToInt32(paiHangs[paiHangs.Length - 1]);
								for (int paiHang = min; paiHang <= max; paiHang++)
								{
									this.allAwardDict.Add(paiHang, myAwardItem);
									this.occAwardDict.Add(paiHang, myAwardItem2);
									this.timeAwardDict.Add(paiHang, timeAwardItem);
								}
							}
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", this.CfgFile, ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool CanGetAnyAward(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				lock (this._allMemberMutex)
				{
					foreach (JieriGiveKingItemData item in this.orderedGiveList)
					{
						if (item.RoleID == client.ClientData.RoleID && item.GetAwardTimes <= 0 && item.Rank > 0)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				lock (this._allMemberMutex)
				{
					JieriGiveKingItemData item = null;
					this.giveDict.TryGetValue(roleId, out item);
					if (item != null)
					{
						item.Rolename = newName;
					}
				}
			}
		}

		
		private readonly string CfgFile = "Config/JieRiGifts/JieRiZengSongKing.xml";

		
		private object _allMemberMutex = new object();

		
		private Dictionary<int, JieriGiveKingItemData> giveDict = new Dictionary<int, JieriGiveKingItemData>();

		
		private List<JieriGiveKingItemData> orderedGiveList = new List<JieriGiveKingItemData>();

		
		public Dictionary<int, AwardItem> allAwardDict = new Dictionary<int, AwardItem>();

		
		public Dictionary<int, AwardItem> occAwardDict = new Dictionary<int, AwardItem>();

		
		private Dictionary<int, AwardEffectTimeItem> timeAwardDict = new Dictionary<int, AwardEffectTimeItem>();

		
		private int RoleCountInList;
	}
}
