using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000819 RID: 2073
	public class YongZheZhanChangManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		// Token: 0x06003A8D RID: 14989 RVA: 0x0031A038 File Offset: 0x00318238
		public static YongZheZhanChangManager getInstance()
		{
			return YongZheZhanChangManager.instance;
		}

		// Token: 0x06003A8E RID: 14990 RVA: 0x0031A050 File Offset: 0x00318250
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06003A8F RID: 14991 RVA: 0x0031A074 File Offset: 0x00318274
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("YongZheZhanChangManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		// Token: 0x06003A90 RID: 14992 RVA: 0x0031A0B4 File Offset: 0x003182B4
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1100, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1101, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1103, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1108, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1102, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 27, YongZheZhanChangManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10002, 27, YongZheZhanChangManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, YongZheZhanChangManager.getInstance());
			return true;
		}

		// Token: 0x06003A91 RID: 14993 RVA: 0x0031A180 File Offset: 0x00318380
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 27, YongZheZhanChangManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10002, 27, YongZheZhanChangManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, YongZheZhanChangManager.getInstance());
			return true;
		}

		// Token: 0x06003A92 RID: 14994 RVA: 0x0031A1D4 File Offset: 0x003183D4
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06003A93 RID: 14995 RVA: 0x0031A1E8 File Offset: 0x003183E8
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06003A94 RID: 14996 RVA: 0x0031A1FC File Offset: 0x003183FC
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1100:
				return this.ProcessYongZheZhanChangJoinCmd(client, nID, bytes, cmdParams);
			case 1101:
				return this.ProcessYongZheZhanChangEnterCmd(client, nID, bytes, cmdParams);
			case 1102:
				return this.ProcessGetYongZheZhanChangAwardInfoCmd(client, nID, bytes, cmdParams);
			case 1103:
				return this.ProcessGetYongZheZhanChangStateCmd(client, nID, bytes, cmdParams);
			case 1108:
				return this.ProcessGetYongZheZhanChangAwardCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		// Token: 0x06003A95 RID: 14997 RVA: 0x0031A28C File Offset: 0x0031848C
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEvent = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEvent)
				{
					if (playerDeadEvent.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEvent.getAttackerRole(), playerDeadEvent.getPlayer());
					}
				}
			}
		}

		// Token: 0x06003A96 RID: 14998 RVA: 0x0031A2E8 File Offset: 0x003184E8
		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 10001:
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
							LogManager.WriteLog(LogTypes.Error, string.Format("通知角色ID={0}拥有进入勇者战场资格,跨服GameID={1}", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10002:
			{
				CaiJiEventObject e2 = eventObject as CaiJiEventObject;
				if (null != e2)
				{
					GameClient client = e2.Source as GameClient;
					Monster monster = e2.Target as Monster;
					this.OnCaiJiFinish(client, monster);
					eventObject.Handled = true;
					eventObject.Result = true;
				}
				break;
			}
			}
		}

		// Token: 0x06003A97 RID: 14999 RVA: 0x0031A424 File Offset: 0x00318624
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.BattleCrystalMonsterDict.Clear();
					fileName = "Config/BattleCrystalMonster.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						BattleCrystalMonsterItem item = new BattleCrystalMonsterItem();
						item.Id = (int)Global.GetSafeAttributeLong(node, "ID");
						item.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonsterID");
						item.GatherTime = (int)Global.GetSafeAttributeLong(node, "GatherTime");
						item.BattleJiFen = (int)Global.GetSafeAttributeLong(node, "BattleJiFen");
						item.PosX = (int)Global.GetSafeAttributeLong(node, "X");
						item.PosY = (int)Global.GetSafeAttributeLong(node, "Y");
						item.FuHuoTime = (int)Global.GetSafeAttributeLong(node, "FuHuoTime") * 1000;
						this.RuntimeData.BattleCrystalMonsterDict[item.Id] = item;
					}
					this.RuntimeData.MapBirthPointDict.Clear();
					fileName = "Config/ThroughServiceRebirth.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						YongZheZhanChangBirthPoint item2 = new YongZheZhanChangBirthPoint();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item2.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item2.BirthRadius = (int)Global.GetSafeAttributeLong(node, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[item2.ID] = item2;
					}
					this.RuntimeData.SceneDataDict.Clear();
					this.RuntimeData.LevelRangeSceneIdDict.Clear();
					fileName = "Config/ThroughServiceBattle.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						YongZheZhanChangSceneInfo sceneItem = new YongZheZhanChangSceneInfo();
						int id = (int)Global.GetSafeAttributeLong(node, "Group");
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
					fileName = "Config/ThroughServiceBattleAward.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						int id = (int)Global.GetSafeAttributeLong(node, "MapCode");
						YongZheZhanChangSceneInfo sceneItem;
						if (this.RuntimeData.SceneDataDict.TryGetValue(id, out sceneItem))
						{
							sceneItem.Exp = Global.GetSafeAttributeLong(node, "Exp");
							sceneItem.BandJinBi = (int)Global.GetSafeAttributeLong(node, "BandJinBi");
							ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "WinGoods"), ref sceneItem.WinAwardsItemList, '|', ',');
							ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "LoseGoods"), ref sceneItem.LoseAwardsItemList, '|', ',');
						}
					}
					this.RuntimeData.SceneDynMonsterDict.Clear();
					fileName = "Config/BattleMonster.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						BattleDynamicMonsterItem item3 = new BattleDynamicMonsterItem();
						item3.Id = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.MapCode = (int)Global.GetSafeAttributeLong(node, "CodeID");
						item3.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonsterID");
						item3.PosX = (int)Global.GetSafeAttributeLong(node, "X");
						item3.PosY = (int)Global.GetSafeAttributeLong(node, "Y");
						item3.DelayBirthMs = (int)Global.GetSafeAttributeLong(node, "Time");
						List<BattleDynamicMonsterItem> itemList = null;
						if (!this.RuntimeData.SceneDynMonsterDict.TryGetValue(item3.MapCode, out itemList))
						{
							itemList = new List<BattleDynamicMonsterItem>();
							this.RuntimeData.SceneDynMonsterDict[item3.MapCode] = itemList;
						}
						itemList.Add(item3);
					}
					this.RuntimeData.WarriorBattleBOssLastAttack = (int)GameManager.systemParamsList.GetParamValueIntByName("WarriorBattleBOssLastAttack", -1);
					this.RuntimeData.WarriorBattleLowestJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("WarriorBattleLowestJiFen", -1);
					double[] doubalArray = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WarriorBattleBossAttack", ',');
					if (doubalArray.Length == 2)
					{
						this.RuntimeData.WarriorBattleBossAttackPercent = doubalArray[0];
						this.RuntimeData.WarriorBattleBossAttackScore = (int)doubalArray[1];
					}
					int[] intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("WarriorBattleUltraKill", ',');
					if (doubalArray.Length == 2)
					{
						this.RuntimeData.WarriorBattleUltraKillParam1 = intArray[0];
						this.RuntimeData.WarriorBattleUltraKillParam2 = intArray[1];
						this.RuntimeData.WarriorBattleUltraKillParam3 = intArray[2];
						this.RuntimeData.WarriorBattleUltraKillParam4 = intArray[3];
					}
					intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("WarriorBattleShutDown", ',');
					if (doubalArray.Length == 2)
					{
						this.RuntimeData.WarriorBattleShutDownParam1 = intArray[0];
						this.RuntimeData.WarriorBattleShutDownParam2 = intArray[1];
						this.RuntimeData.WarriorBattleShutDownParam3 = intArray[2];
						this.RuntimeData.WarriorBattleShutDownParam4 = intArray[3];
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

		// Token: 0x06003A98 RID: 15000 RVA: 0x0031AD9C File Offset: 0x00318F9C
		private void TimerProc(object sender, EventArgs e)
		{
			bool notifyPrepareGame = false;
			bool notifyEnterGame = false;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				bool bInActiveTime = false;
				YongZheZhanChangSceneInfo sceneItem = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<YongZheZhanChangSceneInfo>();
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
				string cmd = string.Format("{0} {1} {2}", "GameState", 2, 5);
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
						client.sendCmd<int>(1101, 1, false);
					}
				}
			}
		}

		// Token: 0x06003A99 RID: 15001 RVA: 0x0031B2FC File Offset: 0x003194FC
		public bool ProcessYongZheZhanChangJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
				{
					YongZheZhanChangSceneInfo sceneItem = null;
					YongZheZhanChangGameStates state = YongZheZhanChangGameStates.None;
					if (!this.CheckMap(client))
					{
						result = -21;
					}
					else
					{
						result = this.CheckCondition(client, ref sceneItem, ref state);
					}
					if (state != YongZheZhanChangGameStates.SignUp)
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
						result = YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 5, gropuId, client.ClientData.CombatForce);
						if (result > 0)
						{
							this.RuntimeData.RoleId2JoinGroup[client.ClientData.RoleID] = gropuId;
							client.ClientData.SignUpGameType = 5;
						}
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

		// Token: 0x06003A9A RID: 15002 RVA: 0x0031B458 File Offset: 0x00319658
		private bool CheckMap(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return sceneType == SceneUIClasses.Normal;
		}

		// Token: 0x06003A9B RID: 15003 RVA: 0x0031B48C File Offset: 0x0031968C
		private int CheckCondition(GameClient client, ref YongZheZhanChangSceneInfo sceneItem, ref YongZheZhanChangGameStates state)
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
								state = YongZheZhanChangGameStates.None;
								result = -2001;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpEndSecs)
							{
								state = YongZheZhanChangGameStates.SignUp;
								result = 1;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i])
							{
								state = YongZheZhanChangGameStates.Wait;
								result = 1;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
							{
								state = YongZheZhanChangGameStates.Start;
								result = 1;
							}
							else
							{
								state = YongZheZhanChangGameStates.None;
								result = -2001;
							}
							break;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06003A9C RID: 15004 RVA: 0x0031B71C File Offset: 0x0031991C
		private TimeSpan GetStartTime(int sceneId)
		{
			YongZheZhanChangSceneInfo sceneItem = null;
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

		// Token: 0x06003A9D RID: 15005 RVA: 0x0031B8C0 File Offset: 0x00319AC0
		public bool ProcessGetYongZheZhanChangAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int err = 1;
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
				{
					return false;
				}
				string awardsInfo = Global.GetRoleParamByName(client, "YongZheZhanChangAwards");
				if (!string.IsNullOrEmpty(awardsInfo))
				{
					int lastGroupId = 0;
					int score = 0;
					int success = 0;
					string mvpInfo = Global.GetRoleParamByName(client, "44");
					int mvprid = 0;
					List<string> mvpParamList = Global.StringToList(mvpInfo, '&');
					if (mvpParamList.Count != 4 && !string.IsNullOrEmpty(mvpInfo))
					{
						byte[] strBytes = Convert.FromBase64String(mvpInfo);
						mvpInfo = new UTF8Encoding().GetString(strBytes);
						mvpParamList = Global.StringToList(mvpInfo, '&');
					}
					ConfigParser.ParseStrInt3(awardsInfo, ref lastGroupId, ref success, ref score, ',');
					List<int> awardsParamList = Global.StringToIntList(awardsInfo, ',');
					lastGroupId = awardsParamList[0];
					bool clear = true;
					if (awardsParamList.Count >= 5 && lastGroupId > 0)
					{
						success = awardsParamList[1];
						score = awardsParamList[2];
						int sideScore = awardsParamList[3];
						int sideScore2 = awardsParamList[4];
						if (mvpParamList.Count >= 4)
						{
							mvprid = Global.SafeConvertToInt32(mvpParamList[0]);
							string mvpname = mvpParamList[1];
							int mvpocc = Global.SafeConvertToInt32(mvpParamList[2]);
							int mvpsex = Global.SafeConvertToInt32(mvpParamList[3]);
						}
						YongZheZhanChangSceneInfo lastSceneItem = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(lastGroupId, out lastSceneItem))
						{
							err = this.GiveRoleAwards(client, success, score, lastSceneItem);
							if (err < 0)
							{
								clear = false;
							}
							if (client.ClientData.RoleID == mvprid)
							{
								GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_YongZheZhanChangMVP, new int[0]));
							}
						}
					}
					if (clear)
					{
						Global.SaveRoleParamsStringToDB(client, "YongZheZhanChangAwards", this.RuntimeData.RoleParamsAwardsDefaultString, true);
						Global.SaveRoleParamsStringToDB(client, "44", "", true);
					}
					client.sendCmd<int>(nID, err, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		// Token: 0x06003A9E RID: 15006 RVA: 0x0031BB30 File Offset: 0x00319D30
		public bool ProcessGetYongZheZhanChangAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
				{
					return false;
				}
				string awardsInfo = Global.GetRoleParamByName(client, "YongZheZhanChangAwards");
				if (!string.IsNullOrEmpty(awardsInfo))
				{
					int lastGroupId = 0;
					int score = 0;
					int success = 0;
					string mvpInfo = Global.GetRoleParamByName(client, "44");
					int mvprid = 0;
					string mvpname = "";
					int mvpocc = 0;
					int mvpsex = 0;
					List<string> mvpParamList = Global.StringToList(mvpInfo, '&');
					if (mvpParamList.Count != 4 && !string.IsNullOrEmpty(mvpInfo))
					{
						byte[] strBytes = Convert.FromBase64String(mvpInfo);
						mvpInfo = new UTF8Encoding().GetString(strBytes);
						mvpParamList = Global.StringToList(mvpInfo, '&');
					}
					ConfigParser.ParseStrInt3(awardsInfo, ref lastGroupId, ref success, ref score, ',');
					List<int> awardsParamList = Global.StringToIntList(awardsInfo, ',');
					lastGroupId = awardsParamList[0];
					bool clear = true;
					if (awardsParamList.Count >= 5 && lastGroupId > 0)
					{
						success = awardsParamList[1];
						score = awardsParamList[2];
						int sideScore = awardsParamList[3];
						int sideScore2 = awardsParamList[4];
						if (mvpParamList.Count >= 4)
						{
							mvprid = Global.SafeConvertToInt32(mvpParamList[0]);
							mvpname = mvpParamList[1];
							mvpocc = Global.SafeConvertToInt32(mvpParamList[2]);
							mvpsex = Global.SafeConvertToInt32(mvpParamList[3]);
						}
						YongZheZhanChangSceneInfo lastSceneItem = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(lastGroupId, out lastSceneItem))
						{
							if (score >= this.RuntimeData.WarriorBattleLowestJiFen)
							{
								clear = false;
							}
							this.NtfCanGetAward(client, success, score, lastSceneItem, sideScore, sideScore2, mvprid, mvpname, mvpocc, mvpsex);
						}
					}
					if (clear)
					{
						Global.SaveRoleParamsStringToDB(client, "YongZheZhanChangAwards", this.RuntimeData.RoleParamsAwardsDefaultString, true);
						Global.SaveRoleParamsStringToDB(client, "44", "", true);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		// Token: 0x06003A9F RID: 15007 RVA: 0x0031BD74 File Offset: 0x00319F74
		public bool ProcessGetYongZheZhanChangStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string awardsInfo = Global.GetRoleParamByName(client, "YongZheZhanChangAwards");
				if (!string.IsNullOrEmpty(awardsInfo))
				{
					int lastGroupId = 0;
					int score = 0;
					int success = 0;
					ConfigParser.ParseStrInt3(awardsInfo, ref lastGroupId, ref success, ref score, ',');
					if (lastGroupId > 0)
					{
						YongZheZhanChangSceneInfo lastSceneItem = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(lastGroupId, out lastSceneItem))
						{
							client.sendCmd<int>(nID, 4, false);
							return true;
						}
					}
				}
				YongZheZhanChangSceneInfo sceneItem = null;
				YongZheZhanChangGameStates timeState = YongZheZhanChangGameStates.None;
				int result = 0;
				int groupId = 0;
				this.RuntimeData.RoleId2JoinGroup.TryGetValue(client.ClientData.RoleID, out groupId);
				this.CheckCondition(client, ref sceneItem, ref timeState);
				if (groupId > 0)
				{
					if (timeState >= YongZheZhanChangGameStates.SignUp && timeState <= YongZheZhanChangGameStates.Wait)
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
					else if (timeState == YongZheZhanChangGameStates.Start)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.ContainsKey(client.ClientData.RoleID))
						{
							result = 3;
						}
					}
				}
				else if (timeState == YongZheZhanChangGameStates.SignUp)
				{
					result = 1;
				}
				else if (timeState == YongZheZhanChangGameStates.Wait || timeState == YongZheZhanChangGameStates.Start)
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

		// Token: 0x06003AA0 RID: 15008 RVA: 0x0031BF40 File Offset: 0x0031A140
		public bool ProcessYongZheZhanChangEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
				{
					client.sendCmd<int>(nID, result, false);
					return true;
				}
				YongZheZhanChangSceneInfo sceneItem = null;
				YongZheZhanChangGameStates state = YongZheZhanChangGameStates.None;
				if (!this.CheckMap(client))
				{
					result = -21;
				}
				else
				{
					result = this.CheckCondition(client, ref sceneItem, ref state);
				}
				if (state == YongZheZhanChangGameStates.Start)
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

		// Token: 0x06003AA1 RID: 15009 RVA: 0x0031C15C File Offset: 0x0031A35C
		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangBirthPoint birthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(side, out birthPoint))
				{
					posX = birthPoint.PosX;
					posY = birthPoint.PosY;
					return side;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		// Token: 0x06003AA2 RID: 15010 RVA: 0x0031C1F8 File Offset: 0x0031A3F8
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
				client.ClientData.BattleWhichSide = kuaFuFuBenRoleData.Side;
				int posX;
				int posY;
				int side = this.GetBirthPoint(client, out posX, out posY);
				if (side <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, "无法获取有效的阵营和出生点,进入跨服失败,side=" + side, null, true);
					result = false;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						kuaFuServerLoginData.FuBenSeqId = fuBenData.SequenceId;
						YongZheZhanChangSceneInfo sceneInfo;
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
			}
			return result;
		}

		// Token: 0x06003AA3 RID: 15011 RVA: 0x0031C4B8 File Offset: 0x0031A6B8
		public bool ClientRelive(GameClient client)
		{
			int toPosX;
			int toPosY;
			int side = this.GetBirthPoint(client, out toPosX, out toPosY);
			bool result;
			if (side <= 0)
			{
				result = false;
			}
			else
			{
				client.ClientData.CurrentLifeV = client.ClientData.LifeV;
				client.ClientData.CurrentMagicV = client.ClientData.MagicV;
				client.ClientData.MoveAndActionNum = 0;
				GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, toPosX, toPosY, -1);
				Global.ClientRealive(client, toPosX, toPosY, -1);
				result = true;
			}
			return result;
		}

		// Token: 0x06003AA4 RID: 15012 RVA: 0x0031C558 File Offset: 0x0031A758
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("YongZheZhanChang") && GlobalNew.IsGongNengOpened(client, GongNengIDs.YongZheZhanChang, hint);
		}

		// Token: 0x06003AA5 RID: 15013 RVA: 0x0031C5A0 File Offset: 0x0031A7A0
		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			BattleCrystalMonsterItem tag = (monster != null) ? (monster.Tag as BattleCrystalMonsterItem) : null;
			int result;
			if (tag == null)
			{
				result = -200;
			}
			else
			{
				result = tag.GatherTime;
			}
			return result;
		}

		// Token: 0x06003AA6 RID: 15014 RVA: 0x0031C5E0 File Offset: 0x0031A7E0
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.YongZheZhanChang)
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
				{
					result = false;
				}
				else
				{
					int fuBenSeqId = copyMap.FuBenSeqID;
					int mapCode = copyMap.MapCode;
					int roleId = client.ClientData.RoleID;
					int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime now = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						YongZheZhanChangScene scene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
						{
							YongZheZhanChangSceneInfo sceneInfo = null;
							YongZheZhanChangFuBenData fuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
							{
								LogManager.WriteLog(LogTypes.Error, "勇者战场没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							if (!this.RuntimeData.SceneDataDict.TryGetValue(fuBenData.GroupIndex, out sceneInfo))
							{
								LogManager.WriteLog(LogTypes.Error, "勇者战场没有为副本找到对应的档位数据,ID:" + fuBenData.GroupIndex, null, true);
							}
							scene = new YongZheZhanChangScene();
							scene.CopyMap = copyMap;
							scene.CleanAllInfo();
							scene.GameId = gameId;
							scene.m_nMapCode = mapCode;
							scene.CopyMapId = copyMap.CopyMapID;
							scene.FuBenSeqId = fuBenSeqId;
							scene.m_nPlarerCount = 1;
							scene.SceneInfo = sceneInfo;
							scene.MapGridWidth = gameMap.MapGridWidth;
							scene.MapGridHeight = gameMap.MapGridHeight;
							DateTime startTime = now.Date.Add(this.GetStartTime(sceneInfo.Id));
							scene.StartTimeTicks = startTime.Ticks / 10000L;
							scene.GameStatisticalData.GameId = gameId;
							this.SceneDict[fuBenSeqId] = scene;
						}
						else
						{
							scene.m_nPlarerCount++;
						}
						YongZheZhanChangClientContextData clientContextData;
						if (!scene.ClientContextDataDict.TryGetValue(roleId, out clientContextData))
						{
							clientContextData = new YongZheZhanChangClientContextData
							{
								RoleId = roleId,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID
							};
							scene.ClientContextDataDict[roleId] = clientContextData;
						}
						else
						{
							clientContextData.KillNum = 0;
						}
						client.SceneContextData2 = clientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(scene.SceneInfo.TotalSecs * 1000));
					}
					YongZheZhanChangClient.getInstance().GameFuBenRoleChangeState(roleId, 5, 0, 0);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06003AA7 RID: 15015 RVA: 0x0031C908 File Offset: 0x0031AB08
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.YongZheZhanChang)
			{
				lock (this.RuntimeData.Mutex)
				{
					YongZheZhanChangScene YongZheZhanChangScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out YongZheZhanChangScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06003AA8 RID: 15016 RVA: 0x0031C980 File Offset: 0x0031AB80
		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			int addScore = 0;
			YongZheZhanChangScene scene;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					return;
				}
				if (scene.m_eStatus != GameSceneStatuses.STATUS_BEGIN)
				{
					return;
				}
				BattleCrystalMonsterItem monsterItem = monster.Tag as BattleCrystalMonsterItem;
				if (monsterItem == null)
				{
					return;
				}
				YongZheZhanChangClientContextData contextData = client.SceneContextData2 as YongZheZhanChangClientContextData;
				if (null != contextData)
				{
					addScore = monsterItem.BattleJiFen;
					contextData.TotalScore += addScore;
					scene.GameStatisticalData.CaiJiScore += addScore;
					if (client.ClientData.BattleWhichSide == 1)
					{
						scene.ScoreData.Score1 += addScore;
					}
					else if (client.ClientData.BattleWhichSide == 2)
					{
						scene.ScoreData.Score2 += addScore;
					}
				}
				this.AddDelayCreateMonster(scene, TimeUtil.NOW() + (long)monsterItem.FuHuoTime, monsterItem);
			}
			if (addScore > 0)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<YongZheZhanChangScoreData>(1104, scene.ScoreData, scene.CopyMap);
				this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
			}
		}

		// Token: 0x06003AA9 RID: 15017 RVA: 0x0031CB20 File Offset: 0x0031AD20
		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (monster.MonsterType == 401)
			{
				YongZheZhanChangClientContextData contextData = client.SceneContextData2 as YongZheZhanChangClientContextData;
				if (null != contextData)
				{
					YongZheZhanChangScene scene = null;
					int addScore = 0;
					if (monster.HandledDead && monster.WhoKillMeID == client.ClientData.RoleID)
					{
						addScore += this.RuntimeData.WarriorBattleBOssLastAttack;
					}
					double jiFenInjure = this.RuntimeData.WarriorBattleBossAttackPercent * monster.MonsterInfo.VLifeMax;
					contextData.InjureBossDelta += (double)injure;
					if (contextData.InjureBossDelta >= jiFenInjure && jiFenInjure > 0.0)
					{
						int calcRate = (int)(contextData.InjureBossDelta / jiFenInjure);
						contextData.InjureBossDelta -= jiFenInjure * (double)calcRate;
						addScore += this.RuntimeData.WarriorBattleBossAttackScore * calcRate;
					}
					lock (this.RuntimeData.Mutex)
					{
						contextData.TotalScore += addScore;
						if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
						{
							if (scene.m_eStatus != GameSceneStatuses.STATUS_BEGIN)
							{
								return;
							}
							if (client.ClientData.BattleWhichSide == 1)
							{
								scene.ScoreData.Score1 += addScore;
							}
							else if (client.ClientData.BattleWhichSide == 2)
							{
								scene.ScoreData.Score2 += addScore;
							}
							scene.GameStatisticalData.BossScore += addScore;
						}
					}
					if (addScore > 0 && scene != null)
					{
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<YongZheZhanChangScoreData>(1104, scene.ScoreData, scene.CopyMap);
						this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
					}
				}
			}
		}

		// Token: 0x06003AAA RID: 15018 RVA: 0x0031CD50 File Offset: 0x0031AF50
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= YongZheZhanChangManager.NextHeartBeatTicks)
			{
				YongZheZhanChangManager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (YongZheZhanChangScene scene in this.SceneDict.Values)
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
									scene.StateTimeData.GameType = 5;
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
									scene.StateTimeData.GameType = 5;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									this.InitCreateDynamicMonster(scene);
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
									copyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 2, 0);
									copyMap.AddGuangMuEvent(3, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 3, 0);
									copyMap.AddGuangMuEvent(4, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 4, 0);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime)
								{
									int successSide = 0;
									if (scene.ScoreData.Score1 > scene.ScoreData.Score2)
									{
										successSide = 1;
									}
									else if (scene.ScoreData.Score2 > scene.ScoreData.Score1)
									{
										successSide = 2;
									}
									this.CompleteScene(scene, successSide);
									scene.m_eStatus = GameSceneStatuses.STATUS_END;
									scene.m_lLeaveTime = scene.m_lEndTime + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
									scene.StateTimeData.GameType = 5;
									scene.StateTimeData.State = 5;
									scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
								}
								else
								{
									this.CheckCreateDynamicMonster(scene, ticks);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
								scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								YongZheZhanChangClient.getInstance().GameFuBenChangeState(scene.GameId, GameFuBenState.End, now);
								this.GiveAwards(scene);
								YongZheZhanChangFuBenData fuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
								{
									fuBenData.State = GameFuBenState.End;
									LogManager.WriteLog(LogTypes.Error, string.Format("勇者战场跨服副本GameID={0},战斗结束", fuBenData.GameId), null, true);
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
										DataHelper.WriteExceptionLogEx(ex, "勇者战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003AAB RID: 15019 RVA: 0x0031D2F0 File Offset: 0x0031B4F0
		private void CreateCrystalMonster(YongZheZhanChangScene scene, BattleCrystalMonsterItem crystal)
		{
			GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, crystal.MonsterID, scene.CopyMapId, 1, crystal.PosX / scene.MapGridWidth, crystal.PosY / scene.MapGridHeight, 0, 0, SceneUIClasses.YongZheZhanChang, crystal, null);
		}

		// Token: 0x06003AAC RID: 15020 RVA: 0x0031D33C File Offset: 0x0031B53C
		private void AddDelayCreateMonster(YongZheZhanChangScene scene, long ticks, object monster)
		{
			lock (this.RuntimeData.Mutex)
			{
				List<object> list = null;
				if (!scene.CreateMonsterQueue.TryGetValue(ticks, out list))
				{
					list = new List<object>();
					scene.CreateMonsterQueue.Add(ticks, list);
				}
				list.Add(monster);
			}
		}

		// Token: 0x06003AAD RID: 15021 RVA: 0x0031D3B8 File Offset: 0x0031B5B8
		private void InitCreateDynamicMonster(YongZheZhanChangScene scene)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (BattleCrystalMonsterItem crystal in this.RuntimeData.BattleCrystalMonsterDict.Values)
				{
					this.AddDelayCreateMonster(scene, scene.m_lBeginTime, crystal);
				}
				List<BattleDynamicMonsterItem> dynMonsterList = null;
				if (this.RuntimeData.SceneDynMonsterDict.TryGetValue(scene.m_nMapCode, out dynMonsterList))
				{
					foreach (BattleDynamicMonsterItem item in dynMonsterList)
					{
						this.AddDelayCreateMonster(scene, scene.m_lBeginTime + (long)item.DelayBirthMs, item);
					}
				}
			}
		}

		// Token: 0x06003AAE RID: 15022 RVA: 0x0031D4E0 File Offset: 0x0031B6E0
		public void CheckCreateDynamicMonster(YongZheZhanChangScene scene, long nowMs)
		{
			lock (this.RuntimeData.Mutex)
			{
				while (scene.CreateMonsterQueue.Count > 0)
				{
					KeyValuePair<long, List<object>> pair = scene.CreateMonsterQueue.First<KeyValuePair<long, List<object>>>();
					if (nowMs < pair.Key)
					{
						break;
					}
					try
					{
						foreach (object obj in pair.Value)
						{
							if (obj is BattleDynamicMonsterItem)
							{
								BattleDynamicMonsterItem crystal = obj as BattleDynamicMonsterItem;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, crystal.MonsterID, scene.CopyMapId, 1, crystal.PosX / scene.MapGridWidth, crystal.PosY / scene.MapGridHeight, 0, 0, SceneUIClasses.YongZheZhanChang, crystal, null);
							}
							else if (obj is BattleCrystalMonsterItem)
							{
								BattleCrystalMonsterItem item = obj as BattleCrystalMonsterItem;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item.MonsterID, scene.CopyMap.CopyMapID, 1, item.PosX / scene.MapGridWidth, item.PosY / scene.MapGridHeight, 0, 0, SceneUIClasses.YongZheZhanChang, item, null);
							}
						}
					}
					finally
					{
						scene.CreateMonsterQueue.RemoveAt(0);
					}
				}
			}
		}

		// Token: 0x06003AAF RID: 15023 RVA: 0x0031D6C0 File Offset: 0x0031B8C0
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true, bool selfScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<YongZheZhanChangScoreData>(1104, scene.ScoreData, false);
					}
					if (selfScore)
					{
						YongZheZhanChangClientContextData clientContextData = client.SceneContextData2 as YongZheZhanChangClientContextData;
						if (null != clientContextData)
						{
							client.sendCmd<int>(1105, clientContextData.TotalScore, false);
						}
					}
				}
			}
		}

		// Token: 0x06003AB0 RID: 15024 RVA: 0x0031D7F0 File Offset: 0x0031B9F0
		public void CompleteScene(YongZheZhanChangScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
			if (successSide != 0)
			{
				List<YongZheZhanChangClientContextData> winSidePlayerList = new List<YongZheZhanChangClientContextData>();
				foreach (YongZheZhanChangClientContextData item in scene.ClientContextDataDict.Values)
				{
					if (item.BattleWhichSide == successSide)
					{
						winSidePlayerList.Add(item);
					}
				}
				winSidePlayerList.Sort(delegate(YongZheZhanChangClientContextData left, YongZheZhanChangClientContextData right)
				{
					int result;
					if (left.TotalScore > right.TotalScore)
					{
						result = -1;
					}
					else if (left.TotalScore < right.TotalScore)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
					return result;
				});
				if (winSidePlayerList.Count != 0)
				{
					scene.ClientContextMVP = winSidePlayerList[0];
				}
			}
		}

		// Token: 0x06003AB1 RID: 15025 RVA: 0x0031D8C0 File Offset: 0x0031BAC0
		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						int addScore = 0;
						int addScoreDie = this.RuntimeData.WarriorBattleDie;
						YongZheZhanChangClientContextData clientLianShaContextData = client.SceneContextData2 as YongZheZhanChangClientContextData;
						YongZheZhanChangClientContextData otherLianShaContextData = other.SceneContextData2 as YongZheZhanChangClientContextData;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
						huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
						huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
						huanYingSiYuanAddScore.ByLianShaNum = 1;
						huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
						huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
						scene.GameStatisticalData.KillScore += this.RuntimeData.WarriorBattleUltraKillParam1;
						if (null != clientLianShaContextData)
						{
							clientLianShaContextData.KillNum++;
							int lianShaScore = this.RuntimeData.WarriorBattleUltraKillParam1 + clientLianShaContextData.KillNum * this.RuntimeData.WarriorBattleUltraKillParam2;
							lianShaScore = Math.Min(this.RuntimeData.WarriorBattleUltraKillParam4, Math.Max(this.RuntimeData.WarriorBattleUltraKillParam3, lianShaScore));
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
							huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
							huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
							huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
							huanYingSiYuanLianSha.LianShaType = Math.Min(clientLianShaContextData.KillNum, 30) / 5;
							huanYingSiYuanLianSha.ExtScore = lianShaScore;
							huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
							addScore += lianShaScore;
							scene.GameStatisticalData.LianShaScore += lianShaScore;
							if (clientLianShaContextData.KillNum % 5 != 0)
							{
								huanYingSiYuanLianSha = null;
							}
						}
						if (null != otherLianShaContextData)
						{
							int overScore = this.RuntimeData.WarriorBattleShutDownParam1 + otherLianShaContextData.KillNum * this.RuntimeData.WarriorBattleShutDownParam2;
							overScore = Math.Min(this.RuntimeData.WarriorBattleShutDownParam4, Math.Max(this.RuntimeData.WarriorBattleShutDownParam3, overScore));
							addScore += overScore;
							scene.GameStatisticalData.ZhongJieScore += overScore;
							if (otherLianShaContextData.KillNum >= 10)
							{
								huanYingSiYuanLianshaOver = new HuanYingSiYuanLianshaOver();
								huanYingSiYuanLianshaOver.KillerName = huanYingSiYuanAddScore.Name;
								huanYingSiYuanLianshaOver.KillerZoneID = huanYingSiYuanAddScore.ZoneID;
								huanYingSiYuanLianshaOver.KillerOccupation = client.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KillerSide = huanYingSiYuanAddScore.Side;
								huanYingSiYuanLianshaOver.KilledName = Global.FormatRoleName4(other);
								huanYingSiYuanLianshaOver.KilledZoneID = other.ClientData.ZoneID;
								huanYingSiYuanLianshaOver.KilledOccupation = other.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KilledSide = other.ClientData.BattleWhichSide;
								huanYingSiYuanLianshaOver.ExtScore = overScore;
							}
							otherLianShaContextData.KillNum = 0;
							otherLianShaContextData.TotalScore += addScoreDie;
							scene.GameStatisticalData.KillScore += addScoreDie;
						}
						huanYingSiYuanAddScore.Score = addScore;
						if (client.ClientData.BattleWhichSide == 1)
						{
							scene.ScoreData.Score1 += addScore;
							scene.ScoreData.Score2 += addScoreDie;
						}
						else
						{
							scene.ScoreData.Score2 += addScore;
							scene.ScoreData.Score1 += addScoreDie;
						}
						if (null != clientLianShaContextData)
						{
							clientLianShaContextData.TotalScore += addScore;
						}
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<YongZheZhanChangScoreData>(1104, scene.ScoreData, scene.CopyMap);
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(1106, huanYingSiYuanLianSha, scene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(1107, huanYingSiYuanLianshaOver, scene.CopyMap);
						}
						this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
						this.NotifyTimeStateInfoAndScoreInfo(other, false, false, true);
					}
				}
			}
		}

		// Token: 0x06003AB2 RID: 15026 RVA: 0x0031DD44 File Offset: 0x0031BF44
		public void GiveAwards(YongZheZhanChangScene scene)
		{
			try
			{
				Dictionary<int, int[]> bhDict = new Dictionary<int, int[]>();
				YongZheZhanChangStatisticalData gameResultData = scene.GameStatisticalData;
				foreach (YongZheZhanChangClientContextData contextData in scene.ClientContextDataDict.Values)
				{
					gameResultData.AllRoleCount++;
					int success;
					if (contextData.BattleWhichSide == scene.SuccessSide)
					{
						success = 1;
						gameResultData.WinRoleCount++;
					}
					else
					{
						success = 0;
						gameResultData.LoseRoleCount++;
					}
					GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleId);
					string awardsInfo = string.Format("{0},{1},{2},{3},{4}", new object[]
					{
						scene.SceneInfo.Id,
						success,
						contextData.TotalScore,
						scene.ScoreData.Score1,
						scene.ScoreData.Score2
					});
					string mvpInfo = string.Format("{0}&{1}&{2}&{3}", new object[]
					{
						scene.ClientContextMVP.RoleId,
						scene.ClientContextMVP.RoleName,
						scene.ClientContextMVP.Occupation,
						scene.ClientContextMVP.RoleSex
					});
					byte[] bytes = new UTF8Encoding().GetBytes(mvpInfo);
					mvpInfo = Convert.ToBase64String(bytes);
					if (client != null)
					{
						int bhid = client.ClientData.Faction;
						int junTuanId = client.ClientData.JunTuanId;
						if (bhid > 0 && junTuanId > 0)
						{
							int[] kv;
							if (!bhDict.TryGetValue(bhid, out kv))
							{
								int[] array = new int[2];
								array[0] = junTuanId;
								kv = array;
								bhDict[bhid] = kv;
							}
							kv[1]++;
						}
						int score = contextData.TotalScore;
						contextData.TotalScore = 0;
						if (score >= this.RuntimeData.WarriorBattleLowestJiFen)
						{
							Global.SaveRoleParamsStringToDB(client, "YongZheZhanChangAwards", awardsInfo, true);
							Global.SaveRoleParamsStringToDB(client, "44", mvpInfo, true);
						}
						else
						{
							Global.SaveRoleParamsStringToDB(client, "YongZheZhanChangAwards", this.RuntimeData.RoleParamsAwardsDefaultString, true);
							Global.SaveRoleParamsStringToDB(client, "44", "", true);
						}
						this.NtfCanGetAward(client, success, score, scene.SceneInfo, scene.ScoreData.Score1, scene.ScoreData.Score2, scene.ClientContextMVP.RoleId, scene.ClientContextMVP.RoleName, scene.ClientContextMVP.Occupation, scene.ClientContextMVP.RoleSex);
					}
					else if (contextData.TotalScore >= this.RuntimeData.WarriorBattleLowestJiFen)
					{
						Global.UpdateRoleParamByNameOffline(contextData.RoleId, "YongZheZhanChangAwards", awardsInfo, contextData.ServerId);
						Global.UpdateRoleParamByNameOffline(contextData.RoleId, "44", mvpInfo, contextData.ServerId);
					}
					else
					{
						Global.UpdateRoleParamByNameOffline(contextData.RoleId, "YongZheZhanChangAwards", this.RuntimeData.RoleParamsAwardsDefaultString, contextData.ServerId);
						Global.UpdateRoleParamByNameOffline(contextData.RoleId, "44", "", contextData.ServerId);
					}
				}
				foreach (KeyValuePair<int, int[]> kv2 in bhDict)
				{
					JunTuanManager.getInstance().AddJunTuanTaskValue(kv2.Key, kv2.Value[0], (int)Global.GetMapSceneType(scene.m_nMapCode), kv2.Value[1]);
				}
				YongZheZhanChangClient.getInstance().PushGameResultData(gameResultData);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "天梯系统清场调度异常");
			}
		}

		// Token: 0x06003AB3 RID: 15027 RVA: 0x0031E1A0 File Offset: 0x0031C3A0
		private void NtfCanGetAward(GameClient client, int success, int score, YongZheZhanChangSceneInfo sceneInfo, int sideScore1, int sideScore2, int mvprid, string mvpname, int mvpocc, int mvpsex)
		{
			long addExp = 0L;
			int addBindJinBi = 0;
			List<AwardsItemData> awardsItemDataList = null;
			if (score >= this.RuntimeData.WarriorBattleLowestJiFen)
			{
				addExp = (long)((double)sceneInfo.Exp * (0.2 + Math.Min(0.8, Math.Pow((double)score, 0.5) / 100.0)));
				addBindJinBi = (int)((double)sceneInfo.BandJinBi * Math.Min(100.0, Math.Pow((double)score, 0.5)));
				if (success > 0)
				{
					awardsItemDataList = sceneInfo.WinAwardsItemList.Items;
				}
				else
				{
					addExp = (long)((double)addExp * 0.8);
					addBindJinBi = (int)((double)addBindJinBi * 0.8);
					awardsItemDataList = sceneInfo.LoseAwardsItemList.Items;
				}
				addExp -= addExp % 10000L;
				addBindJinBi -= addBindJinBi % 10000;
			}
			client.sendCmd<YongZheZhanChangAwardsData>(1102, new YongZheZhanChangAwardsData
			{
				Exp = addExp,
				BindJinBi = addBindJinBi,
				Success = success,
				AwardsItemDataList = awardsItemDataList,
				SideScore1 = sideScore1,
				SideScore2 = sideScore2,
				SelfScore = score,
				MvpRoleName = mvpname,
				MvpOccupation = mvpocc,
				MvpRoleSex = mvpsex
			}, false);
		}

		// Token: 0x06003AB4 RID: 15028 RVA: 0x0031E2F4 File Offset: 0x0031C4F4
		private int GiveRoleAwards(GameClient client, int success, int score, YongZheZhanChangSceneInfo sceneInfo)
		{
			long addExp = 0L;
			int addBindJinBi = 0;
			List<AwardsItemData> awardsItemDataList = null;
			if (score >= this.RuntimeData.WarriorBattleLowestJiFen)
			{
				addExp = (long)((double)sceneInfo.Exp * (0.2 + Math.Min(0.8, Math.Pow((double)score, 0.5) / 100.0)));
				addBindJinBi = (int)((double)sceneInfo.BandJinBi * Math.Min(100.0, Math.Pow((double)score, 0.5)));
				if (success > 0)
				{
					awardsItemDataList = sceneInfo.WinAwardsItemList.Items;
				}
				else
				{
					addExp = (long)((double)addExp * 0.8);
					addBindJinBi = (int)((double)addBindJinBi * 0.8);
					awardsItemDataList = sceneInfo.LoseAwardsItemList.Items;
				}
				addExp -= addExp % 10000L;
				addBindJinBi -= addBindJinBi % 10000;
			}
			int result;
			if (awardsItemDataList != null && !Global.CanAddGoodsNum(client, awardsItemDataList.Count))
			{
				result = -100;
			}
			else
			{
				if (addExp > 0L)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, addExp, true, true, false, "none");
				}
				if (addBindJinBi > 0)
				{
					GameManager.ClientMgr.AddMoney1(client, addBindJinBi, "勇者战场奖励", true);
				}
				if (awardsItemDataList != null)
				{
					foreach (AwardsItemData item in awardsItemDataList)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "勇者战场奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
					}
				}
				if (score >= this.RuntimeData.WarriorBattleLowestJiFen && success > 0)
				{
					GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_YongZheZhanChang, new int[0]));
				}
				result = 1;
			}
			return result;
		}

		// Token: 0x06003AB5 RID: 15029 RVA: 0x0031E53C File Offset: 0x0031C73C
		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					scene.m_nPlarerCount--;
				}
			}
		}

		// Token: 0x06003AB6 RID: 15030 RVA: 0x0031E5B8 File Offset: 0x0031C7B8
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		// Token: 0x040044AB RID: 17579
		public const SceneUIClasses ManagerType = SceneUIClasses.YongZheZhanChang;

		// Token: 0x040044AC RID: 17580
		private static YongZheZhanChangManager instance = new YongZheZhanChangManager();

		// Token: 0x040044AD RID: 17581
		public YongZheZhanChangData RuntimeData = new YongZheZhanChangData();

		// Token: 0x040044AE RID: 17582
		public ConcurrentDictionary<int, YongZheZhanChangScene> SceneDict = new ConcurrentDictionary<int, YongZheZhanChangScene>();

		// Token: 0x040044AF RID: 17583
		private static long NextHeartBeatTicks = 0L;
	}
}
