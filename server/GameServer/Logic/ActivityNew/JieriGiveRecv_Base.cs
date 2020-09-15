using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001B5 RID: 437
	public class JieriGiveRecv_Base : Activity
	{
		// Token: 0x06000555 RID: 1365 RVA: 0x0004AF9C File Offset: 0x0004919C
		public virtual string GetConfigFile()
		{
			throw new Exception("GetConfigFile未实现");
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x0004AFA9 File Offset: 0x000491A9
		public virtual string QueryActInfo(GameClient client)
		{
			throw new Exception("QueryActInfo未实现");
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x0004AFB6 File Offset: 0x000491B6
		public virtual void FlushIcon(GameClient client)
		{
			throw new Exception("OnGetAwardSuccess未实现");
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x0004AFC3 File Offset: 0x000491C3
		public virtual bool IsReachConition(RoleGiveRecvInfo info, int condValue)
		{
			throw new Exception("IsReachConition未实现");
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x0004AFD0 File Offset: 0x000491D0
		protected RoleGiveRecvInfo GetRoleGiveRecvInfo(int roleid)
		{
			bool _bLoadFromDb;
			return this.GetRoleGiveRecvInfo(roleid, out _bLoadFromDb);
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x0004AFEC File Offset: 0x000491EC
		protected RoleGiveRecvInfo GetRoleGiveRecvInfo(int roleid, out bool bLoadFromDb)
		{
			bLoadFromDb = false;
			RoleGiveRecvInfo result;
			lock (this.roleGiveRecvDict_dont_use_directly)
			{
				if (this.roleGiveRecvDict_dont_use_directly.ContainsKey(roleid))
				{
					RoleGiveRecvInfo oldInfo = this.roleGiveRecvDict_dont_use_directly[roleid];
					if (oldInfo.TodayIdxInActPeriod == Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(DateTime.Parse(this.FromDate)) + 1)
					{
						return oldInfo;
					}
				}
				RoleGiveRecvInfo info = new RoleGiveRecvInfo();
				this.roleGiveRecvDict_dont_use_directly[roleid] = info;
				DateTime dtNow = TimeUtil.NowDateTime();
				bool bTodayIsStartDay = Global.GetOffsetDay(dtNow) == Global.GetOffsetDay(DateTime.Parse(this.FromDate));
				bool bTodayIsEndDay = Global.GetOffsetDay(dtNow) == Global.GetOffsetDay(DateTime.Parse(this.ToDate));
				string todayActStart = bTodayIsStartDay ? this.FromDate : (dtNow.ToString("yyyy-MM-dd") + " 00:00:00");
				string todayActEnd = bTodayIsEndDay ? this.ToDate : (dtNow.ToString("yyyy-MM-dd") + " 23:59:59");
				int todayIdxInActPeriod = Global.GetOffsetDay(dtNow) - Global.GetOffsetDay(DateTime.Parse(this.FromDate)) + 1;
				string dbReq = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					roleid,
					this.ActivityType,
					todayActStart.Replace(':', '$'),
					todayActEnd.Replace(':', '$'),
					todayIdxInActPeriod
				});
				string[] dbRsp = Global.ExecuteDBCmd(13202, dbReq, 0);
				if (dbRsp == null || dbRsp.Length != 3)
				{
					info.TotalGive = 0;
					info.TotalRecv = 0;
					info.AwardFlag = 0;
				}
				else
				{
					bLoadFromDb = true;
					info.TotalGive = Convert.ToInt32(dbRsp[0]);
					info.TotalRecv = Convert.ToInt32(dbRsp[1]);
					info.AwardFlag = Convert.ToInt32(dbRsp[2]);
				}
				info.TodayStart = todayActStart;
				info.TodayEnd = todayActEnd;
				info.TodayIdxInActPeriod = todayIdxInActPeriod;
				result = info;
			}
			return result;
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0004B238 File Offset: 0x00049438
		public bool Init()
		{
			string CfgFile = this.GetConfigFile();
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(CfgFile));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(CfgFile));
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
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "Num"));
							myAwardItem.MinAwardCondionValue2 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "Goods"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项1失败", CfgFile), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}活动配置文件中的物品配置项失败", CfgFile), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, CfgFile);
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
									LogManager.WriteLog(LogTypes.Warning, CfgFile, null, true);
								}
								else
								{
									myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, CfgFile);
								}
							}
							string timeGoods = Global.GetSafeAttributeStr(xmlItem, "GoodsThr");
							string timeList = Global.GetSafeAttributeStr(xmlItem, "EffectiveTime");
							timeAwardItem.Init(timeGoods, timeList, CfgFile + " 时效性物品");
							string strID = Global.GetSafeAttributeStr(xmlItem, "ID");
							int id = Convert.ToInt32(strID);
							this.allAwardDict.Add(id, myAwardItem);
							this.occAwardDict.Add(id, myAwardItem2);
							this.timeAwardDict.Add(id, timeAwardItem);
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", CfgFile, ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x0004B590 File Offset: 0x00049790
		public string ProcRoleGetAward(GameClient client, int awardid)
		{
			JieriGiveErrorCode ec;
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
					RoleGiveRecvInfo info = this.GetRoleGiveRecvInfo(client.ClientData.RoleID);
					if (!this.IsReachConition(info, allItem.MinAwardCondionValue) || (info.AwardFlag & 1 << awardid) != 0)
					{
						ec = JieriGiveErrorCode.NotMeetAwardCond;
					}
					else
					{
						int newAwardFlag = info.AwardFlag | 1 << awardid;
						string dbReq = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							client.ClientData.RoleID,
							info.TodayStart.Replace(':', '$'),
							info.TodayEnd.Replace(':', '$'),
							this.ActivityType,
							info.TodayIdxInActPeriod,
							newAwardFlag
						});
						string[] dbRsp = Global.ExecuteDBCmd(13201, dbReq, client.ServerId);
						if (dbRsp == null || dbRsp.Length < 1 || Convert.ToInt32(dbRsp[0]) <= 0)
						{
							ec = JieriGiveErrorCode.DBFailed;
						}
						else
						{
							info.AwardFlag = newAwardFlag;
							if (!base.GiveAward(client, allItem) || !base.GiveAward(client, occItem) || !base.GiveEffectiveTimeAward(client, timeItem.ToAwardItem()))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("节日赠送活动奖品发送失败，但是已经设置为已发放，roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, awardid), null, true);
							}
							ec = JieriGiveErrorCode.Success;
						}
					}
				}
			}
			if (ec == JieriGiveErrorCode.Success)
			{
				this.FlushIcon(client);
			}
			return string.Format("{0}:{1}", (int)ec, awardid);
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x0004B7F0 File Offset: 0x000499F0
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int id)
		{
			AwardItem allItem = null;
			AwardItem occItem = null;
			AwardEffectTimeItem timeItem = null;
			this.allAwardDict.TryGetValue(id, out allItem);
			this.occAwardDict.TryGetValue(id, out occItem);
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

		// Token: 0x0600055E RID: 1374 RVA: 0x0004B8D0 File Offset: 0x00049AD0
		protected bool IsGiveGoodsID(int goodsID)
		{
			foreach (KeyValuePair<int, AwardItem> kvp in this.allAwardDict)
			{
				if (kvp.Value.MinAwardCondionValue2 == goodsID)
				{
					return true;
				}
			}
			foreach (KeyValuePair<int, AwardItem> kvp in this.occAwardDict)
			{
				if (kvp.Value.MinAwardCondionValue2 == goodsID)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x0004B9A0 File Offset: 0x00049BA0
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
				RoleGiveRecvInfo info = this.GetRoleGiveRecvInfo(client.ClientData.RoleID);
				foreach (KeyValuePair<int, AwardItem> kvp in this.allAwardDict)
				{
					int awardid = kvp.Key;
					AwardItem item = kvp.Value;
					if (this.IsReachConition(info, item.MinAwardCondionValue) && (info.AwardFlag & 1 << awardid) == 0)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0004BA7C File Offset: 0x00049C7C
		public void UpdateNewDay(GameClient client)
		{
			if (client != null)
			{
				bool IsYesterdayMayBeActive = false;
				lock (this.roleGiveRecvDict_dont_use_directly)
				{
					if (this.roleGiveRecvDict_dont_use_directly.ContainsKey(client.ClientData.RoleID))
					{
						this.roleGiveRecvDict_dont_use_directly.Remove(client.ClientData.RoleID);
						IsYesterdayMayBeActive = true;
					}
				}
				if (IsYesterdayMayBeActive)
				{
					this.FlushIcon(client);
				}
			}
		}

		// Token: 0x040009C8 RID: 2504
		private Dictionary<int, AwardItem> allAwardDict = new Dictionary<int, AwardItem>();

		// Token: 0x040009C9 RID: 2505
		private Dictionary<int, AwardItem> occAwardDict = new Dictionary<int, AwardItem>();

		// Token: 0x040009CA RID: 2506
		private Dictionary<int, AwardEffectTimeItem> timeAwardDict = new Dictionary<int, AwardEffectTimeItem>();

		// Token: 0x040009CB RID: 2507
		private Dictionary<int, RoleGiveRecvInfo> roleGiveRecvDict_dont_use_directly = new Dictionary<int, RoleGiveRecvInfo>();
	}
}
