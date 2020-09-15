using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020007EF RID: 2031
	public class ArenaBattleManager
	{
		// Token: 0x06003998 RID: 14744 RVA: 0x0030C3B8 File Offset: 0x0030A5B8
		public int GetBattlingState()
		{
			return (int)this.BattlingState;
		}

		// Token: 0x06003999 RID: 14745 RVA: 0x0030C3D0 File Offset: 0x0030A5D0
		public int GetBattlingLeftSecs()
		{
			long ticks = TimeUtil.NOW();
			int paramSecs = 0;
			if (this.BattlingState == BattleStates.PublishMsg)
			{
				paramSecs = this.WaitingEnterSecs;
			}
			else if (this.BattlingState == BattleStates.WaitingFight)
			{
				paramSecs = this.PrepareSecs;
			}
			else if (this.BattlingState == BattleStates.StartFight)
			{
				paramSecs = this.FightingSecs;
			}
			else if (this.BattlingState == BattleStates.EndFight)
			{
				paramSecs = this.ClearRolesSecs;
			}
			else if (this.BattlingState == BattleStates.ClearBattle)
			{
				paramSecs = this.ClearRolesSecs;
			}
			return (int)(((long)(paramSecs * 1000) - (ticks - this.StateStartTicks)) / 1000L);
		}

		// Token: 0x0600399A RID: 14746 RVA: 0x0030C488 File Offset: 0x0030A688
		public void LoadParams()
		{
			SystemXmlItem systemBattle = null;
			if (GameManager.SystemArenaBattle.SystemXmlItemDict.TryGetValue(1, out systemBattle))
			{
				List<string> timePointsList = new List<string>();
				string timePoints = systemBattle.GetStringValue("TimePoints");
				if (timePoints != null && timePoints != "")
				{
					string[] fields = timePoints.Split(new char[]
					{
						','
					});
					for (int i = 0; i < fields.Length; i++)
					{
						timePointsList.Add(fields[i].Trim());
					}
				}
				this.TimePointsList = timePointsList;
				this.MapCode = systemBattle.GetIntValue("MapCode", -1);
				this.MinChangeLifeLev = systemBattle.GetIntValue("MinZhuanSheng", -1);
				this.MinLevel = systemBattle.GetIntValue("MinLevel", -1);
				this.MinRequestNum = systemBattle.GetIntValue("MinRequestNum", -1);
				this.MaxEnterNum = systemBattle.GetIntValue("MaxEnterNum", -1);
				this.FallGiftNum = systemBattle.GetIntValue("FallGiftNum", -1);
				this.FallID = systemBattle.GetIntValue("FallID", -1);
				this.DisableGoodsIDs = systemBattle.GetStringValue("DisableGoodsIDs");
				this.AddExpSecs = systemBattle.GetIntValue("AddExpSecs", -1);
				this.ForceNotifyBattleScoreSec = Global.GMax(20, Global.GMin(100, systemBattle.GetIntValue("NotifyBattleKilledNumSecs", -1)));
				this.WaitingEnterSecs = systemBattle.GetIntValue("WaitingEnterSecs", -1);
				this.PrepareSecs = systemBattle.GetIntValue("PrepareSecs", -1);
				this.FightingSecs = systemBattle.GetIntValue("FightingSecs", -1);
				this.ClearRolesSecs = systemBattle.GetIntValue("ClearRolesSecs", -1);
				this.BattleLineID = Global.GMax(1, systemBattle.GetIntValue("LineID", -1));
				this.ReloadGiveAwardsGoodsDataList(systemBattle);
				ArenaBattleManager.m_nPushMsgDayID = Global.SafeConvertToInt32(GameManager.GameConfigMgr.GetGameConifgItem("PKKingPushMsgDayID"));
			}
		}

		// Token: 0x0600399B RID: 14747 RVA: 0x0030C674 File Offset: 0x0030A874
		public void ReloadGiveAwardsGoodsDataList(SystemXmlItem systemBattle = null)
		{
			if (null == systemBattle)
			{
				if (!GameManager.SystemArenaBattle.SystemXmlItemDict.TryGetValue(1, out systemBattle))
				{
					return;
				}
			}
			List<GoodsData> goodsDataList = new List<GoodsData>();
			string giveGoodsIDs = systemBattle.GetStringValue("GiveGoodsIDs").Trim();
			string[] fields = giveGoodsIDs.Split(new char[]
			{
				','
			});
			if (fields != null && fields.Length > 0)
			{
				for (int i = 0; i < fields.Length; i++)
				{
					if (!string.IsNullOrEmpty(fields[i].Trim()))
					{
						int goodsID = Convert.ToInt32(fields[i].Trim());
						SystemXmlItem systemGoods = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("PK之王配置文件中，配置的固定物品奖励中的物品不存在, GoodsID={0}", goodsID), null, true);
						}
						else
						{
							GoodsData goodsData = new GoodsData
							{
								Id = -1,
								GoodsID = goodsID,
								Using = 0,
								Forge_level = 0,
								Starttime = "1900-01-01 12:00:00",
								Endtime = "1900-01-01 12:00:00",
								Site = 0,
								Quality = 0,
								Props = "",
								GCount = 1,
								Binding = 0,
								Jewellist = "",
								BagIndex = 0,
								AddPropIndex = 0,
								BornIndex = 0,
								Lucky = 0,
								Strong = 0,
								ExcellenceInfo = 0,
								AppendPropLev = 0,
								ChangeLifeLevForEquip = 0
							};
							goodsDataList.Add(goodsData);
						}
					}
				}
			}
			this.GiveAwardsGoodsDataList = goodsDataList;
		}

		// Token: 0x0600399C RID: 14748 RVA: 0x0030C847 File Offset: 0x0030AA47
		public void Init()
		{
			this.LoadParams();
			this.AllowAttack = false;
			this.StartRoleNum = 0;
			this.TotalClientCount = 0;
			this._AllKilledRoleNum = 0;
			Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.TheKingOfPK);
		}

		// Token: 0x0600399D RID: 14749 RVA: 0x0030C878 File Offset: 0x0030AA78
		public void Process()
		{
			if (this.BattlingState > BattleStates.NoBattle)
			{
				this.ProcessBattling();
			}
			else
			{
				this.ProcessNoBattle();
			}
			this._HandleChangeNameEv();
		}

		// Token: 0x0600399E RID: 14750 RVA: 0x0030C8B4 File Offset: 0x0030AAB4
		private void ProcessBattling()
		{
			if (this.BattlingState == BattleStates.PublishMsg)
			{
				int nNow = TimeUtil.NowDateTime().DayOfYear;
				if (ArenaBattleManager.m_nPushMsgDayID != nNow)
				{
					Global.UpdateDBGameConfigg("PKKingPushMsgDayID", nNow.ToString());
					ArenaBattleManager.m_nPushMsgDayID = nNow;
				}
				long ticks = TimeUtil.NOW();
				if (ticks >= this.StateStartTicks + (long)(this.WaitingEnterSecs * 1000))
				{
					GameManager.ClientMgr.NotifyArenaBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 2, this.PrepareSecs);
					this.BattlingState = BattleStates.WaitingFight;
					this.StateStartTicks = TimeUtil.NOW();
				}
			}
			else if (this.BattlingState == BattleStates.WaitingFight)
			{
				long ticks = TimeUtil.NOW();
				if (ticks >= this.StateStartTicks + (long)(this.PrepareSecs * 1000))
				{
					this.AllowAttack = true;
					GameManager.ClientMgr.NotifyArenaBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 3, this.FightingSecs);
					this.StartRoleNum = GameManager.ClientMgr.GetMapClientsCount(this.MapCode);
					this.EnterBattleClientCount = this.StartRoleNum;
					this.AllKilledRoleNum = 0;
					this.BattlingState = BattleStates.StartFight;
					this.StateStartTicks = TimeUtil.NOW();
				}
			}
			else if (this.BattlingState == BattleStates.StartFight)
			{
				long ticks = TimeUtil.NOW();
				if (ticks >= this.StateStartTicks + (long)(this.FightingSecs * 1000))
				{
					this.AllowAttack = false;
					this.BattlingState = BattleStates.EndFight;
					this.StateStartTicks = TimeUtil.NOW();
				}
				else if (ticks >= this.StateStartTicks + 30000L)
				{
					this.ChampionClient = null;
					int roleNum = 0;
					List<GameClient> list = GameManager.ClientMgr.GetMapAliveClients(this.MapCode);
					if (null != list)
					{
						HashSet<int> hashset = new HashSet<int>();
						lock (this.DeadRoleSets)
						{
							foreach (GameClient c in list)
							{
								if (!hashset.Add(c.ClientData.RoleID))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("ArenaBattleManager::PK之王活动中角色{0}({1})对象重复", c.ClientData.RoleID, c.ClientData.RoleName), null, true);
								}
								else if (this.DeadRoleSets.Contains(c.ClientData.RoleID))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("ArenaBattleManager::PK之王活动中角色{0}({1})已经死过", c.ClientData.RoleID, c.ClientData.RoleName), null, true);
								}
								else if (Global.InOnlyObsByXY(ObjectTypes.OT_CLIENT, this.MapCode, c.ClientData.PosX, c.ClientData.PosY))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("ArenaBattleManager::PK之王活动中角色{0}({1})卡障碍,重置位置", c.ClientData.RoleID, c.ClientData.RoleName), null, true);
									GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, -1, -1, -1);
									roleNum++;
								}
								else
								{
									this.ChampionClient = c;
									roleNum++;
								}
							}
						}
					}
					if (roleNum == 0 || (roleNum == 1 && this.ChampionClient != null))
					{
						this.AllowAttack = false;
						this.BattlingState = BattleStates.EndFight;
						this.StateStartTicks = TimeUtil.NOW();
					}
					else
					{
						this.ChampionClient = null;
					}
				}
			}
			else if (this.BattlingState == BattleStates.EndFight)
			{
				this.BattlingState = BattleStates.ClearBattle;
				this.StateStartTicks = TimeUtil.NOW();
				GameManager.ClientMgr.NotifyArenaBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 5, this.ClearRolesSecs);
				this.ProcessBattleResultAwards();
			}
			else if (this.BattlingState == BattleStates.ClearBattle)
			{
				long ticks = TimeUtil.NOW();
				if (ticks >= this.StateStartTicks + (long)(this.ClearRolesSecs * 1000))
				{
					GameManager.ClientMgr.NotifyBattleLeaveMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode);
					this.BattlingState = BattleStates.NoBattle;
					this.StateStartTicks = 0L;
					this.TheKingOfPKGetawardFlag.Clear();
				}
			}
			this.ProcessTimeNotifyBattleKilledNum();
		}

		// Token: 0x0600399F RID: 14751 RVA: 0x0030CDD8 File Offset: 0x0030AFD8
		private void ProcessNoBattle()
		{
			if (this.JugeStartBattle())
			{
				this.StartRoleNum = 0;
				this.TotalClientCount = 0;
				this._AllKilledRoleNum = 0;
				this._LastNotifyClientCount = 0;
				this._bRoleEnterOrLeave = false;
				this.LastNotifyBattleScoreTicks = 0L;
				lock (this.DeadRoleSets)
				{
					this.DeadRoleSets.Clear();
				}
				GameManager.ClientMgr.BattleBeginForceLeaveg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode);
				this.BattlingState = BattleStates.PublishMsg;
				GameManager.ClientMgr.NotifyAllArenaBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MinLevel, 1, 1, this.WaitingEnterSecs);
				this.StateStartTicks = TimeUtil.NOW();
			}
		}

		// Token: 0x060039A0 RID: 14752 RVA: 0x0030CECC File Offset: 0x0030B0CC
		private bool JugeStartBattle()
		{
			string nowTime = TimeUtil.NowDateTime().ToString("HH:mm");
			List<string> timePointsList = this.TimePointsList;
			bool result;
			if (null == timePointsList)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < timePointsList.Count; i++)
				{
					if (timePointsList[i] == nowTime)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060039A1 RID: 14753 RVA: 0x0030CF40 File Offset: 0x0030B140
		public int LeftEnterCount()
		{
			int count = 0;
			string nowTime = TimeUtil.NowDateTime().ToString("HH:mm");
			List<string> timePointsList = this.TimePointsList;
			int result;
			if (null == timePointsList)
			{
				result = 0;
			}
			else
			{
				try
				{
					for (int i = 0; i < timePointsList.Count; i++)
					{
						DateTime tt = DateTime.Parse(timePointsList[i]);
						tt.AddMinutes((double)this.ClearRolesSecs);
						if (tt >= TimeUtil.NowDateTime())
						{
							count++;
						}
					}
				}
				catch (Exception e)
				{
					LogManager.WriteException(e.ToString());
				}
				result = count;
			}
			return result;
		}

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x060039A2 RID: 14754 RVA: 0x0030D000 File Offset: 0x0030B200
		// (set) Token: 0x060039A3 RID: 14755 RVA: 0x0030D018 File Offset: 0x0030B218
		public int TheKingOfPKTopPoint
		{
			get
			{
				return this.TopPoint;
			}
			set
			{
				this.TopPoint = value;
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x060039A4 RID: 14756 RVA: 0x0030D024 File Offset: 0x0030B224
		// (set) Token: 0x060039A5 RID: 14757 RVA: 0x0030D03C File Offset: 0x0030B23C
		public string TheKingOfPKTopRoleName
		{
			get
			{
				return this.TopRoleName;
			}
			set
			{
				this.TopRoleName = value;
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x060039A6 RID: 14758 RVA: 0x0030D048 File Offset: 0x0030B248
		// (set) Token: 0x060039A7 RID: 14759 RVA: 0x0030D060 File Offset: 0x0030B260
		public Dictionary<int, int> TheKingOfPKGetawardFlag
		{
			get
			{
				return this.GetawardFlag;
			}
			set
			{
				this.GetawardFlag = value;
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x060039A8 RID: 14760 RVA: 0x0030D06C File Offset: 0x0030B26C
		public int BattleMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x060039A9 RID: 14761 RVA: 0x0030D084 File Offset: 0x0030B284
		public int BattleServerLineID
		{
			get
			{
				return this.BattleLineID;
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x060039AA RID: 14762 RVA: 0x0030D09C File Offset: 0x0030B29C
		public bool AllowEnterMap
		{
			get
			{
				return this.BattlingState >= BattleStates.PublishMsg && this.BattlingState < BattleStates.StartFight;
			}
		}

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x060039AB RID: 14763 RVA: 0x0030D0C4 File Offset: 0x0030B2C4
		public bool IsFighting
		{
			get
			{
				return this.BattlingState >= BattleStates.StartFight && this.BattlingState < BattleStates.ClearBattle;
			}
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x060039AC RID: 14764 RVA: 0x0030D0EC File Offset: 0x0030B2EC
		// (set) Token: 0x060039AD RID: 14765 RVA: 0x0030D138 File Offset: 0x0030B338
		public bool AllowAttack
		{
			get
			{
				bool allowAttack;
				lock (this.mutex)
				{
					allowAttack = this._AllowAttack;
				}
				return allowAttack;
			}
			set
			{
				lock (this.mutex)
				{
					this._AllowAttack = value;
				}
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x060039AE RID: 14766 RVA: 0x0030D184 File Offset: 0x0030B384
		public int AllowMinChangeLifeLev
		{
			get
			{
				return this.MinChangeLifeLev;
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x060039AF RID: 14767 RVA: 0x0030D19C File Offset: 0x0030B39C
		public int AllowMinLevel
		{
			get
			{
				return this.MinLevel;
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x060039B0 RID: 14768 RVA: 0x0030D1B4 File Offset: 0x0030B3B4
		// (set) Token: 0x060039B1 RID: 14769 RVA: 0x0030D200 File Offset: 0x0030B400
		public int TotalClientCount
		{
			get
			{
				int totalClientCount;
				lock (this.mutex)
				{
					totalClientCount = this._TotalClientCount;
				}
				return totalClientCount;
			}
			set
			{
				lock (this.mutex)
				{
					this._TotalClientCount = value;
				}
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x060039B2 RID: 14770 RVA: 0x0030D24C File Offset: 0x0030B44C
		// (set) Token: 0x060039B3 RID: 14771 RVA: 0x0030D298 File Offset: 0x0030B498
		public int EnterBattleClientCount
		{
			get
			{
				int enterBattleClientCount;
				lock (this.mutex)
				{
					enterBattleClientCount = this._EnterBattleClientCount;
				}
				return enterBattleClientCount;
			}
			set
			{
				lock (this.mutex)
				{
					this._EnterBattleClientCount = value;
				}
			}
		}

		// Token: 0x060039B4 RID: 14772 RVA: 0x0030D2E4 File Offset: 0x0030B4E4
		public bool ClientEnter(GameClient client)
		{
			bool ret = false;
			lock (this.mutex)
			{
				if (this.TheKingOfPKGetawardFlag.ContainsKey(client.ClientData.RoleID))
				{
					return true;
				}
				if (this._TotalClientCount < this.MaxEnterNum)
				{
					this._TotalClientCount++;
					this.TheKingOfPKGetawardFlag.Add(client.ClientData.RoleID, 0);
					ret = true;
				}
			}
			if (ret)
			{
				this._bRoleEnterOrLeave = true;
			}
			return ret;
		}

		// Token: 0x060039B5 RID: 14773 RVA: 0x0030D3AC File Offset: 0x0030B5AC
		protected void ClientLeave(GameClient client)
		{
			lock (this.mutex)
			{
				this._TotalClientCount--;
				this.TheKingOfPKGetawardFlag.Remove(client.ClientData.RoleID);
			}
			this._bRoleEnterOrLeave = true;
		}

		// Token: 0x060039B6 RID: 14774 RVA: 0x0030D420 File Offset: 0x0030B620
		public void LeaveArenaBattleMap(GameClient client)
		{
			if (client.ClientData.MapCode == this.MapCode)
			{
				this.ProcessAward(client);
				this.ClientLeave(client);
			}
		}

		// Token: 0x060039B7 RID: 14775 RVA: 0x0030D458 File Offset: 0x0030B658
		public bool EnterArenaBattleMap(GameClient client)
		{
			return client.ClientData.MapCode == this.MapCode && this.ClientEnter(client);
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x060039B8 RID: 14776 RVA: 0x0030D490 File Offset: 0x0030B690
		public string BattleDisableGoodsIDs
		{
			get
			{
				return this.DisableGoodsIDs;
			}
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x060039B9 RID: 14777 RVA: 0x0030D4A8 File Offset: 0x0030B6A8
		// (set) Token: 0x060039BA RID: 14778 RVA: 0x0030D4F4 File Offset: 0x0030B6F4
		public int StartRoleNum
		{
			get
			{
				int startRoleNum;
				lock (this.mutex)
				{
					startRoleNum = this._StartRoleNum;
				}
				return startRoleNum;
			}
			set
			{
				lock (this.mutex)
				{
					this._StartRoleNum = value;
				}
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x060039BB RID: 14779 RVA: 0x0030D540 File Offset: 0x0030B740
		// (set) Token: 0x060039BC RID: 14780 RVA: 0x0030D58C File Offset: 0x0030B78C
		public int AllKilledRoleNum
		{
			get
			{
				int allKilledRoleNum;
				lock (this.mutex)
				{
					allKilledRoleNum = this._AllKilledRoleNum;
				}
				return allKilledRoleNum;
			}
			set
			{
				lock (this.mutex)
				{
					this._AllKilledRoleNum = value;
				}
			}
		}

		// Token: 0x060039BD RID: 14781 RVA: 0x0030D5D8 File Offset: 0x0030B7D8
		public void SetTotalPointInfo(string sName, int nValue)
		{
			this.TheKingOfPKTopRoleName = sName;
			this.TheKingOfPKTopPoint = nValue;
		}

		// Token: 0x060039BE RID: 14782 RVA: 0x0030D5EC File Offset: 0x0030B7EC
		public bool IsInPkScene(int nMap)
		{
			return nMap == 10000;
		}

		// Token: 0x060039BF RID: 14783 RVA: 0x0030D614 File Offset: 0x0030B814
		public bool ProcessRoleDead(GameClient other)
		{
			int roleID = other.ClientData.RoleID;
			bool firstKill = false;
			lock (this.DeadRoleSets)
			{
				if (!this.DeadRoleSets.Contains(roleID))
				{
					this.DeadRoleSets.Add(roleID);
					this._AllKilledRoleNum++;
					firstKill = true;
				}
			}
			if (firstKill)
			{
			}
			return firstKill;
		}

		// Token: 0x060039C0 RID: 14784 RVA: 0x0030D6B0 File Offset: 0x0030B8B0
		private void ProcessBattleResultAwards()
		{
			GameClient championClient = this.ChampionClient;
			if (null == championClient)
			{
				Global.BroadcastArenaChampionMsg(false, null);
				this.RestorePKingNpc(this.GetPKKingRoleID());
				Global.UpdateDBGameConfigg("PKKingRole", "0");
			}
			else
			{
				this.ProcessAward(championClient);
				Global.BroadcastArenaChampionMsg(true, championClient);
				this.ClearDbKingNpc();
				this.AddBattleBufferAndFlags(championClient);
				this.SetPKKingRoleID(championClient.ClientData.RoleID);
				this.ReplacePKKingNpc(championClient.ClientData.RoleID);
			}
		}

		// Token: 0x060039C1 RID: 14785 RVA: 0x0030D740 File Offset: 0x0030B940
		private void AddBattleBufferAndFlags(GameClient client)
		{
			double[] actionParams = new double[]
			{
				85200.0,
				2000800.0
			};
			client.ClientData.BattleNameStart = TimeUtil.NOW();
			client.ClientData.BattleNameIndex = 1;
			Global.RemoveBufferData(client, 24);
			Global.RemoveBufferData(client, 26);
			Global.RemoveBufferData(client, 25);
			Global.UpdateBufferData(client, BufferItemTypes.PKKingBuffer, actionParams, 0, true);
			GameManager.DBCmdMgr.AddDBCmd(10059, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BattleNameStart, client.ClientData.BattleNameIndex), null, client.ServerId);
			GameManager.ClientMgr.NotifyRoleBattleNameInfo(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.UpdateBattleNum(client, 1, false);
			HuodongCachingMgr.UpdateHeFuPKKingRoleID(client.ClientData.RoleID);
			EventLogManager.AddTitleEvent(client, 1, (int)actionParams[0], "pkKing");
		}

		// Token: 0x060039C2 RID: 14786 RVA: 0x0030D854 File Offset: 0x0030BA54
		public int GetPKKingRoleID()
		{
			return GameManager.GameConfigMgr.GetGameConfigItemInt("PKKingRole", 0);
		}

		// Token: 0x060039C3 RID: 14787 RVA: 0x0030D876 File Offset: 0x0030BA76
		public void SetPKKingRoleID(int roleID)
		{
			Global.UpdateDBGameConfigg("PKKingRole", roleID.ToString());
			GameManager.GameConfigMgr.SetGameConfigItem("PKKingRole", roleID.ToString());
		}

		// Token: 0x060039C4 RID: 14788 RVA: 0x0030D8A4 File Offset: 0x0030BAA4
		private void ProcessTimeNotifyBattleKilledNum()
		{
			bool bNtf2Client = false;
			long ticks = TimeUtil.NOW();
			int nowClientCnt = GameManager.ClientMgr.GetMapClientsCount(this.MapCode);
			if (this._bRoleEnterOrLeave)
			{
				this._bRoleEnterOrLeave = false;
				bNtf2Client = true;
			}
			else if (ticks - this.LastNotifyBattleScoreTicks >= (long)(this.ForceNotifyBattleScoreSec * 1000))
			{
				bNtf2Client = true;
			}
			else if (this._LastNotifyClientCount != nowClientCnt)
			{
				bNtf2Client = true;
			}
			if (bNtf2Client)
			{
				this.LastNotifyBattleScoreTicks = ticks;
				this._LastNotifyClientCount = nowClientCnt;
				GameManager.ClientMgr.NotifyArenaBattleKilledNumCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.AllKilledRoleNum, this.StartRoleNum, this.TotalClientCount);
			}
		}

		// Token: 0x060039C5 RID: 14789 RVA: 0x0030D960 File Offset: 0x0030BB60
		private void ProcessTimeAddRoleExp()
		{
			if (this.BattlingState == BattleStates.StartFight)
			{
				long ticks = TimeUtil.NOW();
				if (ticks - this.LastAddBangZhanAwardsTicks >= 10000L)
				{
					this.LastAddBangZhanAwardsTicks = ticks;
					List<object> objsList = GameManager.ClientMgr.GetMapClients(this.MapCode);
					if (null != objsList)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient c = objsList[i] as GameClient;
							if (c != null)
							{
								BangZhanAwardsMgr.ProcessBangZhanAwards(c);
							}
						}
					}
				}
			}
		}

		// Token: 0x060039C6 RID: 14790 RVA: 0x0030DA08 File Offset: 0x0030BC08
		private void ProcessAward(GameClient client)
		{
			if (this.BattlingState >= BattleStates.StartFight)
			{
				if (this.BattlingState == BattleStates.StartFight)
				{
					long ticks = TimeUtil.NOW();
					if (ticks < this.StateStartTicks + 1000L)
					{
						return;
					}
				}
				lock (this.mutex)
				{
					int nFlag;
					if (!this.TheKingOfPKGetawardFlag.TryGetValue(client.ClientData.RoleID, out nFlag))
					{
						return;
					}
					if (nFlag > 0)
					{
						return;
					}
					this.TheKingOfPKGetawardFlag[client.ClientData.RoleID] = 1;
				}
				if (client.ClientData.KingOfPkCurrentPoint > this.TheKingOfPKTopPoint)
				{
					this.SetTotalPointInfo(client.ClientData.RoleName, client.ClientData.KingOfPkCurrentPoint);
				}
				string strPkAward = GameManager.systemParamsList.GetParamValueByName("PkAward");
				string[] strChengJiu = null;
				string[] strExp = null;
				if (!string.IsNullOrEmpty(strPkAward))
				{
					string[] strFild = strPkAward.Split(new char[]
					{
						'|'
					});
					string strInfo = strFild[0];
					strChengJiu = strInfo.Split(new char[]
					{
						','
					});
					strInfo = strFild[1];
					strExp = strInfo.Split(new char[]
					{
						','
					});
				}
				HeFuAwardTimesActivity activity = HuodongCachingMgr.GetHeFuAwardTimesActivity();
				JieRiMultAwardActivity jieriact = HuodongCachingMgr.GetJieRiMultAwardActivity();
				SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
				double actTimes = 0.0;
				if (activity != null && activity.InActivityTime() && (double)activity.activityTimes > 0.0)
				{
					actTimes += (double)activity.activityTimes;
				}
				if (null != jieriact)
				{
					JieRiMultConfig config = jieriact.GetConfig(3);
					if (null != config)
					{
						actTimes += config.GetMult();
					}
				}
				if (null != spAct)
				{
					actTimes += spAct.GetMult(SpecPActivityBuffType.SPABT_PKKing);
				}
				actTimes = Math.Max(1.0, actTimes);
				int nChengjiuPoint = Global.SafeConvertToInt32(strChengJiu[0]) + Global.GMin(Global.SafeConvertToInt32(strChengJiu[1]), client.ClientData.KingOfPkCurrentPoint) * Global.SafeConvertToInt32(strChengJiu[2]);
				nChengjiuPoint *= (int)actTimes;
				if (nChengjiuPoint > 0)
				{
					ChengJiuManager.AddChengJiuPoints(client, "角斗赛", nChengjiuPoint, true, true);
				}
				double nRate = Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
				long nExp = (long)((int)((double)Global.SafeConvertToInt32(strExp[0]) * nRate + (double)(Global.GMin(Global.SafeConvertToInt32(strExp[1]), client.ClientData.KingOfPkCurrentPoint) * Global.SafeConvertToInt32(strExp[2])) * nRate));
				double dblExperience = 1.0;
				dblExperience += actTimes;
				nExp = (long)((int)((double)nExp * dblExperience));
				if (nExp > 0L)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, nExp, true, true, false, "none");
				}
				string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.KingOfPkCurrentPoint, nChengjiuPoint, nExp);
				client.ClientData.KingOfPkCurrentPoint = 0;
				GameManager.ClientMgr.SendToClient(client, strCmd, 569);
				ProcessTask.ProcessAddTaskVal(client, TaskTypes.PKKing, -1, 1, new object[0]);
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x060039C7 RID: 14791 RVA: 0x0030DDBC File Offset: 0x0030BFBC
		// (set) Token: 0x060039C8 RID: 14792 RVA: 0x0030DE08 File Offset: 0x0030C008
		public RoleDataEx KingRoleData
		{
			get
			{
				RoleDataEx kingRoleData;
				lock (this.kingRoleDataMutex)
				{
					kingRoleData = this._kingRoleData;
				}
				return kingRoleData;
			}
			private set
			{
				lock (this.kingRoleDataMutex)
				{
					this._kingRoleData = value;
				}
			}
		}

		// Token: 0x060039C9 RID: 14793 RVA: 0x0030DE54 File Offset: 0x0030C054
		public void ReShowPKKing()
		{
			int roleID = this.GetPKKingRoleID();
			if (roleID > 0)
			{
				this.ReplacePKKingNpc(roleID);
			}
		}

		// Token: 0x060039CA RID: 14794 RVA: 0x0030DE7C File Offset: 0x0030C07C
		public void ClearDbKingNpc()
		{
			this.KingRoleData = null;
			Global.sendToDB<bool, string>(13232, string.Format("{0}", 1), 0);
		}

		// Token: 0x060039CB RID: 14795 RVA: 0x0030DEA4 File Offset: 0x0030C0A4
		public void ReplacePKKingNpc(int roleId)
		{
			RoleDataEx rd = this.KingRoleData;
			this.KingRoleData = null;
			if (rd == null || rd.RoleID != roleId)
			{
				rd = Global.sendToDB<RoleDataEx, KingRoleGetData>(13230, new KingRoleGetData
				{
					KingType = 1
				}, 0);
				if (rd == null || rd.RoleID != roleId)
				{
					RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, roleId), 0);
					if (dbRd == null || dbRd.RoleID <= 0)
					{
						return;
					}
					rd = dbRd;
					if (!Global.sendToDB<bool, KingRolePutData>(13231, new KingRolePutData
					{
						KingType = 1,
						RoleDataEx = rd
					}, 0))
					{
					}
				}
			}
			if (rd != null && rd.RoleID > 0)
			{
				this.KingRoleData = rd;
				NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.PkKing);
				if (null != npc)
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang, false);
					FakeRoleManager.ProcessNewFakeRole(new SafeClientData
					{
						RoleData = rd
					}, npc.MapCode, FakeRoleTypes.DiaoXiang, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, FakeRoleNpcId.PkKing);
				}
			}
		}

		// Token: 0x060039CC RID: 14796 RVA: 0x0030E034 File Offset: 0x0030C234
		public void RestorePKingNpc(int pkKingRoleID)
		{
			NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.PkKing);
			if (null != npc)
			{
				npc.ShowNpc = true;
				GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang, false);
			}
		}

		// Token: 0x060039CD RID: 14797 RVA: 0x0030E08C File Offset: 0x0030C28C
		public bool IsInArenaBattle(GameClient client)
		{
			return client.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode;
		}

		// Token: 0x060039CE RID: 14798 RVA: 0x0030E0C4 File Offset: 0x0030C2C4
		public void AddArenaBattleKilledNum(GameClient client, object victim)
		{
			if (client.ClientData.MapCode == this.BattleMapCode)
			{
				GameClient other = victim as GameClient;
				if (victim != null && null != other)
				{
					if (this.ProcessRoleDead(other))
					{
						client.ClientData.ArenaBattleKilledNum++;
						client.ClientData.KingOfPkCurrentPoint += client.ClientData.ArenaBattleKilledNum * 5;
						if (client.ClientData.KingOfPkCurrentPoint > client.ClientData.KingOfPkTopPoint)
						{
							client.ClientData.KingOfPkTopPoint = client.ClientData.KingOfPkCurrentPoint;
						}
					}
				}
			}
		}

		// Token: 0x060039CF RID: 14799 RVA: 0x0030E184 File Offset: 0x0030C384
		public void ClientEnterArenaBattle(GameClient client)
		{
			if (this.BattleMapCode < 0)
			{
				GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
			}
			else if (this.BattleServerLineID != GameManager.ServerLineID)
			{
				GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1000 - this.BattleServerLineID, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
			}
			else if (client.ClientData.ChangeLifeCount < this.AllowMinChangeLifeLev)
			{
				GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -11, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
			}
			else
			{
				if (client.ClientData.ChangeLifeCount == this.AllowMinChangeLifeLev)
				{
					if (client.ClientData.Level < this.AllowMinLevel)
					{
						GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -10, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
						return;
					}
				}
				if (!this.AllowEnterMap)
				{
					int errorCode = -2;
					if (this.IsFighting)
					{
						errorCode = -22;
					}
					GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, errorCode, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
				}
				else if (!this.ClientEnter(client))
				{
					GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -3, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
				}
				else
				{
					client.ClientData.ArenaBattleKilledNum = 0;
					client.ClientData.KingOfPkCurrentPoint = 0;
					int toMapCode = this.BattleMapCode;
					GameMap gameMap = null;
					if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
					{
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, -1, -1, -1, 0);
						GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 4, Global.GMax(2, this.GetBattlingState()), this.GetBattlingLeftSecs());
						Global.BroadcastClientEnterArenaBattle(client);
						Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, 4, 1);
					}
				}
			}
		}

		// Token: 0x060039D0 RID: 14800 RVA: 0x0030E438 File Offset: 0x0030C638
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				Tuple<int, string, string> ev = new Tuple<int, string, string>(roleId, oldName, newName);
				lock (this._ChangeNameEvQ)
				{
					this._ChangeNameEvQ.Enqueue(ev);
				}
			}
		}

		// Token: 0x060039D1 RID: 14801 RVA: 0x0030E4B0 File Offset: 0x0030C6B0
		private void _HandleChangeNameEv()
		{
			List<Tuple<int, string, string>> changeNameLst = null;
			lock (this._ChangeNameEvQ)
			{
				changeNameLst = this._ChangeNameEvQ.ToList<Tuple<int, string, string>>();
				this._ChangeNameEvQ.Clear();
			}
			if (changeNameLst != null && changeNameLst.Count != 0)
			{
				foreach (Tuple<int, string, string> ev in changeNameLst)
				{
					int roleId = ev.Item1;
					string oldName = ev.Item2;
					string newName = ev.Item3;
					RoleDataEx rd = this.KingRoleData;
					if (rd != null && rd.RoleID == roleId)
					{
						rd.RoleName = newName;
						if (!Global.sendToDB<bool, KingRolePutData>(13231, new KingRolePutData
						{
							KingType = 1,
							RoleDataEx = rd
						}, 0))
						{
						}
						this.KingRoleData = null;
						this.ReShowPKKing();
					}
					if (!string.IsNullOrEmpty(this.TheKingOfPKTopRoleName) && this.TheKingOfPKTopRoleName == oldName)
					{
						this.TheKingOfPKTopRoleName = newName;
					}
				}
			}
		}

		// Token: 0x04004341 RID: 17217
		private int TopPoint = -1;

		// Token: 0x04004342 RID: 17218
		private string TopRoleName = "";

		// Token: 0x04004343 RID: 17219
		private Dictionary<int, int> GetawardFlag = new Dictionary<int, int>();

		// Token: 0x04004344 RID: 17220
		private List<string> TimePointsList = new List<string>();

		// Token: 0x04004345 RID: 17221
		private int MapCode = -1;

		// Token: 0x04004346 RID: 17222
		private int MinChangeLifeLev = 0;

		// Token: 0x04004347 RID: 17223
		private int MinLevel = 20;

		// Token: 0x04004348 RID: 17224
		private int MinRequestNum = 100;

		// Token: 0x04004349 RID: 17225
		private int MaxEnterNum = 300;

		// Token: 0x0400434A RID: 17226
		private int FallGiftNum = 5;

		// Token: 0x0400434B RID: 17227
		private int FallID = -1;

		// Token: 0x0400434C RID: 17228
		private string DisableGoodsIDs = "";

		// Token: 0x0400434D RID: 17229
		private List<GoodsData> GiveAwardsGoodsDataList = null;

		// Token: 0x0400434E RID: 17230
		private int AddExpSecs = 60;

		// Token: 0x0400434F RID: 17231
		private int ForceNotifyBattleScoreSec = 10;

		// Token: 0x04004350 RID: 17232
		private int WaitingEnterSecs = 30;

		// Token: 0x04004351 RID: 17233
		private int PrepareSecs = 30;

		// Token: 0x04004352 RID: 17234
		private int FightingSecs = 300;

		// Token: 0x04004353 RID: 17235
		private int ClearRolesSecs = 30;

		// Token: 0x04004354 RID: 17236
		private int BattleLineID = 1;

		// Token: 0x04004355 RID: 17237
		public static int m_nPushMsgDayID = -1;

		// Token: 0x04004356 RID: 17238
		private Queue<Tuple<int, string, string>> _ChangeNameEvQ = new Queue<Tuple<int, string, string>>();

		// Token: 0x04004357 RID: 17239
		private BattleStates BattlingState = BattleStates.NoBattle;

		// Token: 0x04004358 RID: 17240
		private long StateStartTicks = 0L;

		// Token: 0x04004359 RID: 17241
		private long KeepSingleTicks;

		// Token: 0x0400435A RID: 17242
		private long LastNotifyBattleScoreTicks = 0L;

		// Token: 0x0400435B RID: 17243
		private HashSet<int> DeadRoleSets = new HashSet<int>();

		// Token: 0x0400435C RID: 17244
		private GameClient ChampionClient = null;

		// Token: 0x0400435D RID: 17245
		private object mutex = new object();

		// Token: 0x0400435E RID: 17246
		private bool _AllowAttack = false;

		// Token: 0x0400435F RID: 17247
		private int _TotalClientCount = 0;

		// Token: 0x04004360 RID: 17248
		private int _LastNotifyClientCount = 0;

		// Token: 0x04004361 RID: 17249
		private bool _bRoleEnterOrLeave = false;

		// Token: 0x04004362 RID: 17250
		private int _EnterBattleClientCount = 0;

		// Token: 0x04004363 RID: 17251
		private int _StartRoleNum = 0;

		// Token: 0x04004364 RID: 17252
		private int _AllKilledRoleNum = 0;

		// Token: 0x04004365 RID: 17253
		private long LastAddBangZhanAwardsTicks = 0L;

		// Token: 0x04004366 RID: 17254
		private object kingRoleDataMutex = new object();

		// Token: 0x04004367 RID: 17255
		private RoleDataEx _kingRoleData = null;
	}
}
