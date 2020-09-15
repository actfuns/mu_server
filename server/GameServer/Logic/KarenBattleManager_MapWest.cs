using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000324 RID: 804
	public class KarenBattleManager_MapWest : IManager, IEventListener, IEventListenerEx, IManager2
	{
		// Token: 0x06000D54 RID: 3412 RVA: 0x000CFA28 File Offset: 0x000CDC28
		public static KarenBattleManager_MapWest getInstance()
		{
			return KarenBattleManager_MapWest.instance;
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x000CFA40 File Offset: 0x000CDC40
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x000CFA64 File Offset: 0x000CDC64
		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x000CFA78 File Offset: 0x000CDC78
		public bool startup()
		{
			GlobalEventSource4Scene.getInstance().registerListener(33, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource.getInstance().registerListener(10, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource.getInstance().registerListener(11, KarenBattleManager_MapWest.getInstance());
			return true;
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x000CFAEC File Offset: 0x000CDCEC
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(33, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(27, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource.getInstance().removeListener(10, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource.getInstance().removeListener(11, KarenBattleManager_MapWest.getInstance());
			return true;
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x000CFB60 File Offset: 0x000CDD60
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x000CFB74 File Offset: 0x000CDD74
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEvent = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEvent)
				{
				}
			}
			if (eventType == 11)
			{
				MonsterDeadEventObject e = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(e.getAttacker(), e.getMonster());
			}
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x000CFBD0 File Offset: 0x000CDDD0
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num != 27)
			{
				if (num != 30)
				{
					if (num == 33)
					{
						PreMonsterInjureEventObject obj = eventObject as PreMonsterInjureEventObject;
						if (obj != null && obj.SceneType == 41)
						{
							Monster injureMonster = obj.Monster;
							if (injureMonster != null)
							{
								if (this.IsQiZhiExtensionID(injureMonster.MonsterInfo.ExtensionID))
								{
									this.RuntimeData.KarenBattleDamage.TryGetValue(injureMonster.MonsterInfo.ExtensionID, out obj.Injure);
									eventObject.Handled = true;
									eventObject.Result = true;
								}
							}
						}
					}
				}
				else
				{
					OnCreateMonsterEventObject e = eventObject as OnCreateMonsterEventObject;
					if (null != e)
					{
						KarenBattleQiZhiConfig_West qiZhiConfig = e.Monster.Tag as KarenBattleQiZhiConfig_West;
						if (null != qiZhiConfig)
						{
							e.Monster.Camp = qiZhiConfig.BattleWhichSide;
							e.Result = true;
							e.Handled = true;
						}
					}
				}
			}
			else
			{
				ProcessClickOnNpcEventObject e2 = eventObject as ProcessClickOnNpcEventObject;
				if (null != e2)
				{
					if (null != e2.Npc)
					{
						int npcId = e2.Npc.NpcID;
					}
					if (this.OnSpriteClickOnNpc(e2.Client, e2.NpcId, e2.ExtensionID))
					{
						e2.Result = false;
						e2.Handled = true;
					}
				}
			}
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x000CFD5C File Offset: 0x000CDF5C
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.MapBirthPointDict.Clear();
					fileName = "Config/LegionsWestBirthPoint.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KarenBattleBirthPoint item = new KarenBattleBirthPoint();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item.BirthRadius = (int)Global.GetSafeAttributeLong(node, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[item.ID] = item;
					}
					this.RuntimeData.KarenBattleDamage.Clear();
					List<string> damageList = GameManager.systemParamsList.GetParamValueStringListByName("LegionsWest", '|');
					if (null != damageList)
					{
						foreach (string item2 in damageList)
						{
							string[] intArray = item2.Split(new char[]
							{
								','
							});
							int key = Global.SafeConvertToInt32(intArray[0]);
							int value = Global.SafeConvertToInt32(intArray[1]);
							this.RuntimeData.KarenBattleDamage[key] = value;
						}
					}
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					fileName = "Config/LegionsWest.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KarenBattleQiZhiConfig_West item3 = new KarenBattleQiZhiConfig_West();
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.QiZhiID = (int)Global.GetSafeAttributeLong(node, "QiZhiID");
						item3.QiZuoID = (int)Global.GetSafeAttributeLong(node, "QiZuoID");
						string SiteString = Global.GetSafeAttributeStr(node, "QiZuoSite");
						string[] SiteFields = SiteString.Split(new char[]
						{
							'|'
						});
						if (2 == SiteFields.Length)
						{
							item3.PosX = Global.SafeConvertToInt32(SiteFields[0]);
							item3.PosY = Global.SafeConvertToInt32(SiteFields[1]);
						}
						SiteString = Global.GetSafeAttributeStr(node, "RebirthSite");
						SiteFields = SiteString.Split(new char[]
						{
							'|'
						});
						if (2 == SiteFields.Length)
						{
							item3.BirthX = Global.SafeConvertToInt32(SiteFields[0]);
							item3.BirthY = Global.SafeConvertToInt32(SiteFields[1]);
						}
						item3.BirthRadius = (int)Global.GetSafeAttributeLong(node, "RebirthRadius");
						item3.ProduceTime = (int)Global.GetSafeAttributeLong(node, "ProduceTime");
						item3.ProduceNum = (int)Global.GetSafeAttributeLong(node, "ProduceNum");
						this.RuntimeData.NPCID2QiZhiConfigDict[item3.QiZuoID] = item3;
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

		// Token: 0x06000D5D RID: 3421 RVA: 0x000D0198 File Offset: 0x000CE398
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x000D01A4 File Offset: 0x000CE3A4
		private void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				KarenBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					scene.m_nPlarerCount--;
				}
				JunTuanClient.getInstance().GameFuBenRoleChangeState(client.ServerId, client.ClientData.RoleID, scene.GameId, client.ClientData.BattleWhichSide, 7);
			}
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x000D0250 File Offset: 0x000CE450
		private void InitScene(KarenBattleScene scene, GameClient client)
		{
			foreach (KarenBattleQiZhiConfig_West item in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				scene.NPCID2QiZhiConfigDict.Add(item.QiZuoID, item.Clone() as KarenBattleQiZhiConfig_West);
			}
			scene.ScoreData.Clear();
			for (int sideloop = 1; sideloop <= scene.SceneInfo.MaxLegions; sideloop++)
			{
				JunTuanRankData RankData = KarenBattleManager.getInstance().GetJunTuanRankDataBySide(sideloop);
				KarenBattleScoreData scoreData = new KarenBattleScoreData();
				if (null != RankData)
				{
					scoreData.LegionID = RankData.JunTuanId;
					scoreData.Name = RankData.JunTuanName;
				}
				scene.ScoreData.Add(scoreData);
			}
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x000D0340 File Offset: 0x000CE540
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.KarenWest)
			{
				lock (this.RuntimeData.Mutex)
				{
					KarenBattleScene BattleScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out BattleScene);
					this.RuntimeData.FuBenItemData.Remove(BattleScene.GameId);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x000D03D0 File Offset: 0x000CE5D0
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.KarenWest)
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
						KarenBattleScene scene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
						{
							KarenFuBenData fuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
							{
								LogManager.WriteLog(LogTypes.Error, "阿卡伦战场没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							KarenBattleSceneInfo sceneInfo;
							if (null == (sceneInfo = KarenBattleManager.getInstance().TryGetKarenBattleSceneInfo(mapCode)))
							{
								LogManager.WriteLog(LogTypes.Error, "阿卡伦战场没有为副本找到对应的档位数据,ID:" + mapCode, null, true);
							}
							scene = new KarenBattleScene();
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
							DateTime startTime = now.Date.Add(KarenBattleManager.getInstance().GetStartTime(sceneInfo.MapCode));
							scene.StartTimeTicks = startTime.Ticks / 10000L;
							this.InitScene(scene, client);
							this.SceneDict[fuBenSeqId] = scene;
						}
						else
						{
							scene.m_nPlarerCount++;
						}
						KarenBattleClientContextData clientContextData;
						if (!scene.ClientContextDataDict.TryGetValue(roleId, out clientContextData))
						{
							clientContextData = new KarenBattleClientContextData
							{
								RoleId = roleId,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide
							};
							scene.ClientContextDataDict[roleId] = clientContextData;
						}
						client.SceneObject = scene;
						client.SceneGameId = (long)scene.GameId;
						client.SceneContextData2 = clientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(scene.SceneInfo.TotalSecs * 1000));
					}
					JunTuanClient.getInstance().GameFuBenRoleChangeState(client.ServerId, roleId, gameId, client.ClientData.BattleWhichSide, 5);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x000D06C0 File Offset: 0x000CE8C0
		public void BroadcastSceneScoreInfo(KarenBattleScene scene)
		{
			List<GameClient> objsList = scene.CopyMap.GetClientsList();
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i];
					if (c != null && c.ClientData.CopyMapID == scene.CopyMap.CopyMapID)
					{
						this.NotifyTimeStateInfoAndScoreInfo(c, false, true);
					}
				}
			}
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x000D0744 File Offset: 0x000CE944
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				KarenBattleScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<List<KarenBattleScoreData>>(1213, scene.ScoreData, false);
					}
				}
			}
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x000D07EC File Offset: 0x000CE9EC
		public void CompleteScene(KarenBattleScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x000D07F8 File Offset: 0x000CE9F8
		private void ProcessEnd(KarenBattleScene scene, int successSide, long nowTicks)
		{
			this.CompleteScene(scene, successSide);
			scene.m_eStatus = GameSceneStatuses.STATUS_END;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 19;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x000D0878 File Offset: 0x000CEA78
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= KarenBattleManager_MapWest.NextHeartBeatTicks)
			{
				KarenBattleManager_MapWest.NextHeartBeatTicks = nowTicks + 510L;
				foreach (KarenBattleScene scene in this.SceneDict.Values)
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
							}
							if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								if (ticks >= scene.StartTimeTicks)
								{
									scene.m_lPrepareTime = scene.StartTimeTicks;
									scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.PrepareSecs * 1000);
									scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
									scene.StateTimeData.GameType = 19;
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
									scene.StateTimeData.GameType = 19;
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
									long successTicks = long.MaxValue;
									int successScore = 0;
									int successSide = 0;
									for (int sideloop = 0; sideloop < scene.ScoreData.Count; sideloop++)
									{
										KarenBattleScoreData scoreData = scene.ScoreData[sideloop];
										if (scoreData.Score > successScore)
										{
											successSide = sideloop + 1;
											successTicks = scoreData.ticks;
											successScore = scoreData.Score;
										}
										else if (scoreData.Score == successScore && scoreData.ticks < successTicks)
										{
											successSide = sideloop + 1;
											successTicks = scoreData.ticks;
										}
									}
									this.ProcessEnd(scene, successSide, ticks);
								}
								else
								{
									this.CheckSceneScoreTime(scene, ticks);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
								scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								JunTuanClient.getInstance().GameFuBenRoleChangeState(-1, -1, scene.GameId, -1, 6);
								KarenBattleManager.getInstance().GiveAwards(scene);
								KarenFuBenData fuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
								{
									fuBenData.State = GameFuBenState.End;
									LogManager.WriteLog(LogTypes.Error, string.Format("阿卡伦西战场跨服GameID={0},战斗结束", fuBenData.GameId), null, true);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= scene.m_lLeaveTime)
								{
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
										DataHelper.WriteExceptionLogEx(ex, "阿卡伦西战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x000D0DAC File Offset: 0x000CEFAC
		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			if (client != null && this.IsQiZhiExtensionID(monster.MonsterInfo.ExtensionID))
			{
				KarenBattleScene scene = client.SceneObject as KarenBattleScene;
				KarenBattleQiZhiConfig_West qizhiConfig = monster.Tag as KarenBattleQiZhiConfig_West;
				if (scene != null && null != qizhiConfig)
				{
					lock (this.RuntimeData.Mutex)
					{
						scene.ScoreData[qizhiConfig.BattleWhichSide - 1].ResourceList.Remove(qizhiConfig.ID);
						qizhiConfig.DeadTicks = TimeUtil.NOW();
						qizhiConfig.Alive = false;
						qizhiConfig.BattleWhichSide = client.ClientData.BattleWhichSide;
						qizhiConfig.OwnTicks = 0L;
						qizhiConfig.OwnTicksDelta = 0L;
						this.BroadcastSceneScoreInfo(scene);
					}
				}
			}
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x000D0EA8 File Offset: 0x000CF0A8
		private void CheckSceneScoreTime(KarenBattleScene karenBattleScene, long nowTicks)
		{
			lock (this.RuntimeData.Mutex)
			{
				bool NotifyScoreData = false;
				foreach (KeyValuePair<int, KarenBattleQiZhiConfig_West> item in karenBattleScene.NPCID2QiZhiConfigDict)
				{
					KarenBattleQiZhiConfig_West qizhi = item.Value;
					if (qizhi.BattleWhichSide != 0 && qizhi.Alive)
					{
						qizhi.OwnTicksDelta += nowTicks - qizhi.OwnTicks;
						qizhi.OwnTicks = nowTicks;
						if (qizhi.OwnTicksDelta >= (long)(qizhi.ProduceTime * 1000) && qizhi.ProduceTime > 0)
						{
							int calRate = (int)(qizhi.OwnTicksDelta / (long)(qizhi.ProduceTime * 1000));
							qizhi.OwnTicksDelta -= (long)(calRate * qizhi.ProduceTime * 1000);
							karenBattleScene.ScoreData[qizhi.BattleWhichSide - 1].Score += calRate * qizhi.ProduceNum;
							karenBattleScene.ScoreData[qizhi.BattleWhichSide - 1].ticks = nowTicks;
							NotifyScoreData = true;
						}
					}
				}
				if (NotifyScoreData)
				{
					List<GameClient> objsList = karenBattleScene.CopyMap.GetClientsList();
					if (objsList != null && objsList.Count > 0)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient c = objsList[i];
							if (c != null && c.ClientData.CopyMapID == karenBattleScene.CopyMap.CopyMapID)
							{
								this.NotifyTimeStateInfoAndScoreInfo(c, false, true);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x000D10D8 File Offset: 0x000CF2D8
		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			KarenBattleQiZhiConfig_West item = null;
			bool isQiZuo = false;
			bool installJunQi = false;
			KarenBattleScene scene = client.SceneObject as KarenBattleScene;
			bool result;
			if (null == scene)
			{
				result = isQiZuo;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (scene.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out item))
					{
						isQiZuo = true;
						if (item.Alive)
						{
							return isQiZuo;
						}
						if (client.ClientData.BattleWhichSide == item.BattleWhichSide || Math.Abs(TimeUtil.NOW() - item.DeadTicks) >= 5000L)
						{
							if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (Math.Abs(client.ClientData.PosX - item.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - item.PosY) <= 1000)
								{
									installJunQi = true;
								}
							}
						}
					}
					if (installJunQi)
					{
						this.InstallJunQi(scene, client, item);
					}
				}
				result = isQiZuo;
			}
			return result;
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x000D123C File Offset: 0x000CF43C
		public bool IsQiZhiExtensionID(int QiZhiID)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (KarenBattleQiZhiConfig_West item in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
				{
					if (item.QiZhiID == QiZhiID)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000D6B RID: 3435 RVA: 0x000D12F4 File Offset: 0x000CF4F4
		public void InstallJunQi(KarenBattleScene scene, GameClient client, KarenBattleQiZhiConfig_West item)
		{
			CopyMap copyMap = scene.CopyMap;
			GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.m_nMapCode);
			if (copyMap != null && null != gameMap)
			{
				item.Alive = true;
				item.BattleWhichSide = client.ClientData.BattleWhichSide;
				item.OwnTicks = TimeUtil.NOW();
				scene.ScoreData[item.BattleWhichSide - 1].ResourceList.Add(item.ID);
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, item.QiZhiID, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, SceneUIClasses.KarenWest, item, null);
				SystemXmlItem systemNPC = null;
				if (GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(item.QiZuoID, out systemNPC))
				{
					string param = systemNPC.GetStringValue("SName");
					string param2 = client.ClientData.JunTuanName;
					KarenBattleManager.getInstance().NtfKarenNotifyMsg(scene, KarenNotifyMsgType.Own, client.ClientData.JunTuanId, param, param2);
				}
				this.BroadcastSceneScoreInfo(scene);
			}
		}

		// Token: 0x06000D6C RID: 3436 RVA: 0x000D1418 File Offset: 0x000CF618
		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			KarenFuBenData fuBenData;
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
				if (KarenBattleManager.getInstance().GMTest)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.FuBenItemData.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
						{
							fuBenData = new KarenFuBenData();
							fuBenData.GameId = (int)kuaFuServerLoginData.GameId;
							fuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
							this.RuntimeData.FuBenItemData[fuBenData.GameId] = fuBenData;
						}
					}
				}
				else
				{
					KarenFuBenData newFuBenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData((int)kuaFuServerLoginData.GameId);
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
			}
			if (KarenBattleManager.getInstance().GMTest)
			{
				client.ClientData.BattleWhichSide = Global.GetRandomNumber(1, 5);
			}
			else
			{
				KarenFuBenRoleData kroleData = JunTuanClient.getInstance().GetKarenFuBenRoleData((int)kuaFuServerLoginData.GameId, client.ClientData.RoleID);
				if (null == kroleData)
				{
					return false;
				}
				client.ClientData.BattleWhichSide = kroleData.Side;
			}
			int posX;
			int posY;
			int side = this.GetBirthPoint(client, out posX, out posY, true);
			bool result;
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
					KarenBattleSceneInfo sceneInfo;
					if (null == (sceneInfo = KarenBattleManager.getInstance().TryGetKarenBattleSceneInfo((int)kuaFuServerLoginData.GameId)))
					{
						return false;
					}
					client.ClientData.MapCode = sceneInfo.MapCode;
				}
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
				KarenBattleManager.getInstance().OnInitGame(SceneUIClasses.KarenWest, client);
				result = true;
			}
			return result;
		}

		// Token: 0x06000D6D RID: 3437 RVA: 0x000D17DC File Offset: 0x000CF9DC
		public bool ClientRelive(GameClient client)
		{
			int toPosX;
			int toPosY;
			int side = this.GetBirthPoint(client, out toPosX, out toPosY, false);
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

		// Token: 0x06000D6E RID: 3438 RVA: 0x000D1880 File Offset: 0x000CFA80
		public int GetBirthPoint(GameClient client, out int posX, out int posY, bool isLogin = false)
		{
			posX = 0;
			posY = 0;
			double distance = 0.0;
			int side = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				KarenBattleBirthPoint birthPoint = null;
				if (!this.RuntimeData.MapBirthPointDict.TryGetValue(side, out birthPoint))
				{
					return -1;
				}
				posX = birthPoint.PosX;
				posY = birthPoint.PosY;
				distance = Global.GetTwoPointDistance(new Point((double)posX, (double)posY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
			}
			int result;
			if (isLogin)
			{
				result = side;
			}
			else
			{
				KarenBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					foreach (KeyValuePair<int, KarenBattleQiZhiConfig_West> item in scene.NPCID2QiZhiConfigDict)
					{
						if (item.Value.BattleWhichSide == side)
						{
							double tempdis = Global.GetTwoPointDistance(new Point((double)item.Value.BirthX, (double)item.Value.BirthY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
							if (tempdis < distance)
							{
								distance = tempdis;
								Point BirthPoint = Global.GetMapPointByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, item.Value.BirthX / scene.MapGridWidth, item.Value.BirthY / scene.MapGridHeight, item.Value.BirthRadius / scene.MapGridWidth, 0, false);
								posX = (int)BirthPoint.X;
								posY = (int)BirthPoint.Y;
							}
						}
					}
				}
				result = side;
			}
			return result;
		}

		// Token: 0x040014C8 RID: 5320
		public const SceneUIClasses ManagerType = SceneUIClasses.KarenWest;

		// Token: 0x040014C9 RID: 5321
		public ConcurrentDictionary<int, KarenBattleScene> SceneDict = new ConcurrentDictionary<int, KarenBattleScene>();

		// Token: 0x040014CA RID: 5322
		private static long NextHeartBeatTicks = 0L;

		// Token: 0x040014CB RID: 5323
		private static KarenBattleManager_MapWest instance = new KarenBattleManager_MapWest();

		// Token: 0x040014CC RID: 5324
		public KarenBattleDataWest RuntimeData = new KarenBattleDataWest();
	}
}
