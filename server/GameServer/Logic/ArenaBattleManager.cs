using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class ArenaBattleManager
	{
		
		public int GetBattlingState()
		{
			return (int)this.BattlingState;
		}

		
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

		
		public void Init()
		{
			this.LoadParams();
			this.AllowAttack = false;
			this.StartRoleNum = 0;
			this.TotalClientCount = 0;
			this._AllKilledRoleNum = 0;
			Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.TheKingOfPK);
		}

		
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

		
		
		public int BattleMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		
		
		public int BattleServerLineID
		{
			get
			{
				return this.BattleLineID;
			}
		}

		
		
		public bool AllowEnterMap
		{
			get
			{
				return this.BattlingState >= BattleStates.PublishMsg && this.BattlingState < BattleStates.StartFight;
			}
		}

		
		
		public bool IsFighting
		{
			get
			{
				return this.BattlingState >= BattleStates.StartFight && this.BattlingState < BattleStates.ClearBattle;
			}
		}

		
		
		
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

		
		
		public int AllowMinChangeLifeLev
		{
			get
			{
				return this.MinChangeLifeLev;
			}
		}

		
		
		public int AllowMinLevel
		{
			get
			{
				return this.MinLevel;
			}
		}

		
		
		
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

		
		protected void ClientLeave(GameClient client)
		{
			lock (this.mutex)
			{
				this._TotalClientCount--;
				this.TheKingOfPKGetawardFlag.Remove(client.ClientData.RoleID);
			}
			this._bRoleEnterOrLeave = true;
		}

		
		public void LeaveArenaBattleMap(GameClient client)
		{
			if (client.ClientData.MapCode == this.MapCode)
			{
				this.ProcessAward(client);
				this.ClientLeave(client);
			}
		}

		
		public bool EnterArenaBattleMap(GameClient client)
		{
			return client.ClientData.MapCode == this.MapCode && this.ClientEnter(client);
		}

		
		
		public string BattleDisableGoodsIDs
		{
			get
			{
				return this.DisableGoodsIDs;
			}
		}

		
		
		
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

		
		public void SetTotalPointInfo(string sName, int nValue)
		{
			this.TheKingOfPKTopRoleName = sName;
			this.TheKingOfPKTopPoint = nValue;
		}

		
		public bool IsInPkScene(int nMap)
		{
			return nMap == 10000;
		}

		
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

		
		public int GetPKKingRoleID()
		{
			return GameManager.GameConfigMgr.GetGameConfigItemInt("PKKingRole", 0);
		}

		
		public void SetPKKingRoleID(int roleID)
		{
			Global.UpdateDBGameConfigg("PKKingRole", roleID.ToString());
			GameManager.GameConfigMgr.SetGameConfigItem("PKKingRole", roleID.ToString());
		}

		
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

		
		public void ReShowPKKing()
		{
			int roleID = this.GetPKKingRoleID();
			if (roleID > 0)
			{
				this.ReplacePKKingNpc(roleID);
			}
		}

		
		public void ClearDbKingNpc()
		{
			this.KingRoleData = null;
			Global.sendToDB<bool, string>(13232, string.Format("{0}", 1), 0);
		}

		
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

		
		public bool IsInArenaBattle(GameClient client)
		{
			return client.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode;
		}

		
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

		
		private int TopPoint = -1;

		
		private string TopRoleName = "";

		
		private Dictionary<int, int> GetawardFlag = new Dictionary<int, int>();

		
		private List<string> TimePointsList = new List<string>();

		
		private int MapCode = -1;

		
		private int MinChangeLifeLev = 0;

		
		private int MinLevel = 20;

		
		private int MinRequestNum = 100;

		
		private int MaxEnterNum = 300;

		
		private int FallGiftNum = 5;

		
		private int FallID = -1;

		
		private string DisableGoodsIDs = "";

		
		private List<GoodsData> GiveAwardsGoodsDataList = null;

		
		private int AddExpSecs = 60;

		
		private int ForceNotifyBattleScoreSec = 10;

		
		private int WaitingEnterSecs = 30;

		
		private int PrepareSecs = 30;

		
		private int FightingSecs = 300;

		
		private int ClearRolesSecs = 30;

		
		private int BattleLineID = 1;

		
		public static int m_nPushMsgDayID = -1;

		
		private Queue<Tuple<int, string, string>> _ChangeNameEvQ = new Queue<Tuple<int, string, string>>();

		
		private BattleStates BattlingState = BattleStates.NoBattle;

		
		private long StateStartTicks = 0L;

		
		private long KeepSingleTicks;

		
		private long LastNotifyBattleScoreTicks = 0L;

		
		private HashSet<int> DeadRoleSets = new HashSet<int>();

		
		private GameClient ChampionClient = null;

		
		private object mutex = new object();

		
		private bool _AllowAttack = false;

		
		private int _TotalClientCount = 0;

		
		private int _LastNotifyClientCount = 0;

		
		private bool _bRoleEnterOrLeave = false;

		
		private int _EnterBattleClientCount = 0;

		
		private int _StartRoleNum = 0;

		
		private int _AllKilledRoleNum = 0;

		
		private long LastAddBangZhanAwardsTicks = 0L;

		
		private object kingRoleDataMutex = new object();

		
		private RoleDataEx _kingRoleData = null;
	}
}
