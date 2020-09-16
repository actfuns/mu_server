using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.Building;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Marriage.CoupleWish;
using GameServer.Logic.ZhuanPan;
using GameServer.Server;
using Server.Data;
using Server.Tools.Pattern;

namespace GameServer.Logic.RefreshIconState
{
	
	public class IconStateManager
	{
		
		public bool AddFlushIconState(ushort nIconOrder, bool bIconState)
		{
			ushort iState = (ushort)(bIconState ? 1 : 0);
			return this.AddFlushIconState(nIconOrder, iState);
		}

		
		public bool AddFlushIconState(ushort nIconOrder, ushort iState)
		{
			ushort nIconInfo = (ushort)(((int)nIconOrder << 1) + (int)iState);
			ushort nOldState = 0;
			bool result;
			lock (this.m_StateIconsDict)
			{
				if (!this.m_StateCacheIconsDict.TryGetValue(nIconOrder, out nOldState))
				{
					this.m_StateCacheIconsDict[nIconOrder] = nIconInfo;
					this.m_StateIconsDict[nIconOrder] = nIconInfo;
					result = true;
				}
				else if ((nOldState & 1) == iState)
				{
					result = false;
				}
				else
				{
					this.m_StateCacheIconsDict[nIconOrder] = nIconInfo;
					this.m_StateIconsDict[nIconOrder] = nIconInfo;
					result = true;
				}
			}
			return result;
		}

		
		public void ResetIconStateDict(bool bIsLogin)
		{
			lock (this.m_StateIconsDict)
			{
				if (bIsLogin)
				{
					this.m_StateCacheIconsDict.Clear();
				}
				this.m_StateIconsDict.Clear();
			}
		}

		
		public void SendIconStateToClient(GameClient client)
		{
			ushort[] arrState = null;
			lock (this.m_StateIconsDict)
			{
				int nIconStateCount = this.m_StateIconsDict.Count<KeyValuePair<ushort, ushort>>();
				if (nIconStateCount > 0)
				{
					arrState = new ushort[nIconStateCount];
					nIconStateCount = 0;
					foreach (KeyValuePair<ushort, ushort> kvp in this.m_StateIconsDict)
					{
						arrState[nIconStateCount++] = kvp.Value;
					}
				}
				if (arrState != null && arrState.Length > 0)
				{
					this.m_ActivityIconStateData.arrIconState = arrState;
					client.sendCmd<ActivityIconStateData>(614, this.m_ActivityIconStateData, false);
					this.ResetIconStateDict(false);
				}
			}
		}

		
		public void LoginGameFlushIconState(GameClient client)
		{
			this.ResetIconStateDict(true);
			this.CheckHuangJinBoss(client);
			this.CheckShiJieBoss(client);
			this.CheckHuoDongState(client);
			this.CheckFuLiMeiRiHuoYue(client);
			this.CheckFuLiLianXuDengLu(client);
			this.CheckFuLiLeiJiDengLu(client);
			this.CheckFuMeiRiZaiXian(client);
			this.CheckFuUpLevelGift(client);
			this.CheckFuLiYueKaFanLi(client);
			this.CheckCombatGift(client);
			this.FlushChongZhiIconState(client);
			this.FlushUsedMoneyconState(client);
			this.CheckJingJiChangLeftTimes(client);
			this.CheckJingJiChangJiangLi(client);
			this.CheckJingJiChangJunXian(client);
			this.CheckZiYuanZhaoHui(client);
			this.CheckEmailCount(client, false);
			this.CheckFreeImpetrateState(client);
			this.CheckChengJiuUpLevelState(client);
			this.CheckPaiHangState(client);
			this.CheckVIPLevelAwardState(client);
			this.CheckThemeActivity(client);
			this.CheckReborn(client);
			this.CheckHeFuActivity(client);
			this.CheckJieRiActivity(client, true);
			this.CheckSpecialActivity(client);
			this.CheckEverydayActivity(client);
			this.CheckSpecPriorityActivity(client);
			this.CheckGuildIcon(client, true);
			this.CheckGuildIcon(client, true);
			this.CheckPetIcon(client);
			this.CheckBuildingIcon(client, true);
			this.CheckJunTuanEraIcon(client);
			this.SendIconStateToClient(client);
			this.CheckBuChangState(client);
			this.CheckCaiJiState(client);
			GameManager.MerlinMagicBookMgr.CheckMerlinSecretAttr(client);
			this.CheckFreeZhuanPanChouState(client);
			this.CheckShenYouAwardIcon(client);
			this.CheckFuMoMailIcon(client);
		}

		
		public bool FlushChongZhiIconState(GameClient client)
		{
			this.CheckShouCiChongZhi(client);
			this.CheckMeiRiChongZhi(client);
			this.CheckLeiJiChongZhi(client);
			this.CheckOneDollarChongZhi(client);
			this.CheckInputFanLiNewActivity(client);
			this.CheckOneDollarBuy(client);
			this.CheckXinFuChongZhiMoney(client);
			this.CheckXinFuFreeGetMoney(client);
			this.CheckSpecialActivity(client);
			this.CheckEverydayActivity(client);
			this.CheckSpecPriorityActivity(client);
			return false;
		}

		
		public bool FlushUsedMoneyconState(GameClient client)
		{
			this.CheckLeiJiXiaoFei(client);
			this.CheckXinFuUseMoney(client);
			this.CheckSpecialActivity(client);
			this.CheckEverydayActivity(client);
			this.CheckInputFanLiNewActivity(client);
			return false;
		}

		
		public bool CheckFuLiMeiRiHuoYue(GameClient client)
		{
			foreach (KeyValuePair<int, SystemXmlItem> kvp in GameManager.systemDailyActiveAward.SystemXmlItemDict)
			{
				int nAwardDailyActiveValue = Math.Max(0, kvp.Value.GetIntValue("NeedhuoYue", -1));
				int nID = kvp.Value.GetIntValue("ID", -1);
				if (nAwardDailyActiveValue <= client.ClientData.DailyActiveValues)
				{
					if (DailyActiveManager.IsDailyActiveAwardFetched(client, nID) <= 0)
					{
						return this.AddFlushIconState(3006, true);
					}
				}
			}
			return this.AddFlushIconState(3006, false);
		}

		
		public bool CheckFuLiLianXuDengLuReward(GameClient client)
		{
			int nDay = TimeUtil.NowDateTime().DayOfYear;
			bool bFulsh = true;
			if (client.ClientData.MyHuodongData.SeriesLoginAwardDayID == nDay && client.ClientData.MyHuodongData.SeriesLoginGetAwardStep <= client.ClientData.SeriesLoginNum)
			{
				bFulsh = false;
			}
			return bFulsh;
		}

		
		public bool CheckFuLiLianXuDengLu(GameClient client)
		{
			bool bFulsh = this.CheckFuLiLianXuDengLuReward(client);
			return this.AddFlushIconState(3007, bFulsh);
		}

		
		public bool CheckFuLiLeiJiDengLuReward(GameClient client)
		{
			int nFlag = Global.GetRoleParamsInt32FromDB(client, "TotalLoginAwardFlag");
			int nLoginNum = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalDayLogin);
			int nMaxLoginNum = Data.GetTotalLoginInfoNum();
			bool bFulsh = false;
			int i = 0;
			while (i < 7 && i < nLoginNum && i < nMaxLoginNum)
			{
				if ((nFlag & 1 << i + 1) == 0)
				{
					bFulsh = true;
					break;
				}
				i++;
			}
			if (nLoginNum == 30)
			{
				if ((nFlag & 1024) == 0)
				{
					bFulsh = true;
				}
			}
			if (nLoginNum == 21)
			{
				if ((nFlag & 512) == 0)
				{
					bFulsh = true;
				}
			}
			if (nLoginNum == 14)
			{
				if ((nFlag & 256) == 0)
				{
					bFulsh = true;
				}
			}
			return bFulsh;
		}

		
		public bool CheckShenYouAwardIcon(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (GoodsUtil.GetMeditateBagGoodsCnt(client) > 0)
			{
				result = this.AddFlushIconState(3036, true);
			}
			else
			{
				result = this.AddFlushIconState(3036, false);
			}
			return result;
		}

		
		public bool CheckFuMoMailIcon(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				string cmd = string.Format("{0}", client.ClientData.RoleID);
				int emailCount = Global.sendToDB<int, string>(14103, cmd, client.ServerId);
				if (emailCount > 0)
				{
					if (this.AddFlushIconState(3037, true))
					{
						this.SendIconStateToClient(client);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = this.AddFlushIconState(3037, false);
				}
			}
			return result;
		}

		
		public bool CheckFuLiLeiJiDengLu(GameClient client)
		{
			bool bFulsh = this.CheckFuLiLeiJiDengLuReward(client);
			return this.AddFlushIconState(3008, bFulsh);
		}

		
		public bool CheckFuLiYueKaFanLi(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				bool bFlush = this.CheckFuLiYueKaFanLiAward(client);
				int dayIdx = client.ClientData.YKDetail.CurDayOfPerYueKa() - 1;
				if (client.ClientData.YKDetail.HasYueKa == 1 && dayIdx >= 0 && dayIdx < client.ClientData.YKDetail.AwardInfo.Length && client.ClientData.YKDetail.AwardInfo[dayIdx] == '1')
				{
					result = (bFlush | this.AddFlushIconState(3013, false));
				}
				else
				{
					result = (bFlush | this.AddFlushIconState(3013, true));
				}
			}
			return result;
		}

		
		public bool CheckFuLiYueKaFanLiAward(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				int dayIdx = client.ClientData.YKDetail.CurDayOfPerYueKa() - 1;
				if (client.ClientData.YKDetail.HasYueKa == 1 && dayIdx >= 0 && dayIdx < client.ClientData.YKDetail.AwardInfo.Length && client.ClientData.YKDetail.AwardInfo[dayIdx] == '0')
				{
					result = this.AddFlushIconState(3015, true);
				}
				else
				{
					result = this.AddFlushIconState(3015, false);
				}
			}
			return result;
		}

		
		public bool CheckFuMeiRiZaiXian(GameClient client)
		{
			int nDate = TimeUtil.NowDateTime().DayOfYear;
			if (client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID != nDate)
			{
				client.ClientData.MyHuodongData.EveryDayOnLineAwardStep = 0;
				client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID = nDate;
			}
			int nSetp = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
			int nTotal = HuodongCachingMgr.GetEveryDayOnLineItemCount();
			bool result;
			if (nTotal == client.ClientData.MyHuodongData.EveryDayOnLineAwardStep)
			{
				result = this.AddFlushIconState(3009, false);
			}
			else
			{
				int nIndex = nTotal - client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
				for (int i = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep + 1; i <= nTotal; i++)
				{
					EveryDayOnLineAward EveryDayOnLineAwardItem = HuodongCachingMgr.GetEveryDayOnLineItem(i);
					if (null == EveryDayOnLineAwardItem)
					{
						return false;
					}
					if (client.ClientData.DayOnlineSecond >= EveryDayOnLineAwardItem.TimeSecs)
					{
						return this.AddFlushIconState(3009, true);
					}
				}
				result = this.AddFlushIconState(3009, false);
			}
			return result;
		}

		
		public bool CheckCombatGift(GameClient client)
		{
			long combatFlag = Global.GetRoleParamsInt64FromDB(client, "10154");
			bool exist = false;
			for (int i = 0; i < HuodongCachingMgr.CombatGiftMaxVal; i++)
			{
				if (Global.GetLongSomeBit(combatFlag, i * 2) == 1L && Global.GetLongSomeBit(combatFlag, i * 2 + 1) == 0L)
				{
					exist = true;
					break;
				}
			}
			return this.AddFlushIconState(3014, exist);
		}

		
		public bool CheckFuUpLevelGift(GameClient client)
		{
			List<int> flagList = Global.GetRoleParamsIntListFromDB(client, "UpLevelGiftFlags");
			bool exist = false;
			for (int i = 0; i < flagList.Count * 16; i++)
			{
				if (Global.GetBitValue(flagList, i * 2) == 1 && Global.GetBitValue(flagList, i * 2 + 1) == 0)
				{
					exist = true;
					break;
				}
			}
			return this.AddFlushIconState(3010, exist);
		}

		
		public bool CheckOneDollarBuy(GameClient client)
		{
			OneDollarBuyActivity act = HuodongCachingMgr.GetOneDollarBuyActivity();
			bool result;
			if (null == act)
			{
				result = this.AddFlushIconState(15052, false);
			}
			else
			{
				result = this.AddFlushIconState(15052, act.CheckClientCanBuy(client));
			}
			return result;
		}

		
		public bool CheckFuLiChongZhiHuiKui(GameClient client)
		{
			bool bShouCiChongZhi = this.CheckShouCiChongZhi(client);
			bool bMeiRiChongZhi = this.CheckMeiRiChongZhi(client);
			bool bLeiJiChongZhi = this.CheckLeiJiChongZhi(client);
			bool bLeiJiXiaoFei = this.CheckLeiJiXiaoFei(client);
			bool result;
			if (bShouCiChongZhi || bMeiRiChongZhi || bLeiJiChongZhi || bLeiJiXiaoFei)
			{
				result = this.AddFlushIconState(3001, true);
			}
			else
			{
				result = this.AddFlushIconState(3001, false);
			}
			return result;
		}

		
		public bool CheckShouCiChongZhi(GameClient client)
		{
			int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
			if (totalChongZhiMoney > 0)
			{
				if (Global.CanGetFirstChongZhiDaLiByUserID(client))
				{
					this.AddFlushIconState(3011, 0);
					return this.AddFlushIconState(3002, 1);
				}
				this.AddFlushIconState(3011, 1);
			}
			else
			{
				this.AddFlushIconState(3011, 0);
			}
			return this.AddFlushIconState(3002, 0);
		}

		
		public bool CheckOneDollarChongZhi(GameClient client)
		{
			bool hasGet;
			bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.OneDollarChongZhi, out hasGet);
			return this.AddFlushIconState(15051, ret);
		}

		
		public bool CheckInputFanLiNewActivity(GameClient client)
		{
			bool hasGet;
			bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.InputFanLiNew, out hasGet);
			return this.AddFlushIconState(15054, ret);
		}

		
		public bool CheckMeiRiChongZhi(GameClient client)
		{
			bool hasGet;
			bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.MeiRiChongZhiHaoLi, out hasGet);
			this.AddFlushIconState(3012, hasGet);
			WeedEndInputActivity act = HuodongCachingMgr.GetWeekEndInputActivity();
			if (act != null && act.InAwardTime())
			{
				int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
				if (act.GetWeekEndInputOpenDay(client) != currday)
				{
					ret = true;
				}
			}
			return this.AddFlushIconState(3003, ret);
		}

		
		public bool CheckLeiJiChongZhi(GameClient client)
		{
			bool hasGet;
			bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.TotalCharge, out hasGet);
			return this.AddFlushIconState(3004, ret);
		}

		
		public bool CheckLeiJiXiaoFei(GameClient client)
		{
			bool hasGet;
			bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.TotalConsume, out hasGet);
			return this.AddFlushIconState(3005, ret);
		}

		
		public bool CheckMainXinFuIcon(GameClient client)
		{
			return false;
		}

		
		public bool CheckXinFuKillBoss(GameClient client)
		{
			return false;
		}

		
		public bool CheckXinFuChongZhiMoney(GameClient client)
		{
			return false;
		}

		
		public bool CheckXinFuUseMoney(GameClient client)
		{
			return false;
		}

		
		public bool CheckXinFuFreeGetMoney(GameClient client)
		{
			return false;
		}

		
		public bool CheckJingJiChangLeftTimes(GameClient client)
		{
			bool result;
			if (JingJiChangManager.getInstance().checkEnterNum(client, JingJiChangConstants.Enter_Type_Free) == ResultCode.Success)
			{
				result = this.AddFlushIconState(4003, true);
			}
			else
			{
				result = this.AddFlushIconState(4003, false);
			}
			return result;
		}

		
		public bool CheckJingJiChangJiangLi(GameClient client)
		{
			bool result;
			if (JingJiChangManager.getInstance().CanGetrankingReward(client))
			{
				result = this.AddFlushIconState(4001, true);
			}
			else
			{
				result = this.AddFlushIconState(4001, false);
			}
			return result;
		}

		
		public bool CheckJingJiChangJunXian(GameClient client)
		{
			bool result;
			if (JingJiChangManager.getInstance().CanGradeJunXian(client))
			{
				result = this.AddFlushIconState(4002, true);
			}
			else
			{
				result = this.AddFlushIconState(4002, false);
			}
			return result;
		}

		
		public bool CheckShiJieBoss(GameClient client)
		{
			bool result;
			if (TimerBossManager.getInstance().HaveWorldBoss(client))
			{
				result = this.AddFlushIconState(1002, true);
			}
			else
			{
				result = this.AddFlushIconState(1002, false);
			}
			return result;
		}

		
		public bool CheckHuoDongState(GameClient client)
		{
			bool result;
			if (GameManager.AngelTempleMgr.CanEnterAngelTempleOnTime())
			{
				result = this.AddFlushIconState(1007, true);
			}
			else
			{
				this.AddFlushIconState(1007, false);
				result = true;
			}
			return result;
		}

		
		public bool CheckHuangJinBoss(GameClient client)
		{
			bool result;
			if (TimerBossManager.getInstance().HaveHuangJinBoss(client))
			{
				result = this.AddFlushIconState(1005, true);
			}
			else
			{
				result = this.AddFlushIconState(1005, false);
			}
			return result;
		}

		
		public bool CheckZiYuanZhaoHui(GameClient client)
		{
			bool result;
			if (CGetOldResourceManager.HasOldResource(client))
			{
				result = this.AddFlushIconState(7001, true);
			}
			else
			{
				result = this.AddFlushIconState(7001, false);
			}
			return result;
		}

		
		public bool CheckEmailCount(GameClient client, bool sendToClient = true)
		{
			string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 1, 1);
			int emailCount = Global.sendToDB<int, string>(649, cmd, client.ServerId);
			bool result;
			if (emailCount > 0)
			{
				result = this.AddFlushIconState(5002, true);
			}
			else
			{
				result = this.AddFlushIconState(5002, false);
			}
			if (result && sendToClient)
			{
				this.SendIconStateToClient(client);
			}
			return result;
		}

		
		public bool CheckPaiHangState(GameClient client)
		{
			return this.AddFlushIconState(7500, Global.GetAdmireCount(client) < 10);
		}

		
		public bool CheckChengJiuUpLevelState(GameClient client)
		{
			bool result = this.AddFlushIconState(9000, ChengJiuManager.CanActiveNextChengHao(client));
			this.SendIconStateToClient(client);
			return result;
		}

		
		public bool CheckVIPLevelAwardState(GameClient client)
		{
			for (int nIndex = 1; nIndex <= client.ClientData.VipLevel; nIndex++)
			{
				int nFlag = client.ClientData.VipAwardFlag & Global.GetBitValue(nIndex + 1);
				if (nFlag < 1)
				{
					return this.AddFlushIconState(10001, true);
				}
			}
			return this.AddFlushIconState(10001, false);
		}

		
		public bool CheckFreeImpetrateState(GameClient client)
		{
			bool bFlush = false;
			DateTime dTime = TimeUtil.NowDateTime();
			DateTime dTime2 = Global.GetRoleParamsDateTimeFromDB(client, "ImpetrateTime");
			double dSecond = (dTime - dTime2).TotalSeconds;
			double dRet = Global.GMax(0.0, (double)Data.FreeImpetrateIntervalTime - dSecond);
			if (dRet <= 0.0)
			{
				bFlush = true;
			}
			return this.AddFlushIconState(8000, bFlush);
		}

		
		public bool CheckBuChangState(GameClient client)
		{
			bool bFlush = BuChangManager.CheckGiveBuChang(client);
			return this.AddFlushIconState(11000, bFlush);
		}

		
		public bool CheckRebornUpgrade(GameClient client)
		{
			return this.AddFlushIconState(21001, RebornManager.getInstance().CheckRebornUpgradeIcon(client));
		}

		
		public bool CheckReborn(GameClient client)
		{
			this.AddFlushIconState(21001, false);
			bool bFlush = false;
			return this.CheckRebornUpgrade(client) || bFlush;
		}

		
		public bool CheckThemeActivity(GameClient client)
		{
			this.AddFlushIconState(11501, false);
			this.AddFlushIconState(11502, false);
			bool bFlush = false;
			bFlush = (this.CheckThemeZhiGou(client) || bFlush);
			return this.CheckThemeDaLiBao(client) || bFlush;
		}

		
		public bool CheckThemeZhiGou(GameClient client)
		{
			ThemeZhiGouActivity act = HuodongCachingMgr.GetThemeZhiGouActivity();
			bool result;
			if (null == act)
			{
				result = false;
			}
			else if (!act.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = act.CheckClientCanBuy(client);
				result = this.AddFlushIconState(11501, bFlush);
			}
			return result;
		}

		
		public bool CheckRegressZhiGou(GameClient client)
		{
			RegressActiveDayBuy act = HuodongCachingMgr.GetRegressActiveDayBuy();
			bool result;
			if (null == act)
			{
				result = false;
			}
			else if (!act.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = act.CheckClientCanBuy(client);
				result = this.AddFlushIconState(15058, bFlush);
			}
			return result;
		}

		
		public bool CheckThemeDaLiBao(GameClient client)
		{
			bool bFlush = false;
			ThemeDaLiBaoActivity act = HuodongCachingMgr.GetThemeDaLiBaoActivity();
			bool result;
			if (null == act)
			{
				result = false;
			}
			else if (!act.InActivityTime())
			{
				result = false;
			}
			else
			{
				string[] dbFields = null;
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 908, Global.GetActivityRequestCmdString(ActivityTypes.ThemeDaLiBao, client, 0), out dbFields, client.ServerId);
				if (null != dbFields)
				{
					if (dbFields != null && 3 == dbFields.Length)
					{
						int hasgettimes = Convert.ToInt32(dbFields[2]);
						if (hasgettimes == 0)
						{
							bFlush = true;
						}
					}
				}
				result = this.AddFlushIconState(11502, bFlush);
			}
			return result;
		}

		
		public bool CheckHeFuActivity(GameClient client)
		{
			this.AddFlushIconState(12001, false);
			this.AddFlushIconState(12002, false);
			this.AddFlushIconState(12003, false);
			this.AddFlushIconState(12004, false);
			this.AddFlushIconState(12005, false);
			bool bFlush = false;
			bFlush = (this.CheckHeFuLogin(client) || bFlush);
			bFlush = (this.CheckHeFuTotalLogin(client) || bFlush);
			bFlush = (this.CheckHeFuRecharge(client) || bFlush);
			bFlush = (this.CheckHeFuPKKing(client) || bFlush);
			bFlush = (this.CheckHeFuLuoLan(client) || bFlush);
			return this.AddFlushIconState(12000, bFlush);
		}

		
		public bool CheckHeFuLogin(GameClient client)
		{
			HeFuLoginActivity activity = HuodongCachingMgr.GetHeFuLoginActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				int nFlag = Global.GetRoleParamsInt32FromDB(client, "HeFuLoginFlag");
				int nValue = Global.GetIntSomeBit(nFlag, 1);
				if (nValue != 0)
				{
					nValue = Global.GetIntSomeBit(nFlag, 2);
					if (nValue == 0)
					{
						bFlush = true;
					}
					else
					{
						nValue = Global.GetIntSomeBit(nFlag, 3);
						if (nValue == 0)
						{
							if (Global.IsVip(client))
							{
								bFlush = true;
							}
						}
					}
				}
				result = this.AddFlushIconState(12001, bFlush);
			}
			return result;
		}

		
		public bool CheckHeFuTotalLogin(GameClient client)
		{
			HeFuTotalLoginActivity activity = HuodongCachingMgr.GetHeFuTotalLoginActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				int totalloginnum = Global.GetRoleParamsInt32FromDB(client, "HeFuTotalLoginNum");
				for (int i = 1; i <= totalloginnum; i++)
				{
					if (activity.GetAward(i) != null)
					{
						int nFlag = Global.GetRoleParamsInt32FromDB(client, "HeFuTotalLoginFlag");
						int nValue = Global.GetIntSomeBit(nFlag, i);
						if (nValue == 0)
						{
							bFlush = true;
							break;
						}
					}
				}
				result = this.AddFlushIconState(12002, bFlush);
			}
			return result;
		}

		
		public bool CheckHeFuRecharge(GameClient client)
		{
			int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
			int hefuday = Global.GetOffsetDay(Global.GetHefuStartDay());
			bool result;
			if (currday == hefuday)
			{
				result = false;
			}
			else
			{
				HeFuRechargeActivity activity = HuodongCachingMgr.GetHeFuRechargeActivity();
				if (null == activity)
				{
					result = false;
				}
				else if (!activity.InActivityTime() && !activity.InAwardTime())
				{
					result = false;
				}
				else
				{
					bool bFlush = false;
					string[] dbFields = null;
					TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10160, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						client.ClientData.RoleID,
						23,
						hefuday,
						Global.GetOffsetDay(DateTime.Parse(activity.ToDate)),
						activity.strcoe
					}), out dbFields, client.ServerId);
					if (null != dbFields)
					{
						if (dbFields != null && 1 == dbFields.Length)
						{
							string[] strrebate = dbFields[0].Split(new char[]
							{
								'|'
							});
							if (1 <= dbFields.Length)
							{
								bFlush = (Convert.ToInt32(strrebate[0]) > 0);
							}
						}
					}
					result = this.AddFlushIconState(12003, bFlush);
				}
			}
			return result;
		}

		
		public bool CheckHeFuPKKing(GameClient client)
		{
			HeFuPKKingActivity activity = HuodongCachingMgr.GetHeFuPKKingActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				if (client.ClientData.RoleID == HuodongCachingMgr.GetHeFuPKKingRoleID())
				{
					int nFlag = Global.GetRoleParamsInt32FromDB(client, "HeFuPKKingFlag");
					if (nFlag == 0)
					{
						bFlush = true;
					}
				}
				result = this.AddFlushIconState(12004, bFlush);
			}
			return result;
		}

		
		public bool CheckHeFuLuoLan(GameClient client)
		{
			HeFuLuoLanActivity activity = HuodongCachingMgr.GetHeFuLuoLanActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				int guildwinnum = 0;
				int chengzhuwinnum = 0;
				int guizuwinnum = 0;
				string strHefuLuolanGuildid = GameManager.GameConfigMgr.GetGameConfigItemStr("hefu_luolan_guildid", "");
				string[] strFields = strHefuLuolanGuildid.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < strFields.Length; i++)
				{
					string[] strInfos = strFields[i].Split(new char[]
					{
						','
					});
					if (2 == strInfos.Length)
					{
						if (Convert.ToInt32(strInfos[0]) == client.ClientData.Faction)
						{
							guildwinnum++;
							if (Convert.ToInt32(strInfos[1]) != client.ClientData.RoleID)
							{
								guizuwinnum++;
							}
						}
						if (Convert.ToInt32(strInfos[1]) == client.ClientData.RoleID)
						{
							chengzhuwinnum++;
						}
					}
				}
				int nFlag = Global.GetRoleParamsInt32FromDB(client, "HeFuLuoLanAwardFlag");
				foreach (KeyValuePair<int, HeFuLuoLanAward> item in activity.HeFuLuoLanAwardDict)
				{
					HeFuLuoLanAward hefuLuoLanAward = item.Value;
					if (1 == hefuLuoLanAward.status)
					{
						if (chengzhuwinnum >= hefuLuoLanAward.winNum)
						{
							int nValue = Global.GetIntSomeBit(nFlag, item.Key);
							if (0 == nValue)
							{
								bFlush = true;
								break;
							}
						}
					}
					else if (2 == hefuLuoLanAward.status)
					{
						if (guizuwinnum >= hefuLuoLanAward.winNum)
						{
							int nValue = Global.GetIntSomeBit(nFlag, item.Key);
							if (0 == nValue)
							{
								bFlush = true;
								break;
							}
						}
					}
				}
				result = this.AddFlushIconState(12005, bFlush);
			}
			return result;
		}

		
		public bool CheckCaiJiState(GameClient client)
		{
			return this.AddFlushIconState(13000, CaiJiLogic.HasLeftnum(client));
		}

		
		public bool CheckSpecialActivity(GameClient client)
		{
			SpecialActivity act = HuodongCachingMgr.GetSpecialActivity();
			bool result;
			if (null == act)
			{
				result = false;
			}
			else
			{
				bool bFlush = act.CheckIconState(client);
				result = this.AddFlushIconState(14110, bFlush);
			}
			return result;
		}

		
		public bool CheckEverydayActivity(GameClient client)
		{
			EverydayActivity act = HuodongCachingMgr.GetEverydayActivity();
			bool result;
			if (null == act)
			{
				result = false;
			}
			else
			{
				bool bFlush = act.CheckIconState(client);
				result = this.AddFlushIconState(14114, bFlush);
			}
			return result;
		}

		
		public bool CheckSpecPriorityActivity(GameClient client)
		{
			SpecPriorityActivity act = HuodongCachingMgr.GetSpecPriorityActivity();
			bool result;
			if (null == act)
			{
				result = false;
			}
			else
			{
				bool bFlush = act.CheckIconState(client);
				result = this.AddFlushIconState(14115, bFlush);
			}
			return result;
		}

		
		public bool CheckJieRiActivity(GameClient client, bool isLogin)
		{
			if (isLogin)
			{
				this.AddFlushIconState(14000, false);
				this.AddFlushIconState(14001, false);
				this.AddFlushIconState(14002, false);
				this.AddFlushIconState(14003, false);
				this.AddFlushIconState(14004, false);
				this.AddFlushIconState(14005, false);
				this.AddFlushIconState(14006, false);
				this.AddFlushIconState(14007, false);
				this.AddFlushIconState(14008, false);
				this.AddFlushIconState(14009, false);
				this.AddFlushIconState(14010, false);
				this.AddFlushIconState(14011, false);
				this.AddFlushIconState(14012, false);
				this.AddFlushIconState(14013, false);
				this.AddFlushIconState(14014, false);
				this.AddFlushIconState(14015, false);
				this.AddFlushIconState(14016, false);
				this.AddFlushIconState(14017, false);
				this.AddFlushIconState(14018, false);
				this.AddFlushIconState(14019, false);
				this.AddFlushIconState(14020, false);
				this.AddFlushIconState(14021, false);
				this.AddFlushIconState(14023, false);
				this.AddFlushIconState(14027, false);
				this.AddFlushIconState(14028, false);
				this.AddFlushIconState(14033, false);
				this.AddFlushIconState(14034, false);
				this.AddFlushIconState(14035, false);
			}
			bool bAnyChildTipChanged = false;
			bAnyChildTipChanged |= this.CheckJieRiLogin(client);
			bAnyChildTipChanged |= this.CheckJieRiTotalLogin(client);
			bAnyChildTipChanged |= this.CheckJieRiDayCZ(client);
			bAnyChildTipChanged |= this.CheckJieRiLeiJiXF(client);
			bAnyChildTipChanged |= this.CheckJieRiLeiJiCZ(client);
			bAnyChildTipChanged |= this.CheckJieRiCZKING(client);
			bAnyChildTipChanged |= this.CheckJieRiXFKING(client);
			bAnyChildTipChanged |= this.CheckJieRiLeiJi(client);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieriWing);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieriAddon);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieriStrengthen);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieriAchievement);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieriMilitaryRank);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieriVIPFanli);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieriAmulet);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieriArchangel);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieriMarriage);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieRiHuiJi);
			bAnyChildTipChanged |= this.CheckJieRiFanLi(client, ActivityTypes.JieRiFuWen);
			bAnyChildTipChanged |= this.CheckJieriGive(client);
			bAnyChildTipChanged |= this.CheckJieriGiveKing(client);
			bAnyChildTipChanged |= this.CheckJieriRecvKing(client);
			bAnyChildTipChanged |= this.CheckJieriLianXuCharge(client);
			bAnyChildTipChanged |= this.CheckJieriRecv(client);
			bAnyChildTipChanged |= this.CheckJieriIPointsExchg(client);
			bAnyChildTipChanged |= this.CheckJieriDanBiChongZhi(client);
			bAnyChildTipChanged |= this.CheckJieRiHongBaoBang(client);
			bAnyChildTipChanged |= this.CheckJieRiPCKingEveryDay(client);
			bool isJieRiActivityTipActived = this.IsAnyJieRiTipActived();
			return this.AddFlushIconState(14000, isJieRiActivityTipActived) || bAnyChildTipChanged;
		}

		
		public bool IsAnyJieRiTipActived()
		{
			return this.IsAnyTipActived(IconStateManager.m_jieRiIconList);
		}

		
		public bool IsAnyTipActived(List<ActivityTipTypes> iconTipList)
		{
			bool bAnyActived = false;
			if (iconTipList != null)
			{
				lock (this.m_StateCacheIconsDict)
				{
					foreach (ActivityTipTypes e_tip in iconTipList)
					{
						ushort state = 0;
						if (this.m_StateCacheIconsDict.TryGetValue((ushort)e_tip, out state))
						{
							if ((state & 1) == 1)
							{
								bAnyActived = true;
								break;
							}
						}
					}
				}
			}
			return bAnyActived;
		}

		
		public bool CheckJieRiLogin(GameClient client)
		{
			JieriDaLiBaoActivity activity = HuodongCachingMgr.GetJieriDaLiBaoActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InActivityTime() && !activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				string[] dbFields = null;
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 461, Global.GetActivityRequestCmdString(ActivityTypes.JieriDaLiBao, client, 0), out dbFields, client.ServerId);
				if (null != dbFields)
				{
					if (dbFields != null && 3 == dbFields.Length)
					{
						int hasgettimes = Convert.ToInt32(dbFields[2]);
						if (hasgettimes == 0)
						{
							bFlush = true;
						}
					}
				}
				result = this.AddFlushIconState(14001, bFlush);
			}
			return result;
		}

		
		public bool CheckJieRiTotalLogin(GameClient client)
		{
			JieRiDengLuActivity activity = HuodongCachingMgr.GetJieRiDengLuActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				string[] dbFields = null;
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 462, Global.GetActivityRequestCmdString(ActivityTypes.JieriDengLuHaoLi, client, 0), out dbFields, client.ServerId);
				if (null != dbFields)
				{
					if (dbFields != null && 4 == dbFields.Length)
					{
						int hasgettimes = Convert.ToInt32(dbFields[2]);
						int dengLuTimes = Convert.ToInt32(dbFields[3]);
						for (int i = 0; i < dengLuTimes; i++)
						{
							if (activity.GetAward(client, i + 1) != null)
							{
								int nValue = Global.GetIntSomeBit(hasgettimes, i);
								if (nValue == 0)
								{
									bFlush = true;
									break;
								}
							}
						}
					}
				}
				result = this.AddFlushIconState(14002, bFlush);
			}
			return result;
		}

		
		public bool CheckJieRiDayCZ(GameClient client)
		{
			JieriCZSongActivity activity = HuodongCachingMgr.GetJieriCZSongActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				string[] dbFields = null;
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 464, Global.GetActivityRequestCmdString(ActivityTypes.JieriCZSong, client, 0), out dbFields, client.ServerId);
				if (null != dbFields)
				{
					if (dbFields != null && 5 == dbFields.Length)
					{
						int roleYuanBaoInPeriod = Convert.ToInt32(dbFields[3]);
						int hasgettimes = Convert.ToInt32(dbFields[4]);
						foreach (KeyValuePair<int, AwardItem> item in activity.AwardItemDict)
						{
							if (roleYuanBaoInPeriod >= item.Value.MinAwardCondionValue)
							{
								int nValue = Global.GetIntSomeBit(hasgettimes, item.Key - 1);
								if (nValue == 0)
								{
									bFlush = true;
									break;
								}
							}
						}
					}
				}
				result = this.AddFlushIconState(14003, bFlush);
			}
			return result;
		}

		
		public bool CheckJieRiLeiJiXF(GameClient client)
		{
			JieRiTotalConsumeActivity activity = HuodongCachingMgr.GetJieRiTotalConsumeActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				string[] dbFields = null;
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 683, Global.GetActivityRequestCmdString(ActivityTypes.JieriTotalConsume, client, 0), out dbFields, client.ServerId);
				if (null != dbFields)
				{
					if (dbFields != null && 4 == dbFields.Length)
					{
						int roleYuanBaoInPeriod = Convert.ToInt32(dbFields[2]);
						int hasgettimes = Convert.ToInt32(dbFields[3]);
						foreach (KeyValuePair<int, AwardItem> item in activity.AwardItemDict)
						{
							if (roleYuanBaoInPeriod >= item.Value.MinAwardCondionValue)
							{
								int nValue = Global.GetIntSomeBit(hasgettimes, item.Key - 1);
								if (nValue == 0)
								{
									bFlush = true;
									break;
								}
							}
						}
					}
				}
				result = this.AddFlushIconState(14004, bFlush);
			}
			return result;
		}

		
		public bool CheckJieRiLeiJiCZ(GameClient client)
		{
			JieRiLeiJiCZActivity activity = HuodongCachingMgr.GetJieRiLeiJiCZActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				string[] dbFields = null;
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 465, Global.GetActivityRequestCmdString(ActivityTypes.JieriLeiJiCZ, client, 0), out dbFields, client.ServerId);
				if (null != dbFields)
				{
					if (dbFields != null && 4 == dbFields.Length)
					{
						int roleYuanBaoInPeriod = Convert.ToInt32(dbFields[2]);
						int hasgettimes = Convert.ToInt32(dbFields[3]);
						foreach (KeyValuePair<int, AwardItem> item in activity.AwardItemDict)
						{
							if (roleYuanBaoInPeriod >= item.Value.MinAwardCondionValue)
							{
								int nValue = Global.GetIntSomeBit(hasgettimes, item.Key - 1);
								if (nValue == 0)
								{
									bFlush = true;
									break;
								}
							}
						}
					}
				}
				result = this.AddFlushIconState(14005, bFlush);
			}
			return result;
		}

		
		public bool CheckJieRiLeiJi(GameClient client)
		{
			JieRiMeiRiLeiJiActivity activity = HuodongCachingMgr.GetJieriMeiRiLeiJiActivity();
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bFlush = false;
				DateTime today = TimeUtil.NowDateTime();
				DateTime beginDay = DateTime.Parse(activity.FromDate);
				int days = (int)(today - beginDay).TotalDays + 1;
				for (int i = 0; i < days; i++)
				{
					string[] dbFields = null;
					TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 1806, Global.GetActivityRequestCmdString(ActivityTypes.JieRiMeiRiLeiJi, client, 1000 * (i + 1)), out dbFields, client.ServerId);
					if (null == dbFields)
					{
						break;
					}
					if (dbFields == null || 4 != dbFields.Length)
					{
						break;
					}
					int roleYuanBaoInPeriod = Convert.ToInt32(dbFields[2]);
					int hasgettimes = Convert.ToInt32(dbFields[3]);
					if (!activity.DayAwardItemDict.ContainsKey(i + 1))
					{
						break;
					}
					int index = 0;
					foreach (AwardItem item in activity.DayAwardItemDict[i + 1])
					{
						if (roleYuanBaoInPeriod >= item.MinAwardCondionValue)
						{
							int nValue = Global.GetIntSomeBit(hasgettimes, index);
							if (nValue == 0)
							{
								bFlush = true;
								break;
							}
						}
						index++;
					}
					if (bFlush)
					{
						break;
					}
				}
				result = this.AddFlushIconState(14028, bFlush);
			}
			return result;
		}

		
		public bool CheckJieRiHongBaoBang(GameClient client)
		{
			JieriHongBaoKingActivity activity = JieriHongBaoKingActivity.getInstance();
			bool bFlush = activity.CanGetAnyAward(client);
			return this.AddFlushIconState(14032, bFlush);
		}

		
		public bool CheckJieRiPCKingEveryDay(GameClient client)
		{
			bool bFlush = false;
			JieriPlatChargeKingEveryDay activity = HuodongCachingMgr.GetJieriPCKingEveryDayActivity();
			if (null != activity)
			{
				bFlush |= activity.CanGetAnyAward(client);
			}
			return this.AddFlushIconState(14035, bFlush);
		}

		
		public bool CheckJieRiCZKING(GameClient client)
		{
			KingActivity activity = HuodongCachingMgr.GetJieRiCZKingActivity();
			bool result2;
			if (null == activity)
			{
				result2 = false;
			}
			else if (!activity.InAwardTime())
			{
				result2 = false;
			}
			else
			{
				bool bFlush = false;
				string[] dbFields = null;
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 468, Global.GetActivityRequestCmdString(ActivityTypes.JieriPTCZKing, client, 1), out dbFields, client.ServerId);
				if (dbFields != null && 3 == dbFields.Length)
				{
					int result = Convert.ToInt32(dbFields[0]);
					int roleid = Convert.ToInt32(dbFields[1]);
					int hasgettimes = Convert.ToInt32(dbFields[2]);
					if (1 == result)
					{
						if (roleid == client.ClientData.RoleID)
						{
							bFlush = (hasgettimes == 1);
						}
					}
				}
				result2 = this.AddFlushIconState(14006, bFlush);
			}
			return result2;
		}

		
		public bool CheckJieRiXFKING(GameClient client)
		{
			KingActivity activity = HuodongCachingMgr.GetJieriXiaoFeiKingActivity();
			bool result2;
			if (null == activity)
			{
				result2 = false;
			}
			else if (!activity.InAwardTime())
			{
				result2 = false;
			}
			else
			{
				bool bFlush = false;
				string[] dbFields = null;
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 467, Global.GetActivityRequestCmdString(ActivityTypes.JieriPTXiaoFeiKing, client, 1), out dbFields, client.ServerId);
				if (dbFields != null && 3 == dbFields.Length)
				{
					int result = Convert.ToInt32(dbFields[0]);
					int roleid = Convert.ToInt32(dbFields[1]);
					int hasgettimes = Convert.ToInt32(dbFields[2]);
					if (1 == result)
					{
						if (roleid == client.ClientData.RoleID)
						{
							bFlush = (hasgettimes == 1);
						}
					}
				}
				result2 = this.AddFlushIconState(14007, bFlush);
			}
			return result2;
		}

		
		public bool CheckJieriGive(GameClient client)
		{
			JieriGiveActivity act = HuodongCachingMgr.GetJieriGiveActivity();
			bool result;
			if (act == null || !act.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool hasCanGetAward = act.CanGetAnyAward(client);
				result = this.AddFlushIconState(14008, hasCanGetAward);
			}
			return result;
		}

		
		public bool CheckJieriRecv(GameClient client)
		{
			JieriRecvActivity act = HuodongCachingMgr.GetJieriRecvActivity();
			bool result;
			if (act == null || !act.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool hasCanGetAward = act.CanGetAnyAward(client);
				result = this.AddFlushIconState(14021, hasCanGetAward);
			}
			return result;
		}

		
		public bool CheckJieriDanBiChongZhi(GameClient client)
		{
			DanBiChongZhiActivity act = HuodongCachingMgr.GetDanBiChongZhiActivity();
			bool result;
			if (act == null || !act.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool hasCanGetAward = act.CanGetAnyAward(client);
				result = this.AddFlushIconState(14027, hasCanGetAward);
			}
			return result;
		}

		
		public bool CheckJieriIPointsExchg(GameClient client)
		{
			JieriIPointsExchgActivity act = HuodongCachingMgr.GetJieriIPointsExchgActivity();
			bool result;
			if (act == null || !act.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool hasCanGetAward = act.CanGetAnyAward(client);
				result = this.AddFlushIconState(14023, hasCanGetAward);
			}
			return result;
		}

		
		public bool CheckJieriGiveKing(GameClient client)
		{
			JieRiGiveKingActivity act = HuodongCachingMgr.GetJieriGiveKingActivity();
			bool result;
			if (act == null || !act.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool hasCanGetAward = act.CanGetAnyAward(client);
				result = this.AddFlushIconState(14009, hasCanGetAward);
			}
			return result;
		}

		
		public bool CheckJieriRecvKing(GameClient client)
		{
			JieRiRecvKingActivity act = HuodongCachingMgr.GetJieriRecvKingActivity();
			bool result;
			if (act == null || !act.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool hasCanGetAward = act.CanGetAnyAward(client);
				result = this.AddFlushIconState(14010, hasCanGetAward);
			}
			return result;
		}

		
		public bool CheckJieriLianXuCharge(GameClient client)
		{
			JieriLianXuChargeActivity act = HuodongCachingMgr.GetJieriLianXuChargeActivity();
			bool result;
			if (act == null || !act.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool hasCanGetAward = act.CanGetAnyAward(client);
				result = this.AddFlushIconState(14020, hasCanGetAward);
			}
			return result;
		}

		
		public bool CheckGuildIcon(GameClient client, bool isLogin)
		{
			if (isLogin)
			{
				this.AddFlushIconState(15000, false);
				this.AddFlushIconState(15001, false);
			}
			else
			{
				ProcessTask.ProcessRoleTaskVal(client, TaskTypes.InZhanMeng, -1);
			}
			bool bFlush = false;
			bFlush |= this.CheckGuildCopyMap(client);
			return this.AddFlushIconState(15000, bFlush);
		}

		
		public bool CheckGuildCopyMap(GameClient client)
		{
			bool bFlush = false;
			int mapid = -1;
			int seqid = -1;
			int mapcode = -1;
			GameManager.GuildCopyMapMgr.CheckCurrGuildCopyMap(client, out mapid, out seqid, mapcode);
			bool result;
			if (mapid < 0)
			{
				result = false;
			}
			else
			{
				int nGuildCopyMapAwardFlag = Global.GetRoleParamsInt32FromDB(client, "GuildCopyMapAwardFlag");
				for (int i = 0; i < GameManager.GuildCopyMapMgr.GuildCopyMapOrderList.Count; i++)
				{
					int fubenID = GameManager.GuildCopyMapMgr.GuildCopyMapOrderList[i];
					if (mapid != 0)
					{
						bFlush = true;
						break;
					}
					if (mapid > 0 && fubenID >= mapid)
					{
						break;
					}
					if (!GameManager.GuildCopyMapMgr.GetGuildCopyMapAwardDayFlag(nGuildCopyMapAwardFlag, i, 2))
					{
						bFlush = true;
						break;
					}
				}
				result = this.AddFlushIconState(15001, bFlush);
			}
			return result;
		}

		
		public bool CheckPetIcon(GameClient client)
		{
			this.AddFlushIconState(16000, false);
			this.AddFlushIconState(16001, false);
			bool bFlush = false;
			bFlush |= this.CheckPetBagIcon(client);
			return bFlush | this.CheckCallPetIcon(client);
		}

		
		public bool CheckBuildingFreeQueue(GameClient client)
		{
			BuildingManager BuildingMgr = BuildingManager.getInstance();
			int free = 0;
			int pay = 0;
			BuildingMgr.GetTaskNumInEachTeam(client, out free, out pay);
			return free < 4;
		}

		
		public bool CheckBuildingAward(GameClient client)
		{
			bool bFlush = false;
			BuildingManager BuildingMgr = BuildingManager.getInstance();
			bFlush |= BuildingMgr.CheckCanGetAnyAllLevelAward(client);
			return bFlush | BuildingMgr.CheckAnyTaskFinish(client);
		}

		
		public bool CheckBuildingIcon(GameClient client, bool isLogin)
		{
			if (isLogin)
			{
				this.AddFlushIconState(15050, false);
			}
			bool bFlush = false;
			bFlush |= this.CheckBuildingFreeQueue(client);
			bFlush |= this.CheckBuildingAward(client);
			return this.AddFlushIconState(15050, bFlush);
		}

		
		public bool CheckJunTuanEraIcon(GameClient client)
		{
			return this.AddFlushIconState(15053, EraManager.getInstance().CheckJunTuanEraIcon(client));
		}

		
		public bool CheckKF5V5DDailyPaiHang(GameClient client)
		{
			DuanWeiRankAward duanWeiRankAward;
			bool result;
			if (TianTi5v5Manager.getInstance().CanGetMonthRankAwards(client, out duanWeiRankAward))
			{
				result = this.AddFlushIconState(15013, true);
			}
			else
			{
				result = this.AddFlushIconState(15013, false);
			}
			return result;
		}

		
		public bool CheckTianTiMonthPaiMingAwards(GameClient client)
		{
			ushort iState = 0;
			DuanWeiRankAward duanWeiRankAward = null;
			if (TianTiManager.getInstance().CanGetMonthRankAwards(client, out duanWeiRankAward))
			{
				iState = 1;
			}
			bool result;
			if (this.AddFlushIconState(1008, iState))
			{
				this.SendIconStateToClient(client);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool CheckPetBagIcon(GameClient client)
		{
			return false;
		}

		
		public bool CheckCallPetIcon(GameClient client)
		{
			bool bFlush = CallPetManager.getFreeSec(client) <= 0L;
			return this.AddFlushIconState(16001, bFlush);
		}

		
		public void DoSpriteIconTicks(GameClient client)
		{
			long startTicks = TimeUtil.NOW();
			if (startTicks >= this.m_LastTicksBuilding)
			{
				this.m_LastTicksBuilding = startTicks + 5000L;
				if (client._IconStateMgr.CheckBuildingIcon(client, false))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
			if (startTicks >= this.m_LastTicks)
			{
				if (this.m_LastTicks == 0L)
				{
					this.m_LastTicks = startTicks + 5000L;
				}
				else
				{
					this.m_LastTicks = startTicks + 20000L;
					client._IconStateMgr.CheckPetIcon(client);
					client._IconStateMgr.CheckTianTiMonthPaiMingAwards(client);
					LangHunLingYuManager.getInstance().CheckTipsIconState(client);
					SingletonTemplate<ZhengBaManager>.Instance().CheckTipsIconState(client);
					SingletonTemplate<CoupleArenaManager>.Instance().CheckTipsIconState(client);
					SingletonTemplate<CoupleWishManager>.Instance().CheckTipsIconState(client);
					ZhengDuoManager.getInstance().CheckTipsIconState(client);
				}
			}
		}

		
		public bool CheckJieRiFanLi(GameClient client, ActivityTypes nActType)
		{
			JieriFanLiActivity activity = HuodongCachingMgr.GetJieriFanLiActivity(nActType);
			bool result;
			if (null == activity)
			{
				result = false;
			}
			else if (!activity.InAwardTime())
			{
				result = false;
			}
			else
			{
				string[] dbFields = null;
				string sCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					activity.FromDate.Replace(':', '$'),
					activity.ToDate.Replace(':', '$'),
					(int)nActType,
					0
				});
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 927, sCmd, out dbFields, client.ServerId);
				if (dbFields == null || 2 != dbFields.Length)
				{
					result = false;
				}
				else
				{
					int hasgettimes = Convert.ToInt32(dbFields[1]);
					bool bFlush = false;
					for (int i = 1; i <= 5; i++)
					{
						int bitVal = Global.GetBitValue(i);
						if ((hasgettimes & bitVal) != bitVal)
						{
							bFlush = activity.CheckCondition(client, i);
							if (bFlush)
							{
								break;
							}
						}
					}
					ushort usIconTypes = 0;
					switch (nActType)
					{
					case ActivityTypes.JieriWing:
						usIconTypes = 14011;
						break;
					case ActivityTypes.JieriAddon:
						usIconTypes = 14012;
						break;
					case ActivityTypes.JieriStrengthen:
						usIconTypes = 14013;
						break;
					case ActivityTypes.JieriAchievement:
						usIconTypes = 14014;
						break;
					case ActivityTypes.JieriMilitaryRank:
						usIconTypes = 14015;
						break;
					case ActivityTypes.JieriVIPFanli:
						usIconTypes = 14016;
						break;
					case ActivityTypes.JieriAmulet:
						usIconTypes = 14017;
						break;
					case ActivityTypes.JieriArchangel:
						usIconTypes = 14018;
						break;
					case ActivityTypes.JieriLianXuCharge:
						break;
					case ActivityTypes.JieriMarriage:
						usIconTypes = 14019;
						break;
					default:
						switch (nActType)
						{
						case ActivityTypes.JieRiHuiJi:
							usIconTypes = 14033;
							break;
						case ActivityTypes.JieRiFuWen:
							usIconTypes = 14034;
							break;
						}
						break;
					}
					result = this.AddFlushIconState(usIconTypes, bFlush);
				}
			}
			return result;
		}

		
		public void xxxxx(GameClient client)
		{
			bool bFlush = false;
			DateTime dTime = TimeUtil.NowDateTime();
			DateTime dTime2 = Global.GetRoleParamsDateTimeFromDB(client, "ImpetrateTime");
			double dSecond = (dTime - dTime2).TotalSeconds;
			double dRet = Global.GMax(0.0, (double)Data.FreeImpetrateIntervalTime - dSecond);
			if (dRet <= 0.0)
			{
				bFlush = true;
			}
			this.AddFlushIconState(8000, bFlush);
			client._IconStateMgr.SendIconStateToClient(client);
		}

		
		public bool CheckFreeZhuanPanChouState(GameClient client)
		{
			bool bFlush = false;
			int freeTime = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("ZhuanPanFree"));
			bool result;
			if (freeTime < 0)
			{
				bFlush = false;
				result = this.AddFlushIconState(18002, bFlush);
			}
			else
			{
				DateTime dTime = TimeUtil.NowDateTime();
				DateTime dTime2 = Global.GetRoleParamsDateTimeFromDB(client, "10155");
				DateTime regTime = ZhuanPanManager.getInstance().GetBeginTime();
				if (dTime2 < regTime)
				{
					dTime2 = regTime;
					Global.SaveRoleParamsDateTimeToDB(client, "10155", dTime2, true);
				}
				double dSecond = (dTime - dTime2).TotalSeconds;
				freeTime *= 3600;
				double dRet = Global.GMax(0.0, (double)freeTime - dSecond);
				if (dRet <= 0.0)
				{
					bFlush = true;
				}
				result = this.AddFlushIconState(18002, bFlush);
			}
			return result;
		}

		
		private Dictionary<ushort, ushort> m_StateIconsDict = new Dictionary<ushort, ushort>();

		
		private Dictionary<ushort, ushort> m_StateCacheIconsDict = new Dictionary<ushort, ushort>();

		
		private ActivityIconStateData m_ActivityIconStateData = new ActivityIconStateData();

		
		private long m_LastTicks = 0L;

		
		private long m_LastTicksBuilding = 0L;

		
		private static List<ActivityTipTypes> m_jieRiIconList = new List<ActivityTipTypes>
		{
			ActivityTipTypes.JieRiLogin,
			ActivityTipTypes.JieRiTotalLogin,
			ActivityTipTypes.JieRiDayCZ,
			ActivityTipTypes.JieRiLeiJiXF,
			ActivityTipTypes.JieRiLeiJiCZ,
			ActivityTipTypes.JieRiCZKING,
			ActivityTipTypes.JieRiXFKING,
			ActivityTipTypes.JieRiGive,
			ActivityTipTypes.JieRiGiveKing,
			ActivityTipTypes.JieRiRecvKing,
			ActivityTipTypes.JieRiRecv,
			ActivityTipTypes.JieriWing,
			ActivityTipTypes.JieriAddon,
			ActivityTipTypes.JieriStrengthen,
			ActivityTipTypes.JieriAchievement,
			ActivityTipTypes.JieriMilitaryRank,
			ActivityTipTypes.JieriVIPFanli,
			ActivityTipTypes.JieriAmulet,
			ActivityTipTypes.JieriArchangel,
			ActivityTipTypes.JieriMarriage,
			ActivityTipTypes.JieRiLianXuCharge,
			ActivityTipTypes.JieRiIPointsExchg,
			ActivityTipTypes.JieRiPlatChargeKing,
			ActivityTipTypes.JieRiHuiJi,
			ActivityTipTypes.JieRiFuWen,
			ActivityTipTypes.JieRiPCKingEveryDay
		};
	}
}
