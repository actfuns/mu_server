using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
	
	public class KarenBattleManager_MapEast : IManager, IEventListener, IEventListenerEx, IManager2
	{
		
		public static KarenBattleManager_MapEast getInstance()
		{
			return KarenBattleManager_MapEast.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		
		public bool startup()
		{
			GlobalEventSource4Scene.getInstance().registerListener(10002, 42, KarenBattleManager_MapEast.getInstance());
			GlobalEventSource.getInstance().registerListener(10, KarenBattleManager_MapEast.getInstance());
			GlobalEventSource.getInstance().registerListener(31, KarenBattleManager_MapEast.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10002, 42, KarenBattleManager_MapEast.getInstance());
			GlobalEventSource.getInstance().removeListener(10, KarenBattleManager_MapEast.getInstance());
			GlobalEventSource.getInstance().removeListener(31, KarenBattleManager_MapEast.getInstance());
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 31)
			{
				ClientRegionEventObject e = eventObject as ClientRegionEventObject;
				if (null != e)
				{
					if (e.EventType == 1 && e.Flag == 1)
					{
						this.SubmitCrystalBuff(e.Client, e.AreaLuaID);
					}
				}
			}
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEvent = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEvent)
				{
					GameClient clientDead = playerDeadEvent.getPlayer();
					if (null != clientDead)
					{
						KarenBattleScene scene;
						if (this.SceneDict.TryGetValue(clientDead.ClientData.FuBenSeqID, out scene))
						{
							this.RemoveBattleSceneBuffForRole(scene, clientDead);
						}
					}
				}
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10002)
			{
				CaiJiEventObject e = eventObject as CaiJiEventObject;
				if (null != e)
				{
					GameClient client = e.Source as GameClient;
					Monster monster = e.Target as Monster;
					this.OnCaiJiFinish(client, monster);
					eventObject.Handled = true;
					eventObject.Result = true;
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
					fileName = "Config/LegionsEastBirthPoint.xml";
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
					this.RuntimeData.KarenCenterConfigDict.Clear();
					fileName = "Config/LegionsEast.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KarenCenterConfig item2 = new KarenCenterConfig();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.NPCID = (int)Global.GetSafeAttributeLong(node, "NPCID");
						string SiteString = Global.GetSafeAttributeStr(node, "NPCSite");
						string[] SiteFields = SiteString.Split(new char[]
						{
							'|'
						});
						if (3 == SiteFields.Length)
						{
							item2.PosX = Global.SafeConvertToInt32(SiteFields[0]);
							item2.PosY = Global.SafeConvertToInt32(SiteFields[1]);
							item2.Radius = Global.SafeConvertToInt32(SiteFields[2]);
						}
						item2.ProduceTime = (int)Global.GetSafeAttributeLong(node, "ProduceTime");
						item2.ProduceNum = (int)Global.GetSafeAttributeLong(node, "ProduceNum");
						item2.OccupyTime = (int)Global.GetSafeAttributeLong(node, "OccupyTime");
						this.RuntimeData.KarenCenterConfigDict[item2.ID] = item2;
					}
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					fileName = "Config/LegionsEastFlag.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KarenBattleQiZhiConfig_East item3 = new KarenBattleQiZhiConfig_East();
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonsterID");
						string SiteString = Global.GetSafeAttributeStr(node, "Site");
						string[] SiteFields = SiteString.Split(new char[]
						{
							'|'
						});
						if (2 == SiteFields.Length)
						{
							item3.PosX = Global.SafeConvertToInt32(SiteFields[0]);
							item3.PosY = Global.SafeConvertToInt32(SiteFields[1]);
						}
						item3.BeginTime = (int)Global.GetSafeAttributeLong(node, "BeginTime");
						item3.RefreshCD = (int)Global.GetSafeAttributeLong(node, "RefreshCD");
						item3.CollectTime = (int)Global.GetSafeAttributeLong(node, "CollectTime");
						item3.HandInNum = (int)Global.GetSafeAttributeLong(node, "HandInNum");
						item3.HoldTme = (int)Global.GetSafeAttributeLong(node, "HoldTme");
						item3.BuffGoodsID = (int)Global.GetSafeAttributeLong(node, "Icon");
						this.RuntimeData.NPCID2QiZhiConfigDict[item3.MonsterID] = item3;
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

		
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		
		private void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				KarenBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					scene.m_nPlarerCount--;
				}
				this.RemoveBattleSceneBuffForRole(scene, client);
				JunTuanClient.getInstance().GameFuBenRoleChangeState(client.ServerId, client.ClientData.RoleID, scene.GameId, client.ClientData.BattleWhichSide, 7);
			}
		}

		
		private void InitScene(KarenBattleScene scene, GameClient client)
		{
			foreach (KarenCenterConfig item in this.RuntimeData.KarenCenterConfigDict.Values)
			{
				scene.KarenCenterConfigDict.Add(item.ID, item.Clone() as KarenCenterConfig);
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

		
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.KarenEast)
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

		
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.KarenEast)
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

		
		public void CompleteScene(KarenBattleScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
		}

		
		private void ProcessEnd(KarenBattleScene scene, int successSide, long nowTicks)
		{
			this.CompleteScene(scene, successSide);
			scene.m_eStatus = GameSceneStatuses.STATUS_END;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 20;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= KarenBattleManager_MapEast.NextHeartBeatTicks)
			{
				KarenBattleManager_MapEast.NextHeartBeatTicks = nowTicks + 510L;
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
								this.CheckCreateDynamicMonster(scene, ticks);
							}
							if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								if (ticks >= scene.StartTimeTicks)
								{
									scene.m_lPrepareTime = scene.StartTimeTicks;
									scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.PrepareSecs * 1000);
									scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
									scene.StateTimeData.GameType = 20;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									this.InitCreateDynamicMonster(scene);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (ticks >= scene.m_lBeginTime)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.m_lEndTime = scene.m_lBeginTime + (long)(scene.SceneInfo.FightingSecs * 1000);
									scene.StateTimeData.GameType = 20;
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
									this.CheckSceneGameClients(scene, ticks);
									this.CheckSceneBufferTime(scene, ticks);
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
									LogManager.WriteLog(LogTypes.Error, string.Format("阿卡伦东战场跨服GameID={0},战斗结束", fuBenData.GameId), null, true);
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
										DataHelper.WriteExceptionLogEx(ex, "阿卡伦东战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void AddDelayCreateMonster(KarenBattleScene scene, long ticks, object monster)
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

		
		public void CheckCreateDynamicMonster(KarenBattleScene scene, long nowMs)
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
							if (obj is KarenBattleQiZhiConfig_East)
							{
								KarenBattleQiZhiConfig_East item = obj as KarenBattleQiZhiConfig_East;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item.MonsterID, scene.CopyMapId, 1, item.PosX / scene.MapGridWidth, item.PosY / scene.MapGridHeight, 0, 0, SceneUIClasses.KarenEast, item, null);
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

		
		private bool CheckSceneCenterState(KarenBattleScene karenBattleScene, KarenCenterConfig center, long nowTicks)
		{
			List<object> enemiesObjList = new List<object>();
			GameManager.ClientMgr.LookupEnemiesInCircle(karenBattleScene.m_nMapCode, karenBattleScene.CopyMapId, center.PosX, center.PosY, center.Radius, enemiesObjList);
			Dictionary<int, GameClient> OwnerSideDict = new Dictionary<int, GameClient>();
			int newBattleSide = 0;
			foreach (object item in enemiesObjList)
			{
				GameClient client = item as GameClient;
				if (client.ClientData.CurrentLifeV > 0)
				{
					OwnerSideDict[client.ClientData.BattleWhichSide] = client;
				}
			}
			if (OwnerSideDict.Count == 1)
			{
				newBattleSide = OwnerSideDict.Keys.FirstOrDefault<int>();
			}
			bool result;
			if (newBattleSide == 0 || newBattleSide == center.BattleWhichSide)
			{
				center.OwnCalculateSide = 0L;
				center.OwnCalculateTicks = 0L;
				result = false;
			}
			else if (center.OwnCalculateSide != (long)newBattleSide)
			{
				center.OwnCalculateSide = (long)newBattleSide;
				center.OwnCalculateTicks = nowTicks;
				result = false;
			}
			else if (nowTicks - center.OwnCalculateTicks >= (long)(center.OccupyTime * 1000))
			{
				if (center.BattleWhichSide != 0)
				{
					karenBattleScene.ScoreData[center.BattleWhichSide - 1].ResourceList.Remove(center.ID);
				}
				center.OwnTicksDelta = 0L;
				center.OwnTicks = nowTicks;
				center.BattleWhichSide = newBattleSide;
				karenBattleScene.ScoreData[center.BattleWhichSide - 1].ResourceList.Add(center.ID);
				GameClient client = OwnerSideDict.Values.FirstOrDefault<GameClient>();
				SystemXmlItem systemNPC = null;
				if (GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(center.NPCID, out systemNPC))
				{
					string param = systemNPC.GetStringValue("SName");
					string param2 = client.ClientData.JunTuanName;
					KarenBattleManager.getInstance().NtfKarenNotifyMsg(karenBattleScene, KarenNotifyMsgType.Own, client.ClientData.JunTuanId, param, param2);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		private void CheckSceneGameClients(KarenBattleScene karenBattleScene, long nowTicks)
		{
			List<GameClient> objsList = karenBattleScene.CopyMap.GetClientsList();
			if (objsList != null && objsList.Count != 0)
			{
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(karenBattleScene.m_nMapCode, out gameMap))
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient c = objsList[i];
						if (c != null)
						{
							if (gameMap.InSafeRegionList(c.CurrentGrid))
							{
								if (c.SceneContextData != null)
								{
									this.RemoveBattleSceneBuffForRole(karenBattleScene, c);
								}
							}
						}
					}
				}
			}
		}

		
		private void CheckSceneScoreTime(KarenBattleScene karenBattleScene, long nowTicks)
		{
			lock (this.RuntimeData.Mutex)
			{
				bool NotifyScoreData = false;
				foreach (KeyValuePair<int, KarenCenterConfig> item in karenBattleScene.KarenCenterConfigDict)
				{
					KarenCenterConfig center = item.Value;
					NotifyScoreData |= this.CheckSceneCenterState(karenBattleScene, center, nowTicks);
					if (center.BattleWhichSide != 0)
					{
						center.OwnTicksDelta += nowTicks - center.OwnTicks;
						center.OwnTicks = nowTicks;
						if (center.OwnTicksDelta >= (long)(center.ProduceTime * 1000) && center.ProduceTime > 0)
						{
							int calRate = (int)(center.OwnTicksDelta / (long)(center.ProduceTime * 1000));
							center.OwnTicksDelta -= (long)(calRate * center.ProduceTime * 1000);
							karenBattleScene.ScoreData[center.BattleWhichSide - 1].Score += calRate * center.ProduceNum;
							karenBattleScene.ScoreData[center.BattleWhichSide - 1].ticks = nowTicks;
							NotifyScoreData = true;
						}
					}
				}
				if (NotifyScoreData)
				{
					this.BroadcastSceneScoreInfo(karenBattleScene);
				}
			}
		}

		
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
			int side = this.GetBirthPoint(client, out posX, out posY);
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
				KarenBattleManager.getInstance().OnInitGame(SceneUIClasses.KarenEast, client);
				result = true;
			}
			return result;
		}

		
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

		
		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			posX = 0;
			posY = 0;
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
				double distance = Global.GetTwoPointDistance(new Point((double)posX, (double)posY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
			}
			return side;
		}

		
		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			KarenBattleQiZhiConfig_East tag = (monster != null) ? (monster.Tag as KarenBattleQiZhiConfig_East) : null;
			int result;
			if (tag == null)
			{
				result = -200;
			}
			else
			{
				result = tag.CollectTime;
			}
			return result;
		}

		
		public void RemoveBattleSceneBuffForRole(KarenBattleScene scene, GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (scene.SceneBuffDict.Count != 0)
				{
					List<KarenBattleSceneBuff> sceneBuffDeleteList = new List<KarenBattleSceneBuff>();
					foreach (KarenBattleSceneBuff contextData in scene.SceneBuffDict.Values)
					{
						if (contextData.RoleID == client.ClientData.RoleID)
						{
							sceneBuffDeleteList.Add(contextData);
						}
					}
					if (sceneBuffDeleteList.Count != 0)
					{
						foreach (KarenBattleSceneBuff contextData in sceneBuffDeleteList)
						{
							if (contextData.RoleID != 0)
							{
								this.UpdateBuff4GameClient(client, contextData.BuffID, contextData.tagInfo, false);
							}
							KarenBattleQiZhiConfig_East CrystalItem = contextData.tagInfo as KarenBattleQiZhiConfig_East;
							if (null != CrystalItem)
							{
								this.AddDelayCreateMonster(scene, TimeUtil.NOW() + (long)(CrystalItem.RefreshCD * 1000), contextData.tagInfo);
							}
						}
					}
				}
			}
		}

		
		private string BuildSceneBuffKey(GameClient client, int bufferGoodsID)
		{
			return string.Format("{0}_{1}", client.ClientData.RoleID, bufferGoodsID);
		}

		
		private void UpdateBuff4GameClient(GameClient client, int bufferGoodsID, object tagInfo, bool add)
		{
			try
			{
				KarenBattleQiZhiConfig_East CrystalItem = tagInfo as KarenBattleQiZhiConfig_East;
				if (null != CrystalItem)
				{
					int BuffTime = 0;
					BufferItemTypes buffItemType = BufferItemTypes.None;
					if (null != CrystalItem)
					{
						BuffTime = CrystalItem.HoldTme;
						buffItemType = BufferItemTypes.KarenEastCrystal;
					}
					KarenBattleScene scene;
					if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
					{
						EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(bufferGoodsID);
						if (null != item)
						{
							if (add)
							{
								double[] actionParams = new double[]
								{
									(double)BuffTime,
									(double)bufferGoodsID
								};
								Global.UpdateBufferData(client, buffItemType, actionParams, 1, true);
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BufferByGoodsProps,
									bufferGoodsID,
									item.ExtProps
								});
								string Key = this.BuildSceneBuffKey(client, bufferGoodsID);
								scene.SceneBuffDict[Key] = new KarenBattleSceneBuff
								{
									RoleID = client.ClientData.RoleID,
									BuffID = bufferGoodsID,
									EndTicks = TimeUtil.NOW() + (long)(BuffTime * 1000),
									tagInfo = tagInfo
								};
								if (buffItemType == BufferItemTypes.KarenEastCrystal)
								{
									client.SceneContextData = tagInfo;
								}
							}
							else
							{
								double[] array = new double[2];
								double[] actionParams = array;
								Global.UpdateBufferData(client, buffItemType, actionParams, 1, true);
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BufferByGoodsProps,
									bufferGoodsID,
									PropsCacheManager.ConstExtProps
								});
								Global.RemoveBufferData(client, (int)buffItemType);
								string Key = this.BuildSceneBuffKey(client, bufferGoodsID);
								scene.SceneBuffDict.Remove(Key);
								if (buffItemType == BufferItemTypes.KarenEastCrystal)
								{
									client.SceneContextData = null;
								}
							}
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			lock (this.RuntimeData.Mutex)
			{
				KarenBattleScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						KarenBattleQiZhiConfig_East monsterItem = monster.Tag as KarenBattleQiZhiConfig_East;
						if (monsterItem != null)
						{
							KarenBattleQiZhiConfig_East crystalItem = client.SceneContextData as KarenBattleQiZhiConfig_East;
							if (null != crystalItem)
							{
								this.AddDelayCreateMonster(scene, TimeUtil.NOW() + (long)(crystalItem.RefreshCD * 1000), crystalItem);
							}
							this.UpdateBuff4GameClient(client, monsterItem.BuffGoodsID, monsterItem, true);
						}
					}
				}
			}
		}

		
		public void SubmitCrystalBuff(GameClient client, int areaLuaID)
		{
			KarenBattleQiZhiConfig_East crystalItem = client.SceneContextData as KarenBattleQiZhiConfig_East;
			if (null != crystalItem)
			{
				lock (this.RuntimeData.Mutex)
				{
					KarenBattleScene scene;
					if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
					{
						KarenCenterConfig center = null;
						if (scene.KarenCenterConfigDict.TryGetValue(areaLuaID, out center))
						{
							if (center.BattleWhichSide == client.ClientData.BattleWhichSide)
							{
								KarenBattleClientContextData contextData = client.SceneContextData2 as KarenBattleClientContextData;
								if (contextData != null && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
								{
									int addScore = crystalItem.HandInNum;
									scene.ScoreData[client.ClientData.BattleWhichSide - 1].Score += addScore;
									scene.ScoreData[client.ClientData.BattleWhichSide - 1].ticks = TimeUtil.NOW();
									if (addScore > 0)
									{
										this.NotifyTimeStateInfoAndScoreInfo(client, false, true);
									}
								}
								SystemXmlItem systemNPC = null;
								if (GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(center.NPCID, out systemNPC))
								{
									string param = client.ClientData.JunTuanName;
									string param2 = systemNPC.GetStringValue("SName");
									KarenBattleManager.getInstance().NtfKarenNotifyMsg(scene, KarenNotifyMsgType.Submit, client.ClientData.JunTuanId, param, param2);
								}
								this.UpdateBuff4GameClient(client, crystalItem.BuffGoodsID, crystalItem, false);
								this.AddDelayCreateMonster(scene, TimeUtil.NOW() + (long)(crystalItem.RefreshCD * 1000), crystalItem);
							}
						}
					}
				}
			}
		}

		
		private void InitCreateDynamicMonster(KarenBattleScene scene)
		{
			foreach (KarenBattleQiZhiConfig_East junqi in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				this.AddDelayCreateMonster(scene, scene.m_lPrepareTime + (long)(junqi.BeginTime * 1000), junqi);
			}
		}

		
		private void CheckSceneBufferTime(KarenBattleScene karenBattleScene, long nowTicks)
		{
			List<KarenBattleSceneBuff> sceneBuffDeleteList = new List<KarenBattleSceneBuff>();
			lock (this.RuntimeData.Mutex)
			{
				if (karenBattleScene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
				{
					if (karenBattleScene.SceneBuffDict.Count != 0)
					{
						foreach (KarenBattleSceneBuff contextData in karenBattleScene.SceneBuffDict.Values)
						{
							if (contextData.EndTicks < nowTicks)
							{
								sceneBuffDeleteList.Add(contextData);
							}
						}
						if (sceneBuffDeleteList.Count != 0)
						{
							foreach (KarenBattleSceneBuff contextData in sceneBuffDeleteList)
							{
								if (contextData.RoleID != 0)
								{
									GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleID);
									if (null != client)
									{
										this.UpdateBuff4GameClient(client, contextData.BuffID, contextData.tagInfo, false);
									}
								}
								KarenBattleQiZhiConfig_East CrystalItem = contextData.tagInfo as KarenBattleQiZhiConfig_East;
								if (null != CrystalItem)
								{
									this.AddDelayCreateMonster(karenBattleScene, TimeUtil.NOW() + (long)(CrystalItem.RefreshCD * 1000), contextData.tagInfo);
								}
							}
						}
					}
				}
			}
		}

		
		public const SceneUIClasses ManagerType = SceneUIClasses.KarenEast;

		
		public ConcurrentDictionary<int, KarenBattleScene> SceneDict = new ConcurrentDictionary<int, KarenBattleScene>();

		
		private static long NextHeartBeatTicks = 0L;

		
		private static KarenBattleManager_MapEast instance = new KarenBattleManager_MapEast();

		
		public KarenBattleDataEast RuntimeData = new KarenBattleDataEast();
	}
}
