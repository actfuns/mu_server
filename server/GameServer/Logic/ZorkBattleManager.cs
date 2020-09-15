using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Reborn;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x02000844 RID: 2116
	public class ZorkBattleManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		// Token: 0x06003B6F RID: 15215 RVA: 0x00328144 File Offset: 0x00326344
		public static ZorkBattleManager getInstance()
		{
			return ZorkBattleManager.instance;
		}

		// Token: 0x06003B70 RID: 15216 RVA: 0x0032815C File Offset: 0x0032635C
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06003B71 RID: 15217 RVA: 0x00328180 File Offset: 0x00326380
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZorkBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 2000);
			return true;
		}

		// Token: 0x06003B72 RID: 15218 RVA: 0x003281C0 File Offset: 0x003263C0
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2100, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2101, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2103, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2105, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2108, 2, 2, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2109, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10033, 57, ZorkBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 57, ZorkBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(63, 10000, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(64, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, ZorkBattleManager.getInstance());
			return true;
		}

		// Token: 0x06003B73 RID: 15219 RVA: 0x003282F0 File Offset: 0x003264F0
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10033, 57, ZorkBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 57, ZorkBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(63, 10000, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(64, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, ZorkBattleManager.getInstance());
			return true;
		}

		// Token: 0x06003B74 RID: 15220 RVA: 0x00328390 File Offset: 0x00326590
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06003B75 RID: 15221 RVA: 0x003283A4 File Offset: 0x003265A4
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06003B76 RID: 15222 RVA: 0x003283B8 File Offset: 0x003265B8
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (nID != 2103)
			{
				if (!this.IsGongNengOpened(client, false))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "", GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
			}
			switch (nID)
			{
			case 2100:
				return this.ProcessGetZorkBattleBaseDataCmd(client, nID, bytes, cmdParams);
			case 2101:
				return this.ProcessZorkBattleEnterCmd(client, nID, bytes, cmdParams);
			case 2103:
				return this.ProcessGetZorkBattleStateCmd(client, nID, bytes, cmdParams);
			case 2105:
				return this.ProcessGetZorkBattleRankInfoCmd(client, nID, bytes, cmdParams);
			case 2108:
				return this.ProcessGetZorkBattleAwardCmd(client, nID, bytes, cmdParams);
			case 2109:
				return this.ProcessZorkBattleJoinCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		// Token: 0x06003B77 RID: 15223 RVA: 0x003284A4 File Offset: 0x003266A4
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 28)
			{
				OnStartPlayGameEventObject e = eventObject as OnStartPlayGameEventObject;
				this.OnStartPlayGame(e.Client);
			}
			else if (eventType == 10)
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
			else if (eventType == 11)
			{
				MonsterDeadEventObject e2 = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(e2.getAttacker(), e2.getMonster());
			}
			else if (eventType == 64)
			{
				this.UpdateChengHaoBuffer(eventObject.Params[0] as GameClient);
			}
		}

		// Token: 0x06003B78 RID: 15224 RVA: 0x00328580 File Offset: 0x00326780
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num != 30)
			{
				if (num != 63)
				{
					if (num == 10033)
					{
						this.HandleNtfEnterEvent((eventObject as KFZorkBattleNtfEnterData).Data);
						eventObject.Handled = true;
					}
				}
				else
				{
					PreZhanDuiChangeMemberEventObject eventObj = (PreZhanDuiChangeMemberEventObject)eventObject;
					eventObj.Handled = this.OnPreZhanDuiChangeMember(eventObj);
				}
			}
			else
			{
				OnCreateMonsterEventObject e = eventObject as OnCreateMonsterEventObject;
				if (null != e)
				{
					ZorkBattleMonsterCreateTag tagInfo = e.Monster.Tag as ZorkBattleMonsterCreateTag;
					if (tagInfo != null && tagInfo.monsterTag.ArmyType == ZorkBattleArmyType.Boss)
					{
						int buffID = tagInfo.monsterTag.RandomBuffID();
						CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(e.Monster.CopyMapID);
						ZorkBattleScene scene = null;
						if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out scene))
						{
							this.UpdateBuff4GameScene(scene, e.Monster, buffID, tagInfo, true);
							scene.ScoreData.BossBuffID = buffID;
							this.BroadScoreInfo(copyMap, -1);
						}
					}
				}
			}
		}

		// Token: 0x06003B79 RID: 15225 RVA: 0x003286B4 File Offset: 0x003268B4
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.MapBirthPointDict = new Dictionary<int, ZorkBattleBirthPoint>();
					fileName = "Config/ZorkPlayPoint.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						ZorkBattleBirthPoint item = new ZorkBattleBirthPoint();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						string[] strFields = Global.GetSafeAttributeStr(node, "MapTeamPoint").Split(new char[]
						{
							','
						});
						if (strFields.Length == 2)
						{
							item.PosX = Global.SafeConvertToInt32(strFields[0]);
							item.PosY = Global.SafeConvertToInt32(strFields[1]);
						}
						item.BirthRadius = (int)Global.GetSafeAttributeLong(node, "MapTeamRange");
						this.RuntimeData.MapBirthPointDict[item.ID] = item;
					}
					this.RuntimeData.SceneDataDict = new Dictionary<int, ZorkBattleSceneInfo>();
					fileName = "Config/ZorkActivityRules.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						ZorkBattleSceneInfo sceneItem = new ZorkBattleSceneInfo();
						int id = (int)Global.GetSafeAttributeLong(node, "ID");
						int mapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						sceneItem.Id = id;
						sceneItem.MapCode = mapCode;
						sceneItem.MaxEnterNum = (int)Global.GetSafeAttributeLong(node, "MaxEnterNum");
						sceneItem.PrepareSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
						sceneItem.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
						sceneItem.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
						sceneItem.BattleSignSecs = (int)Global.GetSafeAttributeLong(node, "BattleSignSecs");
						sceneItem.SignCondition = Global.GetSafeAttributeIntArray(node, "SignCondition", -1, '|');
						sceneItem.SeasonFightRound = (int)Global.GetSafeAttributeLong(node, "SeasonFightDay");
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
						this.RuntimeData.SceneDataDict[mapCode] = sceneItem;
					}
					this.RuntimeData.ZorkBattleArmyList = new List<ZorkBattleArmyConfig>();
					fileName = "Config/ZorkScene.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						ZorkBattleArmyConfig item2 = new ZorkBattleArmyConfig();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "BuffAreID");
						string[] strFields = Global.GetSafeAttributeStr(node, "BuffArePlace").Split(new char[]
						{
							'|'
						});
						if (strFields.Length == 2)
						{
							string[] tempFields = strFields[0].Split(new char[]
							{
								','
							});
							if (tempFields.Length == 2)
							{
								item2.PosX = Global.SafeConvertToInt32(tempFields[0]);
								item2.PosY = Global.SafeConvertToInt32(tempFields[1]);
							}
							item2.PursuitRadius = Global.SafeConvertToInt32(strFields[1]);
						}
						item2.Range = (int)Global.GetSafeAttributeLong(node, "ArmyRefreshRange");
						item2.ArmyType = (ZorkBattleArmyType)Global.GetSafeAttributeLong(node, "ArmyType");
						item2.ArmyGroupRound = Global.GetSafeAttributeIntArray(node, "ArmyGroupRound", -1, '|');
						item2.GuardGroupID = Global.GetSafeAttributeIntArray(node, "GuardGroupID", -1, '|');
						item2.FirstArmyTime = (int)Global.GetSafeAttributeLong(node, "FirstArmyTime");
						item2.NextArmyRefresTime = (int)Global.GetSafeAttributeLong(node, "NextArmyRefresTime");
						this.RuntimeData.ZorkBattleArmyList.Add(item2);
					}
					this.RuntimeData.ZorkBattleMonsterDict = new Dictionary<int, List<ZorkBattleMonsterConfig>>();
					fileName = "Config/ZorkMonster.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						ZorkBattleMonsterConfig item3 = new ZorkBattleMonsterConfig();
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.GroupID = (int)Global.GetSafeAttributeLong(node, "GroupID");
						item3.ArmyType = (ZorkBattleArmyType)Global.GetSafeAttributeLong(node, "MonsterType");
						item3.MonsterId = (int)Global.GetSafeAttributeLong(node, "MonsterId");
						item3.MonsterNum = (int)Global.GetSafeAttributeLong(node, "MonsterNum");
						item3.MonsterDropBuffId = (int)Global.GetSafeAttributeLong(node, "MonsterDropBuffId");
						item3.BuffEffictTime = (int)Global.GetSafeAttributeLong(node, "BuffEffictTime");
						item3.RewardIntegral = (int)Global.GetSafeAttributeLong(node, "RewardIntegral");
						item3.BossBlood = Global.GetSafeAttributeDouble(node, "BossBlood");
						item3.BuffRefreshTime = (int)Global.GetSafeAttributeDouble(node, "BuffRefreshTime");
						item3.BossBuffGroup = Global.GetSafeAttributeIntArray(node, "BossBuffGroup", -1, '|');
						item3.BossBuffRound = Global.GetSafeAttributeIntArray(node, "BossBuffRound", -1, '|');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "BossLostSkill"), ref item3.BossKillAwardsItemList, '|', ',');
						List<ZorkBattleMonsterConfig> configList = null;
						if (!this.RuntimeData.ZorkBattleMonsterDict.TryGetValue(item3.GroupID, out configList))
						{
							configList = new List<ZorkBattleMonsterConfig>();
							this.RuntimeData.ZorkBattleMonsterDict[item3.GroupID] = configList;
						}
						configList.Add(item3);
					}
					this.RuntimeData.ZorkAchievementDict = new Dictionary<int, ZorkAchievementConfig>();
					fileName = "Config/ZorkAchievement.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						ZorkAchievementConfig item4 = new ZorkAchievementConfig();
						item4.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item4.ATarType = (int)Global.GetSafeAttributeLong(node, "AchievementTarget");
						item4.TargetNum = (int)Global.GetSafeAttributeLong(node, "TargetNum");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "AchievementReward"), ref item4.AAwardsItemList, '|', ',');
						this.RuntimeData.ZorkAchievementDict[item4.ID] = item4;
					}
					this.RuntimeData.ZorkLevelRangeList = new List<ZorkBattleAwardConfig>();
					fileName = "Config/ZorkDanAward.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						ZorkBattleAwardConfig item5 = new ZorkBattleAwardConfig();
						item5.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item5.RankValue = (int)Global.GetSafeAttributeLong(node, "RankValue");
						item5.WinRankValue = (int)Global.GetSafeAttributeLong(node, "WinRankValue");
						item5.LoseRankValue = (int)Global.GetSafeAttributeLong(node, "LoseRankValue");
						item5.RankLevel = Global.GetSafeAttributeStr(node, "RankLevel");
						AwardsItemList FirstWinAwardsItemList = new AwardsItemList();
						item5.FirstWinAwardsItemList = FirstWinAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "FirstBattleReward"), ref FirstWinAwardsItemList, '|', ',');
						AwardsItemList SeasonAwardsItemList = new AwardsItemList();
						item5.SeasonAwardsItemList = SeasonAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "SeasonReward"), ref SeasonAwardsItemList, '|', ',');
						AwardsItemList WinAwardsItemList = new AwardsItemList();
						item5.WinAwardsItemList = WinAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "WinRankReward"), ref WinAwardsItemList, '|', ',');
						AwardsItemList LoseAwardsItemList = new AwardsItemList();
						item5.LoseAwardsItemList = LoseAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "LoseRankReward"), ref LoseAwardsItemList, '|', ',');
						this.RuntimeData.ZorkLevelRangeList.Add(item5);
					}
					this.RuntimeData.BossHurtCleanTime = (int)GameManager.systemParamsList.GetParamValueIntByName("BossHurtCleanTime", -1);
					this.RuntimeData.ZorkWarEnterMapSet.Clear();
					int[] ZorkWarEnterMap = GameManager.systemParamsList.GetParamValueIntArrayByName("ZorkWarEnterMap", ',');
					if (null != ZorkWarEnterMap)
					{
						foreach (int id in ZorkWarEnterMap)
						{
							this.RuntimeData.ZorkWarEnterMapSet.Add(id);
						}
					}
					int[] intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("ZorkBattleUltraKill", ',');
					if (intArray.Length == 4)
					{
						this.RuntimeData.ZorkBattleUltraKillParam1 = intArray[0];
						this.RuntimeData.ZorkBattleUltraKillParam2 = intArray[1];
						this.RuntimeData.ZorkBattleUltraKillParam3 = intArray[2];
						this.RuntimeData.ZorkBattleUltraKillParam4 = intArray[3];
					}
					intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("ZorkBattleShutDown", ',');
					if (intArray.Length == 4)
					{
						this.RuntimeData.ZorkBattleShutDownParam1 = intArray[0];
						this.RuntimeData.ZorkBattleShutDownParam2 = intArray[1];
						this.RuntimeData.ZorkBattleShutDownParam3 = intArray[2];
						this.RuntimeData.ZorkBattleShutDownParam4 = intArray[3];
					}
					this.RuntimeData.ZorkEnterPlayNumMin = (int)GameManager.systemParamsList.GetParamValueIntByName("ZorkEnterPlayNum", 4);
					DateTime.TryParse(GameManager.systemParamsList.GetParamValueByName("ZorkStartTime"), out this.RuntimeData.ZorkStartTime);
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		// Token: 0x06003B7A RID: 15226 RVA: 0x003292C0 File Offset: 0x003274C0
		private void TimerProc(object sender, EventArgs e)
		{
			if (this.IsGongNengOpened(null, false))
			{
				ZorkBattleSyncData SyncData = TianTiClient.getInstance().SyncData_ZorkBattle(TimeUtil.NOW(), this.ZorkBattleSyncDataCache.ZorkBattleRankInfoDict.Age);
				if (null != SyncData)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (this.ZorkBattleSyncDataCache.CurSeasonID != SyncData.CurSeasonID)
						{
							this.ZorkBattleSyncDataCache.CurSeasonID = SyncData.CurSeasonID;
						}
						this.ZorkBattleSyncDataCache.CurRound = SyncData.CurRound;
						if (this.ZorkBattleSyncDataCache.TopZhanDui != SyncData.TopZhanDui)
						{
							this.ZorkBattleSyncDataCache.TopZhanDui = SyncData.TopZhanDui;
							int count = GameManager.ClientMgr.GetMaxClientCount();
							for (int i = 0; i < count; i++)
							{
								GameClient client = GameManager.ClientMgr.FindClientByNid(i);
								if (null != client)
								{
									this.UpdateChengHaoBuffer(client);
								}
							}
						}
						this.ZorkBattleSyncDataCache.TopZhanDuiName = SyncData.TopZhanDuiName;
						if (this.ZorkBattleSyncDataCache.TopKiller != SyncData.TopKiller)
						{
							this.ZorkBattleSyncDataCache.TopKiller = SyncData.TopKiller;
							int count = GameManager.ClientMgr.GetMaxClientCount();
							for (int i = 0; i < count; i++)
							{
								GameClient client = GameManager.ClientMgr.FindClientByNid(i);
								if (null != client)
								{
									this.UpdateChengHaoBuffer(client);
								}
							}
						}
						this.ZorkBattleSyncDataCache.DiffKFCenterSeconds = SyncData.DiffKFCenterSeconds;
						if (this.ZorkBattleSyncDataCache.ZorkBattleRankInfoDict.Age != SyncData.ZorkBattleRankInfoDict.Age)
						{
							this.ZorkBattleSyncDataCache.ZorkBattleRankInfoDict = SyncData.ZorkBattleRankInfoDict;
						}
					}
					if (!GameManager.IsKuaFuServer)
					{
						DateTime SeasonTime = ZorkBattleUtils.GetSeasonDateTm(this.ZorkBattleSyncDataCache.CurSeasonID);
						ZorkBattleSceneInfo sceneInfo = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
						int ZorkAwardSeasonID = GameManager.GameConfigMgr.GetGameConfigItemInt("ZorkAwardSeasonID", 0);
						if (sceneInfo != null && ZorkAwardSeasonID != this.ZorkBattleSyncDataCache.CurSeasonID && this.ZorkBattleSyncDataCache.CurRound > sceneInfo.SeasonFightRound)
						{
							ZorkAwardSeasonID = this.ZorkBattleSyncDataCache.CurSeasonID;
							lock (TianTi5v5Manager.getInstance().RuntimeData.Mutex)
							{
								List<TianTi5v5ZhanDuiMiniData> zhanduiMiniDataList = TianTi5v5Manager.getInstance().GetZhanDuiMiniDataList(int.MaxValue, GameManager.ServerId);
								if (null != zhanduiMiniDataList)
								{
									foreach (TianTi5v5ZhanDuiMiniData minidata in zhanduiMiniDataList)
									{
										TianTi5v5ZhanDuiData zhanduiData = this.GetZhanDuiData(minidata.ZhanDuiID, GameManager.ServerId);
										if (zhanduiData == null || zhanduiData.ZorkLastFightTime < SeasonTime)
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("魔域夺宝{0}赛季奖励 战队ID:{1} 获取zhanduiData失败", this.ZorkBattleSyncDataCache.CurSeasonID, minidata.ZhanDuiID), null, true);
										}
										else
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("魔域夺宝{0}赛季奖励 战队ID:{1} 积分:{2} 成功！", this.ZorkBattleSyncDataCache.CurSeasonID, zhanduiData.ZhanDuiID, zhanduiData.ZorkJiFen), null, true);
											ZorkBattleAwardConfig awardConfig = this.GetZorkBattleAwardConfigByJiFen(zhanduiData.ZorkJiFen);
											if (null != awardConfig)
											{
												AwardsItemList SeasonAwardsItemList = awardConfig.SeasonAwardsItemList as AwardsItemList;
												if (null != SeasonAwardsItemList)
												{
													foreach (TianTi5v5ZhanDuiRoleData role in zhanduiData.teamerList)
													{
														List<GoodsData> AwardGoods = Global.ConvertToGoodsDataList(SeasonAwardsItemList.Items, -1);
														string sContent = string.Format(GLang.GetLang(8006, new object[0]), awardConfig.RankLevel);
														Global.UseMailGivePlayerAward3(role.RoleID, AwardGoods, GLang.GetLang(8003, new object[0]), sContent, 0, 0, 0);
													}
												}
											}
										}
									}
									GameManager.GameConfigMgr.SetGameConfigItem("ZorkAwardSeasonID", ZorkAwardSeasonID.ToString());
									Global.UpdateDBGameConfigg("ZorkAwardSeasonID", ZorkAwardSeasonID.ToString());
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003B7B RID: 15227 RVA: 0x003297D4 File Offset: 0x003279D4
		public List<int> BuildZorkBattleAnalysisData(GameClient client)
		{
			List<int> listAnalysisData = new List<int>(new int[7]);
			List<int> roleAnalysisData = this.GetZorkBattleRoleAnalysisData(client);
			if (null != roleAnalysisData)
			{
				listAnalysisData[0] = roleAnalysisData[2];
				listAnalysisData[1] = roleAnalysisData[3];
				listAnalysisData[2] = roleAnalysisData[4];
				listAnalysisData[3] = roleAnalysisData[5];
				listAnalysisData[4] = roleAnalysisData[8];
				listAnalysisData[5] = roleAnalysisData[9];
				listAnalysisData[6] = roleAnalysisData[10];
			}
			return listAnalysisData;
		}

		// Token: 0x06003B7C RID: 15228 RVA: 0x00329870 File Offset: 0x00327A70
		public bool ProcessGetZorkBattleBaseDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				ZorkBattleBaseData baseData = new ZorkBattleBaseData();
				baseData.listAnalysisData = this.BuildZorkBattleAnalysisData(client);
				Dictionary<int, ZorkAchievementConfig> tempAchiDict = null;
				lock (this.RuntimeData.Mutex)
				{
					tempAchiDict = this.RuntimeData.ZorkAchievementDict;
				}
				if (null != tempAchiDict)
				{
					foreach (KeyValuePair<int, ZorkAchievementConfig> item in tempAchiDict)
					{
						baseData.ArchievementAwardDict[item.Key] = 0;
					}
				}
				List<int> achievementData = this.GetZorkBattleAchievementAwardData(client);
				if (null != achievementData)
				{
					for (int idx = 2; idx < achievementData.Count; idx++)
					{
						baseData.ArchievementAwardDict[achievementData[idx]] = 1;
					}
				}
				TianTi5v5ZhanDuiData zhanduiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				if (null != zhanduiData)
				{
					ZorkBattleAwardConfig awardConfig = this.GetZorkBattleAwardConfigByJiFen(zhanduiData.ZorkJiFen);
					if (null != awardConfig)
					{
						baseData.TeamDuanWei = awardConfig.ID;
					}
				}
				client.sendCmd<ZorkBattleBaseData>(nID, baseData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06003B7D RID: 15229 RVA: 0x00329A38 File Offset: 0x00327C38
		public bool ProcessZorkBattleEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result;
				if (!this.IsGongNengOpened(client, true))
				{
					result = -13;
				}
				else
				{
					string gamestate = TianTiClient.getInstance().GetKuaFuGameState_ZorkBattle(client.ClientData.ZhanDuiID);
					if (string.IsNullOrEmpty(gamestate))
					{
						result = -11003;
					}
					else
					{
						int signState = Global.SafeConvertToInt32(gamestate);
						if (signState == -4036)
						{
							result = -4036;
						}
						else if (signState == -4006)
						{
							result = -4006;
						}
						else if (signState == -4035)
						{
							result = -4035;
						}
						else
						{
							if (signState == -4034)
							{
							}
							ZorkBattleSceneInfo sceneItem = null;
							ZorkBattleGameStates state = ZorkBattleGameStates.None;
							if (!this.CheckMap(client))
							{
								result = -21;
							}
							else
							{
								result = this.CheckCondition(client, ref sceneItem, ref state);
								if (state != ZorkBattleGameStates.Start)
								{
									result = -2001;
								}
								else
								{
									KuaFuServerInfo kfserverInfo = null;
									KuaFu5v5FuBenData fubenData = TianTiClient.getInstance().GetFuBenDataByZhanDuiId_ZorkBattle(client.ClientData.ZhanDuiID);
									if (fubenData == null || !KuaFuManager.getInstance().TryGetValue(fubenData.ServerId, out kfserverInfo))
									{
										result = -11000;
									}
									else
									{
										KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
										if (null != clientKuaFuServerLoginData)
										{
											clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
											clientKuaFuServerLoginData.GameId = (long)fubenData.GameId;
											clientKuaFuServerLoginData.GameType = 36;
											clientKuaFuServerLoginData.EndTicks = 0L;
											clientKuaFuServerLoginData.ServerId = client.ServerId;
											clientKuaFuServerLoginData.ServerIp = kfserverInfo.Ip;
											clientKuaFuServerLoginData.ServerPort = kfserverInfo.Port;
										}
										GlobalNew.RecordSwitchKuaFuServerLog(client);
										client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
									}
								}
							}
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

		// Token: 0x06003B7E RID: 15230 RVA: 0x00329C68 File Offset: 0x00327E68
		public bool ProcessGetZorkBattleStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.CheckOpenState(TimeUtil.NowDateTime()))
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						4,
						this.ZorkBattleSyncDataCache.CurRound,
						this.ZorkBattleSyncDataCache.DiffKFCenterSeconds,
						this.ZorkBattleSyncDataCache.TopZhanDuiName
					}), false);
					return true;
				}
				string gamestate = TianTiClient.getInstance().GetKuaFuGameState_ZorkBattle(client.ClientData.ZhanDuiID);
				if (string.IsNullOrEmpty(gamestate))
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						0,
						this.ZorkBattleSyncDataCache.CurRound,
						this.ZorkBattleSyncDataCache.DiffKFCenterSeconds,
						this.ZorkBattleSyncDataCache.TopZhanDuiName
					}), false);
					return true;
				}
				int signState = Global.SafeConvertToInt32(gamestate);
				ZorkBattleSceneInfo sceneItem = null;
				ZorkBattleGameStates timeState = ZorkBattleGameStates.None;
				this.CheckCondition(client, ref sceneItem, ref timeState);
				if (signState == -4036)
				{
					timeState = ZorkBattleGameStates.Bye;
				}
				else if (signState == -4006)
				{
					timeState = ZorkBattleGameStates.End;
				}
				else if (signState == -4034)
				{
					if (timeState == ZorkBattleGameStates.SignUp || timeState == ZorkBattleGameStates.Wait)
					{
						timeState = ZorkBattleGameStates.Wait;
					}
				}
				else if (timeState == ZorkBattleGameStates.Wait || timeState == ZorkBattleGameStates.Start)
				{
					timeState = ZorkBattleGameStates.NotJoin;
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					(int)timeState,
					this.ZorkBattleSyncDataCache.CurRound,
					this.ZorkBattleSyncDataCache.DiffKFCenterSeconds,
					this.ZorkBattleSyncDataCache.TopZhanDuiName
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06003B7F RID: 15231 RVA: 0x00329EAC File Offset: 0x003280AC
		public bool ProcessGetZorkBattleRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				ZorkBattleRankInfo returnRankInfo = new ZorkBattleRankInfo();
				lock (this.RuntimeData.Mutex)
				{
					foreach (KeyValuePair<int, List<KFZorkRankInfo>> kvp in this.ZorkBattleSyncDataCache.ZorkBattleRankInfoDict.V)
					{
						returnRankInfo.rankInfo2Client[kvp.Key] = new List<KFZorkRankInfo>(kvp.Value);
					}
				}
				client.sendCmd<ZorkBattleRankInfo>(nID, returnRankInfo, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06003B80 RID: 15232 RVA: 0x00329FB8 File Offset: 0x003281B8
		public bool ProcessGetZorkBattleAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int achievementID = Global.SafeConvertToInt32(cmdParams[1]);
				ZorkAchievementConfig achievementConfig;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.ZorkAchievementDict.TryGetValue(achievementID, out achievementConfig))
					{
						result = -3;
						goto IL_21C;
					}
				}
				HashSet<int> archievementAwardSet = new HashSet<int>();
				List<int> achievementAwardData = this.GetZorkBattleAchievementAwardData(client);
				if (null != achievementAwardData)
				{
					for (int idx = 2; idx < achievementAwardData.Count; idx++)
					{
						archievementAwardSet.Add(achievementAwardData[idx]);
					}
				}
				if (archievementAwardSet.Contains(achievementID))
				{
					result = -200;
				}
				else
				{
					List<int> listAnalysisData = this.BuildZorkBattleAnalysisData(client);
					if (null == listAnalysisData)
					{
						result = -15;
					}
					else if (listAnalysisData[achievementConfig.ATarType - 1] < achievementConfig.TargetNum)
					{
						result = -12;
					}
					else
					{
						List<AwardsItemData> awardsItemDataList = achievementConfig.AAwardsItemList.Items;
						if (null != awardsItemDataList)
						{
							int BagInt;
							if (!RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, awardsItemDataList, out BagInt))
							{
								if (BagInt == 1)
								{
									result = -101;
									goto IL_21C;
								}
								result = -100;
								goto IL_21C;
							}
							else
							{
								foreach (AwardsItemData item in awardsItemDataList)
								{
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "魔域夺宝成就奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
								}
							}
						}
						achievementAwardData.Add(achievementID);
						this.SaveZorkBattleAchievementAwardData(client, achievementAwardData);
					}
				}
				IL_21C:
				client.sendCmd(nID, string.Format("{0}:{1}", result, achievementID), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06003B81 RID: 15233 RVA: 0x0032A278 File Offset: 0x00328478
		public bool ProcessZorkBattleJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (this.IsGongNengOpened(client, false))
				{
					int zhanDuiID = client.ClientData.ZhanDuiID;
					if (zhanDuiID <= 0)
					{
						result = -4013;
					}
					else if (client.ClientData.ZhanDuiZhiWu != 1)
					{
						result = -4016;
					}
					else
					{
						ZorkBattleSceneInfo sceneItem = null;
						ZorkBattleGameStates state = ZorkBattleGameStates.None;
						if (!this.CheckMap(client))
						{
							result = -21;
						}
						else
						{
							result = this.CheckCondition(client, ref sceneItem, ref state);
						}
						TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Manager.getInstance().GetZhanDuiData(zhanDuiID, GameManager.ServerId);
						if (null == zhanDuiData)
						{
							result = -4013;
						}
						else if (zhanDuiData.teamerList.Count < this.RuntimeData.ZorkEnterPlayNumMin)
						{
							result = -4026;
						}
						else
						{
							int checkRebornRoleNum = 0;
							foreach (TianTi5v5ZhanDuiRoleData role in zhanDuiData.teamerList)
							{
								if (client.ClientData.RoleID != role.RoleID && role.RebornLevel >= sceneItem.SignCondition[1])
								{
									checkRebornRoleNum++;
								}
							}
							if (checkRebornRoleNum < sceneItem.SignCondition[0] || client.ClientData.RebornLevel < sceneItem.SignCondition[1])
							{
								result = -19;
							}
							else
							{
								string gamestate = TianTiClient.getInstance().GetKuaFuGameState_ZorkBattle(zhanDuiID);
								if (string.IsNullOrEmpty(gamestate))
								{
									result = -11003;
								}
								else
								{
									int signState = Global.SafeConvertToInt32(gamestate);
									if (state != ZorkBattleGameStates.SignUp)
									{
										result = -2001;
									}
									else if (signState == -4034)
									{
										result = -12;
									}
									if (result >= 0)
									{
										result = TianTiClient.getInstance().SignUp_ZorkBattle(zhanDuiID, GameManager.ServerId);
										if (result >= 0)
										{
											client.ClientData.SignUpGameType = 36;
										}
									}
								}
							}
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

		// Token: 0x06003B82 RID: 15234 RVA: 0x0032A508 File Offset: 0x00328708
		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				ZorkBattleBirthPoint birthPoint = null;
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

		// Token: 0x06003B83 RID: 15235 RVA: 0x0032A5A4 File Offset: 0x003287A4
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.Zork5v5, hint);
		}

		// Token: 0x06003B84 RID: 15236 RVA: 0x0032A5C0 File Offset: 0x003287C0
		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			KuaFu5v5FuBenData kfFuBenData = TianTiClient.getInstance().GetFuBenDataByGameId_ZorkBattle((int)kuaFuServerLoginData.GameId);
			bool result;
			if (kfFuBenData == null || kfFuBenData.ServerId != GameManager.ServerId)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06003B85 RID: 15237 RVA: 0x0032A62C File Offset: 0x0032882C
		public bool OnInitGameKuaFu(GameClient client)
		{
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			KuaFu5v5FuBenData fuBenData;
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
				KuaFu5v5FuBenData newFuBenData = TianTiClient.getInstance().GetFuBenDataByGameId_ZorkBattle((int)kuaFuServerLoginData.GameId);
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
						fuBenData.FuBenSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenItemData[fuBenData.GameId] = fuBenData;
					}
				}
			}
			bool result;
			if (fuBenData == null || fuBenData.State >= GameFuBenState.End)
			{
				result = false;
			}
			else
			{
				KuaFuFuBenRoleData role;
				if (fuBenData.RoleDict.TryGetValue(client.ClientData.RoleID, out role))
				{
					client.ClientData.BattleWhichSide = role.Side;
				}
				int posX;
				int posY;
				int side = this.GetBirthPoint(client, out posX, out posY);
				if (side <= 0)
				{
					result = false;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						kuaFuServerLoginData.FuBenSeqId = fuBenData.FuBenSeqID;
						ZorkBattleSceneInfo sceneInfo = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
						if (null == sceneInfo)
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

		// Token: 0x06003B86 RID: 15238 RVA: 0x0032A914 File Offset: 0x00328B14
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.ZorkBattle)
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
					long gameId = Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime now = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						ZorkBattleScene scene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
						{
							ZorkBattleSceneInfo sceneInfo = null;
							KuaFu5v5FuBenData fuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue((int)gameId, out fuBenData))
							{
								LogManager.WriteLog(LogTypes.Error, "魔域夺宝没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							if (!this.RuntimeData.SceneDataDict.TryGetValue(client.ClientData.MapCode, out sceneInfo))
							{
								LogManager.WriteLog(LogTypes.Error, "魔域夺宝没有为副本找到对应的场景数据,MapCodeID:" + client.ClientData.MapCode, null, true);
							}
							scene = new ZorkBattleScene();
							scene.CleanAllInfo();
							scene.GameId = (int)gameId;
							scene.CopyMap = copyMap;
							scene.m_nMapCode = mapCode;
							scene.CopyMapId = copyMap.CopyMapID;
							scene.FuBenSeqId = fuBenSeqId;
							scene.SceneInfo = sceneInfo;
							scene.MapGridWidth = gameMap.MapGridWidth;
							scene.MapGridHeight = gameMap.MapGridHeight;
							DateTime startTime = now.Date.Add(this.GetStartTime(sceneInfo.MapCode));
							scene.StartTimeTicks = startTime.Ticks / 10000L;
							this.InitScene(scene, client);
							scene.GameStatisticalData.GameId = (int)gameId;
							this.SceneDict[fuBenSeqId] = scene;
							foreach (KeyValuePair<int, string> zhandui in fuBenData.ZhanDuiNameDict)
							{
								ZorkBattleTeamInfo zhanduiInfo = new ZorkBattleTeamInfo
								{
									TeamID = zhandui.Key,
									TeamName = zhandui.Value
								};
								scene.ScoreData.ZorkBattleTeamList.Add(zhanduiInfo);
							}
						}
						if (!scene.GameStatisticalData.ZhanDuiDict.Keys.Contains(client.ClientData.ZhanDuiID))
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
							if (null != zhanDuiData)
							{
								scene.GameStatisticalData.ZhanDuiDict[client.ClientData.ZhanDuiID] = zhanDuiData;
								scene.GameStatisticalData.ZhanDuiIDVsServerIDDict[client.ClientData.ZhanDuiID] = client.ServerId;
							}
						}
						List<ZorkBattleRoleInfo> clientContextDataList;
						if (!scene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out clientContextDataList))
						{
							clientContextDataList = new List<ZorkBattleRoleInfo>();
							scene.ClientContextDataDict[client.ClientData.ZhanDuiID] = clientContextDataList;
						}
						ZorkBattleRoleInfo clientContextData = clientContextDataList.Find((ZorkBattleRoleInfo x) => x.RoleID == roleId);
						if (null == clientContextData)
						{
							clientContextData = new ZorkBattleRoleInfo
							{
								RoleID = roleId,
								Name = client.ClientData.RoleName,
								RebornLevel = client.ClientData.RebornLevel,
								RebornCount = client.ClientData.RebornCount,
								ZoneID = client.ClientData.ZoneID,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								LifeV = client.ClientData.CurrentLifeV,
								MaxLifeV = client.ClientData.LifeV,
								ZhanDuiID = client.ClientData.ZhanDuiID,
								OnLine = true
							};
							clientContextDataList.Add(clientContextData);
						}
						else
						{
							clientContextData.Occupation = client.ClientData.Occupation;
							clientContextData.RoleSex = client.ClientData.RoleSex;
							clientContextData.LifeV = client.ClientData.CurrentLifeV;
							clientContextData.MaxLifeV = client.ClientData.LifeV;
							clientContextData.OnLine = true;
						}
						client.SceneObject = scene;
						client.SceneGameId = (long)scene.GameId;
						client.SceneContextData2 = clientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(scene.SceneInfo.TotalSecs * 1000));
					}
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06003B87 RID: 15239 RVA: 0x0032AE4C File Offset: 0x0032904C
		private void HandleNtfEnterEvent(KuaFu5v5FuBenData data)
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				if (this.IsGongNengOpened(client, false) && this.CheckMap(client))
				{
					if (client != null && data.ZhanDuiDict.Keys.Contains(client.ClientData.ZhanDuiID))
					{
						client.sendCmd<int>(2101, 1, false);
					}
				}
			}
			string zhanduiIdArray = string.Join<int>("|", data.ZhanDuiDict.Keys.ToArray<int>());
			LogManager.WriteLog(LogTypes.Error, string.Format("通知战队ID={0} 拥有进入魔域夺宝资格", zhanduiIdArray), null, true);
		}

		// Token: 0x06003B88 RID: 15240 RVA: 0x0032AF28 File Offset: 0x00329128
		private bool CheckMap(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.ZorkWarEnterMapSet.Contains(client.ClientData.MapCode))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003B89 RID: 15241 RVA: 0x0032AF9C File Offset: 0x0032919C
		public ZorkBattleAwardConfig GetZorkBattleAwardConfigByJiFen(int jifen)
		{
			ZorkBattleAwardConfig awardConfig = null;
			lock (this.RuntimeData.Mutex)
			{
				foreach (ZorkBattleAwardConfig item in this.RuntimeData.ZorkLevelRangeList)
				{
					if ((item.RankValue < 0 || jifen >= item.RankValue) && (awardConfig == null || item.ID > awardConfig.ID))
					{
						awardConfig = item;
					}
				}
			}
			return awardConfig;
		}

		// Token: 0x06003B8A RID: 15242 RVA: 0x0032B074 File Offset: 0x00329274
		public List<int> GetZorkBattleRoleAnalysisData(int rid, int serverid)
		{
			List<int> result;
			if (0 == this.ZorkBattleSyncDataCache.CurSeasonID)
			{
				result = null;
			}
			else
			{
				List<int> countList = Global.GetRoleParamsIntListFromDBOffline(rid, "154", serverid);
				this.FilterZorkBattleAnalysisData(countList);
				result = countList;
			}
			return result;
		}

		// Token: 0x06003B8B RID: 15243 RVA: 0x0032B0B8 File Offset: 0x003292B8
		public List<int> GetZorkBattleRoleAnalysisData(GameClient client)
		{
			List<int> result;
			if (0 == this.ZorkBattleSyncDataCache.CurSeasonID)
			{
				result = null;
			}
			else
			{
				List<int> countList = Global.GetRoleParamsIntListFromDB(client, "154");
				this.FilterZorkBattleAnalysisData(countList);
				result = countList;
			}
			return result;
		}

		// Token: 0x06003B8C RID: 15244 RVA: 0x0032B0F8 File Offset: 0x003292F8
		private void SaveZorkBattleAchievementAwardData(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "155", true);
		}

		// Token: 0x06003B8D RID: 15245 RVA: 0x0032B10C File Offset: 0x0032930C
		public List<int> GetZorkBattleAchievementAwardData(GameClient client)
		{
			List<int> result;
			if (0 == this.ZorkBattleSyncDataCache.CurSeasonID)
			{
				result = null;
			}
			else
			{
				List<int> countList = Global.GetRoleParamsIntListFromDB(client, "155");
				this.FilterZorkBattleAchievementAwardData(countList);
				result = countList;
			}
			return result;
		}

		// Token: 0x06003B8E RID: 15246 RVA: 0x0032B164 File Offset: 0x00329364
		public void FilterZorkBattleAchievementAwardData(List<int> countList)
		{
			if (countList.Count != 3)
			{
				for (int i = countList.Count; i < 3; i++)
				{
					countList.Add(0);
				}
			}
			int dayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			if (this.ZorkBattleSyncDataCache.CurSeasonID != countList[0])
			{
				countList[0] = this.ZorkBattleSyncDataCache.CurSeasonID;
				for (int idx = 2; idx < countList.Count; idx++)
				{
					countList[idx] = 0;
				}
			}
			if (dayID != countList[1])
			{
				countList[1] = dayID;
			}
			countList.RemoveAll((int x) => x == 0);
		}

		// Token: 0x06003B8F RID: 15247 RVA: 0x0032B238 File Offset: 0x00329438
		private void FilterZorkBattleAnalysisData(List<int> countList)
		{
			if (countList.Count != 11)
			{
				for (int i = countList.Count; i < 11; i++)
				{
					countList.Add(0);
				}
			}
			int dayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			if (this.ZorkBattleSyncDataCache.CurSeasonID != countList[0])
			{
				countList[0] = this.ZorkBattleSyncDataCache.CurSeasonID;
				countList[2] = 0;
				countList[3] = 0;
				countList[4] = 0;
				countList[5] = 0;
				countList[7] = 0;
				countList[8] = 0;
				countList[9] = 0;
				countList[10] = 0;
			}
			if (dayID != countList[1])
			{
				countList[1] = dayID;
				countList[6] = 0;
			}
		}

		// Token: 0x06003B90 RID: 15248 RVA: 0x0032B318 File Offset: 0x00329518
		private void SaveZorkBattleRoleAnalysisDataOffline(int rid, List<int> countList, int serverid)
		{
			Global.SaveRoleParamsIntListToDBOffline(rid, countList, "154", serverid);
		}

		// Token: 0x06003B91 RID: 15249 RVA: 0x0032B329 File Offset: 0x00329529
		private void SaveZorkBattleRoleAnalysisData(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "154", true);
		}

		// Token: 0x06003B92 RID: 15250 RVA: 0x0032B33C File Offset: 0x0032953C
		private void UpdateChengHaoBuffer(GameClient client)
		{
			if (this.ZorkBattleSyncDataCache.TopZhanDui > 0 && client.ClientData.ZhanDuiID == this.ZorkBattleSyncDataCache.TopZhanDui)
			{
				double[] bufferParams = new double[]
				{
					1.0
				};
				Global.UpdateBufferData(client, BufferItemTypes.ZorkTopTeam_Title, bufferParams, 1, true);
			}
			else
			{
				double[] array = new double[1];
				double[] bufferParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.ZorkTopTeam_Title, bufferParams, 1, true);
			}
			if (this.ZorkBattleSyncDataCache.TopKiller > 0 && client.ClientData.RoleID == this.ZorkBattleSyncDataCache.TopKiller)
			{
				double[] bufferParams = new double[]
				{
					1.0
				};
				Global.UpdateBufferData(client, BufferItemTypes.ZorkTopKiller_Title, bufferParams, 1, true);
			}
			else
			{
				double[] array = new double[1];
				double[] bufferParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.ZorkTopKiller_Title, bufferParams, 1, true);
			}
		}

		// Token: 0x06003B93 RID: 15251 RVA: 0x0032B430 File Offset: 0x00329630
		public void InitZorkBattleZhanDuiData(TianTi5v5ZhanDuiData zhanduiData)
		{
			if (zhanduiData != null && 0 != this.ZorkBattleSyncDataCache.CurSeasonID)
			{
				DateTime SeasonTime = ZorkBattleUtils.GetSeasonDateTm(this.ZorkBattleSyncDataCache.CurSeasonID);
				if (zhanduiData.ZorkLastFightTime < SeasonTime)
				{
					zhanduiData.ZorkJiFen = 0;
					zhanduiData.ZorkBossInjure = 0;
					zhanduiData.ZorkWin = 0;
					zhanduiData.ZorkWinStreak = 0;
				}
			}
		}

		// Token: 0x06003B94 RID: 15252 RVA: 0x0032B4A0 File Offset: 0x003296A0
		public TianTi5v5ZhanDuiData GetZhanDuiData(int zhanDuiID, int serverID)
		{
			TianTi5v5ZhanDuiData zhanduiData = TianTi5v5Manager.getInstance().GetZhanDuiData(zhanDuiID, serverID);
			if (null == zhanduiData)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("获取战队数据失败 ZhanDuiID={0} ServerID={1}", zhanDuiID, serverID), null, true);
			}
			this.InitZorkBattleZhanDuiData(zhanduiData);
			return zhanduiData;
		}

		// Token: 0x06003B95 RID: 15253 RVA: 0x0032B4F4 File Offset: 0x003296F4
		public bool OnPreZhanDuiChangeMember(PreZhanDuiChangeMemberEventObject e)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZorkBattleSceneInfo sceneItem = null;
				ZorkBattleGameStates timeState = ZorkBattleGameStates.None;
				this.CheckCondition(null, ref sceneItem, ref timeState);
				if (ZorkBattleGameStates.None == timeState)
				{
					return false;
				}
				string gamestate = TianTiClient.getInstance().GetKuaFuGameState_ZorkBattle(e.ZhanDuiID);
				if (string.IsNullOrEmpty(gamestate))
				{
					return false;
				}
				int signState = Global.SafeConvertToInt32(gamestate);
				if (signState != -4034)
				{
					return false;
				}
				e.Result = false;
			}
			bool result;
			if (!e.Result)
			{
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(8001, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06003B96 RID: 15254 RVA: 0x0032B5F0 File Offset: 0x003297F0
		public bool CheckOpenState(DateTime now)
		{
			bool result;
			lock (this.RuntimeData.Mutex)
			{
				if (now < this.RuntimeData.ZorkStartTime)
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06003B97 RID: 15255 RVA: 0x0032B658 File Offset: 0x00329858
		private void InitScene(ZorkBattleScene scene, GameClient client)
		{
			foreach (ZorkBattleArmyConfig item in this.RuntimeData.ZorkBattleArmyList)
			{
				scene.ZorkBattleArmyList.Add(item.Clone());
			}
			foreach (KeyValuePair<int, List<ZorkBattleMonsterConfig>> item2 in this.RuntimeData.ZorkBattleMonsterDict)
			{
				List<ZorkBattleMonsterConfig> configList = new List<ZorkBattleMonsterConfig>();
				foreach (ZorkBattleMonsterConfig config in item2.Value)
				{
					configList.Add(config.Clone());
				}
				scene.ZorkBattleMonsterDict.Add(item2.Key, configList);
			}
		}

		// Token: 0x06003B98 RID: 15256 RVA: 0x0032B780 File Offset: 0x00329980
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.ZorkBattle)
			{
				lock (this.RuntimeData.Mutex)
				{
					ZorkBattleScene ZorkBattleScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out ZorkBattleScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06003B99 RID: 15257 RVA: 0x0032B7F8 File Offset: 0x003299F8
		private int CheckCondition(GameClient client, ref ZorkBattleSceneInfo sceneItem, ref ZorkBattleGameStates state)
		{
			int result = 0;
			sceneItem = null;
			lock (this.RuntimeData.Mutex)
			{
				sceneItem = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
				if (null == sceneItem)
				{
					return -12;
				}
			}
			state = ZorkBattleGameStates.None;
			result = 0;
			DateTime now = TimeUtil.NowDateTime();
			double TotalSeconds = now.TimeOfDay.TotalSeconds + (double)this.ZorkBattleSyncDataCache.DiffKFCenterSeconds;
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && TotalSeconds >= sceneItem.SecondsOfDay[i] && TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						int RoundSeconds = sceneItem.BattleSignSecs + sceneItem.PrepareSecs + sceneItem.FightingSecs + sceneItem.ClearRolesSecs;
						int MatchPerRound = (int)(sceneItem.SecondsOfDay[i + 1] - sceneItem.SecondsOfDay[i]) / RoundSeconds;
						for (int matchloop = 0; matchloop < MatchPerRound; matchloop++)
						{
							int signSeconds = (int)sceneItem.SecondsOfDay[i] + RoundSeconds * matchloop;
							int startSeconds = signSeconds + sceneItem.BattleSignSecs;
							int endSeconds = startSeconds + RoundSeconds - sceneItem.BattleSignSecs;
							if (TotalSeconds >= (double)signSeconds && TotalSeconds < (double)startSeconds)
							{
								state = ZorkBattleGameStates.SignUp;
							}
							else if (TotalSeconds >= (double)startSeconds && TotalSeconds < (double)endSeconds)
							{
								state = ZorkBattleGameStates.Start;
							}
						}
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06003B9A RID: 15258 RVA: 0x0032BA50 File Offset: 0x00329C50
		private TimeSpan GetStartTime(int MapCodeID)
		{
			ZorkBattleSceneInfo sceneItem = null;
			TimeSpan startTime = TimeSpan.MinValue;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				sceneItem = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
				if (null == sceneItem)
				{
					goto IL_212;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						int RoundSeconds = sceneItem.BattleSignSecs + sceneItem.PrepareSecs + sceneItem.FightingSecs + sceneItem.ClearRolesSecs;
						int MatchPerRound = (int)(sceneItem.SecondsOfDay[i + 1] - sceneItem.SecondsOfDay[i]) / RoundSeconds;
						for (int matchloop = 0; matchloop < MatchPerRound; matchloop++)
						{
							int signSeconds = (int)sceneItem.SecondsOfDay[i] + RoundSeconds * matchloop;
							int startSeconds = signSeconds + sceneItem.BattleSignSecs;
							int endSeconds = startSeconds + RoundSeconds - sceneItem.BattleSignSecs;
							if (now.TimeOfDay.TotalSeconds >= (double)signSeconds && now.TimeOfDay.TotalSeconds < (double)endSeconds)
							{
								startTime = TimeSpan.FromSeconds((double)startSeconds);
							}
						}
						break;
					}
				}
			}
			IL_212:
			if (startTime < TimeSpan.Zero)
			{
				startTime = now.TimeOfDay;
			}
			return startTime;
		}

		// Token: 0x06003B9B RID: 15259 RVA: 0x0032BCCC File Offset: 0x00329ECC
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
				ZorkBattleScene scene = client.SceneObject as ZorkBattleScene;
				this.BroadScoreInfo(scene.CopyMap, client.ClientData.ZhanDuiID);
				result = true;
			}
			return result;
		}

		// Token: 0x06003B9C RID: 15260 RVA: 0x0032BDCC File Offset: 0x00329FCC
		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			ZorkBattleRoleInfo contextData = client.SceneContextData2 as ZorkBattleRoleInfo;
			ZorkBattleScene scene = client.SceneObject as ZorkBattleScene;
			ZorkBattleMonsterCreateTag tagInfo = monster.Tag as ZorkBattleMonsterCreateTag;
			if (scene != null && tagInfo != null && null != contextData)
			{
				ZorkBattleTeamInfo teamInfo = scene.ScoreData.ZorkBattleTeamList.Find((ZorkBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
				if (null != teamInfo)
				{
					int addScore = 0;
					lock (this.RuntimeData.Mutex)
					{
						if (scene.m_eStatus != GameSceneStatuses.STATUS_BEGIN)
						{
							return;
						}
						if (tagInfo.monsterTag.ArmyType == ZorkBattleArmyType.Guard)
						{
							contextData.KillGuardNum++;
							addScore += tagInfo.monsterTag.RewardIntegral;
							tagInfo.armyTag.MonsterDeadNum++;
							if (tagInfo.armyTag.MonsterDeadNum >= tagInfo.monsterTag.MonsterNum)
							{
								tagInfo.armyTag.MonsterDeadNum = 0;
								int groupID = tagInfo.armyTag.RandomGroupID();
								List<ZorkBattleMonsterConfig> monsterConfigList = null;
								if (scene.ZorkBattleMonsterDict.TryGetValue(groupID, out monsterConfigList))
								{
									foreach (ZorkBattleMonsterConfig item in monsterConfigList)
									{
										ZorkBattleMonsterCreateTag tag = new ZorkBattleMonsterCreateTag
										{
											armyTag = tagInfo.armyTag,
											monsterTag = item
										};
										long refreshMs = TimeUtil.NOW() + (long)(tagInfo.armyTag.NextArmyRefresTime * 1000);
										DateTime refreshTm = new DateTime(refreshMs * 10000L);
										scene.ScoreData.MosterNextTimeDict[tagInfo.armyTag.ID] = refreshTm.ToString("yyyy-MM-dd HH:mm:ss");
										this.AddDelayCreateMonster(scene, refreshMs, tag);
									}
								}
							}
						}
						else if (tagInfo.monsterTag.ArmyType == ZorkBattleArmyType.Monster)
						{
							addScore += tagInfo.monsterTag.RewardIntegral;
							tagInfo.armyTag.MonsterDeadNum++;
							if (tagInfo.armyTag.MonsterDeadNum >= tagInfo.monsterTag.MonsterNum)
							{
								tagInfo.armyTag.MonsterDeadNum = 0;
								int groupID = tagInfo.armyTag.RandomGroupID();
								List<ZorkBattleMonsterConfig> monsterConfigList = null;
								if (scene.ZorkBattleMonsterDict.TryGetValue(groupID, out monsterConfigList))
								{
									foreach (ZorkBattleMonsterConfig item in monsterConfigList)
									{
										ZorkBattleMonsterCreateTag tag = new ZorkBattleMonsterCreateTag
										{
											armyTag = tagInfo.armyTag,
											monsterTag = item
										};
										long refreshMs = TimeUtil.NOW() + (long)(tagInfo.armyTag.NextArmyRefresTime * 1000);
										this.AddDelayCreateMonster(scene, refreshMs, tag);
									}
								}
							}
						}
						else if (tagInfo.monsterTag.ArmyType == ZorkBattleArmyType.Boss)
						{
							contextData.BossKillAwardsItemList = tagInfo.monsterTag.BossKillAwardsItemList;
							contextData.KillBossNum++;
							this.ProcessEnd(scene, true, TimeUtil.NOW());
						}
						contextData.TotalScore += addScore;
						teamInfo.JiFen += addScore;
						if (tagInfo.monsterTag.MonsterDropBuffId > 0)
						{
							List<ZorkBattleRoleInfo> clientContextDataList;
							if (scene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out clientContextDataList))
							{
								this.UpdateBuff4GameScene(scene, clientContextDataList, tagInfo.monsterTag.MonsterDropBuffId, tagInfo, true);
							}
						}
					}
					if (addScore > 0 && scene != null)
					{
						string msg = string.Format(GLang.GetLang(8007, new object[0]), addScore);
						GameManager.ClientMgr.NotifyImportantMsg(client, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						this.ResortZhanDuiRankByJiFen(scene);
						this.BroadScoreInfo(scene.CopyMap, -1);
					}
				}
			}
		}

		// Token: 0x06003B9D RID: 15261 RVA: 0x0032C2D0 File Offset: 0x0032A4D0
		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (401 == monster.MonsterType)
			{
				ZorkBattleRoleInfo contextData = client.SceneContextData2 as ZorkBattleRoleInfo;
				ZorkBattleMonsterCreateTag tagInfo = monster.Tag as ZorkBattleMonsterCreateTag;
				if (contextData != null && tagInfo != null && tagInfo.monsterTag.ArmyType == ZorkBattleArmyType.Boss)
				{
					ZorkBattleScene scene = client.SceneObject as ZorkBattleScene;
					if (scene != null && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						ZorkBattleTeamInfo teamInfo = scene.ScoreData.ZorkBattleTeamList.Find((ZorkBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
						if (null != teamInfo)
						{
							int addScore = 0;
							bool broad = false;
							double jiFenInjure = tagInfo.monsterTag.BossBlood * monster.MonsterInfo.VLifeMax;
							lock (this.RuntimeData.Mutex)
							{
								double InjureBossDelta = 0.0;
								contextData.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out InjureBossDelta);
								InjureBossDelta += (double)injure;
								if (InjureBossDelta >= jiFenInjure && jiFenInjure > 0.0)
								{
									int calcRate = (int)(InjureBossDelta / jiFenInjure);
									InjureBossDelta -= jiFenInjure * (double)calcRate;
									addScore += tagInfo.monsterTag.RewardIntegral * calcRate;
								}
								contextData.InjureBossDeltaDict[monster.MonsterInfo.ExtensionID] = InjureBossDelta;
								contextData.BossInjure += injure;
								contextData.TotalScore += addScore;
								teamInfo.JiFen += addScore;
								teamInfo.BossInjureTicks = TimeUtil.NOW();
								teamInfo.BossInjure += injure;
								int OldInjurePct = teamInfo.BossInjurePct;
								teamInfo.BossInjurePct = (int)((double)teamInfo.BossInjure / monster.MonsterInfo.VLifeMax * 100.0);
								if (OldInjurePct != teamInfo.BossInjurePct)
								{
									broad = true;
								}
							}
							if (addScore > 0 && scene != null)
							{
								broad = true;
								string msg = string.Format(GLang.GetLang(8007, new object[0]), addScore);
								GameManager.ClientMgr.NotifyImportantMsg(client, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								this.ResortZhanDuiRankByJiFen(scene);
							}
							if (broad)
							{
								this.BroadScoreInfo(scene.CopyMap, -1);
							}
						}
					}
				}
			}
		}

		// Token: 0x06003B9E RID: 15262 RVA: 0x0032C66C File Offset: 0x0032A86C
		private void ProcessEnd(ZorkBattleScene scene, bool bossKill, long nowTicks)
		{
			if (scene.m_eStatus < GameSceneStatuses.STATUS_END)
			{
				if (bossKill)
				{
					scene.ScoreData.ZorkBattleTeamList.Sort(delegate(ZorkBattleTeamInfo left, ZorkBattleTeamInfo right)
					{
						int result;
						if (left.BossInjure > right.BossInjure)
						{
							result = -1;
						}
						else if (left.BossInjure < right.BossInjure)
						{
							result = 1;
						}
						else if (left.BossInjureTicks > right.BossInjureTicks)
						{
							result = 1;
						}
						else if (left.BossInjureTicks < right.BossInjureTicks)
						{
							result = -1;
						}
						else if (left.TeamID > right.TeamID)
						{
							result = -1;
						}
						else if (left.TeamID < right.TeamID)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					scene.GameStatisticalData.ZhanDuiIDWin = scene.ScoreData.ZorkBattleTeamList[0].TeamID;
				}
				else
				{
					this.ResortZhanDuiRankByJiFen(scene);
					scene.GameStatisticalData.ZhanDuiIDWin = scene.ScoreData.ZorkBattleTeamList[0].TeamID;
				}
				foreach (KeyValuePair<int, List<ZorkBattleRoleInfo>> kvp in scene.ClientContextDataDict)
				{
					TianTi5v5ZhanDuiData zhanduiData2;
					if (scene.GameStatisticalData.ZhanDuiDict.TryGetValue(kvp.Key, out zhanduiData2))
					{
						int ServerID;
						if (scene.GameStatisticalData.ZhanDuiIDVsServerIDDict.TryGetValue(kvp.Key, out ServerID))
						{
							foreach (ZorkBattleRoleInfo contextData in kvp.Value)
							{
								GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleID);
								List<int> roleAnalysisData;
								if (client != null)
								{
									roleAnalysisData = this.GetZorkBattleRoleAnalysisData(client);
								}
								else
								{
									roleAnalysisData = this.GetZorkBattleRoleAnalysisData(contextData.RoleID, ServerID);
								}
								if (null != roleAnalysisData)
								{
									List<int> list;
									(list = roleAnalysisData)[2] = list[2] + contextData.KillRoleNum;
									(list = roleAnalysisData)[3] = list[3] + 1;
									(list = roleAnalysisData)[4] = list[4] + contextData.KillGuardNum;
									(list = roleAnalysisData)[5] = list[5] + contextData.KillBossNum;
									if (contextData.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
									{
										(list = roleAnalysisData)[6] = list[6] + 1;
										(list = roleAnalysisData)[7] = list[7] + 1;
										if (roleAnalysisData[7] > roleAnalysisData[8])
										{
											roleAnalysisData[8] = roleAnalysisData[7];
										}
										(list = roleAnalysisData)[9] = list[9] + 1;
									}
									else
									{
										roleAnalysisData[7] = 0;
									}
									(list = roleAnalysisData)[10] = list[10] + (int)(contextData.BossInjure / 10000L);
								}
								if (client != null)
								{
									this.SaveZorkBattleRoleAnalysisData(client, roleAnalysisData);
								}
								else
								{
									this.SaveZorkBattleRoleAnalysisDataOffline(contextData.RoleID, roleAnalysisData, ServerID);
								}
								if (contextData.KillRoleNum > 0)
								{
									scene.GameStatisticalData.ClientContextDataList.Add(contextData);
								}
							}
						}
					}
				}
				foreach (TianTi5v5ZhanDuiData item in scene.GameStatisticalData.ZhanDuiDict.Values)
				{
					TianTi5v5ZhanDuiData zhanduiData = item;
					int ServerID;
					if (scene.GameStatisticalData.ZhanDuiIDVsServerIDDict.TryGetValue(zhanduiData.ZhanDuiID, out ServerID))
					{
						ZorkBattleTeamInfo teamInfo = scene.ScoreData.ZorkBattleTeamList.Find((ZorkBattleTeamInfo x) => x.TeamID == zhanduiData.ZhanDuiID);
						if (null != teamInfo)
						{
							ZorkBattleAwardConfig awardConfig = this.GetZorkBattleAwardConfigByJiFen(zhanduiData.ZorkJiFen);
							if (null != awardConfig)
							{
								if (zhanduiData.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
								{
									zhanduiData.ZorkWin++;
									zhanduiData.ZorkWinStreak++;
									zhanduiData.ZorkJiFen += awardConfig.WinRankValue;
								}
								else
								{
									zhanduiData.ZorkWinStreak = 0;
									zhanduiData.ZorkJiFen -= awardConfig.LoseRankValue;
								}
								zhanduiData.ZorkBossInjure += (int)(teamInfo.BossInjure / 10000L);
								zhanduiData.ZorkJiFen = Math.Max(zhanduiData.ZorkJiFen, 0);
								zhanduiData.ZorkLastFightTime = TimeUtil.NowDateTime();
								TianTi5v5Manager.getInstance().UpdateZorkZhanDuiData2DB(zhanduiData, ServerID);
							}
						}
					}
				}
				scene.m_eStatus = GameSceneStatuses.STATUS_END;
				scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
				scene.StateTimeData.GameType = 36;
				scene.StateTimeData.State = 5;
				scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			}
		}

		// Token: 0x06003B9F RID: 15263 RVA: 0x0032CC2C File Offset: 0x0032AE2C
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= ZorkBattleManager.NextHeartBeatTicks)
			{
				ZorkBattleManager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (ZorkBattleScene scene in this.SceneDict.Values)
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
							if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE || scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								this.CheckCreateDynamicMonster(scene, ticks);
							}
							if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								if (ticks >= scene.StartTimeTicks)
								{
									scene.m_lPrepareTime = scene.StartTimeTicks;
									scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.PrepareSecs * 1000);
									scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
									scene.StateTimeData.GameType = 36;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									this.InitCreateDynamicMonster(scene, ticks);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (ticks >= scene.m_lBeginTime)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.m_lEndTime = scene.m_lBeginTime + (long)(scene.SceneInfo.FightingSecs * 1000);
									scene.StateTimeData.GameType = 36;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									for (int guangMuId = 1; guangMuId <= 4; guangMuId++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, guangMuId, 0);
									}
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime)
								{
									this.ProcessEnd(scene, false, ticks);
								}
								else
								{
									this.CheckSceneScoreData(scene, ticks);
									this.CheckSceneBufferTime(scene, ticks);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								this.GiveAwards(scene);
								GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
								KuaFu5v5FuBenData fuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("魔域夺宝跨服副本GameID={0},战斗结束", fuBenData.GameId), null, true);
									this.RuntimeData.FuBenItemData.Remove(scene.GameId);
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
										DataHelper.WriteExceptionLogEx(ex, "魔域夺宝系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003BA0 RID: 15264 RVA: 0x0032D0D4 File Offset: 0x0032B2D4
		private void InitCreateDynamicMonster(ZorkBattleScene scene, long nowMs)
		{
			foreach (ZorkBattleArmyConfig army in scene.ZorkBattleArmyList)
			{
				int groupID = army.RandomGroupID();
				List<ZorkBattleMonsterConfig> monsterConfigList = null;
				if (scene.ZorkBattleMonsterDict.TryGetValue(groupID, out monsterConfigList))
				{
					foreach (ZorkBattleMonsterConfig monster in monsterConfigList)
					{
						ZorkBattleMonsterCreateTag tag = new ZorkBattleMonsterCreateTag
						{
							armyTag = army,
							monsterTag = monster
						};
						long refreshMs = scene.m_lPrepareTime + (long)(army.FirstArmyTime * 1000);
						if (army.ArmyType == ZorkBattleArmyType.Boss || army.ArmyType == ZorkBattleArmyType.Guard)
						{
							DateTime refreshTm = new DateTime(refreshMs * 10000L);
							scene.ScoreData.MosterNextTimeDict[army.ID] = refreshTm.ToString("yyyy-MM-dd HH:mm:ss");
						}
						this.AddDelayCreateMonster(scene, refreshMs, tag);
					}
				}
			}
		}

		// Token: 0x06003BA1 RID: 15265 RVA: 0x0032D244 File Offset: 0x0032B444
		private void AddDelayCreateMonster(ZorkBattleScene scene, long ticks, object monster)
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

		// Token: 0x06003BA2 RID: 15266 RVA: 0x0032D2C0 File Offset: 0x0032B4C0
		public void CheckCreateDynamicMonster(ZorkBattleScene scene, long nowMs)
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
							if (obj is ZorkBattleMonsterCreateTag)
							{
								ZorkBattleMonsterCreateTag item = obj as ZorkBattleMonsterCreateTag;
								ZorkBattleArmyConfig armyConfig = item.armyTag;
								ZorkBattleMonsterConfig monsterConfig = item.monsterTag;
								if (null != armyConfig)
								{
									GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, monsterConfig.MonsterId, scene.CopyMapId, monsterConfig.MonsterNum, armyConfig.PosX / scene.MapGridWidth, armyConfig.PosY / scene.MapGridHeight, armyConfig.Range, armyConfig.PursuitRadius, SceneUIClasses.ZorkBattle, item, null);
									if (armyConfig.ArmyType == ZorkBattleArmyType.Boss || armyConfig.ArmyType == ZorkBattleArmyType.Guard)
									{
										scene.ScoreData.MosterNextTimeDict[armyConfig.ID] = "";
									}
								}
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

		// Token: 0x06003BA3 RID: 15267 RVA: 0x0032D4AC File Offset: 0x0032B6AC
		public void BroadScoreInfo(CopyMap copyMap, int specZhanDui = -1)
		{
			List<GameClient> objsList = copyMap.GetClientsList();
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i];
					if (specZhanDui <= 0 || c.ClientData.ZhanDuiID == specZhanDui)
					{
						if (c != null && c.ClientData.CopyMapID == copyMap.CopyMapID)
						{
							this.NotifyTimeStateInfoAndScoreInfo(c, false, true);
						}
					}
				}
			}
		}

		// Token: 0x06003BA4 RID: 15268 RVA: 0x0032D540 File Offset: 0x0032B740
		public void NotifySpriteInjured(GameClient client)
		{
			ZorkBattleScene scene = client.SceneObject as ZorkBattleScene;
			if (null != scene)
			{
				this.BroadScoreInfo(scene.CopyMap, client.ClientData.ZhanDuiID);
			}
		}

		// Token: 0x06003BA5 RID: 15269 RVA: 0x0032D5F4 File Offset: 0x0032B7F4
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZorkBattleScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (sideScore)
					{
						ZorkBattleSideScore ScoreData = scene.ScoreData.Clone();
						List<ZorkBattleRoleInfo> clientContextDataList;
						if (scene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out clientContextDataList))
						{
							ScoreData.ZorkBattleRoleList = clientContextDataList.FindAll((ZorkBattleRoleInfo x) => x.OnLine && x.RoleID != client.ClientData.RoleID);
							using (List<ZorkBattleRoleInfo>.Enumerator enumerator = ScoreData.ZorkBattleRoleList.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									ZorkBattleRoleInfo role = enumerator.Current;
									List<GameClient> objsList = scene.CopyMap.GetClientsList();
									if (objsList != null && objsList.Count >= 0)
									{
										GameClient other = objsList.Find((GameClient x) => x.ClientData.RoleID == role.RoleID);
										if (null != other)
										{
											role.LifeV = other.ClientData.CurrentLifeV;
											role.MaxLifeV = other.ClientData.LifeV;
										}
									}
								}
							}
						}
						client.sendCmd<ZorkBattleSideScore>(2104, ScoreData, false);
					}
				}
			}
		}

		// Token: 0x06003BA6 RID: 15270 RVA: 0x0032D858 File Offset: 0x0032BA58
		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZorkBattleScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						int addScore = 0;
						ZorkBattleRoleInfo clientLianShaContextData = client.SceneContextData2 as ZorkBattleRoleInfo;
						ZorkBattleRoleInfo otherLianShaContextData = other.SceneContextData2 as ZorkBattleRoleInfo;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
						huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
						huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
						huanYingSiYuanAddScore.ByLianShaNum = 1;
						huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
						huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
						if (null != clientLianShaContextData)
						{
							clientLianShaContextData.KillNum++;
							clientLianShaContextData.KillRoleNum++;
							int lianShaScore = this.RuntimeData.ZorkBattleUltraKillParam1 + clientLianShaContextData.KillNum * this.RuntimeData.ZorkBattleUltraKillParam2;
							lianShaScore = Math.Min(this.RuntimeData.ZorkBattleUltraKillParam4, Math.Max(this.RuntimeData.ZorkBattleUltraKillParam3, lianShaScore));
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
							huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
							huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
							huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
							huanYingSiYuanLianSha.LianShaType = Math.Min(clientLianShaContextData.KillNum, 30) / 5;
							huanYingSiYuanLianSha.ExtScore = lianShaScore;
							huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
							addScore += lianShaScore;
							if (clientLianShaContextData.KillNum % 5 != 0)
							{
								huanYingSiYuanLianSha = null;
							}
						}
						if (null != otherLianShaContextData)
						{
							int overScore = this.RuntimeData.ZorkBattleShutDownParam1 + otherLianShaContextData.KillNum * this.RuntimeData.ZorkBattleShutDownParam2;
							overScore = Math.Min(this.RuntimeData.ZorkBattleShutDownParam4, Math.Max(this.RuntimeData.ZorkBattleShutDownParam3, overScore));
							addScore += overScore;
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
						}
						huanYingSiYuanAddScore.Score = addScore;
						ZorkBattleTeamInfo killTeamInfo = scene.ScoreData.ZorkBattleTeamList.Find((ZorkBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
						if (null != killTeamInfo)
						{
							killTeamInfo.JiFen += addScore;
						}
						if (null != clientLianShaContextData)
						{
							clientLianShaContextData.TotalScore += addScore;
							string msg = string.Format(GLang.GetLang(8007, new object[0]), addScore);
							GameManager.ClientMgr.NotifyImportantMsg(client, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(2106, huanYingSiYuanLianSha, scene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(2107, huanYingSiYuanLianshaOver, scene.CopyMap);
						}
						this.ResortZhanDuiRankByJiFen(scene);
						this.BroadScoreInfo(scene.CopyMap, -1);
					}
				}
			}
		}

		// Token: 0x06003BA7 RID: 15271 RVA: 0x0032DEEC File Offset: 0x0032C0EC
		public void ResortZhanDuiRankByJiFen(ZorkBattleScene scene)
		{
            lock(this.RuntimeData.Mutex)
			{
				List<ZorkBattleTeamInfo> ZorkBattleTeamListOld = new List<ZorkBattleTeamInfo>(scene.ScoreData.ZorkBattleTeamList);
				List<ZorkBattleRoleInfo> clientContextDataList;
				scene.ScoreData.ZorkBattleTeamList.Sort(delegate(ZorkBattleTeamInfo left, ZorkBattleTeamInfo right)
				{
					int result;
					if (left.JiFen > right.JiFen)
					{
						result = -1;
					}
					else if (left.JiFen < right.JiFen)
					{
						result = 1;
					}
					else
					{
						int leftIdx = ZorkBattleTeamListOld.FindIndex((ZorkBattleTeamInfo x) => x.TeamID == left.TeamID);
						int rightIdx = ZorkBattleTeamListOld.FindIndex((ZorkBattleTeamInfo x) => x.TeamID == right.TeamID);
						if (leftIdx < rightIdx)
						{
							result = -1;
						}
						else if (leftIdx > rightIdx)
						{
							result = 1;
						}
						else
						{
							int leftEnterNum = 0;
							int rightEnterNum = 0;
							if (scene.ClientContextDataDict.TryGetValue(left.TeamID, out clientContextDataList))
							{
								leftEnterNum = clientContextDataList.Count;
							}
							if (scene.ClientContextDataDict.TryGetValue(right.TeamID, out clientContextDataList))
							{
								rightEnterNum = clientContextDataList.Count;
							}
							if (leftEnterNum > rightEnterNum)
							{
								result = -1;
							}
							else if (leftEnterNum < rightEnterNum)
							{
								result = 1;
							}
							else if (left.TeamID > right.TeamID)
							{
								result = -1;
							}
							else if (left.TeamID < right.TeamID)
							{
								result = 1;
							}
							else
							{
								result = 0;
							}
						}
					}
					return result;
				});
			}
        }

		// Token: 0x06003BA8 RID: 15272 RVA: 0x0032DFE8 File Offset: 0x0032C1E8
		public void GiveAwards(ZorkBattleScene scene)
		{
			try
			{
				using (Dictionary<int, List<ZorkBattleRoleInfo>>.Enumerator enumerator = scene.ClientContextDataDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, List<ZorkBattleRoleInfo>> kvp = enumerator.Current;
						Dictionary<int, TianTi5v5ZhanDuiData> zhanDuiDict = scene.GameStatisticalData.ZhanDuiDict;
						KeyValuePair<int, List<ZorkBattleRoleInfo>> kvp3 = kvp;
						TianTi5v5ZhanDuiData zhanduiData;
						if (zhanDuiDict.TryGetValue(kvp3.Key, out zhanduiData))
						{
							ZorkBattleTeamInfo teamInfo = scene.ScoreData.ZorkBattleTeamList.Find(delegate(ZorkBattleTeamInfo x)
							{
								int teamID = x.TeamID;
								KeyValuePair<int, List<ZorkBattleRoleInfo>> kvp2 = kvp;
								return teamID == kvp2.Key;
							});
							if (null != teamInfo)
							{
								int ranknum = scene.ScoreData.ZorkBattleTeamList.FindIndex(delegate(ZorkBattleTeamInfo x)
								{
									int teamID = x.TeamID;
									KeyValuePair<int, List<ZorkBattleRoleInfo>> kvp2 = kvp;
									return teamID == kvp2.Key;
								}) + 1;
								kvp3 = kvp;
								foreach (ZorkBattleRoleInfo contextData in kvp3.Value)
								{
									int success;
									if (contextData.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
									{
										success = 1;
									}
									else
									{
										success = 0;
									}
									GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleID);
									if (client != null && client.ClientData.MapCode == scene.m_nMapCode)
									{
										this.NtfCanGetAward(client, success, ranknum, zhanduiData, teamInfo);
										this.GiveRoleAwards(client, success, zhanduiData, teamInfo);
									}
								}
							}
						}
					}
				}
				this.PushGameResultData(scene);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "魔域夺宝系统清场调度异常");
			}
		}

		// Token: 0x06003BA9 RID: 15273 RVA: 0x0032E20C File Offset: 0x0032C40C
		public void PushGameResultData(ZorkBattleScene scene)
		{
			TianTiClient.getInstance().GameFuBenComplete_ZorkBattle(scene.GameStatisticalData);
		}

		// Token: 0x06003BAA RID: 15274 RVA: 0x0032E220 File Offset: 0x0032C420
		private void NtfCanGetAward(GameClient client, int success, int ranknum, TianTi5v5ZhanDuiData zhanduiData, ZorkBattleTeamInfo teamInfo)
		{
			ZorkBattleRoleInfo contextData = client.SceneContextData2 as ZorkBattleRoleInfo;
			ZorkBattleAwardConfig awardConfig = this.GetZorkBattleAwardConfigByJiFen(zhanduiData.ZorkJiFen);
			if (awardConfig != null && null != contextData)
			{
				ZorkBattleAwardsData awardsData = new ZorkBattleAwardsData();
				awardsData.Success = success;
				awardsData.RankNum = ranknum;
				awardsData.AwardID = awardConfig.ID;
				awardsData.SelfJiFen = contextData.TotalScore;
				awardsData.JiFen = teamInfo.JiFen;
				List<int> roleAnalysisData = this.GetZorkBattleRoleAnalysisData(client);
				if (null != roleAnalysisData)
				{
					awardsData.WinToDay = roleAnalysisData[6];
				}
				if (null != contextData.BossKillAwardsItemList)
				{
					awardsData.BossAwardGoodsDataList = (contextData.BossKillAwardsItemList as AwardsItemList).Items;
				}
				client.sendCmd<ZorkBattleAwardsData>(2102, awardsData, false);
			}
		}

		// Token: 0x06003BAB RID: 15275 RVA: 0x0032E2F0 File Offset: 0x0032C4F0
		private int GiveRoleAwards(GameClient client, int success, TianTi5v5ZhanDuiData zhanduiData, ZorkBattleTeamInfo teamInfo)
		{
			ZorkBattleRoleInfo contextData = client.SceneContextData2 as ZorkBattleRoleInfo;
			ZorkBattleAwardConfig awardConfig = this.GetZorkBattleAwardConfigByJiFen(zhanduiData.ZorkJiFen);
			int result;
			if (awardConfig == null || null == contextData)
			{
				result = -5;
			}
			else
			{
				List<AwardsItemData> awardsItemDataList;
				string sContent;
				if (success > 0)
				{
					awardsItemDataList = (awardConfig.WinAwardsItemList as AwardsItemList).Items;
					sContent = GLang.GetLang(8004, new object[0]);
				}
				else
				{
					awardsItemDataList = (awardConfig.LoseAwardsItemList as AwardsItemList).Items;
					sContent = GLang.GetLang(8005, new object[0]);
				}
				string sSubject = "魔域夺宝奖励";
				int RoleWinToDay = 0;
				List<int> roleAnalysisData = this.GetZorkBattleRoleAnalysisData(client);
				if (null != roleAnalysisData)
				{
					RoleWinToDay = roleAnalysisData[6];
				}
				List<AwardsItemData> AllAwardsItemDataList = new List<AwardsItemData>();
				if (awardConfig.FirstWinAwardsItemList != null && success > 0 && 1 == RoleWinToDay)
				{
					AllAwardsItemDataList.AddRange((awardConfig.FirstWinAwardsItemList as AwardsItemList).Items);
				}
				else
				{
					AllAwardsItemDataList.AddRange(awardsItemDataList);
				}
				if (null != contextData.BossKillAwardsItemList)
				{
					AllAwardsItemDataList.AddRange((contextData.BossKillAwardsItemList as AwardsItemList).Items);
				}
				int BagInt;
				if (AllAwardsItemDataList != null && !RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, AllAwardsItemDataList, out BagInt))
				{
					Global.UseMailGivePlayerAward2(client, AllAwardsItemDataList, GLang.GetLang(8002, new object[0]), sContent, 0, 0, 0);
				}
				else if (AllAwardsItemDataList != null)
				{
					foreach (AwardsItemData item in AllAwardsItemDataList)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, sSubject, "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
					}
				}
				result = 1;
			}
			return result;
		}

		// Token: 0x06003BAC RID: 15276 RVA: 0x0032E52C File Offset: 0x0032C72C
		public void LeaveFuBen(GameClient client)
		{
			ZorkBattleScene scene = client.SceneObject as ZorkBattleScene;
			if (null != scene)
			{
				ZorkBattleRoleInfo clientLianShaContextData = client.SceneContextData2 as ZorkBattleRoleInfo;
				clientLianShaContextData.OnLine = false;
				this.BroadScoreInfo(scene.CopyMap, -1);
			}
		}

		// Token: 0x06003BAD RID: 15277 RVA: 0x0032E571 File Offset: 0x0032C771
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		// Token: 0x06003BAE RID: 15278 RVA: 0x0032E57C File Offset: 0x0032C77C
		private void CheckSceneScoreData(ZorkBattleScene zorkBattleScene, long nowTicks)
		{
			bool NtfScoreInfo = false;
			foreach (ZorkBattleTeamInfo zhandui in zorkBattleScene.ScoreData.ZorkBattleTeamList)
			{
				if (nowTicks - zhandui.BossInjureTicks > (long)(this.RuntimeData.BossHurtCleanTime * 1000))
				{
					zhandui.BossInjure = 0L;
					zhandui.BossInjurePct = 0;
					NtfScoreInfo = true;
				}
			}
			if (NtfScoreInfo)
			{
				this.BroadScoreInfo(zorkBattleScene.CopyMap, -1);
			}
		}

		// Token: 0x06003BAF RID: 15279 RVA: 0x0032E624 File Offset: 0x0032C824
		private void CheckSceneBufferTime(ZorkBattleScene zorkBattleScene, long nowTicks)
		{
			List<ZorkBattleSceneBuff> sceneBuffDeleteList = new List<ZorkBattleSceneBuff>();
			lock (this.RuntimeData.Mutex)
			{
				if (zorkBattleScene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
				{
					if (zorkBattleScene.SceneBuffDict.Count != 0)
					{
						foreach (ZorkBattleSceneBuff contextData in zorkBattleScene.SceneBuffDict.Values)
						{
							if (contextData.EndTicks < nowTicks)
							{
								sceneBuffDeleteList.Add(contextData);
							}
						}
						if (sceneBuffDeleteList.Count != 0)
						{
							foreach (ZorkBattleSceneBuff contextData in sceneBuffDeleteList)
							{
								if (contextData.RoleID != 0)
								{
									GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleID);
									if (null != client)
									{
										this.UpdateBuff4GameScene(zorkBattleScene, client, contextData.BuffID, contextData.tagInfo, false);
									}
								}
								else if (contextData.ZhanDuiID != 0)
								{
									List<ZorkBattleRoleInfo> clientContextDataList;
									if (zorkBattleScene.ClientContextDataDict.TryGetValue(contextData.ZhanDuiID, out clientContextDataList))
									{
										this.UpdateBuff4GameScene(zorkBattleScene, clientContextDataList, contextData.BuffID, contextData.tagInfo, false);
									}
								}
								else if (contextData.MonsterID != 0)
								{
									Monster monster = GameManager.MonsterMgr.FindMonster(zorkBattleScene.m_nMapCode, contextData.MonsterID);
									if (null != monster)
									{
										this.UpdateBuff4GameScene(zorkBattleScene, monster, contextData.BuffID, contextData.tagInfo, false);
										ZorkBattleMonsterCreateTag tagInfo = monster.Tag as ZorkBattleMonsterCreateTag;
										if (null != tagInfo)
										{
											int buffID = tagInfo.monsterTag.RandomBuffID();
											this.UpdateBuff4GameScene(zorkBattleScene, monster, buffID, tagInfo, true);
											zorkBattleScene.ScoreData.BossBuffID = buffID;
											this.BroadScoreInfo(zorkBattleScene.CopyMap, -1);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003BB0 RID: 15280 RVA: 0x0032E8CC File Offset: 0x0032CACC
		public void OnStartPlayGame(GameClient client)
		{
			ZorkBattleScene scene = client.SceneObject as ZorkBattleScene;
			if (null != scene)
			{
				lock (this.RuntimeData.Mutex)
				{
					long nowTicks = TimeUtil.NOW();
					foreach (ZorkBattleSceneBuff buff in scene.SceneBuffDict.Values)
					{
						if (buff.ZhanDuiID == client.ClientData.ZhanDuiID)
						{
							if (nowTicks < buff.EndTicks)
							{
								EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(buff.BuffID);
								if (null != item)
								{
									int BuffTime = (int)((buff.EndTicks - nowTicks) / 1000L);
									if (BuffTime > 0)
									{
										double[] actionParams = new double[]
										{
											(double)BuffTime,
											(double)buff.BuffID
										};
										Global.UpdateBufferData(client, (BufferItemTypes)buff.BuffID, actionParams, 1, true);
										client.ClientData.PropsCacheManager.SetExtProps(new object[]
										{
											PropsSystemTypes.BufferByGoodsProps,
											buff.BuffID,
											item.ExtProps
										});
									}
								}
							}
						}
					}
				}
				this.BroadScoreInfo(scene.CopyMap, -1);
			}
			this.UpdateChengHaoBuffer(client);
		}

		// Token: 0x06003BB1 RID: 15281 RVA: 0x0032EAA4 File Offset: 0x0032CCA4
		private string BuildSceneBuffKey(object bufferOwner, int bufferGoodsID)
		{
			GameClient client = bufferOwner as GameClient;
			Monster monster = bufferOwner as Monster;
			List<ZorkBattleRoleInfo> zhanduiRoleList = bufferOwner as List<ZorkBattleRoleInfo>;
			string key = "";
			if (null != zhanduiRoleList)
			{
				key = string.Format("Team_{0}_{1}", zhanduiRoleList[0].ZhanDuiID, bufferGoodsID);
			}
			else if (null != client)
			{
				key = string.Format("Role_{0}_{1}", client.ClientData.RoleID, bufferGoodsID);
			}
			else if (null != monster)
			{
				key = string.Format("Monster_{0}_{1}", monster.RoleID, bufferGoodsID);
			}
			return key;
		}

		// Token: 0x06003BB2 RID: 15282 RVA: 0x0032EB5C File Offset: 0x0032CD5C
		private void UpdateBuff4GameScene(ZorkBattleScene scene, object bufferOwner, int bufferGoodsID, object tagInfo, bool add)
		{
			try
			{
				GameClient client = bufferOwner as GameClient;
				Monster monster = bufferOwner as Monster;
				List<ZorkBattleRoleInfo> zhanduiRoleList = bufferOwner as List<ZorkBattleRoleInfo>;
				if (client == null && monster == null && null == zhanduiRoleList)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("魔域夺宝 BuffGoodsID:{0} 获取Buff拥有者失败", bufferGoodsID), null, true);
				}
				else
				{
					ZorkBattleMonsterCreateTag mTagInfo = tagInfo as ZorkBattleMonsterCreateTag;
					if (null == mTagInfo)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("魔域夺宝 BuffGoodsID:{0} 获取Monster附件信息失败", bufferGoodsID), null, true);
					}
					else
					{
						int BuffTime = 0;
						if (null != mTagInfo)
						{
							if (mTagInfo.monsterTag.ArmyType == ZorkBattleArmyType.Boss)
							{
								BuffTime = mTagInfo.monsterTag.BuffRefreshTime;
							}
							else
							{
								BuffTime = mTagInfo.monsterTag.BuffEffictTime;
							}
						}
						int ZhanDuiID = 0;
						int RoleID = 0;
						int MonsterID = 0;
						if (null != zhanduiRoleList)
						{
							ZhanDuiID = zhanduiRoleList[0].ZhanDuiID;
						}
						else if (null != client)
						{
							RoleID = client.ClientData.RoleID;
						}
						else
						{
							MonsterID = monster.RoleID;
						}
						EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(bufferGoodsID);
						if (null == item)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔域夺宝 BuffGoodsID:{0} 获取属性信息失败", bufferGoodsID), null, true);
						}
						else
						{
							if (add)
							{
								if (null != zhanduiRoleList)
								{
									foreach (ZorkBattleRoleInfo role in zhanduiRoleList)
									{
										GameClient zhanduiMem = GameManager.ClientMgr.FindClient(role.RoleID);
										if (zhanduiMem != null && zhanduiMem.ClientData.MapCode == scene.m_nMapCode)
										{
											double[] actionParams = new double[]
											{
												(double)BuffTime,
												(double)bufferGoodsID
											};
											Global.UpdateBufferData(zhanduiMem, (BufferItemTypes)bufferGoodsID, actionParams, 1, true);
											zhanduiMem.ClientData.PropsCacheManager.SetExtProps(new object[]
											{
												PropsSystemTypes.BufferByGoodsProps,
												bufferGoodsID,
												item.ExtProps
											});
										}
									}
								}
								else if (null != client)
								{
									double[] actionParams = new double[]
									{
										(double)BuffTime,
										(double)bufferGoodsID
									};
									Global.UpdateBufferData(client, (BufferItemTypes)bufferGoodsID, actionParams, 1, true);
									client.ClientData.PropsCacheManager.SetExtProps(new object[]
									{
										PropsSystemTypes.BufferByGoodsProps,
										bufferGoodsID,
										item.ExtProps
									});
								}
								else
								{
									for (int i = 0; i < item.ExtProps.Length; i++)
									{
										monster.DynamicData.ExtProps[i] += item.ExtProps[i];
									}
								}
								string Key = this.BuildSceneBuffKey(bufferOwner, bufferGoodsID);
								scene.SceneBuffDict[Key] = new ZorkBattleSceneBuff
								{
									ZhanDuiID = ZhanDuiID,
									RoleID = RoleID,
									MonsterID = MonsterID,
									BuffID = bufferGoodsID,
									EndTicks = TimeUtil.NOW() + (long)(BuffTime * 1000),
									tagInfo = tagInfo
								};
								LogManager.WriteLog(LogTypes.Error, string.Format("魔域夺宝 BuffKey:{0} Add:{1}", Key, add), null, true);
							}
							else
							{
								if (null != zhanduiRoleList)
								{
									foreach (ZorkBattleRoleInfo role in zhanduiRoleList)
									{
										GameClient zhanduiMem = GameManager.ClientMgr.FindClient(role.RoleID);
										if (zhanduiMem != null && zhanduiMem.ClientData.MapCode == scene.m_nMapCode)
										{
											Global.RemoveBufferData(zhanduiMem, bufferGoodsID);
											zhanduiMem.ClientData.PropsCacheManager.SetExtProps(new object[]
											{
												PropsSystemTypes.BufferByGoodsProps,
												bufferGoodsID,
												PropsCacheManager.ConstExtProps
											});
										}
									}
								}
								else if (null != client)
								{
									Global.RemoveBufferData(client, bufferGoodsID);
									client.ClientData.PropsCacheManager.SetExtProps(new object[]
									{
										PropsSystemTypes.BufferByGoodsProps,
										bufferGoodsID,
										PropsCacheManager.ConstExtProps
									});
								}
								else
								{
									for (int i = 0; i < item.ExtProps.Length; i++)
									{
										monster.DynamicData.ExtProps[i] -= item.ExtProps[i];
									}
								}
								string Key = this.BuildSceneBuffKey(bufferOwner, bufferGoodsID);
								scene.SceneBuffDict.Remove(Key);
								LogManager.WriteLog(LogTypes.Error, string.Format("魔域夺宝 BuffKey:{0} Add:{1}", Key, add), null, true);
							}
							if (null != zhanduiRoleList)
							{
								foreach (ZorkBattleRoleInfo role in zhanduiRoleList)
								{
									GameClient zhanduiMem = GameManager.ClientMgr.FindClient(role.RoleID);
									if (zhanduiMem != null && zhanduiMem.ClientData.MapCode == scene.m_nMapCode)
									{
										GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, zhanduiMem);
									}
								}
							}
							else if (null != client)
							{
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x04004625 RID: 17957
		public const SceneUIClasses ManagerType = SceneUIClasses.ZorkBattle;

		// Token: 0x04004626 RID: 17958
		private static ZorkBattleManager instance = new ZorkBattleManager();

		// Token: 0x04004627 RID: 17959
		public ZorkBattleData RuntimeData = new ZorkBattleData();

		// Token: 0x04004628 RID: 17960
		public ZorkBattleSyncData ZorkBattleSyncDataCache = new ZorkBattleSyncData();

		// Token: 0x04004629 RID: 17961
		public ConcurrentDictionary<int, ZorkBattleScene> SceneDict = new ConcurrentDictionary<int, ZorkBattleScene>();

		// Token: 0x0400462A RID: 17962
		private static long NextHeartBeatTicks = 0L;
	}
}
