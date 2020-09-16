using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class KuaFuBossManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		
		public static KuaFuBossManager getInstance()
		{
			return KuaFuBossManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KuaFuBossManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1120, 1, 1, KuaFuBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1121, 1, 1, KuaFuBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1123, 1, 1, KuaFuBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 31, KuaFuBossManager.getInstance());
			GlobalEventSource.getInstance().registerListener(56, KuaFuBossManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 31, KuaFuBossManager.getInstance());
			GlobalEventSource.getInstance().removeListener(56, KuaFuBossManager.getInstance());
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1120:
				return this.ProcessKuaFuBossJoinCmd(client, nID, bytes, cmdParams);
			case 1121:
				return this.ProcessKuaFuBossEnterCmd(client, nID, bytes, cmdParams);
			case 1123:
				return this.ProcessGetKuaFuBossStateCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 56)
			{
				KillMonsterEventObject monsterDeadEvent = eventObject as KillMonsterEventObject;
				if (null != monsterDeadEvent)
				{
					this.OnKillMonster(monsterDeadEvent.getAttacker(), monsterDeadEvent.getMonster());
				}
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10001)
			{
				KuaFuNotifyEnterGameEvent e = eventObject as KuaFuNotifyEnterGameEvent;
				if (null != e)
				{
					KuaFuServerLoginData kuaFuServerLoginData = e.Arg as KuaFuServerLoginData;
					if (null != kuaFuServerLoginData)
					{
						lock (this.RuntimeData.Mutex)
						{
							this.RuntimeData.RoleIdKuaFuLoginDataDict[kuaFuServerLoginData.RoleId] = kuaFuServerLoginData;
							LogManager.WriteLog(LogTypes.Error, string.Format("通知角色ID={0}拥有进入跨服Boss资格,跨服GameID={1}", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						}
					}
					eventObject.Handled = true;
				}
			}
		}

		
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.MapBirthPointDict.Clear();
					fileName = "Config/ThroughServiceBossRebirth.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KuaFuBossBirthPoint item = new KuaFuBossBirthPoint();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item.BirthRadius = (int)Global.GetSafeAttributeLong(node, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[item.ID] = item;
					}
					this.RuntimeData.SceneDataDict.Clear();
					this.RuntimeData.LevelRangeSceneIdDict.Clear();
					this.RuntimeData.SceneDynMonsterDict.Clear();
					fileName = "Config/ThroughServiceBoss.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KuaFuBossSceneInfo sceneItem = new KuaFuBossSceneInfo();
						int id = (int)Global.GetSafeAttributeLong(node, "MapCode");
						int mapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						sceneItem.Id = id;
						sceneItem.MapCode = mapCode;
						sceneItem.MinLevel = (int)Global.GetSafeAttributeLong(node, "MinLevel");
						sceneItem.MaxLevel = (int)Global.GetSafeAttributeLong(node, "MaxLevel");
						sceneItem.MinZhuanSheng = (int)Global.GetSafeAttributeLong(node, "MinZhuanSheng");
						sceneItem.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(node, "MaxZhuanSheng");
						sceneItem.PrepareSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
						sceneItem.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(node, "WaitingEnterSecs");
						sceneItem.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
						sceneItem.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
						ConfigParser.ParseStrInt2(Global.GetSafeAttributeStr(node, "ApplyTime"), ref sceneItem.SignUpStartSecs, ref sceneItem.SignUpEndSecs, ',');
						sceneItem.SignUpStartSecs += sceneItem.SignUpEndSecs;
						if (!ConfigParser.ParserTimeRangeListWithDay(sceneItem.TimePoints, Global.GetSafeAttributeStr(node, "TimePoints"), true, '|', '-', ','))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, string.Format("读取{0}时间配置(TimePoints)出错", fileName), null, true);
						}
						for (int i = 0; i < sceneItem.TimePoints.Count; i++)
						{
							TimeSpan ts = new TimeSpan(sceneItem.TimePoints[i].Hours, sceneItem.TimePoints[i].Minutes, sceneItem.TimePoints[i].Seconds);
							sceneItem.SecondsOfDay.Add(ts.TotalSeconds);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, string.Format("地图配置中缺少{0}所需的地图:{1}", fileName, mapCode), null, true);
						}
						RangeKey range = new RangeKey(Global.GetUnionLevel(sceneItem.MinZhuanSheng, sceneItem.MinLevel, false), Global.GetUnionLevel(sceneItem.MaxZhuanSheng, sceneItem.MaxLevel, false), null);
						this.RuntimeData.LevelRangeSceneIdDict[range] = sceneItem;
						this.RuntimeData.SceneDataDict[id] = sceneItem;
					}
					fileName = "Config/ThroughServiceBossMonster.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						BattleDynamicMonsterItem item2 = new BattleDynamicMonsterItem();
						item2.Id = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.MapCode = (int)Global.GetSafeAttributeLong(node, "CodeID");
						item2.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonsterID");
						item2.PosX = (int)Global.GetSafeAttributeLong(node, "X");
						item2.PosY = (int)Global.GetSafeAttributeLong(node, "Y");
						item2.DelayBirthMs = (int)Global.GetSafeAttributeLong(node, "Time");
						item2.PursuitRadius = (int)Global.GetSafeAttributeLong(node, "PursuitRadius");
						item2.Num = (int)Global.GetSafeAttributeLong(node, "Num");
						item2.Radius = (int)Global.GetSafeAttributeLong(node, "Radius");
						List<BattleDynamicMonsterItem> itemList = null;
						if (!this.RuntimeData.SceneDynMonsterDict.TryGetValue(item2.MapCode, out itemList))
						{
							itemList = new List<BattleDynamicMonsterItem>();
							this.RuntimeData.SceneDynMonsterDict[item2.MapCode] = itemList;
						}
						itemList.Add(item2);
					}
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		
		private void TimerProc(object sender, EventArgs e)
		{
			bool notifyPrepareGame = false;
			bool notifyEnterGame = false;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				bool bInActiveTime = false;
				KuaFuBossSceneInfo sceneItem = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<KuaFuBossSceneInfo>();
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						double secs = sceneItem.SecondsOfDay[i] - now.TimeOfDay.TotalSeconds;
						bInActiveTime = true;
						if (!this.RuntimeData.PrepareGame)
						{
							if (secs > 0.0 && secs < (double)(sceneItem.SignUpEndSecs / 2))
							{
								LogManager.WriteLog(LogTypes.Error, "报名截止5分钟时间过半,通知跨服中心开始分配所有报名玩家的活动场次", null, true);
								this.RuntimeData.PrepareGame = true;
								notifyPrepareGame = true;
								break;
							}
						}
						else if (secs < 0.0)
						{
							LogManager.WriteLog(LogTypes.Error, "报名截止状态结束,可以通知已分配到场次的玩家进入游戏了", null, true);
							notifyEnterGame = true;
							this.RuntimeData.PrepareGame = false;
							break;
						}
					}
				}
				if (!bInActiveTime)
				{
					if (this.RuntimeData.RoleIdKuaFuLoginDataDict.Count > 0)
					{
						this.RuntimeData.RoleIdKuaFuLoginDataDict.Clear();
					}
					if (this.RuntimeData.RoleId2JoinGroup.Count > 0)
					{
						this.RuntimeData.RoleId2JoinGroup.Clear();
					}
				}
			}
			if (notifyPrepareGame)
			{
				LogManager.WriteLog(LogTypes.Error, "通知跨服中心开始分配所有报名玩家的活动场次", null, true);
				string cmd = string.Format("{0} {1} {2}", "GameState", 2, 6);
				YongZheZhanChangClient.getInstance().ExecuteCommand(cmd);
			}
			if (notifyEnterGame)
			{
				lock (this.RuntimeData.Mutex)
				{
					foreach (KuaFuServerLoginData kuaFuServerLoginData in this.RuntimeData.RoleIdKuaFuLoginDataDict.Values)
					{
						this.RuntimeData.NotifyRoleEnterDict.Add(kuaFuServerLoginData.RoleId, kuaFuServerLoginData);
					}
				}
			}
			List<KuaFuServerLoginData> list = null;
			lock (this.RuntimeData.Mutex)
			{
				int count = this.RuntimeData.NotifyRoleEnterDict.Count;
				if (count > 0)
				{
					list = new List<KuaFuServerLoginData>();
					KuaFuServerLoginData kuaFuServerLoginData = this.RuntimeData.NotifyRoleEnterDict.First<KeyValuePair<int, KuaFuServerLoginData>>().Value;
					foreach (KeyValuePair<int, KuaFuServerLoginData> kv in this.RuntimeData.NotifyRoleEnterDict)
					{
						if (kv.Key % 15 == kuaFuServerLoginData.RoleId % 15)
						{
							list.Add(kv.Value);
						}
					}
					foreach (KuaFuServerLoginData data in list)
					{
						this.RuntimeData.NotifyRoleEnterDict.Remove(data.RoleId);
					}
				}
			}
			if (null != list)
			{
				foreach (KuaFuServerLoginData kuaFuServerLoginData in list)
				{
					GameClient client = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
					if (null != client)
					{
						client.sendCmd<int>(1121, 1, false);
					}
				}
			}
		}

		
		public bool ProcessKuaFuBossJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuBossSceneInfo sceneItem = null;
				KuaFuBossGameStates state = KuaFuBossGameStates.None;
				int result;
				if (!this.CheckMap(client))
				{
					result = -21;
				}
				else
				{
					result = this.CheckCondition(client, ref sceneItem, ref state);
				}
				if (state != KuaFuBossGameStates.SignUp)
				{
					result = -2001;
				}
				else if (this.RuntimeData.RoleId2JoinGroup.ContainsKey(client.ClientData.RoleID))
				{
					result = -12;
				}
				if (result >= 0)
				{
					int gropuId = sceneItem.Id;
					result = YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 6, gropuId, client.ClientData.CombatForce);
					if (result > 0)
					{
						this.RuntimeData.RoleId2JoinGroup[client.ClientData.RoleID] = gropuId;
						client.ClientData.SignUpGameType = 6;
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool CheckMap(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return sceneType == SceneUIClasses.Normal;
		}

		
		private int CheckCondition(GameClient client, ref KuaFuBossSceneInfo sceneItem, ref KuaFuBossGameStates state)
		{
			int result = 0;
			sceneItem = null;
			if (!this.IsGongNengOpened(client, true))
			{
				result = -13;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.LevelRangeSceneIdDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out sceneItem))
					{
						return -12;
					}
				}
				result = -2001;
				DateTime now = TimeUtil.NowDateTime();
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
					{
						if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
						{
							if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs)
							{
								state = KuaFuBossGameStates.None;
								result = -2001;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpEndSecs)
							{
								state = KuaFuBossGameStates.SignUp;
								result = 1;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i])
							{
								state = KuaFuBossGameStates.Wait;
								result = 1;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
							{
								state = KuaFuBossGameStates.Start;
								result = 1;
							}
							else
							{
								state = KuaFuBossGameStates.None;
								result = -2001;
							}
							break;
						}
					}
				}
			}
			return result;
		}

		
		private TimeSpan GetStartTime(int sceneId)
		{
			KuaFuBossSceneInfo sceneItem = null;
			TimeSpan startTime = TimeSpan.MinValue;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.SceneDataDict.TryGetValue(sceneId, out sceneItem))
				{
					goto IL_153;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						startTime = TimeSpan.FromSeconds(sceneItem.SecondsOfDay[i]);
						break;
					}
				}
			}
			IL_153:
			if (startTime < TimeSpan.Zero)
			{
				startTime = now.TimeOfDay;
			}
			return startTime;
		}

		
		public bool ProcessGetKuaFuBossStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuBossSceneInfo sceneItem = null;
				KuaFuBossGameStates timeState = KuaFuBossGameStates.None;
				int result = 0;
				int groupId = 0;
				this.RuntimeData.RoleId2JoinGroup.TryGetValue(client.ClientData.RoleID, out groupId);
				this.CheckCondition(client, ref sceneItem, ref timeState);
				if (groupId > 0)
				{
					if (timeState >= KuaFuBossGameStates.SignUp && timeState <= KuaFuBossGameStates.Wait)
					{
						int state = YongZheZhanChangClient.getInstance().GetKuaFuRoleState(client.ClientData.RoleID);
						if (state >= 1)
						{
							result = 2;
						}
						else
						{
							result = 5;
						}
					}
					else if (timeState == KuaFuBossGameStates.Start)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.ContainsKey(client.ClientData.RoleID))
						{
							result = 3;
						}
					}
				}
				else if (timeState == KuaFuBossGameStates.SignUp)
				{
					result = 1;
				}
				else if (timeState == KuaFuBossGameStates.Wait || timeState == KuaFuBossGameStates.Start)
				{
					result = 5;
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessKuaFuBossEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuBossSceneInfo sceneItem = null;
				KuaFuBossGameStates state = KuaFuBossGameStates.None;
				int result = 0;
				if (!this.CheckMap(client))
				{
					result = -21;
				}
				else
				{
					result = this.CheckCondition(client, ref sceneItem, ref state);
				}
				if (state == KuaFuBossGameStates.Start)
				{
					KuaFuServerLoginData kuaFuServerLoginData = null;
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.TryGetValue(client.ClientData.RoleID, out kuaFuServerLoginData))
						{
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = kuaFuServerLoginData.RoleId;
								clientKuaFuServerLoginData.GameId = kuaFuServerLoginData.GameId;
								clientKuaFuServerLoginData.GameType = kuaFuServerLoginData.GameType;
								clientKuaFuServerLoginData.EndTicks = kuaFuServerLoginData.EndTicks;
								clientKuaFuServerLoginData.ServerId = kuaFuServerLoginData.ServerId;
								clientKuaFuServerLoginData.ServerIp = kuaFuServerLoginData.ServerIp;
								clientKuaFuServerLoginData.ServerPort = kuaFuServerLoginData.ServerPort;
								clientKuaFuServerLoginData.FuBenSeqId = kuaFuServerLoginData.FuBenSeqId;
							}
						}
						else
						{
							result = -11000;
						}
					}
					if (result >= 0)
					{
						result = YongZheZhanChangClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.EnterGame, false);
						if (result >= 0)
						{
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
						}
					}
				}
				else
				{
					result = -2001;
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int result;
			lock (this.RuntimeData.Mutex)
			{
				KuaFuBossBirthPoint birthPoint = null;
				int side = Global.GetRandomNumber(1, this.RuntimeData.MapBirthPointDict.Count);
				if (!this.RuntimeData.MapBirthPointDict.TryGetValue(side, out birthPoint))
				{
					birthPoint = this.RuntimeData.MapBirthPointDict.First<KeyValuePair<int, KuaFuBossBirthPoint>>().Value;
				}
				posX = birthPoint.PosX;
				posY = birthPoint.PosY;
				result = side;
			}
			return result;
		}

		
		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			YongZheZhanChangFuBenData fuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
				{
					fuBenData = null;
				}
				else if (fuBenData.State >= GameFuBenState.End)
				{
					return false;
				}
			}
			if (null == fuBenData)
			{
				YongZheZhanChangFuBenData newFuBenData = YongZheZhanChangClient.getInstance().GetKuaFuFuBenData((int)kuaFuServerLoginData.GameId);
				if (newFuBenData == null || newFuBenData.State == GameFuBenState.End)
				{
					LogManager.WriteLog(LogTypes.Error, ("获取不到有效的副本数据," + newFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
					{
						fuBenData = newFuBenData;
						fuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenItemData[fuBenData.GameId] = fuBenData;
					}
				}
			}
			KuaFuFuBenRoleData kuaFuFuBenRoleData;
			bool result;
			if (!fuBenData.RoleDict.TryGetValue(client.ClientData.RoleID, out kuaFuFuBenRoleData))
			{
				result = false;
			}
			else
			{
				int posX;
				int posY;
				this.GetBirthPoint(client, out posX, out posY);
				lock (this.RuntimeData.Mutex)
				{
					kuaFuServerLoginData.FuBenSeqId = fuBenData.SequenceId;
					KuaFuBossSceneInfo sceneInfo;
					if (!this.RuntimeData.SceneDataDict.TryGetValue(fuBenData.GroupIndex, out sceneInfo))
					{
						return false;
					}
					client.ClientData.MapCode = sceneInfo.MapCode;
				}
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
				result = true;
			}
			return result;
		}

		
		public bool ClientRelive(GameClient client)
		{
			int toPosX;
			int toPosY;
			this.GetBirthPoint(client, out toPosX, out toPosY);
			client.ClientData.CurrentLifeV = client.ClientData.LifeV;
			client.ClientData.CurrentMagicV = client.ClientData.MagicV;
			client.ClientData.MoveAndActionNum = 0;
			GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, toPosX, toPosY, -1);
			Global.ClientRealive(client, toPosX, toPosY, -1);
			return true;
		}

		
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("KuaFuBoss") && GlobalNew.IsGongNengOpened(client, GongNengIDs.KuaFuBoss, hint);
		}

		
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.KuaFuBoss)
			{
				int fuBenSeqId = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				int roleId = client.ClientData.RoleID;
				int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
				DateTime now = TimeUtil.NowDateTime();
				KuaFuBossScene scene = null;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
					{
						KuaFuBossSceneInfo sceneInfo = null;
						YongZheZhanChangFuBenData fuBenData;
						if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
						{
							LogManager.WriteLog(LogTypes.Error, "跨服Boss没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
						}
						if (!this.RuntimeData.SceneDataDict.TryGetValue(fuBenData.GroupIndex, out sceneInfo))
						{
							LogManager.WriteLog(LogTypes.Error, "跨服Boss没有为副本找到对应的档位数据,ID:" + fuBenData.GroupIndex, null, true);
						}
						scene = new KuaFuBossScene();
						scene.CopyMap = copyMap;
						scene.CleanAllInfo();
						scene.GameId = gameId;
						scene.m_nMapCode = mapCode;
						scene.CopyMapId = copyMap.CopyMapID;
						scene.FuBenSeqId = fuBenSeqId;
						scene.m_nPlarerCount = 1;
						scene.SceneInfo = sceneInfo;
						DateTime startTime = now.Date.Add(this.GetStartTime(sceneInfo.Id));
						scene.StartTimeTicks = startTime.Ticks / 10000L;
						scene.GameStatisticalData.GameId = gameId;
						this.SceneDict[fuBenSeqId] = scene;
						List<BattleDynamicMonsterItem> dynMonsterList;
						if (this.RuntimeData.SceneDynMonsterDict.TryGetValue(mapCode, out dynMonsterList))
						{
							scene.DynMonsterList = dynMonsterList;
						}
					}
					else
					{
						scene.m_nPlarerCount++;
					}
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(scene.SceneInfo.TotalSecs * 1000));
				}
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(copyMap.MapCode, out gameMap))
				{
					scene.MapGridWidth = gameMap.MapGridWidth;
					scene.MapGridHeight = gameMap.MapGridHeight;
				}
				YongZheZhanChangClient.getInstance().GameFuBenRoleChangeState(roleId, 5, 0, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.KuaFuBoss)
			{
				lock (this.RuntimeData.Mutex)
				{
					KuaFuBossScene KuaFuBossScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out KuaFuBossScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		private void NotifySceneData(KuaFuBossScene scene)
		{
			if (null != scene)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<KuaFuBossSceneStateData>(1122, scene.SceneStateData, scene.CopyMap);
			}
		}

		
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				KuaFuBossScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<KuaFuBossSceneStateData>(1122, scene.SceneStateData, false);
					}
				}
			}
		}

		
		public void CheckCreateDynamicMonster(KuaFuBossScene scene, long nowMs)
		{
			lock (this.RuntimeData.Mutex)
			{
				List<BattleDynamicMonsterItem> dynMonsterList = scene.DynMonsterList;
				if (dynMonsterList != null)
				{
					foreach (BattleDynamicMonsterItem item in dynMonsterList)
					{
						if (!scene.DynMonsterSet.Contains(item.Id))
						{
							if (nowMs - scene.m_lBeginTime >= (long)item.DelayBirthMs)
							{
								Monster seedMonster = GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item.MonsterID, scene.CopyMap.CopyMapID, item.Num, item.PosX / scene.MapGridWidth, item.PosY / scene.MapGridHeight, item.Radius, item.PursuitRadius, SceneUIClasses.KuaFuBoss, null, null);
								scene.DynMonsterSet.Add(item.Id);
								if (null != seedMonster)
								{
									if (seedMonster.MonsterType == 401)
									{
										scene.SceneStateData.TotalBossNum += item.Num;
										scene.SceneStateData.BossNum += item.Num;
									}
									else
									{
										scene.SceneStateData.TotalNormalNum += item.Num;
										scene.SceneStateData.MonsterNum += item.Num;
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void OnKillMonster(GameClient client, Monster monster)
		{
			if (monster.ManagerType == SceneUIClasses.KuaFuBoss)
			{
				KuaFuBossScene scene;
				lock (this.RuntimeData.Mutex)
				{
					if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
					{
						if (scene.m_eStatus != GameSceneStatuses.STATUS_BEGIN)
						{
							return;
						}
						scene.GameStatisticalData.MonsterDieTimeList.Add(monster.MonsterInfo.ExtensionID);
						scene.GameStatisticalData.MonsterDieTimeList.Add(scene.ElapsedSeconds);
						if (monster.MonsterType == 401)
						{
							scene.SceneStateData.BossNum = Math.Max(0, scene.SceneStateData.BossNum - 1);
							if (null != client)
							{
								string msgText = string.Format(GLang.GetLang(401, new object[0]), Global.FormatRoleName4(client), monster.MonsterInfo.VSName);
								GameManager.ClientMgr.BroadSpecialCopyMapMsg(scene.CopyMap, msgText, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
							}
						}
						else
						{
							scene.SceneStateData.MonsterNum = Math.Max(0, scene.SceneStateData.MonsterNum - 1);
						}
					}
				}
				this.NotifySceneData(scene);
			}
		}

		
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= KuaFuBossManager.NextHeartBeatTicks)
			{
				KuaFuBossManager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (KuaFuBossScene scene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int nID = scene.FuBenSeqId;
						int nCopyID = scene.CopyMapId;
						int nMapCodeID = scene.m_nMapCode;
						if (nID >= 0 && nCopyID >= 0 && nMapCodeID >= 0)
						{
							CopyMap copyMap = scene.CopyMap;
							DateTime now = TimeUtil.NowDateTime();
							long ticks = TimeUtil.NOW();
							if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								if (ticks >= scene.StartTimeTicks)
								{
									scene.m_lPrepareTime = scene.StartTimeTicks;
									scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.PrepareSecs * 1000);
									scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
									scene.StateTimeData.GameType = 6;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (ticks >= scene.m_lBeginTime)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.m_lEndTime = scene.m_lBeginTime + (long)(scene.SceneInfo.FightingSecs * 1000);
									scene.StateTimeData.GameType = 6;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_END;
									scene.m_lLeaveTime = scene.m_lEndTime + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
									scene.StateTimeData.GameType = 6;
									scene.StateTimeData.State = 5;
									scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									this.NotifySceneData(scene);
								}
								else
								{
									scene.ElapsedSeconds = (int)Math.Min((nowTicks - scene.m_lBeginTime) / 1000L, (long)scene.SceneInfo.TotalSecs);
									this.CheckCreateDynamicMonster(scene, ticks);
									if (nowTicks > scene.NextNotifySceneStateDataTicks)
									{
										scene.NextNotifySceneStateDataTicks = nowTicks + 3000L;
										this.NotifySceneData(scene);
									}
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
								scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								YongZheZhanChangClient.getInstance().PushGameResultData(scene.GameStatisticalData);
								YongZheZhanChangClient.getInstance().GameFuBenChangeState(scene.GameId, GameFuBenState.End, now);
								YongZheZhanChangFuBenData fuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
								{
									fuBenData.State = GameFuBenState.End;
									LogManager.WriteLog(LogTypes.Error, string.Format("跨服Boss跨服副本GameID={0},战斗结束", fuBenData.GameId), null, true);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= scene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(scene.m_lLeaveTime);
									scene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
									try
									{
										List<GameClient> objsList = copyMap.GetClientsList();
										if (objsList != null && objsList.Count > 0)
										{
											for (int i = 0; i < objsList.Count; i++)
											{
												GameClient c = objsList[i];
												if (c != null)
												{
													KuaFuManager.getInstance().GotoLastMap(c);
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "跨服Boss系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				KuaFuBossScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					scene.m_nPlarerCount--;
				}
			}
		}

		
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		
		public const SceneUIClasses ManagerType = SceneUIClasses.KuaFuBoss;

		
		private static KuaFuBossManager instance = new KuaFuBossManager();

		
		public KuaFuBossData RuntimeData = new KuaFuBossData();

		
		public ConcurrentDictionary<int, KuaFuBossScene> SceneDict = new ConcurrentDictionary<int, KuaFuBossScene>();

		
		private static long NextHeartBeatTicks = 0L;
	}
}
