﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001C4 RID: 452
	public class JieRiRecvKingActivity : Activity
	{
		// Token: 0x1700001B RID: 27
		
		private int RANK_LVL_CNT
		{
			get
			{
				return this.allAwardDict.Count;
			}
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0004F250 File Offset: 0x0004D450
		public void OnRecv(int receiver, int goods, int cnt, int serverId)
		{
			if (this.InActivityTime())
			{
				lock (this._allMemberMutex)
				{
					bool bLoadFromDb;
					JieriRecvKingItemData detail = this.GetRoleRecvKingInfo(receiver, out bLoadFromDb, serverId);
					if (detail == null)
					{
						return;
					}
					if (!bLoadFromDb)
					{
						detail.TotalRecv += cnt;
					}
					bool bExist = this.orderedRecvList.Any((JieriRecvKingItemData detail1) => detail1.RoleID == receiver);
					bool bAdd = false;
					if (!bExist && (this.orderedRecvList.Count < this.RANK_LVL_CNT || this.orderedRecvList[this.RANK_LVL_CNT - 1].TotalRecv < detail.TotalRecv))
					{
						this.orderedRecvList.Add(detail);
						bAdd = true;
					}
					if (bExist || bAdd)
					{
						this.buildRankingList(this.orderedRecvList);
					}
				}
				GameClient client = GameManager.ClientMgr.FindClient(receiver);
				if (client != null && client._IconStateMgr.CheckJieriRecvKing(client))
				{
					client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x0004F3F8 File Offset: 0x0004D5F8
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/JieRiShouQuKing.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/JieRiShouQuKing.xml"));
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
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiShouQuKing.xml"), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiShouQuKing.xml"), null, true);
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
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiShouQuKing.xml"), null, true);
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
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiShouQuKing.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0004F7A4 File Offset: 0x0004D9A4
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
					foreach (JieriRecvKingItemData item in this.orderedRecvList)
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

		// Token: 0x060005A4 RID: 1444 RVA: 0x0004F88C File Offset: 0x0004DA8C
		public void LoadRankFromDB()
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				string req = string.Format("{0}:{1}:{2}", this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'), this.RANK_LVL_CNT);
				List<JieriRecvKingItemData> items = Global.sendToDB<List<JieriRecvKingItemData>, string>(13206, req, 0);
				lock (this._allMemberMutex)
				{
					this.recvDict.Clear();
					this.orderedRecvList.Clear();
					if (items != null && items.Count != 0)
					{
						foreach (JieriRecvKingItemData item in items)
						{
							this.recvDict[item.RoleID] = item;
							this.orderedRecvList.Add(item);
						}
						this.buildRankingList(this.orderedRecvList);
					}
				}
			}
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0004FA2C File Offset: 0x0004DC2C
		private void buildRankingList(List<JieriRecvKingItemData> rankingList)
		{
			rankingList.Sort(delegate(JieriRecvKingItemData left, JieriRecvKingItemData right)
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
					JieriRecvKingItemData kingItem = rankingList[procListIdx];
					if (kingItem.TotalRecv >= award.MinAwardCondionValue)
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

		// Token: 0x060005A6 RID: 1446 RVA: 0x0004FB18 File Offset: 0x0004DD18
		public byte[] QueryActivityInfo(GameClient client)
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				lock (this._allMemberMutex)
				{
					return DataHelper.ObjectToBytes<JieriRecvKingData>(new JieriRecvKingData
					{
						MyData = this.GetRoleRecvKingInfo(client.ClientData.RoleID, client.ServerId),
						RankingList = this.orderedRecvList.GetRange(0, this.RoleCountInList)
					});
				}
			}
			return null;
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x0004FBC0 File Offset: 0x0004DDC0
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
						JieriRecvKingItemData myData = this.GetRoleRecvKingInfo(client.ClientData.RoleID, client.ServerId);
						if (myData == null || myData.TotalRecv < allItem.MinAwardCondionValue || myData.GetAwardTimes > 0 || myData.Rank != awardid)
						{
							ec = JieriGiveErrorCode.NotMeetAwardCond;
							goto IL_1DB;
						}
						string dbReq = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
						string[] dbRsp = Global.ExecuteDBCmd(13208, dbReq, client.ServerId);
						if (dbRsp == null || dbRsp.Length != 1 || Convert.ToInt32(dbRsp[0]) <= 0)
						{
							ec = JieriGiveErrorCode.DBFailed;
							goto IL_1DB;
						}
						myData.GetAwardTimes = 1;
					}
					if (!base.GiveAward(client, allItem) || !base.GiveAward(client, occItem) || !base.GiveEffectiveTimeAward(client, timeItem.ToAwardItem()))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("发送节日收取王奖励的时候，发送失败，但是已经设置为领取成功, roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, awardid), null, true);
					}
					ec = JieriGiveErrorCode.Success;
				}
			}
			IL_1DB:
			if (ec == JieriGiveErrorCode.Success)
			{
				if (client._IconStateMgr.CheckJieriRecvKing(client))
				{
					client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
			return string.Format("{0}:{1}", (int)ec, awardid);
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x0004FE24 File Offset: 0x0004E024
		private JieriRecvKingItemData GetRoleRecvKingInfo(int roleid, int serverId)
		{
			bool _bLoadFromDb;
			return this.GetRoleRecvKingInfo(roleid, out _bLoadFromDb, serverId);
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x0004FE40 File Offset: 0x0004E040
		private JieriRecvKingItemData GetRoleRecvKingInfo(int roleid, out bool bLoadFromDb, int serverId)
		{
			bLoadFromDb = false;
			JieriRecvKingItemData item = null;
			if (!this.recvDict.TryGetValue(roleid, out item))
			{
				string cmd = string.Format("{0}:{1}:{2}", roleid, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				item = Global.sendToDB<JieriRecvKingItemData, string>(13207, cmd, serverId);
				if (item != null)
				{
					bLoadFromDb = true;
					this.recvDict[roleid] = item;
				}
			}
			return item;
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x0004FEF0 File Offset: 0x0004E0F0
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

		// Token: 0x060005AB RID: 1451 RVA: 0x0004FFD0 File Offset: 0x0004E1D0
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				lock (this._allMemberMutex)
				{
					JieriRecvKingItemData item = null;
					this.recvDict.TryGetValue(roleId, out item);
					if (item != null)
					{
						item.Rolename = newName;
					}
				}
			}
		}

		// Token: 0x04000A00 RID: 2560
		private const string CfgFile = "Config/JieRiGifts/JieRiShouQuKing.xml";

		// Token: 0x04000A01 RID: 2561
		private object _allMemberMutex = new object();

		// Token: 0x04000A02 RID: 2562
		private Dictionary<int, JieriRecvKingItemData> recvDict = new Dictionary<int, JieriRecvKingItemData>();

		// Token: 0x04000A03 RID: 2563
		private List<JieriRecvKingItemData> orderedRecvList = new List<JieriRecvKingItemData>();

		// Token: 0x04000A04 RID: 2564
		public Dictionary<int, AwardItem> allAwardDict = new Dictionary<int, AwardItem>();

		// Token: 0x04000A05 RID: 2565
		public Dictionary<int, AwardItem> occAwardDict = new Dictionary<int, AwardItem>();

		// Token: 0x04000A06 RID: 2566
		private Dictionary<int, AwardEffectTimeItem> timeAwardDict = new Dictionary<int, AwardEffectTimeItem>();

		// Token: 0x04000A07 RID: 2567
		private int RoleCountInList;
	}
}
