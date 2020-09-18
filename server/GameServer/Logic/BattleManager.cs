using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class BattleManager
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

		
		public void Init()
		{
			this.LoadParams();
			this.AllowAttack = false;
			this.StartRoleNum = 0;
			this.TotalClientCount = 0;
			this.AllKilledRoleNum = 0;
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
		}

		
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

		
		
		public object ExternalMutex
		{
			get
			{
				return this.mutex;
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
				return this.BattlingState >= BattleStates.PublishMsg && this.BattlingState < BattleStates.EndFight;
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

		
		
		public int NeedMinChangeLev
		{
			get
			{
				return this.m_NeedMinChangeLev;
			}
		}

		
		
		
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

		
		public static void SetTotalPointInfo(string sName, int nValue)
		{
			BattleManager.BattleMaxPointName = sName;
			BattleManager.BattleMaxPoint = nValue;
		}

		
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

		
		public void ClientLeave()
		{
			lock (this.mutex)
			{
				this._TotalClientCount--;
			}
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

		
		public void ClearBattleExpByLevels()
		{
			this.BattleExpByLevels = null;
		}

		
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

		
		public void ClearBattleAwardByScore()
		{
			this._BattleAwardByScore = null;
		}

		
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

		
		public long GetRoleLeaveTicks(int roleID)
		{
			long ticks = 0L;
			lock (this._RoleLeaveTicksDict)
			{
				this._RoleLeaveTicksDict.TryGetValue(roleID, out ticks);
			}
			return ticks;
		}

		
		public int GetRoleLeaveJiFen(int roleID)
		{
			int jiFen = 0;
			lock (this._RoleLeaveJiFenDict)
			{
				this._RoleLeaveJiFenDict.TryGetValue(roleID, out jiFen);
			}
			return jiFen;
		}

		
		public int GetRoleLeaveSideID(int roleID)
		{
			int sideID = 0;
			lock (this._RoleLeaveSideDict)
			{
				this._RoleLeaveSideDict.TryGetValue(roleID, out sideID);
			}
			return sideID;
		}

		
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

		
		public const int ConstTopPointNumber = 5;

		
		public const int ConstJiFenByKillRole = 5;

		
		public const int ConstJiFenByKilled = 1;

		
		private List<string> TimePointsList = new List<string>();

		
		private int MapCode = -1;

		
		private int MinLevel = 20;

		
		private int MinRequestNum = 100;

		
		private int MaxEnterNum = 30;

		
		private int FallGiftNum = 5;

		
		private int FallID = -1;

		
		private string DisableGoodsIDs = "";

		
		private List<GoodsData> GiveAwardsGoodsDataList = null;

		
		private int AddExpSecs = 60;

		
		private int NotifyBattleKilledNumSecs = 30;

		
		private int WaitingEnterSecs = 30;

		
		private int PrepareSecs = 30;

		
		private int FightingSecs = 300;

		
		private int ClearRolesSecs = 30;

		
		private int m_NeedMinChangeLev = 0;

		
		private static int m_BattleMaxPoint = 0;

		
		private static string m_BattleMaxPointName = "";

		
		private static int m_BattleMaxPointNow = 0;

		
		private static int m_nPushMsgDayID = -1;

		
		private int BattleLineID = 1;

		
		public static SystemXmlItems systemBattleAwardMgr = null;

		
		private BattleStates BattlingState = BattleStates.NoBattle;

		
		private long StateStartTicks = 0L;

		
		private object RolePointMutex = new object();

		
		private BattlePointInfo[] TopPointList = new BattlePointInfo[6];

		
		private Dictionary<int, BattlePointInfo> RolePointDict = new Dictionary<int, BattlePointInfo>();

		
		private long LastAddBattleExpTicks = 0L;

		
		private long LastNotifyBattleKilledNumTicks = 0L;

		
		private object mutex = new object();

		
		private bool _AllowAttack = false;

		
		private int _TotalClientCount = 0;

		
		private int _StartRoleNum = 0;

		
		private int _AllKilledRoleNum = 0;

		
		private long _SuiLastKillEmemyTime = 0L;

		
		private int _SuiKilledNum = 0;

		
		private long _TangLastKillEmemyTime = 0L;

		
		private int _TangKilledNum = 0;

		
		private int _SuiClientCount = 0;

		
		private int _TangClientCount = 0;

		
		private long[] BattleExpByLevels = null;

		
		private List<BattleManager.Award> _BattleAwardByScore = null;

		
		private Dictionary<int, int> _RoleLeaveJiFenDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _RoleLeaveSideDict = new Dictionary<int, int>();

		
		private Dictionary<int, long> _RoleLeaveTicksDict = new Dictionary<int, long>();

		
		private int LastSuiKilledNum = -1;

		
		private int LastTangKilledNum = -1;

		
		protected class Award
		{
			
			public int MinJiFen = 0;

			
			public int MaxJiFen = 200;

			
			public int ExpXiShu = 5000;

			
			public double MoJingXiShu = 4.0;

			
			public double ChengJiuXiShu = 1.0;

			
			public int MinExp = 3000;

			
			public int MaxExp = 600000;

			
			public int MinMoJing = 10;

			
			public int MaxMoJing = 20;

			
			public int MinChengJiu = 100;

			
			public int MaxChengJiu = 200;
		}
	}
}
