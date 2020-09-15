using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020005C6 RID: 1478
	public class BattleManager
	{
		// Token: 0x06001AD3 RID: 6867 RVA: 0x00198FA4 File Offset: 0x001971A4
		public int GetBattlingState()
		{
			return (int)this.BattlingState;
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x00198FBC File Offset: 0x001971BC
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

		// Token: 0x06001AD5 RID: 6869 RVA: 0x00199074 File Offset: 0x00197274
		public void LoadParams()
		{
			SystemXmlItem systemBattle = null;
			if (GameManager.SystemBattle.SystemXmlItemDict.TryGetValue(1, out systemBattle))
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
				this.MinLevel = systemBattle.GetIntValue("MinLevel", -1);
				this.MinRequestNum = systemBattle.GetIntValue("MinRequestNum", -1);
				this.MaxEnterNum = systemBattle.GetIntValue("MaxEnterNum", -1);
				this.FallGiftNum = systemBattle.GetIntValue("FallGiftNum", -1);
				this.FallID = systemBattle.GetIntValue("FallID", -1);
				this.DisableGoodsIDs = systemBattle.GetStringValue("DisableGoodsIDs");
				this.AddExpSecs = systemBattle.GetIntValue("AddExpSecs", -1);
				this.NotifyBattleKilledNumSecs = Global.GMax(5, Global.GMin(100, systemBattle.GetIntValue("NotifyBattleKilledNumSecs", -1)));
				this.WaitingEnterSecs = systemBattle.GetIntValue("WaitingEnterSecs", -1);
				this.PrepareSecs = systemBattle.GetIntValue("PrepareSecs", -1);
				this.FightingSecs = systemBattle.GetIntValue("FightingSecs", -1);
				this.ClearRolesSecs = systemBattle.GetIntValue("ClearRolesSecs", -1);
				this.m_NeedMinChangeLev = systemBattle.GetIntValue("MinZhuanSheng", -1);
				this.BattleLineID = Global.GMax(1, systemBattle.GetIntValue("LineID", -1));
				this.ReloadGiveAwardsGoodsDataList(systemBattle);
				Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.CampBattle);
				BattleManager.PushMsgDayID = Global.SafeConvertToInt32(GameManager.GameConfigMgr.GetGameConifgItem("BattlePushMsgDayID"));
			}
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x00199264 File Offset: 0x00197464
		public void ReloadGiveAwardsGoodsDataList(SystemXmlItem systemBattle = null)
		{
			if (null == systemBattle)
			{
				if (!GameManager.SystemBattle.SystemXmlItemDict.TryGetValue(1, out systemBattle))
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
							LogManager.WriteLog(LogTypes.Error, string.Format("角斗场配置文件中，配置的固定物品奖励中的物品不存在, GoodsID={0}", goodsID), null, true);
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

		// Token: 0x06001AD7 RID: 6871 RVA: 0x00199437 File Offset: 0x00197637
		public void Init()
		{
			this.LoadParams();
			this.AllowAttack = false;
			this.StartRoleNum = 0;
			this.TotalClientCount = 0;
			this.AllKilledRoleNum = 0;
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x00199464 File Offset: 0x00197664
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
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x00199498 File Offset: 0x00197698
		private void ProcessBattling()
		{
			if (this.BattlingState == BattleStates.PublishMsg)
			{
				int nNow = TimeUtil.NowDateTime().DayOfYear;
				if (BattleManager.PushMsgDayID != nNow)
				{
					Global.UpdateDBGameConfigg("BattlePushMsgDayID", nNow.ToString());
					BattleManager.PushMsgDayID = nNow;
				}
				long ticks = TimeUtil.NOW();
				if (ticks >= this.StateStartTicks + (long)(this.WaitingEnterSecs * 1000))
				{
					GameManager.ClientMgr.NotifyBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 2, this.PrepareSecs);
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
					GameManager.ClientMgr.NotifyBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 3, this.FightingSecs);
					this.StartRoleNum = GameManager.ClientMgr.GetMapClientsCount(this.MapCode);
					this.BattlingState = BattleStates.StartFight;
					this.StateStartTicks = TimeUtil.NOW();
					this.LastAddBattleExpTicks = this.StateStartTicks;
					this.LastNotifyBattleKilledNumTicks = this.StateStartTicks;
					this._SuiLastKillEmemyTime = TimeUtil.NOW() * 10000L;
					this._TangLastKillEmemyTime = this._SuiLastKillEmemyTime;
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
				else
				{
					this.ProcessTimeAddRoleExp();
					this.ProcessTimeNotifyBattleKilledNum();
				}
			}
			else if (this.BattlingState == BattleStates.EndFight)
			{
				this.BattlingState = BattleStates.ClearBattle;
				this.StateStartTicks = TimeUtil.NOW();
				GameManager.ClientMgr.NotifyBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 5, this.ClearRolesSecs);
				this.ProcessBattleResultAwards2();
			}
			else if (this.BattlingState == BattleStates.ClearBattle)
			{
				long ticks = TimeUtil.NOW();
				if (ticks >= this.StateStartTicks + (long)(this.ClearRolesSecs * 1000))
				{
					GameManager.ClientMgr.NotifyBattleLeaveMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode);
					this.BattlingState = BattleStates.NoBattle;
					this.StateStartTicks = 0L;
					this.ClearAllRoleLeaveInfo();
					this.ClearAllRolePointInfo();
				}
			}
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x00199764 File Offset: 0x00197964
		private void ProcessNoBattle()
		{
			if (this.JugeStartBattle())
			{
				this.StartRoleNum = 0;
				this.TotalClientCount = 0;
				this.AllKilledRoleNum = 0;
				this.SuiClientCount = 0;
				this.TangClientCount = 0;
				this.SuiKilledNum = 0;
				this.TangKilledNum = 0;
				this.BattlingState = BattleStates.PublishMsg;
				GameManager.ClientMgr.NotifyAllBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MinLevel, 1, 1, this.WaitingEnterSecs);
				this.StateStartTicks = TimeUtil.NOW();
			}
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x001997F8 File Offset: 0x001979F8
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

		// Token: 0x06001ADC RID: 6876 RVA: 0x0019986C File Offset: 0x00197A6C
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

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06001ADD RID: 6877 RVA: 0x0019992C File Offset: 0x00197B2C
		public object ExternalMutex
		{
			get
			{
				return this.mutex;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06001ADE RID: 6878 RVA: 0x00199944 File Offset: 0x00197B44
		public int BattleMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06001ADF RID: 6879 RVA: 0x0019995C File Offset: 0x00197B5C
		public int BattleServerLineID
		{
			get
			{
				return this.BattleLineID;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06001AE0 RID: 6880 RVA: 0x00199974 File Offset: 0x00197B74
		public bool AllowEnterMap
		{
			get
			{
				return this.BattlingState >= BattleStates.PublishMsg && this.BattlingState < BattleStates.EndFight;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06001AE1 RID: 6881 RVA: 0x0019999C File Offset: 0x00197B9C
		// (set) Token: 0x06001AE2 RID: 6882 RVA: 0x001999E8 File Offset: 0x00197BE8
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

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06001AE3 RID: 6883 RVA: 0x00199A34 File Offset: 0x00197C34
		public int AllowMinLevel
		{
			get
			{
				return this.MinLevel;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06001AE4 RID: 6884 RVA: 0x00199A4C File Offset: 0x00197C4C
		// (set) Token: 0x06001AE5 RID: 6885 RVA: 0x00199A98 File Offset: 0x00197C98
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

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06001AE6 RID: 6886 RVA: 0x00199AE4 File Offset: 0x00197CE4
		public int NeedMinChangeLev
		{
			get
			{
				return this.m_NeedMinChangeLev;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06001AE7 RID: 6887 RVA: 0x00199AFC File Offset: 0x00197CFC
		// (set) Token: 0x06001AE8 RID: 6888 RVA: 0x00199B13 File Offset: 0x00197D13
		public static int BattleMaxPoint
		{
			get
			{
				return BattleManager.m_BattleMaxPoint;
			}
			set
			{
				BattleManager.m_BattleMaxPoint = value;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06001AE9 RID: 6889 RVA: 0x00199B1C File Offset: 0x00197D1C
		// (set) Token: 0x06001AEA RID: 6890 RVA: 0x00199B33 File Offset: 0x00197D33
		public static string BattleMaxPointName
		{
			get
			{
				return BattleManager.m_BattleMaxPointName;
			}
			set
			{
				BattleManager.m_BattleMaxPointName = value;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06001AEB RID: 6891 RVA: 0x00199B3C File Offset: 0x00197D3C
		// (set) Token: 0x06001AEC RID: 6892 RVA: 0x00199B53 File Offset: 0x00197D53
		public static int BattleMaxPointNow
		{
			get
			{
				return BattleManager.m_BattleMaxPointNow;
			}
			set
			{
				BattleManager.m_BattleMaxPointNow = value;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06001AED RID: 6893 RVA: 0x00199B5C File Offset: 0x00197D5C
		// (set) Token: 0x06001AEE RID: 6894 RVA: 0x00199B73 File Offset: 0x00197D73
		public static int PushMsgDayID
		{
			get
			{
				return BattleManager.m_nPushMsgDayID;
			}
			set
			{
				BattleManager.m_nPushMsgDayID = value;
			}
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x00199B7C File Offset: 0x00197D7C
		public static void SetTotalPointInfo(string sName, int nValue)
		{
			BattleManager.BattleMaxPointName = sName;
			BattleManager.BattleMaxPoint = nValue;
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x00199B90 File Offset: 0x00197D90
		private void ClearAllRolePointInfo()
		{
			lock (this.RolePointMutex)
			{
				this.RolePointDict.Clear();
				for (int i = 0; i < this.TopPointList.Length; i++)
				{
					this.TopPointList[i] = null;
				}
			}
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x00199C04 File Offset: 0x00197E04
		public void UpdateRolePointInfo(GameClient client)
		{
			int roleID = client.ClientData.RoleID;
			int rolePoint = client.ClientData.BattleKilledNum;
			List<RoleDamage> top5PointArray = null;
			BattlePointInfo pointInfo = null;
			bool needSend = false;
			lock (this.RolePointMutex)
			{
				if (this.RolePointDict.TryGetValue(roleID, out pointInfo))
				{
					needSend = (pointInfo.m_DamagePoint == rolePoint);
					pointInfo.m_DamagePoint = rolePoint;
				}
				else
				{
					pointInfo = new BattlePointInfo();
					pointInfo.m_RoleID = roleID;
					pointInfo.m_RoleName = Global.FormatRoleName4(client);
					pointInfo.m_DamagePoint = rolePoint;
					this.RolePointDict[roleID] = pointInfo;
				}
				if (needSend || pointInfo.CompareTo(this.TopPointList[4]) < 0)
				{
					if (pointInfo.Ranking < 0)
					{
						this.TopPointList[5] = pointInfo;
					}
					Array.Sort<BattlePointInfo>(this.TopPointList, new Comparison<BattlePointInfo>(pointInfo.Compare));
					if (null != this.TopPointList[5])
					{
						this.TopPointList[5].Ranking = -1;
					}
					needSend = true;
				}
				if (pointInfo.Side != client.ClientData.BattleWhichSide)
				{
					pointInfo.Side = client.ClientData.BattleWhichSide;
					needSend = true;
				}
				if (needSend)
				{
					top5PointArray = new List<RoleDamage>(5);
					int i = 0;
					while (this.TopPointList[i] != null && i < 5)
					{
						this.TopPointList[i].Ranking = i;
						top5PointArray.Add(new RoleDamage(this.TopPointList[i].m_RoleID, (long)this.TopPointList[i].m_DamagePoint, this.TopPointList[i].m_RoleName, new int[]
						{
							this.TopPointList[i].Side
						}));
						i++;
					}
				}
			}
			if (needSend)
			{
				List<GameClient> clientList = GameManager.ClientMgr.GetMapGameClients(this.MapCode);
				foreach (GameClient c in clientList)
				{
					c.sendCmd<List<RoleDamage>>(647, top5PointArray, false);
				}
			}
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x00199EA4 File Offset: 0x001980A4
		public void SendScoreInfoListToClient(GameClient client)
		{
			int roleID = client.ClientData.RoleID;
			List<RoleDamage> top5PointArray = new List<RoleDamage>(5);
			lock (this.RolePointMutex)
			{
				int i = 0;
				while (this.TopPointList[i] != null && i < 5)
				{
					top5PointArray.Add(new RoleDamage(this.TopPointList[i].m_RoleID, (long)this.TopPointList[i].m_DamagePoint, this.TopPointList[i].m_RoleName, new int[]
					{
						this.TopPointList[i].Side
					}));
					i++;
				}
			}
			if (null != top5PointArray)
			{
				client.sendCmd<List<RoleDamage>>(647, top5PointArray, false);
			}
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x00199F8C File Offset: 0x0019818C
		public bool ClientEnter()
		{
			bool ret = false;
			lock (this.mutex)
			{
				if (this._TotalClientCount < this.MaxEnterNum)
				{
					this._TotalClientCount++;
					ret = true;
				}
			}
			return ret;
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x0019A004 File Offset: 0x00198204
		public void ClientLeave()
		{
			lock (this.mutex)
			{
				this._TotalClientCount--;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06001AF5 RID: 6901 RVA: 0x0019A058 File Offset: 0x00198258
		public string BattleDisableGoodsIDs
		{
			get
			{
				return this.DisableGoodsIDs;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06001AF6 RID: 6902 RVA: 0x0019A070 File Offset: 0x00198270
		// (set) Token: 0x06001AF7 RID: 6903 RVA: 0x0019A0BC File Offset: 0x001982BC
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

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06001AF8 RID: 6904 RVA: 0x0019A108 File Offset: 0x00198308
		// (set) Token: 0x06001AF9 RID: 6905 RVA: 0x0019A154 File Offset: 0x00198354
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

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06001AFA RID: 6906 RVA: 0x0019A1A0 File Offset: 0x001983A0
		// (set) Token: 0x06001AFB RID: 6907 RVA: 0x0019A1EC File Offset: 0x001983EC
		public int SuiKilledNum
		{
			get
			{
				int suiKilledNum;
				lock (this.mutex)
				{
					suiKilledNum = this._SuiKilledNum;
				}
				return suiKilledNum;
			}
			set
			{
				lock (this.mutex)
				{
					this._SuiLastKillEmemyTime = TimeUtil.NOW() * 10000L;
					this._SuiKilledNum = value;
				}
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06001AFC RID: 6908 RVA: 0x0019A24C File Offset: 0x0019844C
		// (set) Token: 0x06001AFD RID: 6909 RVA: 0x0019A298 File Offset: 0x00198498
		public int TangKilledNum
		{
			get
			{
				int tangKilledNum;
				lock (this.mutex)
				{
					tangKilledNum = this._TangKilledNum;
				}
				return tangKilledNum;
			}
			set
			{
				lock (this.mutex)
				{
					this._TangLastKillEmemyTime = TimeUtil.NOW() * 10000L;
					this._TangKilledNum = value;
				}
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06001AFE RID: 6910 RVA: 0x0019A2F8 File Offset: 0x001984F8
		// (set) Token: 0x06001AFF RID: 6911 RVA: 0x0019A344 File Offset: 0x00198544
		public int SuiClientCount
		{
			get
			{
				int suiClientCount;
				lock (this.mutex)
				{
					suiClientCount = this._SuiClientCount;
				}
				return suiClientCount;
			}
			set
			{
				lock (this.mutex)
				{
					this._SuiClientCount = value;
				}
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06001B00 RID: 6912 RVA: 0x0019A390 File Offset: 0x00198590
		// (set) Token: 0x06001B01 RID: 6913 RVA: 0x0019A3DC File Offset: 0x001985DC
		public int TangClientCount
		{
			get
			{
				int tangClientCount;
				lock (this.mutex)
				{
					tangClientCount = this._TangClientCount;
				}
				return tangClientCount;
			}
			set
			{
				lock (this.mutex)
				{
					this._TangClientCount = value;
				}
			}
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x0019A428 File Offset: 0x00198628
		public void ClearBattleExpByLevels()
		{
			this.BattleExpByLevels = null;
		}

		// Token: 0x06001B03 RID: 6915 RVA: 0x0019A434 File Offset: 0x00198634
		private long GetBattleExpByLevel(GameClient client, int level)
		{
			long[] expByLevels = this.BattleExpByLevels;
			if (null == expByLevels)
			{
				SystemXmlItem systemXmlItem = null;
				expByLevels = new long[Data.LevelUpExperienceList.Length - 1];
				for (int i = 0; i < expByLevels.Length; i++)
				{
					if (GameManager.systemBattleExpMgr.SystemXmlItemDict.TryGetValue(i + 1, out systemXmlItem))
					{
						expByLevels[i] = (long)Global.GMax(0, systemXmlItem.GetIntValue("Experience", -1));
					}
				}
				this.BattleExpByLevels = expByLevels;
			}
			int index = level - 1;
			long result;
			if (index < 0 || index >= this.BattleExpByLevels.Length)
			{
				result = 0L;
			}
			else
			{
				int nChangeLev = client.ClientData.ChangeLifeCount;
				double nRate;
				if (nChangeLev == 0)
				{
					nRate = 1.0;
				}
				else
				{
					nRate = Data.ChangeLifeEverydayExpRate[nChangeLev];
				}
				result = (long)((int)((double)expByLevels[index] * nRate));
			}
			return result;
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06001B05 RID: 6917 RVA: 0x0019A53C File Offset: 0x0019873C
		// (set) Token: 0x06001B04 RID: 6916 RVA: 0x0019A52F File Offset: 0x0019872F
		private List<BattleManager.Award> BattleAwardByScore
		{
			get
			{
				List<BattleManager.Award> awardByScore = this._BattleAwardByScore;
				if (null == awardByScore)
				{
					awardByScore = new List<BattleManager.Award>();
					foreach (SystemXmlItem val in GameManager.systemBattleAwardMgr.SystemXmlItemDict.Values)
					{
						BattleManager.Award award = new BattleManager.Award
						{
							MinJiFen = Math.Max(0, val.GetIntValue("MinJiFen", -1)),
							MaxJiFen = Math.Max(0, val.GetIntValue("MaxJiFen", -1)),
							ExpXiShu = Math.Max(0, val.GetIntValue("ExpXiShu", -1)),
							MoJingXiShu = Math.Max(0.0, val.GetDoubleValue("MoJingXiShu")),
							ChengJiuXiShu = Math.Max(0.0, val.GetDoubleValue("ChengJiuXiShu")),
							MinExp = Math.Max(0, val.GetIntValue("MinExp", -1)),
							MaxExp = Math.Max(0, val.GetIntValue("MaxExp", -1)),
							MinMoJing = Math.Max(0, val.GetIntValue("MinMoJing", -1)),
							MaxMoJing = Math.Max(0, val.GetIntValue("MaxMoJing", -1)),
							MinChengJiu = Math.Max(0, val.GetIntValue("MinChengJiu", -1)),
							MaxChengJiu = Math.Max(0, val.GetIntValue("MaxChengJiu", -1))
						};
						if (award.MinJiFen > award.MaxJiFen)
						{
							award.MaxJiFen = 268435455;
						}
						awardByScore.Add(award);
					}
					this._BattleAwardByScore = awardByScore;
				}
				return awardByScore;
			}
			set
			{
				this._BattleAwardByScore = value;
			}
		}

		// Token: 0x06001B06 RID: 6918 RVA: 0x0019A728 File Offset: 0x00198928
		public void ClearBattleAwardByScore()
		{
			this._BattleAwardByScore = null;
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x0019A734 File Offset: 0x00198934
		private int GetSuccessSide()
		{
			int successSide = -1;
			if (this.TangKilledNum > this.SuiKilledNum)
			{
				successSide = 2;
			}
			else if (this.TangKilledNum < this.SuiKilledNum)
			{
				successSide = 1;
			}
			else if (this._SuiLastKillEmemyTime < this._TangLastKillEmemyTime)
			{
				successSide = 1;
			}
			else if (this._SuiLastKillEmemyTime > this._TangLastKillEmemyTime)
			{
				successSide = 2;
			}
			return successSide;
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x0019A7BC File Offset: 0x001989BC
		private bool IsSuccessClient(GameClient client)
		{
			int successSide;
			if (this.TangKilledNum > this.SuiKilledNum)
			{
				successSide = 2;
			}
			else if (this.TangKilledNum < this.SuiKilledNum)
			{
				successSide = 1;
			}
			else if (this._SuiLastKillEmemyTime < this._TangLastKillEmemyTime)
			{
				successSide = 1;
			}
			else
			{
				if (this._SuiLastKillEmemyTime <= this._TangLastKillEmemyTime)
				{
					return false;
				}
				successSide = 2;
			}
			return successSide == client.ClientData.BattleWhichSide;
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x0019A860 File Offset: 0x00198A60
		private void ProcessRoleBattleExpAndFlagAward(GameClient client, int successSide, int paiMing)
		{
			ProcessTask.ProcessAddTaskVal(client, TaskTypes.Battle, -1, 1, new object[0]);
			List<BattleManager.Award> awardByScore = this.BattleAwardByScore;
			if (null == awardByScore)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("处理大乱斗结束奖励时, 奖励列表项未空", new object[0]), null, true);
			}
			else
			{
				double expAward = 0.0;
				double MoJingAward = 0.0;
				double chengJiuAward = 0.0;
				AwardsItemList awardsItemList = new AwardsItemList();
				bool successed = successSide == client.ClientData.BattleWhichSide;
				double awardmuti = 0.0;
				HeFuAwardTimesActivity hefuact = HuodongCachingMgr.GetHeFuAwardTimesActivity();
				if (hefuact != null && hefuact.InActivityTime())
				{
					awardmuti += (double)hefuact.activityTimes;
				}
				JieRiMultAwardActivity jieriact = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null != jieriact)
				{
					JieRiMultConfig config = jieriact.GetConfig(2);
					if (null != config)
					{
						awardmuti += config.GetMult();
					}
				}
				SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != spAct)
				{
					awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_Battle);
				}
				awardmuti = Math.Max(1.0, awardmuti);
				foreach (BattleManager.Award award in awardByScore)
				{
					if (client.ClientData.BattleKilledNum >= award.MinJiFen && client.ClientData.BattleKilledNum < award.MaxJiFen)
					{
						expAward = (double)(client.ClientData.BattleKilledNum * award.ExpXiShu);
						if (award.MoJingXiShu > 0.0)
						{
							MoJingAward = (double)((int)((double)client.ClientData.BattleKilledNum * award.MoJingXiShu));
						}
						if (award.ChengJiuXiShu > 0.0)
						{
							chengJiuAward = (double)((int)((double)client.ClientData.BattleKilledNum * award.ChengJiuXiShu));
						}
						if (!successed)
						{
							if (expAward > 0.0)
							{
								expAward *= 0.8;
							}
							if (MoJingAward > 0.0)
							{
								MoJingAward *= 0.8;
							}
							if (chengJiuAward > 0.0)
							{
								chengJiuAward *= 0.8;
							}
						}
						expAward = (double)((long)(expAward * Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount]));
						expAward = Math.Max(expAward, (double)award.MinExp * Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount]);
						expAward = Math.Min(expAward, (double)award.MaxExp * Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount]);
						MoJingAward = Math.Max(MoJingAward, (double)award.MinMoJing);
						MoJingAward = Math.Min(MoJingAward, (double)award.MaxMoJing);
						chengJiuAward = Math.Max(chengJiuAward, (double)award.MinChengJiu);
						chengJiuAward = Math.Min(chengJiuAward, (double)award.MaxChengJiu);
						if (expAward > 0.0)
						{
							expAward = (double)((int)(expAward * awardmuti));
						}
						if (MoJingAward > 0.0)
						{
							MoJingAward = (double)((int)(MoJingAward * awardmuti));
						}
						if (chengJiuAward > 0.0)
						{
							chengJiuAward = (double)((int)(chengJiuAward * awardmuti));
						}
						break;
					}
				}
				foreach (SystemXmlItem xml in GameManager.SystemBattlePaiMingAwards.SystemXmlItemDict.Values)
				{
					if (null != xml)
					{
						int min = xml.GetIntValue("MinPaiMing", -1) - 1;
						int max = xml.GetIntValue("MaxPaiMing", -1) - 1;
						if (paiMing >= min && paiMing <= max)
						{
							awardsItemList.AddNoRepeat(xml.GetStringValue("Goods"));
						}
					}
				}
				if (expAward > 0.0)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, (long)expAward, true, false, false, "none");
				}
				if (MoJingAward > 0.0)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, (int)MoJingAward, "阵营战", false, true, false);
				}
				if (chengJiuAward > 0.0)
				{
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, (int)chengJiuAward, "阵营战", false, true);
				}
				List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(awardsItemList.Items, -1);
				if (!Global.CanAddGoodsDataList(client, goodsDataList))
				{
					GameManager.ClientMgr.SendMailWhenPacketFull(client, goodsDataList, GLang.GetLang(13, new object[0]), string.Format(GLang.GetLang(14, new object[0]), paiMing + 1));
				}
				else
				{
					for (int i = 0; i < goodsDataList.Count; i++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "阵营战排名奖励", "1900-01-01 12:00:00", 0, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, 0, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, 0, null, null, 0, true);
					}
				}
				GameManager.ClientMgr.NotifySelfSuiTangBattleAward(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, this.TangKilledNum, this.SuiKilledNum, (long)expAward, (int)MoJingAward, (int)chengJiuAward, successed, paiMing, awardsItemList.ToString());
			}
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x0019AEB0 File Offset: 0x001990B0
		private void ProcessBattleResultAwards()
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(this.MapCode);
			if (null != objsList)
			{
				int successSide = this.GetSuccessSide();
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						this.ProcessRoleBattleExpAndFlagAward(c, successSide, i);
					}
				}
				GameManager.GoodsPackMgr.ProcessBattle(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, objsList, null, 0, 0);
			}
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x0019AF74 File Offset: 0x00199174
		private void ProcessBattleResultAwards2()
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(this.MapCode);
			if (null != objsList)
			{
				int successSide = this.GetSuccessSide();
				List<GameClient> clientList = new List<GameClient>();
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						clientList.Add(c);
					}
				}
				clientList.Sort((GameClient x, GameClient y) => y.ClientData.BattleKilledNum - x.ClientData.BattleKilledNum);
				for (int i = 0; i < clientList.Count; i++)
				{
					this.ProcessRoleBattleExpAndFlagAward(clientList[i], successSide, i);
				}
				GameManager.GoodsPackMgr.ProcessBattle(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, objsList, null, 0, 0);
			}
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x0019B064 File Offset: 0x00199264
		public void ClearRoleLeaveInfo(int roleID)
		{
			lock (this._RoleLeaveJiFenDict)
			{
				this._RoleLeaveJiFenDict.Remove(roleID);
			}
			lock (this._RoleLeaveTicksDict)
			{
				this._RoleLeaveTicksDict.Remove(roleID);
			}
			lock (this._RoleLeaveSideDict)
			{
				this._RoleLeaveSideDict.Remove(roleID);
			}
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x0019B140 File Offset: 0x00199340
		public void ClearAllRoleLeaveInfo()
		{
			lock (this._RoleLeaveJiFenDict)
			{
				this._RoleLeaveJiFenDict.Clear();
			}
			lock (this._RoleLeaveTicksDict)
			{
				this._RoleLeaveTicksDict.Clear();
			}
			lock (this._RoleLeaveSideDict)
			{
				this._RoleLeaveSideDict.Clear();
			}
			BattleManager.m_BattleMaxPointNow = 0;
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x0019B220 File Offset: 0x00199420
		public long GetRoleLeaveTicks(int roleID)
		{
			long ticks = 0L;
			lock (this._RoleLeaveTicksDict)
			{
				this._RoleLeaveTicksDict.TryGetValue(roleID, out ticks);
			}
			return ticks;
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x0019B280 File Offset: 0x00199480
		public int GetRoleLeaveJiFen(int roleID)
		{
			int jiFen = 0;
			lock (this._RoleLeaveJiFenDict)
			{
				this._RoleLeaveJiFenDict.TryGetValue(roleID, out jiFen);
			}
			return jiFen;
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x0019B2E0 File Offset: 0x001994E0
		public int GetRoleLeaveSideID(int roleID)
		{
			int sideID = 0;
			lock (this._RoleLeaveSideDict)
			{
				this._RoleLeaveSideDict.TryGetValue(roleID, out sideID);
			}
			return sideID;
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x0019B340 File Offset: 0x00199540
		public void LeaveBattleMap(GameClient client, bool regLastInfo)
		{
			if (client.ClientData.MapCode == GameManager.BattleMgr.MapCode)
			{
				GameManager.BattleMgr.ClientLeave();
				if (1 == client.ClientData.BattleWhichSide)
				{
					GameManager.BattleMgr.SuiClientCount--;
				}
				else
				{
					GameManager.BattleMgr.TangClientCount--;
				}
				if (!regLastInfo)
				{
					GameManager.BattleMgr.ClearRoleLeaveInfo(client.ClientData.RoleID);
				}
				else if (GameManager.BattleMgr.GetBattlingState() < 1 || GameManager.BattleMgr.GetBattlingState() >= 4)
				{
					GameManager.BattleMgr.ClearRoleLeaveInfo(client.ClientData.RoleID);
				}
				else
				{
					int roleID = client.ClientData.RoleID;
					int jiFen = client.ClientData.BattleKilledNum;
					int sideID = client.ClientData.BattleWhichSide;
					lock (this._RoleLeaveJiFenDict)
					{
						this._RoleLeaveJiFenDict[roleID] = jiFen;
					}
					long ticks = TimeUtil.NOW();
					lock (this._RoleLeaveTicksDict)
					{
						this._RoleLeaveTicksDict[roleID] = ticks;
					}
					lock (this._RoleLeaveSideDict)
					{
						this._RoleLeaveSideDict[roleID] = sideID;
					}
					client.ClientData.BattleWhichSide = 0;
				}
			}
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x0019B534 File Offset: 0x00199734
		private void ProcessAddRoleExperience(GameClient client)
		{
			long exp = this.GetBattleExpByLevel(client, client.ClientData.Level);
			if (exp > 0L)
			{
				double awardmuti = 0.0;
				JieRiMultAwardActivity jieriact = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null != jieriact)
				{
					JieRiMultConfig config = jieriact.GetConfig(2);
					if (null != config)
					{
						awardmuti += config.GetMult();
					}
				}
				SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != spAct)
				{
					awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_Battle);
				}
				awardmuti = Math.Max(1.0, awardmuti);
				exp = (long)((double)exp * awardmuti);
				GameManager.ClientMgr.ProcessRoleExperience(client, exp, true, false, false, "none");
			}
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x0019B5E8 File Offset: 0x001997E8
		private void ProcessTimeAddRoleExp()
		{
			if (this.BattlingState == BattleStates.StartFight)
			{
				long ticks = TimeUtil.NOW();
				if (ticks - this.LastAddBattleExpTicks >= (long)(this.AddExpSecs * 1000))
				{
					this.LastAddBattleExpTicks = ticks;
					List<object> objsList = GameManager.ClientMgr.GetMapClients(this.MapCode);
					if (null != objsList)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient c = objsList[i] as GameClient;
							if (c != null)
							{
								this.ProcessAddRoleExperience(c);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x0019B698 File Offset: 0x00199898
		private void ProcessTimeNotifyBattleKilledNum()
		{
			if (this.BattlingState == BattleStates.StartFight)
			{
				long ticks = TimeUtil.NOW();
				if (ticks - this.LastNotifyBattleKilledNumTicks >= (long)(this.NotifyBattleKilledNumSecs * 1000))
				{
					this.LastNotifyBattleKilledNumTicks = ticks;
					if (this.LastSuiKilledNum != this.SuiKilledNum || this.LastTangKilledNum != this.TangKilledNum)
					{
						GameManager.ClientMgr.NotifyBattleKilledNumCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.SuiKilledNum, this.TangKilledNum);
						this.LastSuiKilledNum = this.SuiKilledNum;
						this.LastTangKilledNum = this.TangKilledNum;
					}
				}
			}
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x0019B750 File Offset: 0x00199950
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				if (!string.IsNullOrEmpty(BattleManager.m_BattleMaxPointName) && BattleManager.m_BattleMaxPointName == oldName)
				{
					BattleManager.m_BattleMaxPointName = newName;
				}
			}
		}

		// Token: 0x0400298D RID: 10637
		public const int ConstTopPointNumber = 5;

		// Token: 0x0400298E RID: 10638
		public const int ConstJiFenByKillRole = 5;

		// Token: 0x0400298F RID: 10639
		public const int ConstJiFenByKilled = 1;

		// Token: 0x04002990 RID: 10640
		private List<string> TimePointsList = new List<string>();

		// Token: 0x04002991 RID: 10641
		private int MapCode = -1;

		// Token: 0x04002992 RID: 10642
		private int MinLevel = 20;

		// Token: 0x04002993 RID: 10643
		private int MinRequestNum = 100;

		// Token: 0x04002994 RID: 10644
		private int MaxEnterNum = 30;

		// Token: 0x04002995 RID: 10645
		private int FallGiftNum = 5;

		// Token: 0x04002996 RID: 10646
		private int FallID = -1;

		// Token: 0x04002997 RID: 10647
		private string DisableGoodsIDs = "";

		// Token: 0x04002998 RID: 10648
		private List<GoodsData> GiveAwardsGoodsDataList = null;

		// Token: 0x04002999 RID: 10649
		private int AddExpSecs = 60;

		// Token: 0x0400299A RID: 10650
		private int NotifyBattleKilledNumSecs = 30;

		// Token: 0x0400299B RID: 10651
		private int WaitingEnterSecs = 30;

		// Token: 0x0400299C RID: 10652
		private int PrepareSecs = 30;

		// Token: 0x0400299D RID: 10653
		private int FightingSecs = 300;

		// Token: 0x0400299E RID: 10654
		private int ClearRolesSecs = 30;

		// Token: 0x0400299F RID: 10655
		private int m_NeedMinChangeLev = 0;

		// Token: 0x040029A0 RID: 10656
		private static int m_BattleMaxPoint = 0;

		// Token: 0x040029A1 RID: 10657
		private static string m_BattleMaxPointName = "";

		// Token: 0x040029A2 RID: 10658
		private static int m_BattleMaxPointNow = 0;

		// Token: 0x040029A3 RID: 10659
		private static int m_nPushMsgDayID = -1;

		// Token: 0x040029A4 RID: 10660
		private int BattleLineID = 1;

		// Token: 0x040029A5 RID: 10661
		public static SystemXmlItems systemBattleAwardMgr = null;

		// Token: 0x040029A6 RID: 10662
		private BattleStates BattlingState = BattleStates.NoBattle;

		// Token: 0x040029A7 RID: 10663
		private long StateStartTicks = 0L;

		// Token: 0x040029A8 RID: 10664
		private object RolePointMutex = new object();

		// Token: 0x040029A9 RID: 10665
		private BattlePointInfo[] TopPointList = new BattlePointInfo[6];

		// Token: 0x040029AA RID: 10666
		private Dictionary<int, BattlePointInfo> RolePointDict = new Dictionary<int, BattlePointInfo>();

		// Token: 0x040029AB RID: 10667
		private long LastAddBattleExpTicks = 0L;

		// Token: 0x040029AC RID: 10668
		private long LastNotifyBattleKilledNumTicks = 0L;

		// Token: 0x040029AD RID: 10669
		private object mutex = new object();

		// Token: 0x040029AE RID: 10670
		private bool _AllowAttack = false;

		// Token: 0x040029AF RID: 10671
		private int _TotalClientCount = 0;

		// Token: 0x040029B0 RID: 10672
		private int _StartRoleNum = 0;

		// Token: 0x040029B1 RID: 10673
		private int _AllKilledRoleNum = 0;

		// Token: 0x040029B2 RID: 10674
		private long _SuiLastKillEmemyTime = 0L;

		// Token: 0x040029B3 RID: 10675
		private int _SuiKilledNum = 0;

		// Token: 0x040029B4 RID: 10676
		private long _TangLastKillEmemyTime = 0L;

		// Token: 0x040029B5 RID: 10677
		private int _TangKilledNum = 0;

		// Token: 0x040029B6 RID: 10678
		private int _SuiClientCount = 0;

		// Token: 0x040029B7 RID: 10679
		private int _TangClientCount = 0;

		// Token: 0x040029B8 RID: 10680
		private long[] BattleExpByLevels = null;

		// Token: 0x040029B9 RID: 10681
		private List<BattleManager.Award> _BattleAwardByScore = null;

		// Token: 0x040029BA RID: 10682
		private Dictionary<int, int> _RoleLeaveJiFenDict = new Dictionary<int, int>();

		// Token: 0x040029BB RID: 10683
		private Dictionary<int, int> _RoleLeaveSideDict = new Dictionary<int, int>();

		// Token: 0x040029BC RID: 10684
		private Dictionary<int, long> _RoleLeaveTicksDict = new Dictionary<int, long>();

		// Token: 0x040029BD RID: 10685
		private int LastSuiKilledNum = -1;

		// Token: 0x040029BE RID: 10686
		private int LastTangKilledNum = -1;

		// Token: 0x020005C7 RID: 1479
		protected class Award
		{
			// Token: 0x040029C0 RID: 10688
			public int MinJiFen = 0;

			// Token: 0x040029C1 RID: 10689
			public int MaxJiFen = 200;

			// Token: 0x040029C2 RID: 10690
			public int ExpXiShu = 5000;

			// Token: 0x040029C3 RID: 10691
			public double MoJingXiShu = 4.0;

			// Token: 0x040029C4 RID: 10692
			public double ChengJiuXiShu = 1.0;

			// Token: 0x040029C5 RID: 10693
			public int MinExp = 3000;

			// Token: 0x040029C6 RID: 10694
			public int MaxExp = 600000;

			// Token: 0x040029C7 RID: 10695
			public int MinMoJing = 10;

			// Token: 0x040029C8 RID: 10696
			public int MaxMoJing = 20;

			// Token: 0x040029C9 RID: 10697
			public int MinChengJiu = 100;

			// Token: 0x040029CA RID: 10698
			public int MaxChengJiu = 200;
		}
	}
}
