using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	
	public class JieriHongBaoKingActivity : JieRiActivity
	{
		
		
		private int RANK_LVL_CNT
		{
			get
			{
				return this.allAwardDict.Count;
			}
		}

		
		public static JieriHongBaoKingActivity getInstance()
		{
			return JieriHongBaoKingActivity.instance;
		}

		
		public bool OnRecv(GameClient client, int zuanshi, string strFrom)
		{
			bool result;
			if (!this.InActivityTime())
			{
				LogManager.WriteLog(LogTypes.System, string.Format("领取红包失败#已不在活动时间内#rid={0},zuanshi={1}", client.ClientData.RoleID, zuanshi), null, true);
				result = false;
			}
			else
			{
				int rid = client.ClientData.RoleID;
				string rname = client.ClientData.RoleName;
				List<string> args = new List<string>
				{
					this.ActivityKeyStr,
					rid.ToString(),
					"1",
					TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss"),
					rname,
					zuanshi.ToString()
				};
				JieriHongBaoKingItemData detail = Global.sendToDB<JieriHongBaoKingItemData, List<string>>(1436, args, client.ServerId);
				if (detail == null)
				{
					LogManager.WriteLog(LogTypes.System, string.Format("领取红包失败#红包钻石已扣减但无法记录领取者#rid={0},zuanshi={1}", client.ClientData.RoleID, zuanshi), null, true);
				}
				lock (this.Mutex)
				{
					if (detail == null)
					{
						return true;
					}
					JieriHongBaoKingItemData dictItem;
					if (!this.recvDict.TryGetValue(rid, out dictItem))
					{
						dictItem = detail;
						this.recvDict[rid] = dictItem;
					}
					else
					{
						dictItem.TotalRecv = detail.TotalRecv;
						dictItem.GetAwardTimes = detail.GetAwardTimes;
					}
					bool bExist = this.orderedRecvList.Any((JieriHongBaoKingItemData x) => x.RoleID == rid);
					bool bAdd = false;
					if (!bExist && (this.orderedRecvList.Count < this.RANK_LVL_CNT || this.orderedRecvList[this.RANK_LVL_CNT - 1].TotalRecv < detail.TotalRecv))
					{
						this.orderedRecvList.Add(dictItem);
						bAdd = true;
					}
					if (bExist || bAdd)
					{
						this.buildRankingList(this.orderedRecvList);
					}
				}
				result = true;
			}
			return result;
		}

		
		public bool Init()
		{
			try
			{
				this.allAwardDict.Clear();
				this.occAwardDict.Clear();
				this.timeAwardDict.Clear();
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/JieRiHongBaoBang.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/JieRiHongBaoBang.xml"));
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
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "Threshold"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiHongBaoBang.xml"), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiHongBaoBang.xml"), null, true);
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
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiHongBaoBang.xml"), null, true);
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
				this.ActivityKeyStr = string.Format("{0}_{1}", this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiHongBaoBang.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool CanGetAnyAward(GameClient client)
		{
			bool result;
			if (GameManager.IsKuaFuServer)
			{
				result = false;
			}
			else if (client == null)
			{
				result = false;
			}
			else if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				lock (this.Mutex)
				{
					foreach (JieriHongBaoKingItemData item in this.orderedRecvList)
					{
						if (item.RoleID == client.ClientData.RoleID && item.GetAwardTimes <= 0)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		
		public bool LoadRankFromDB()
		{
			bool result;
			if (GameManager.IsKuaFuServer)
			{
				result = false;
			}
			else
			{
				if (this.InActivityTime() || this.InAwardTime())
				{
					List<string> args = new List<string>
					{
						this.ActivityKeyStr,
						"20"
					};
					List<JieriHongBaoKingItemData> items = Global.sendToDB<List<JieriHongBaoKingItemData>, List<string>>(1431, args, 0);
					lock (this.Mutex)
					{
						this.recvDict.Clear();
						this.orderedRecvList.Clear();
						if (items == null || items.Count == 0)
						{
							return true;
						}
						foreach (JieriHongBaoKingItemData item in items)
						{
							this.recvDict[item.RoleID] = item;
							this.orderedRecvList.Add(item);
						}
						this.buildRankingList(this.orderedRecvList);
					}
				}
				result = true;
			}
			return result;
		}

		
		private void buildRankingList(List<JieriHongBaoKingItemData> rankingList)
		{
			rankingList.Sort(delegate(JieriHongBaoKingItemData left, JieriHongBaoKingItemData right)
			{
				int result;
				if (left.TotalRecv > right.TotalRecv)
				{
					result = -1;
				}
				else if (left.TotalRecv == right.TotalRecv)
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
					JieriHongBaoKingItemData kingItem = rankingList[procListIdx];
					if (kingItem.TotalRecv >= award.MinAwardCondionValue)
					{
						kingItem.Rank = i;
						procListIdx++;
					}
				}
				i++;
			}
			for (i = rankingList.Count - 1; i >= procListIdx; i--)
			{
				rankingList[i].Rank = -1;
				rankingList.RemoveAt(i);
			}
		}

		
		public void QueryActivityInfo(GameClient client)
		{
			JieriHongBaoKingData result = new JieriHongBaoKingData();
			result.DataAge = TimeUtil.NOW();
			if (this.InActivityTime() || this.InAwardTime())
			{
				lock (this.Mutex)
				{
					result.RankList = this.orderedRecvList;
				}
				int rid = client.ClientData.RoleID;
				string rname = client.ClientData.RoleName;
				JieriHongBaoKingItemData detail = result.RankList.Find((JieriHongBaoKingItemData x) => x.RoleID == rid);
				if (null != detail)
				{
					result.SelfCount = detail.TotalRecv;
				}
				else
				{
					List<string> args = new List<string>
					{
						this.ActivityKeyStr,
						rid.ToString()
					};
					result.SelfCount = Global.sendToDB<int, List<string>>(1440, args, client.ServerId);
				}
			}
			client.sendCmd<JieriHongBaoKingData>(1429, result, false);
		}

		
		public new void GetAward(GameClient client, int awardid)
		{
			int result = 0;
			if (!this.InAwardTime())
			{
				result = -2001;
			}
			else if (!this.HasEnoughBagSpaceForAwardGoods(client, awardid))
			{
				result = -100;
			}
			else
			{
				AwardItem allItem = null;
				AwardItem occItem = null;
				AwardEffectTimeItem timeItem = null;
				if (!this.allAwardDict.TryGetValue(awardid, out allItem) || !this.occAwardDict.TryGetValue(awardid, out occItem) || !this.timeAwardDict.TryGetValue(awardid, out timeItem))
				{
					result = -3;
				}
				else
				{
					lock (this.Mutex)
					{
						JieriHongBaoKingItemData item;
						if (!this.recvDict.TryGetValue(client.ClientData.RoleID, out item))
						{
							result = -20;
							goto IL_21F;
						}
						if (item.GetAwardTimes > 0)
						{
							result = -200;
							goto IL_21F;
						}
						JieriHongBaoKingItemData myData = this.GetRoleRecvKingInfo(client, 0, 0, client.ServerId);
						if (myData == null || myData.TotalRecv < allItem.MinAwardCondionValue || myData.GetAwardTimes > 0 || myData.Rank != awardid)
						{
							result = -20;
							goto IL_21F;
						}
						List<string> dbReq = new List<string>
						{
							this.ActivityKeyStr,
							client.ClientData.RoleID.ToString(),
							"1"
						};
						int ret = Global.sendToDB<int, List<string>>(1428, dbReq, client.ServerId);
						if (ret < 0)
						{
							result = -15;
							goto IL_21F;
						}
						myData.GetAwardTimes = 1;
					}
					if (!base.GiveAward(client, allItem) || !base.GiveAward(client, occItem) || !base.GiveEffectiveTimeAward(client, timeItem.ToAwardItem()))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("发送节日红包榜奖励的时候，发送失败，但是已经设置为领取成功, roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, awardid), null, true);
					}
					client._IconStateMgr.CheckJieRiHongBaoBang(client);
				}
			}
			IL_21F:
			client.sendCmd<int>(1428, result, false);
		}

		
		private JieriHongBaoKingItemData GetRoleRecvKingInfo(GameClient client, int count, int flags, int serverId)
		{
			int rid = client.ClientData.RoleID;
			JieriHongBaoKingItemData item = null;
			lock (this.Mutex)
			{
				if (this.recvDict.TryGetValue(rid, out item))
				{
					return item;
				}
			}
			return null;
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

		
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				lock (this.Mutex)
				{
					JieriHongBaoKingItemData item = null;
					this.recvDict.TryGetValue(roleId, out item);
					if (item != null)
					{
						item.Rolename = newName;
					}
				}
			}
		}

		
		private const string CfgFile = "Config/JieRiGifts/JieRiHongBaoBang.xml";

		
		private object Mutex = new object();

		
		private Dictionary<int, JieriHongBaoKingItemData> recvDict = new Dictionary<int, JieriHongBaoKingItemData>();

		
		private List<JieriHongBaoKingItemData> orderedRecvList = new List<JieriHongBaoKingItemData>();

		
		public Dictionary<int, AwardItem> allAwardDict = new Dictionary<int, AwardItem>();

		
		public Dictionary<int, AwardItem> occAwardDict = new Dictionary<int, AwardItem>();

		
		private Dictionary<int, AwardEffectTimeItem> timeAwardDict = new Dictionary<int, AwardEffectTimeItem>();

		
		private static JieriHongBaoKingActivity instance = new JieriHongBaoKingActivity();
	}
}
