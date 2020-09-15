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
	// Token: 0x02000784 RID: 1924
	public class IconStateManager
	{
		// Token: 0x0600316C RID: 12652 RVA: 0x002C5B48 File Offset: 0x002C3D48
		public bool AddFlushIconState(ushort nIconOrder, bool bIconState)
		{
			ushort iState = bIconState ? 1 : 0;
			return this.AddFlushIconState(nIconOrder, iState);
		}

		// Token: 0x0600316D RID: 12653 RVA: 0x002C5B6C File Offset: 0x002C3D6C
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

		// Token: 0x0600316E RID: 12654 RVA: 0x002C5C28 File Offset: 0x002C3E28
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

		// Token: 0x0600316F RID: 12655 RVA: 0x002C5C90 File Offset: 0x002C3E90
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

		// Token: 0x06003170 RID: 12656 RVA: 0x002C5D94 File Offset: 0x002C3F94
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

		// Token: 0x06003171 RID: 12657 RVA: 0x002C5EF4 File Offset: 0x002C40F4
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

		// Token: 0x06003172 RID: 12658 RVA: 0x002C5F60 File Offset: 0x002C4160
		public bool FlushUsedMoneyconState(GameClient client)
		{
			this.CheckLeiJiXiaoFei(client);
			this.CheckXinFuUseMoney(client);
			this.CheckSpecialActivity(client);
			this.CheckEverydayActivity(client);
			this.CheckInputFanLiNewActivity(client);
			return false;
		}

		// Token: 0x06003173 RID: 12659 RVA: 0x002C5F9C File Offset: 0x002C419C
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

		// Token: 0x06003174 RID: 12660 RVA: 0x002C606C File Offset: 0x002C426C
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

		// Token: 0x06003175 RID: 12661 RVA: 0x002C60D0 File Offset: 0x002C42D0
		public bool CheckFuLiLianXuDengLu(GameClient client)
		{
			bool bFulsh = this.CheckFuLiLianXuDengLuReward(client);
			return this.AddFlushIconState(3007, bFulsh);
		}

		// Token: 0x06003176 RID: 12662 RVA: 0x002C60F8 File Offset: 0x002C42F8
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

		// Token: 0x06003177 RID: 12663 RVA: 0x002C61E0 File Offset: 0x002C43E0
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

		// Token: 0x06003178 RID: 12664 RVA: 0x002C6230 File Offset: 0x002C4430
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

		// Token: 0x06003179 RID: 12665 RVA: 0x002C62C0 File Offset: 0x002C44C0
		public bool CheckFuLiLeiJiDengLu(GameClient client)
		{
			bool bFulsh = this.CheckFuLiLeiJiDengLuReward(client);
			return this.AddFlushIconState(3008, bFulsh);
		}

		// Token: 0x0600317A RID: 12666 RVA: 0x002C62E8 File Offset: 0x002C44E8
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

		// Token: 0x0600317B RID: 12667 RVA: 0x002C63A0 File Offset: 0x002C45A0
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

		// Token: 0x0600317C RID: 12668 RVA: 0x002C644C File Offset: 0x002C464C
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

		// Token: 0x0600317D RID: 12669 RVA: 0x002C658C File Offset: 0x002C478C
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

		// Token: 0x0600317E RID: 12670 RVA: 0x002C6600 File Offset: 0x002C4800
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

		// Token: 0x0600317F RID: 12671 RVA: 0x002C6674 File Offset: 0x002C4874
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

		// Token: 0x06003180 RID: 12672 RVA: 0x002C66B8 File Offset: 0x002C48B8
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

		// Token: 0x06003181 RID: 12673 RVA: 0x002C6720 File Offset: 0x002C4920
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

		// Token: 0x06003182 RID: 12674 RVA: 0x002C67A4 File Offset: 0x002C49A4
		public bool CheckOneDollarChongZhi(GameClient client)
		{
			bool hasGet;
			bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.OneDollarChongZhi, out hasGet);
			return this.AddFlushIconState(15051, ret);
		}

		// Token: 0x06003183 RID: 12675 RVA: 0x002C67D0 File Offset: 0x002C49D0
		public bool CheckInputFanLiNewActivity(GameClient client)
		{
			bool hasGet;
			bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.InputFanLiNew, out hasGet);
			return this.AddFlushIconState(15054, ret);
		}

		// Token: 0x06003184 RID: 12676 RVA: 0x002C67FC File Offset: 0x002C49FC
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

		// Token: 0x06003185 RID: 12677 RVA: 0x002C6874 File Offset: 0x002C4A74
		public bool CheckLeiJiChongZhi(GameClient client)
		{
			bool hasGet;
			bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.TotalCharge, out hasGet);
			return this.AddFlushIconState(3004, ret);
		}

		// Token: 0x06003186 RID: 12678 RVA: 0x002C68A0 File Offset: 0x002C4AA0
		public bool CheckLeiJiXiaoFei(GameClient client)
		{
			bool hasGet;
			bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.TotalConsume, out hasGet);
			return this.AddFlushIconState(3005, ret);
		}

		// Token: 0x06003187 RID: 12679 RVA: 0x002C68CC File Offset: 0x002C4ACC
		public bool CheckMainXinFuIcon(GameClient client)
		{
			return false;
		}

		// Token: 0x06003188 RID: 12680 RVA: 0x002C68E0 File Offset: 0x002C4AE0
		public bool CheckXinFuKillBoss(GameClient client)
		{
			return false;
		}

		// Token: 0x06003189 RID: 12681 RVA: 0x002C68F4 File Offset: 0x002C4AF4
		public bool CheckXinFuChongZhiMoney(GameClient client)
		{
			return false;
		}

		// Token: 0x0600318A RID: 12682 RVA: 0x002C6908 File Offset: 0x002C4B08
		public bool CheckXinFuUseMoney(GameClient client)
		{
			return false;
		}

		// Token: 0x0600318B RID: 12683 RVA: 0x002C691C File Offset: 0x002C4B1C
		public bool CheckXinFuFreeGetMoney(GameClient client)
		{
			return false;
		}

		// Token: 0x0600318C RID: 12684 RVA: 0x002C6930 File Offset: 0x002C4B30
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

		// Token: 0x0600318D RID: 12685 RVA: 0x002C697C File Offset: 0x002C4B7C
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

		// Token: 0x0600318E RID: 12686 RVA: 0x002C69BC File Offset: 0x002C4BBC
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

		// Token: 0x0600318F RID: 12687 RVA: 0x002C69FC File Offset: 0x002C4BFC
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

		// Token: 0x06003190 RID: 12688 RVA: 0x002C6A3C File Offset: 0x002C4C3C
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

		// Token: 0x06003191 RID: 12689 RVA: 0x002C6A80 File Offset: 0x002C4C80
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

		// Token: 0x06003192 RID: 12690 RVA: 0x002C6AC0 File Offset: 0x002C4CC0
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

		// Token: 0x06003193 RID: 12691 RVA: 0x002C6AFC File Offset: 0x002C4CFC
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

		// Token: 0x06003194 RID: 12692 RVA: 0x002C6B90 File Offset: 0x002C4D90
		public bool CheckPaiHangState(GameClient client)
		{
			return this.AddFlushIconState(7500, Global.GetAdmireCount(client) < 10);
		}

		// Token: 0x06003195 RID: 12693 RVA: 0x002C6BB8 File Offset: 0x002C4DB8
		public bool CheckChengJiuUpLevelState(GameClient client)
		{
			bool result = this.AddFlushIconState(9000, ChengJiuManager.CanActiveNextChengHao(client));
			this.SendIconStateToClient(client);
			return result;
		}

		// Token: 0x06003196 RID: 12694 RVA: 0x002C6BE8 File Offset: 0x002C4DE8
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

		// Token: 0x06003197 RID: 12695 RVA: 0x002C6C58 File Offset: 0x002C4E58
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

		// Token: 0x06003198 RID: 12696 RVA: 0x002C6CE4 File Offset: 0x002C4EE4
		public bool CheckBuChangState(GameClient client)
		{
			bool bFlush = BuChangManager.CheckGiveBuChang(client);
			return this.AddFlushIconState(11000, bFlush);
		}

		// Token: 0x06003199 RID: 12697 RVA: 0x002C6D0C File Offset: 0x002C4F0C
		public bool CheckRebornUpgrade(GameClient client)
		{
			return this.AddFlushIconState(21001, RebornManager.getInstance().CheckRebornUpgradeIcon(client));
		}

		// Token: 0x0600319A RID: 12698 RVA: 0x002C6D34 File Offset: 0x002C4F34
		public bool CheckReborn(GameClient client)
		{
			this.AddFlushIconState(21001, false);
			bool bFlush = false;
			return this.CheckRebornUpgrade(client) || bFlush;
		}

		// Token: 0x0600319B RID: 12699 RVA: 0x002C6D60 File Offset: 0x002C4F60
		public bool CheckThemeActivity(GameClient client)
		{
			this.AddFlushIconState(11501, false);
			this.AddFlushIconState(11502, false);
			bool bFlush = false;
			bFlush = (this.CheckThemeZhiGou(client) || bFlush);
			return this.CheckThemeDaLiBao(client) || bFlush;
		}

		// Token: 0x0600319C RID: 12700 RVA: 0x002C6DA4 File Offset: 0x002C4FA4
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

		// Token: 0x0600319D RID: 12701 RVA: 0x002C6DF0 File Offset: 0x002C4FF0
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

		// Token: 0x0600319E RID: 12702 RVA: 0x002C6E3C File Offset: 0x002C503C
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

		// Token: 0x0600319F RID: 12703 RVA: 0x002C6F0C File Offset: 0x002C510C
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

		// Token: 0x060031A0 RID: 12704 RVA: 0x002C6FA0 File Offset: 0x002C51A0
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

		// Token: 0x060031A1 RID: 12705 RVA: 0x002C7068 File Offset: 0x002C5268
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

		// Token: 0x060031A2 RID: 12706 RVA: 0x002C7124 File Offset: 0x002C5324
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

		// Token: 0x060031A3 RID: 12707 RVA: 0x002C72AC File Offset: 0x002C54AC
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

		// Token: 0x060031A4 RID: 12708 RVA: 0x002C7338 File Offset: 0x002C5538
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

		// Token: 0x060031A5 RID: 12709 RVA: 0x002C7590 File Offset: 0x002C5790
		public bool CheckCaiJiState(GameClient client)
		{
			return this.AddFlushIconState(13000, CaiJiLogic.HasLeftnum(client));
		}

		// Token: 0x060031A6 RID: 12710 RVA: 0x002C75B4 File Offset: 0x002C57B4
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

		// Token: 0x060031A7 RID: 12711 RVA: 0x002C75F4 File Offset: 0x002C57F4
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

		// Token: 0x060031A8 RID: 12712 RVA: 0x002C7634 File Offset: 0x002C5834
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

		// Token: 0x060031A9 RID: 12713 RVA: 0x002C7674 File Offset: 0x002C5874
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

		// Token: 0x060031AA RID: 12714 RVA: 0x002C795C File Offset: 0x002C5B5C
		public bool IsAnyJieRiTipActived()
		{
			return this.IsAnyTipActived(IconStateManager.m_jieRiIconList);
		}

		// Token: 0x060031AB RID: 12715 RVA: 0x002C797C File Offset: 0x002C5B7C
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

		// Token: 0x060031AC RID: 12716 RVA: 0x002C7A50 File Offset: 0x002C5C50
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

		// Token: 0x060031AD RID: 12717 RVA: 0x002C7B2C File Offset: 0x002C5D2C
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

		// Token: 0x060031AE RID: 12718 RVA: 0x002C7C4C File Offset: 0x002C5E4C
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

		// Token: 0x060031AF RID: 12719 RVA: 0x002C7DA8 File Offset: 0x002C5FA8
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

		// Token: 0x060031B0 RID: 12720 RVA: 0x002C7F04 File Offset: 0x002C6104
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

		// Token: 0x060031B1 RID: 12721 RVA: 0x002C8060 File Offset: 0x002C6260
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

		// Token: 0x060031B2 RID: 12722 RVA: 0x002C822C File Offset: 0x002C642C
		public bool CheckJieRiHongBaoBang(GameClient client)
		{
			JieriHongBaoKingActivity activity = JieriHongBaoKingActivity.getInstance();
			bool bFlush = activity.CanGetAnyAward(client);
			return this.AddFlushIconState(14032, bFlush);
		}

		// Token: 0x060031B3 RID: 12723 RVA: 0x002C8258 File Offset: 0x002C6458
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

		// Token: 0x060031B4 RID: 12724 RVA: 0x002C8294 File Offset: 0x002C6494
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

		// Token: 0x060031B5 RID: 12725 RVA: 0x002C8388 File Offset: 0x002C6588
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

		// Token: 0x060031B6 RID: 12726 RVA: 0x002C847C File Offset: 0x002C667C
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

		// Token: 0x060031B7 RID: 12727 RVA: 0x002C84C0 File Offset: 0x002C66C0
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

		// Token: 0x060031B8 RID: 12728 RVA: 0x002C8504 File Offset: 0x002C6704
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

		// Token: 0x060031B9 RID: 12729 RVA: 0x002C8548 File Offset: 0x002C6748
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

		// Token: 0x060031BA RID: 12730 RVA: 0x002C858C File Offset: 0x002C678C
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

		// Token: 0x060031BB RID: 12731 RVA: 0x002C85D0 File Offset: 0x002C67D0
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

		// Token: 0x060031BC RID: 12732 RVA: 0x002C8614 File Offset: 0x002C6814
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

		// Token: 0x060031BD RID: 12733 RVA: 0x002C8658 File Offset: 0x002C6858
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

		// Token: 0x060031BE RID: 12734 RVA: 0x002C86B8 File Offset: 0x002C68B8
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

		// Token: 0x060031BF RID: 12735 RVA: 0x002C8790 File Offset: 0x002C6990
		public bool CheckPetIcon(GameClient client)
		{
			this.AddFlushIconState(16000, false);
			this.AddFlushIconState(16001, false);
			bool bFlush = false;
			bFlush |= this.CheckPetBagIcon(client);
			return bFlush | this.CheckCallPetIcon(client);
		}

		// Token: 0x060031C0 RID: 12736 RVA: 0x002C87D4 File Offset: 0x002C69D4
		public bool CheckBuildingFreeQueue(GameClient client)
		{
			BuildingManager BuildingMgr = BuildingManager.getInstance();
			int free = 0;
			int pay = 0;
			BuildingMgr.GetTaskNumInEachTeam(client, out free, out pay);
			return free < 4;
		}

		// Token: 0x060031C1 RID: 12737 RVA: 0x002C8804 File Offset: 0x002C6A04
		public bool CheckBuildingAward(GameClient client)
		{
			bool bFlush = false;
			BuildingManager BuildingMgr = BuildingManager.getInstance();
			bFlush |= BuildingMgr.CheckCanGetAnyAllLevelAward(client);
			return bFlush | BuildingMgr.CheckAnyTaskFinish(client);
		}

		// Token: 0x060031C2 RID: 12738 RVA: 0x002C8834 File Offset: 0x002C6A34
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

		// Token: 0x060031C3 RID: 12739 RVA: 0x002C8880 File Offset: 0x002C6A80
		public bool CheckJunTuanEraIcon(GameClient client)
		{
			return this.AddFlushIconState(15053, EraManager.getInstance().CheckJunTuanEraIcon(client));
		}

		// Token: 0x060031C4 RID: 12740 RVA: 0x002C88A8 File Offset: 0x002C6AA8
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

		// Token: 0x060031C5 RID: 12741 RVA: 0x002C88EC File Offset: 0x002C6AEC
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

		// Token: 0x060031C6 RID: 12742 RVA: 0x002C893C File Offset: 0x002C6B3C
		public bool CheckPetBagIcon(GameClient client)
		{
			return false;
		}

		// Token: 0x060031C7 RID: 12743 RVA: 0x002C8950 File Offset: 0x002C6B50
		public bool CheckCallPetIcon(GameClient client)
		{
			bool bFlush = CallPetManager.getFreeSec(client) <= 0L;
			return this.AddFlushIconState(16001, bFlush);
		}

		// Token: 0x060031C8 RID: 12744 RVA: 0x002C897C File Offset: 0x002C6B7C
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

		// Token: 0x060031C9 RID: 12745 RVA: 0x002C8A6C File Offset: 0x002C6C6C
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

		// Token: 0x060031CA RID: 12746 RVA: 0x002C8C78 File Offset: 0x002C6E78
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

		// Token: 0x060031CB RID: 12747 RVA: 0x002C8D0C File Offset: 0x002C6F0C
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

		// Token: 0x04003E53 RID: 15955
		private Dictionary<ushort, ushort> m_StateIconsDict = new Dictionary<ushort, ushort>();

		// Token: 0x04003E54 RID: 15956
		private Dictionary<ushort, ushort> m_StateCacheIconsDict = new Dictionary<ushort, ushort>();

		// Token: 0x04003E55 RID: 15957
		private ActivityIconStateData m_ActivityIconStateData = new ActivityIconStateData();

		// Token: 0x04003E56 RID: 15958
		private long m_LastTicks = 0L;

		// Token: 0x04003E57 RID: 15959
		private long m_LastTicksBuilding = 0L;

		// Token: 0x04003E58 RID: 15960
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
