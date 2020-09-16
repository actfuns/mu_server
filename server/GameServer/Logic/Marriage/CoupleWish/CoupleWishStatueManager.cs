using System;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Marriage.CoupleWish
{
	
	internal class CoupleWishStatueManager
	{
		
		public void SetWishConfig(CoupleWishConfig config)
		{
			this._Config = config;
		}

		
		public bool LoadConfig()
		{
			bool result;
			try
			{
				string[] fields = GameManager.systemParamsList.GetParamValueByName("WishHunYanNPC").Split(new char[]
				{
					','
				});
				this.YanHuiMapCode = Convert.ToInt32(fields[0]);
				this.YanHuiNpcId = Convert.ToInt32(fields[1]);
				this.YanHuiNpcX = Convert.ToInt32(fields[2]);
				this.YanHuiNpcY = Convert.ToInt32(fields[3]);
				this.YanHuiNpcDir = Convert.ToInt32(fields[4]);
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				result = false;
			}
			return result;
		}

		
		public void SetDiaoXiang(CoupleWishSyncStatueData newStatue)
		{
			if (newStatue.DbCoupleId > 0 && (newStatue.ManRoleDataEx == null || newStatue.WifeRoleDataEx == null))
			{
				RoleData4Selector manRoleData4Selector = RoleManager.getInstance().GetMainOccupationRoleDataForSelector(newStatue.Man.RoleId, GameManager.ServerId);
				RoleData4Selector wifeRoleData4Selector = RoleManager.getInstance().GetMainOccupationRoleDataForSelector(newStatue.Wife.RoleId, GameManager.ServerId);
				if (manRoleData4Selector != null && wifeRoleData4Selector != null)
				{
					CoupleWishReportStatueData statueReq = new CoupleWishReportStatueData();
					statueReq.DbCoupleId = newStatue.DbCoupleId;
					statueReq.ManStatue = DataHelper.ObjectToBytes<RoleData4Selector>(manRoleData4Selector);
					statueReq.WifeStatue = DataHelper.ObjectToBytes<RoleData4Selector>(wifeRoleData4Selector);
					TianTiClient.getInstance().CoupleWishReportCoupleStatue(statueReq);
				}
			}
			if (newStatue.DbCoupleId > 0 && newStatue.ManRoleDataEx != null && newStatue.WifeRoleDataEx != null)
			{
				if (newStatue.IsDivorced == 1)
				{
					this.ReshowCoupleStatue(null, null);
				}
				else if (this._Statue == null || this._Statue.ManRoleDataEx == null || this._Statue.WifeRoleDataEx == null || this._Statue.DbCoupleId != newStatue.DbCoupleId)
				{
					this.ReshowCoupleStatue(DataHelper.BytesToObject<RoleData4Selector>(newStatue.ManRoleDataEx, 0, newStatue.ManRoleDataEx.Length), DataHelper.BytesToObject<RoleData4Selector>(newStatue.WifeRoleDataEx, 0, newStatue.WifeRoleDataEx.Length));
				}
			}
			else
			{
				this.ReshowCoupleStatue(null, null);
			}
			NPC npc = NPCGeneralManager.GetNPCFromConfig(this.YanHuiMapCode, this.YanHuiNpcId, this.YanHuiNpcX, this.YanHuiNpcY, this.YanHuiNpcDir);
			if (newStatue.DbCoupleId > 0 && npc != null && (this._Statue == null || this._Statue.DbCoupleId != newStatue.DbCoupleId) && newStatue.YanHuiJoinNum < this._Config.YanHuiCfg.TotalMaxJoinNum)
			{
				NPCGeneralManager.AddNpcToMap(npc);
			}
			if (newStatue.DbCoupleId <= 0 || newStatue.YanHuiJoinNum >= this._Config.YanHuiCfg.TotalMaxJoinNum)
			{
				NPCGeneralManager.RemoveMapNpc(this.YanHuiMapCode, this.YanHuiNpcId);
			}
			this._Statue = newStatue;
		}

		
		public CoupleWishYanHuiData HandleQueryParty(GameClient client)
		{
			CoupleWishYanHuiData data = new CoupleWishYanHuiData();
			if (this._Statue != null && this._Statue.Man != null && this._Statue.Wife != null)
			{
				data.Man = this._Statue.Man;
				data.Wife = this._Statue.Wife;
				data.TotalJoinNum = this._Statue.YanHuiJoinNum;
				data.DbCoupleId = this._Statue.DbCoupleId;
				data.MyJoinNum = this.GetJoinPartyNum(client, this._Statue.DbCoupleId);
			}
			return data;
		}

		
		public int HandleJoinParty(GameClient client, int toCouleId)
		{
			int result;
			if (this._Statue == null || this._Statue.DbCoupleId <= 0 || this._Statue.DbCoupleId != toCouleId)
			{
				result = -12;
			}
			else if (this.GetJoinPartyNum(client, toCouleId) >= this._Config.YanHuiCfg.EachRoleMaxJoinNum || this._Statue.YanHuiJoinNum >= this._Config.YanHuiCfg.TotalMaxJoinNum)
			{
				result = -16;
			}
			else if (Global.GetTotalBindTongQianAndTongQianVal(client) < this._Config.YanHuiCfg.CostBindJinBi)
			{
				result = -9;
			}
			else
			{
				int ec = TianTiClient.getInstance().CoupleWishJoinParty(client.ClientData.RoleID, client.ClientData.ZoneID, toCouleId);
				if (ec < 0)
				{
					result = ec;
				}
				else
				{
					Global.SubBindTongQianAndTongQian(client, this._Config.YanHuiCfg.CostBindJinBi, "情侣祝福宴会");
					this.AddJoinPartyNum(client, toCouleId, 1);
					this._Statue.YanHuiJoinNum++;
					if (this._Config.YanHuiCfg.GetExp > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)this._Config.YanHuiCfg.GetExp, false, true, false, "none");
						GameManager.ClientMgr.NotifyAddExpMsg(client, (long)this._Config.YanHuiCfg.GetExp);
					}
					if (this._Config.YanHuiCfg.GetXingHun > 0)
					{
						GameManager.ClientMgr.ModifyStarSoulValue(client, this._Config.YanHuiCfg.GetXingHun, "情侣祝福榜宴会", true, true);
					}
					if (this._Config.YanHuiCfg.GetShengWang > 0)
					{
						GameManager.ClientMgr.ModifyShengWangValue(client, this._Config.YanHuiCfg.GetShengWang, "情侣祝福榜宴会", true, true);
					}
					result = 1;
				}
			}
			return result;
		}

		
		private int GetJoinPartyNum(GameClient client, int toCoupleId)
		{
			int result;
			if (client == null)
			{
				result = 0;
			}
			else
			{
				string szTxt = Global.GetRoleParamByName(client, "31");
				string[] fields = (!string.IsNullOrEmpty(szTxt)) ? szTxt.Split(new char[]
				{
					','
				}) : null;
				if (fields != null && fields.Length == 2 && Convert.ToInt32(fields[0]) == toCoupleId)
				{
					result = Convert.ToInt32(fields[1]);
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		
		private void AddJoinPartyNum(GameClient client, int toCoupleId, int addNum = 1)
		{
			int totalNum = addNum + this.GetJoinPartyNum(client, toCoupleId);
			Global.SaveRoleParamsStringToDB(client, "31", string.Format("{0},{1}", toCoupleId, totalNum), true);
		}

		
		public CoupleWishTop1AdmireData HandleQueryAdmireData(GameClient client)
		{
			CoupleWishTop1AdmireData data = new CoupleWishTop1AdmireData();
			if (this._Statue != null && this._Statue.IsDivorced != 1 && this._Statue.DbCoupleId > 0 && this._Statue.ManRoleDataEx != null && this._Statue.WifeRoleDataEx != null)
			{
				data.DbCoupleId = this._Statue.DbCoupleId;
				data.ManSelector = DataHelper.BytesToObject<RoleData4Selector>(this._Statue.ManRoleDataEx, 0, this._Statue.ManRoleDataEx.Length);
				data.WifeSelector = DataHelper.BytesToObject<RoleData4Selector>(this._Statue.WifeRoleDataEx, 0, this._Statue.WifeRoleDataEx.Length);
				data.BeAdmireCount = this._Statue.BeAdmireCount;
			}
			data.MyAdmireCount = this.GetAdmireCount(client, TimeUtil.MakeYearMonthDay(TimeUtil.NowDateTime()));
			return data;
		}

		
		public int HandleAdmireStatue(GameClient client, int toCoupleId, int admireType)
		{
			int toDay = TimeUtil.MakeYearMonthDay(TimeUtil.NowDateTime());
			MoBaiData MoBaiConfig = null;
			int result;
			if (!Data.MoBaiDataInfoList.TryGetValue(2, out MoBaiConfig))
			{
				result = -3;
			}
			else if (client.ClientData.ChangeLifeCount < MoBaiConfig.MinZhuanSheng || (client.ClientData.ChangeLifeCount == MoBaiConfig.MinZhuanSheng && client.ClientData.Level < MoBaiConfig.MinLevel))
			{
				result = -19;
			}
			else
			{
				int maxAdmireNum = MoBaiConfig.AdrationMaxLimit;
				int hadAdmireCount = this.GetAdmireCount(client, toDay);
				if (this._Statue != null && this._Statue.IsDivorced != 1 && this._Statue.DbCoupleId > 0 && (client.ClientData.RoleID == this._Statue.Man.RoleId || client.ClientData.RoleID == this._Statue.Wife.RoleId))
				{
					maxAdmireNum += MoBaiConfig.ExtraNumber;
				}
				int nVIPLev = client.ClientData.VipLevel;
				int[] nArrayVIPAdded = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPMoBaiNum", ',');
				if (nVIPLev > VIPEumValue.VIPENUMVALUE_MAXLEVEL || nArrayVIPAdded.Length < 1)
				{
					result = -3;
				}
				else
				{
					maxAdmireNum += nArrayVIPAdded[nVIPLev];
					double awardmuti = 0.0;
					JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != activity)
					{
						JieRiMultConfig config = activity.GetConfig(12);
						if (null != config)
						{
							awardmuti += config.GetMult();
						}
					}
					SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != spAct)
					{
						awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_Admire);
					}
					awardmuti = Math.Max(1.0, awardmuti);
					maxAdmireNum = (int)((double)maxAdmireNum * awardmuti);
					if (hadAdmireCount >= maxAdmireNum)
					{
						result = -16;
					}
					else if (admireType == 1 && Global.GetTotalBindTongQianAndTongQianVal(client) < MoBaiConfig.NeedJinBi)
					{
						result = -9;
					}
					else if (admireType == 2 && client.ClientData.UserMoney < MoBaiConfig.NeedZuanShi)
					{
						result = -10;
					}
					else
					{
						int ec = TianTiClient.getInstance().CoupleWishAdmire(client.ClientData.RoleID, client.ClientData.ZoneID, admireType, toCoupleId);
						double nRate = (client.ClientData.ChangeLifeCount == 0) ? 1.0 : Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
						if (admireType == 1)
						{
							Global.SubBindTongQianAndTongQian(client, MoBaiConfig.NeedJinBi, "膜拜情侣祝福");
							long nExp = (long)(nRate * (double)MoBaiConfig.JinBiExpAward);
							if (nExp > 0L)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, nExp, true, true, false, "none");
							}
							if (MoBaiConfig.JinBiZhanGongAward > 0)
							{
								GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.JinBiZhanGongAward, AddBangGongTypes.CoupleWishMoBai, 0);
							}
							if (MoBaiConfig.LingJingAwardByJinBi > 0)
							{
								GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByJinBi, "膜拜情侣祝福", true, true, false);
							}
						}
						if (admireType == 2)
						{
							GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MoBaiConfig.NeedZuanShi, "膜拜情侣祝福", true, true, false, DaiBiSySType.None);
							int nExp2 = (int)(nRate * (double)MoBaiConfig.ZuanShiExpAward);
							if (nExp2 > 0)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, (long)nExp2, true, true, false, "none");
							}
							if (MoBaiConfig.ZuanShiZhanGongAward > 0)
							{
								GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.ZuanShiZhanGongAward, AddBangGongTypes.CoupleWishMoBai, 0);
							}
							if (MoBaiConfig.LingJingAwardByZuanShi > 0)
							{
								GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByZuanShi, "膜拜情侣祝福", true, true, false);
							}
						}
						this.AddAdmireCount(client, toDay, toCoupleId, 1);
						if (this._Statue != null && this._Statue.DbCoupleId > 0 && this._Statue.DbCoupleId == toCoupleId)
						{
							this._Statue.BeAdmireCount++;
						}
						result = 1;
					}
				}
			}
			return result;
		}

		
		private int GetAdmireCount(GameClient client, int toDay)
		{
			int result;
			if (client == null)
			{
				result = 0;
			}
			else
			{
				string szAdmire = Global.GetRoleParamByName(client, "30");
				string[] szAdmireFields = (!string.IsNullOrEmpty(szAdmire)) ? szAdmire.Split(new char[]
				{
					','
				}) : null;
				if (szAdmireFields != null && szAdmireFields.Length == 3 && Convert.ToInt32(szAdmireFields[0]) == toDay)
				{
					result = Convert.ToInt32(szAdmireFields[1]);
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		
		private void AddAdmireCount(GameClient client, int toDay, int toCoupleId, int addCount = 1)
		{
			if (client != null)
			{
				int totalCount = addCount + this.GetAdmireCount(client, toDay);
				Global.SaveRoleParamsStringToDB(client, "30", string.Format("{0},{1},{2}", toDay, totalCount, toCoupleId), true);
			}
		}

		
		private void ReshowCoupleStatue(RoleData4Selector manStatue, RoleData4Selector wifeStatue)
		{
			NPC manNpc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.CoupleWishMan);
			if (null != manNpc)
			{
				if (manStatue == null)
				{
					manNpc.ShowNpc = true;
					GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, manNpc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CoupleWishMan, true);
				}
				else
				{
					manNpc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, manNpc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CoupleWishMan, true);
					FakeRoleManager.ProcessNewFakeRole(manStatue, manNpc.MapCode, FakeRoleTypes.CoupleWishMan, (int)manNpc.CurrentDir, (int)manNpc.CurrentPos.X, (int)manNpc.CurrentPos.Y, FakeRoleNpcId.CoupleWishMan);
				}
			}
			NPC wifeNpc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.CoupleWishWife);
			if (null != wifeNpc)
			{
				if (wifeStatue == null)
				{
					wifeNpc.ShowNpc = true;
					GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, wifeNpc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CoupleWishWife, true);
				}
				else
				{
					wifeNpc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, wifeNpc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CoupleWishWife, true);
					FakeRoleManager.ProcessNewFakeRole(wifeStatue, wifeNpc.MapCode, FakeRoleTypes.CoupleWishWife, (int)wifeNpc.CurrentDir, (int)wifeNpc.CurrentPos.X, (int)wifeNpc.CurrentPos.Y, FakeRoleNpcId.CoupleWishWife);
				}
			}
		}

		
		private int YanHuiMapCode;

		
		private int YanHuiNpcId;

		
		private int YanHuiNpcX;

		
		private int YanHuiNpcY;

		
		private int YanHuiNpcDir;

		
		private CoupleWishSyncStatueData _Statue = null;

		
		private CoupleWishConfig _Config = null;
	}
}
