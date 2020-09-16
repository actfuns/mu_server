using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Tools;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class CopyWolfManager : IManager, IEventListener, IEventListenerEx
	{
		
		public static CopyWolfManager getInstance()
		{
			return CopyWolfManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool startup()
		{
			GlobalEventSource.getInstance().registerListener(11, CopyWolfManager.getInstance());
			GlobalEventSource.getInstance().registerListener(35, CopyWolfManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(33, 34, CopyWolfManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 34, CopyWolfManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(11, CopyWolfManager.getInstance());
			GlobalEventSource.getInstance().removeListener(35, CopyWolfManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(33, 34, CopyWolfManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 34, CopyWolfManager.getInstance());
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 11)
			{
				MonsterDeadEventObject obj = eventObject as MonsterDeadEventObject;
				Monster monster = obj.getMonster();
				GameClient client = obj.getAttacker();
				this.MonsterDead(client, monster);
			}
			if (eventObject.getEventType() == 35)
			{
				MonsterToMonsterDeadEventObject obj2 = eventObject as MonsterToMonsterDeadEventObject;
				Monster attack = obj2.getMonsterAttack();
				Monster monster = obj2.getMonster();
				CreateMonsterTagInfo tagInfo = monster.Tag as CreateMonsterTagInfo;
				if (monster != null && attack != null && tagInfo != null && monster.UniqueID != attack.UniqueID && tagInfo.ManagerType == 34)
				{
					this.FortDead(monster);
				}
				else if (monster != null && attack != null && monster.UniqueID == attack.UniqueID)
				{
					this.MonsterDead(monster);
				}
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			if (eventObject.EventType == 30)
			{
				OnCreateMonsterEventObject e = eventObject as OnCreateMonsterEventObject;
				if (null != e)
				{
					CreateMonsterTagInfo tagInfo = e.Monster.Tag as CreateMonsterTagInfo;
					if (null != tagInfo)
					{
						e.Monster.AllwaySearchEnemy = true;
						if (tagInfo.IsFort)
						{
							e.Monster.Camp = this._runtimeData.CampID;
						}
						e.Result = true;
						e.Handled = true;
					}
				}
			}
			if (eventObject.EventType == 33)
			{
				PreMonsterInjureEventObject obj = eventObject as PreMonsterInjureEventObject;
				if (obj != null && obj.SceneType == 34)
				{
					Monster attacker = obj.Attacker as Monster;
					Monster fortMonster = obj.Monster;
					if (attacker != null && fortMonster != null)
					{
						CreateMonsterTagInfo tagInfo = fortMonster.Tag as CreateMonsterTagInfo;
						if (tagInfo != null && tagInfo.IsFort)
						{
							int fubebSeqID = tagInfo.FuBenSeqId;
							if (fubebSeqID > 0)
							{
								CopyWolfSceneInfo scene = null;
								if (this._runtimeData.SceneDict.TryGetValue(fubebSeqID, out scene) && scene != null)
								{
									int injure = this._runtimeData.GetMonsterHurt(attacker.MonsterInfo.ExtensionID);
									int fortLife = (int)Math.Max(0.0, fortMonster.VLife - (double)injure);
									scene.ScoreData.FortLifeNow = fortLife;
									scene.ScoreData.FortLifeMax = (int)fortMonster.MonsterInfo.VLifeMax;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, scene.ScoreData, scene.CopyMapInfo);
									obj.Injure = injure;
									eventObject.Handled = true;
									eventObject.Result = true;
								}
							}
						}
					}
				}
			}
		}

		
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (CopyWolfManager._mutex)
			{
				try
				{
					this._runtimeData.CopyWolfWaveDic.Clear();
					fileName = Global.GameResPath("Config/LangHunYaoSai.xml");
					XElement xml = CheckHelper.LoadXml(fileName, true);
					if (null == xml)
					{
						return false;
					}
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							CopyWolfWaveInfo config = new CopyWolfWaveInfo();
							config.WaveID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							config.MonsterList.Clear();
							string[] monsterArr = Global.GetDefAttributeStr(xmlItem, "MonstersID", "0,0").Split(new char[]
							{
								'|'
							});
							foreach (string monster in monsterArr)
							{
								string[] i = monster.Split(new char[]
								{
									','
								});
								config.MonsterList.Add(new int[]
								{
									int.Parse(i[0]),
									int.Parse(i[1])
								});
							}
							config.NextTime = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NextTime", "60"));
							config.MonsterSiteDic.Clear();
							string[] siteArr = Global.GetDefAttributeStr(xmlItem, "Site", "0,0,0").Split(new char[]
							{
								'|'
							});
							foreach (string site in siteArr)
							{
								string[] s = site.Split(new char[]
								{
									','
								});
								CopyWolfSiteInfo siteInfo = new CopyWolfSiteInfo();
								siteInfo.X = int.Parse(s[0]);
								siteInfo.Y = int.Parse(s[1]);
								siteInfo.Radius = int.Parse(s[2]);
								config.MonsterSiteDic.Add(siteInfo);
							}
							this._runtimeData.CopyWolfWaveDic.Add(config.WaveID, config);
						}
					}
					string[] monsterHurtArr = GameManager.systemParamsList.GetParamValueByName("LangHunYaoSaiMonstersHurt").Split(new char[]
					{
						'|'
					});
					foreach (string monsterHurt in monsterHurtArr)
					{
						string[] h = monsterHurt.Split(new char[]
						{
							','
						});
						this._runtimeData.MonsterHurtDic.Add(int.Parse(h[0]), int.Parse(h[1]));
					}
					this._runtimeData.ScoreRateTime = (int)GameManager.systemParamsList.GetParamValueIntByName("LangHunYaoSaiTimeNum", -1);
					this._runtimeData.ScoreRateLife = (int)GameManager.systemParamsList.GetParamValueIntByName("LangHunYaoSaiLifeNum", -1);
					int[] forts = GameManager.systemParamsList.GetParamValueIntArrayByName("LangHunYaoSaiMonsters", ',');
					this._runtimeData.FortMonsterID = forts[0];
					this._runtimeData.FortSite.X = (double)forts[1];
					this._runtimeData.FortSite.Y = (double)forts[2];
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		
		public bool AddCopyScene(GameClient client, CopyMap copyMap)
		{
			bool result;
			if (copyMap.MapCode == this._runtimeData.MapID)
			{
				int fuBenSeqId = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (CopyWolfManager._mutex)
				{
					CopyWolfSceneInfo newScene = null;
					if (!this._runtimeData.SceneDict.TryGetValue(fuBenSeqId, out newScene))
					{
						newScene = new CopyWolfSceneInfo();
						newScene.CopyMapInfo = copyMap;
						newScene.CleanAllInfo();
						newScene.GameId = Global.GetClientKuaFuServerLoginData(client).GameId;
						newScene.MapID = mapCode;
						newScene.CopyID = copyMap.CopyMapID;
						newScene.FuBenSeqId = fuBenSeqId;
						newScene.PlayerCount = 1;
						this._runtimeData.SceneDict[fuBenSeqId] = newScene;
					}
					else
					{
						newScene.PlayerCount++;
					}
					client.ClientData.BattleWhichSide = this._runtimeData.CampID;
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this._runtimeData.TotalSecs * 1000));
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, newScene.ScoreData, newScene.CopyMapInfo);
				}
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 11);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool RemoveCopyScene(CopyMap copyMap)
		{
			bool result;
			if (copyMap.MapCode == this._runtimeData.MapID)
			{
				lock (CopyWolfManager._mutex)
				{
					CopyWolfSceneInfo scene;
					this._runtimeData.SceneDict.TryRemove(copyMap.FuBenSeqID, out scene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= CopyWolfManager._nextHeartBeatTicks)
			{
				CopyWolfManager._nextHeartBeatTicks = nowTicks + 1020L;
				long nowSecond = nowTicks / 1000L;
				foreach (CopyWolfSceneInfo scene in this._runtimeData.SceneDict.Values)
				{
					lock (CopyWolfManager._mutex)
					{
						int nID = scene.FuBenSeqId;
						int nCopyID = scene.CopyID;
						int nMapID = scene.MapID;
						if (nID >= 0 && nCopyID >= 0 && nMapID >= 0)
						{
							CopyMap copyMap = scene.CopyMapInfo;
							if (scene.SceneStatus == GameSceneStatuses.STATUS_NULL)
							{
								scene.PrepareTime = nowSecond;
								scene.BeginTime = nowSecond + (long)this._runtimeData.PrepareSecs;
								scene.SceneStatus = GameSceneStatuses.STATUS_PREPARE;
								scene.StateTimeData.GameType = 11;
								scene.StateTimeData.State = (int)scene.SceneStatus;
								scene.StateTimeData.EndTicks = nowTicks + (long)(this._runtimeData.PrepareSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMapInfo);
							}
							else if (scene.SceneStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (nowSecond >= scene.BeginTime)
								{
									scene.SceneStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.EndTime = nowSecond + (long)this._runtimeData.FightingSecs;
									scene.StateTimeData.GameType = 11;
									scene.StateTimeData.State = (int)scene.SceneStatus;
									scene.StateTimeData.EndTicks = nowTicks + (long)(this._runtimeData.FightingSecs * 1000);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMapInfo);
								}
							}
							else if (scene.SceneStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (nowSecond >= scene.EndTime)
								{
									scene.SceneStatus = GameSceneStatuses.STATUS_END;
								}
								else
								{
									if (scene.IsFortFlag <= 0)
									{
										this.CreateFort(scene);
									}
									bool bNeedCreateMonster = false;
									lock (scene)
									{
										CopyWolfWaveInfo configInfo = this._runtimeData.GetWaveConfig(scene.MonsterWave);
										if (configInfo == null)
										{
											scene.MonsterWaveOld = 0;
											scene.MonsterWave = 0;
											scene.SceneStatus = GameSceneStatuses.STATUS_END;
										}
										else
										{
											if (scene.CreateMonsterTime > 0L && nowSecond - scene.CreateMonsterTime >= (long)configInfo.NextTime && configInfo.NextTime > 0)
											{
												bNeedCreateMonster = true;
											}
											if (scene.CreateMonsterTime > 0L && scene.IsMonsterFlag == 0 && scene.KilledMonsterHashSet.Count == scene.MonsterCountCreate)
											{
												bNeedCreateMonster = true;
											}
											if (scene.CreateMonsterTime <= 0L)
											{
												bNeedCreateMonster = true;
												scene.MonsterWave = 0;
											}
											if (bNeedCreateMonster)
											{
												this.CreateMonster(scene, 1);
											}
										}
									}
								}
							}
							else if (scene.SceneStatus == GameSceneStatuses.STATUS_END)
							{
								int leftSecond = 0;
								if (scene.MonsterWave >= scene.MonsterWaveTotal)
								{
									leftSecond = (int)Math.Max(0L, nowSecond - scene.EndTime);
								}
								this.GiveAwards(scene, leftSecond);
								scene.SceneStatus = GameSceneStatuses.STATUS_AWARD;
								scene.EndTime = nowSecond;
								scene.LeaveTime = scene.EndTime + (long)this._runtimeData.ClearRolesSecs;
								scene.StateTimeData.GameType = 11;
								scene.StateTimeData.State = 3;
								scene.StateTimeData.EndTicks = nowTicks + (long)(this._runtimeData.ClearRolesSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMapInfo);
							}
							else if (scene.SceneStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (nowSecond >= scene.LeaveTime)
								{
									copyMap.SetRemoveTicks(scene.LeaveTime);
									scene.SceneStatus = GameSceneStatuses.STATUS_CLEAR;
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
										DataHelper.WriteExceptionLogEx(ex, "【狼魂要塞】清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void CreateMonster(CopyWolfSceneInfo scene, int upWave = 1)
		{
			CopyMap copyMap = scene.CopyMapInfo;
			CopyWolfWaveInfo waveConfig = null;
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapID, out gameMap))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("【狼魂要塞】报错 地图配置 ID = {0}", scene.MapID), null, true);
			}
			else
			{
				long nowTicket = TimeUtil.NOW();
				long nowSecond = nowTicket / 1000L;
				lock (scene)
				{
					if (scene.MonsterWave >= scene.MonsterWaveTotal)
					{
						scene.MonsterWaveOld = scene.MonsterWave;
						scene.MonsterWave = 0;
						scene.SceneStatus = GameSceneStatuses.STATUS_END;
					}
					else
					{
						scene.IsMonsterFlag = 1;
						int wave = scene.MonsterWave + upWave;
						if (wave > scene.MonsterWaveTotal)
						{
							wave = scene.MonsterWaveTotal;
						}
						waveConfig = this._runtimeData.GetWaveConfig(wave);
						if (waveConfig == null)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("【狼魂要塞】报错 刷怪波次 = {0}", wave), null, true);
						}
						else
						{
							scene.MonsterWave = wave;
							scene.CreateMonsterTime = nowSecond;
							int totalCount = 0;
							int gridNum = 0;
							CreateMonsterTagInfo tagInfo = new CreateMonsterTagInfo();
							tagInfo.FuBenSeqId = scene.FuBenSeqId;
							tagInfo.IsFort = false;
							tagInfo.ManagerType = 34;
							foreach (CopyWolfSiteInfo siteInfo in waveConfig.MonsterSiteDic)
							{
								int gridX = gameMap.CorrectWidthPointToGridPoint(siteInfo.X + Global.GetRandomNumber(-siteInfo.Radius, siteInfo.Radius)) / gameMap.MapGridWidth;
								int gridY = gameMap.CorrectHeightPointToGridPoint(siteInfo.Y + Global.GetRandomNumber(-siteInfo.Radius, siteInfo.Radius)) / gameMap.MapGridHeight;
								foreach (int[] monster in waveConfig.MonsterList)
								{
									int monsterID = monster[0];
									int monsterCount = monster[1];
									totalCount += monsterCount;
									GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapID, monsterID, scene.CopyMapInfo.CopyMapID, monsterCount, gridX, gridY, gridNum, 0, SceneUIClasses.CopyWolf, tagInfo, null);
								}
							}
							scene.MonsterCountCreate += totalCount;
							scene.ScoreData.Wave = waveConfig.WaveID;
							scene.ScoreData.EndTime = nowTicket + (long)(waveConfig.NextTime * 1000);
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, scene.ScoreData, scene.CopyMapInfo);
						}
					}
				}
			}
		}

		
		public void CreateFort(CopyWolfSceneInfo scene)
		{
			CopyMap copyMap = scene.CopyMapInfo;
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapID, out gameMap))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("【狼魂要塞】报错 地图配置 ID = {0}", scene.MapID), null, true);
			}
			else
			{
				lock (scene)
				{
					if (scene.IsFortFlag <= 0)
					{
						scene.IsFortFlag = 1;
						int gridX = gameMap.CorrectWidthPointToGridPoint((int)this._runtimeData.FortSite.X) / gameMap.MapGridWidth;
						int gridY = gameMap.CorrectHeightPointToGridPoint((int)this._runtimeData.FortSite.Y) / gameMap.MapGridHeight;
						CreateMonsterTagInfo tagInfo = new CreateMonsterTagInfo();
						tagInfo.FuBenSeqId = scene.FuBenSeqId;
						tagInfo.IsFort = true;
						tagInfo.ManagerType = 34;
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapID, this._runtimeData.FortMonsterID, scene.CopyMapInfo.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.CopyWolf, tagInfo, null);
						XElement xml = GameManager.MonsterZoneMgr.AllMonstersXml;
						if (xml != null)
						{
							XElement monsterXml = Global.GetSafeXElement(xml, "Monster", "ID", this._runtimeData.FortMonsterID.ToString());
							if (monsterXml != null)
							{
								int life = (int)Global.GetSafeAttributeLong(monsterXml, "MaxLife");
								scene.ScoreData.FortLifeNow = life;
								scene.ScoreData.FortLifeMax = life;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, scene.ScoreData, scene.CopyMapInfo);
							}
						}
					}
				}
			}
		}

		
		public bool IsCopyWolf(int fubenID)
		{
			return fubenID == this._runtimeData.CopyID;
		}

		
		public void MonsterDead(Monster monster)
		{
			CreateMonsterTagInfo tagInfo = monster.Tag as CreateMonsterTagInfo;
			if (tagInfo != null)
			{
				int fubebSeqID = tagInfo.FuBenSeqId;
				if (fubebSeqID >= 0 && monster.CopyMapID >= 0 && this.IsCopyWolf(monster.CurrentMapCode))
				{
					CopyWolfSceneInfo scene = null;
					if (this._runtimeData.SceneDict.TryGetValue(fubebSeqID, out scene) && scene != null)
					{
						if (scene.SceneStatus < GameSceneStatuses.STATUS_END)
						{
							if (scene.AddKilledMonster(monster))
							{
								if (scene.SceneStatus < GameSceneStatuses.STATUS_END)
								{
									lock (scene)
									{
										if (scene.IsMonsterFlag == 1 && scene.KilledMonsterHashSet.Count == scene.MonsterCountCreate)
										{
											scene.MonsterWaveOld = scene.MonsterWave;
											if (scene.MonsterWave >= scene.MonsterWaveTotal)
											{
												scene.SceneStatus = GameSceneStatuses.STATUS_END;
											}
											else
											{
												scene.IsMonsterFlag = 0;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void MonsterDead(GameClient client, Monster monster)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsCopyWolf(client.ClientData.FuBenID))
			{
				CopyWolfSceneInfo scene = null;
				if (this._runtimeData.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene) && scene != null)
				{
					if (scene.AddKilledMonster(monster))
					{
						if (scene.SceneStatus < GameSceneStatuses.STATUS_END)
						{
							lock (scene)
							{
								int score = scene.AddMonsterScore(client.ClientData.RoleID, monster.MonsterInfo.WolfScore);
								scene.ScoreData.RoleMonsterScore = scene.RoleMonsterScore;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, scene.ScoreData, scene.CopyMapInfo);
								if (scene.IsMonsterFlag == 1 && scene.KilledMonsterHashSet.Count == scene.MonsterCountCreate)
								{
									scene.MonsterWaveOld = scene.MonsterWave;
									if (scene.MonsterWave >= scene.MonsterWaveTotal)
									{
										scene.SceneStatus = GameSceneStatuses.STATUS_END;
									}
									else
									{
										scene.IsMonsterFlag = 0;
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void FortDead(Monster fortMonster)
		{
			CreateMonsterTagInfo tagInfo = fortMonster.Tag as CreateMonsterTagInfo;
			if (tagInfo != null)
			{
				int fubebSeqID = tagInfo.FuBenSeqId;
				if (fubebSeqID >= 0 && fortMonster.CopyMapID >= 0 && this.IsCopyWolf(fortMonster.CurrentMapCode))
				{
					CopyWolfSceneInfo scene = null;
					if (this._runtimeData.SceneDict.TryGetValue(fubebSeqID, out scene) && scene != null)
					{
						if (scene.SceneStatus < GameSceneStatuses.STATUS_END)
						{
							lock (scene)
							{
								scene.SceneStatus = GameSceneStatuses.STATUS_END;
								scene.ScoreData.FortLifeNow = 0;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, scene.ScoreData, scene.CopyMapInfo);
							}
						}
					}
				}
			}
		}

		
		public void GiveAwards(CopyWolfSceneInfo scene, int leftSecond)
		{
			try
			{
				FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(scene.CopyMapInfo.FubenMapID, scene.MapID);
				if (fuBenMapItem != null)
				{
					int zhanLi = 0;
					List<GameClient> objsList = scene.CopyMapInfo.GetClientsList();
					if (objsList != null && objsList.Count > 0)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient client = objsList[i];
							if (client != null && client == GameManager.ClientMgr.FindClient(client.ClientData.RoleID))
							{
								int wave = scene.MonsterWaveOld;
								if (wave > scene.MonsterWaveTotal)
								{
									wave = scene.MonsterWaveTotal;
								}
								int scoreMonster = scene.GetMonsterScore(client.ClientData.RoleID);
								int life = scene.ScoreData.FortLifeNow;
								int scoreAll = this.GetScore(scoreMonster, leftSecond, life);
								long nExp = (long)this.AwardExp(fuBenMapItem.Experience, scoreAll);
								int money = this.AwardGoldBind(fuBenMapItem.Money1, scoreAll);
								int wolfMoney = this.AwardWolfMoney(fuBenMapItem.WolfMoney, scoreAll);
								if (nExp > 0L)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, nExp, false, true, false, "none");
								}
								if (money > 0)
								{
									GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money, string.Format("副本{0}通关奖励", scene.CopyID), false);
								}
								if (wolfMoney > 0)
								{
									GameManager.ClientMgr.ModifyLangHunFenMoValue(client, wolfMoney, "狼魂要塞", true, true);
								}
								CopyWolfAwardsData awardsData = new CopyWolfAwardsData
								{
									Wave = scene.MonsterWaveOld,
									Exp = nExp,
									Money = money,
									WolfMoney = wolfMoney,
									RoleScore = scene.GetMonsterScore(client.ClientData.RoleID)
								};
								GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 1, 0, 11);
								client.sendCmd<CopyWolfAwardsData>(1026, awardsData, false);
								zhanLi += client.ClientData.CombatForce;
								Global.UpdateFuBenDataForQuickPassTimer(client, scene.CopyMapInfo.FubenMapID, 0, 1);
							}
						}
					}
					if (objsList != null && objsList.Count > 0)
					{
						int roleCount = objsList.Count;
						zhanLi /= roleCount;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "【狼魂要塞】清场调度异常");
			}
		}

		
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool scoreInfo = true)
		{
			lock (CopyWolfManager._mutex)
			{
				CopyWolfSceneInfo scene;
				if (this._runtimeData.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (scoreInfo)
					{
						client.sendCmd<CopyWolfScoreData>(1025, scene.ScoreData, false);
					}
				}
			}
		}

		
		public void LeaveFuBen(GameClient client)
		{
			CopyWolfSceneInfo scene = null;
			lock (this._runtimeData.SceneDict)
			{
				if (!this._runtimeData.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					return;
				}
			}
			lock (scene)
			{
				scene.PlayerCount--;
				if (scene.SceneStatus != GameSceneStatuses.STATUS_END && scene.SceneStatus != GameSceneStatuses.STATUS_AWARD && scene.SceneStatus != GameSceneStatuses.STATUS_CLEAR)
				{
					KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
				}
			}
		}

		
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		
		public bool ClientRelive(GameClient client)
		{
			GameMap gameMap = null;
			bool result;
			if (GameManager.MapMgr.DictMaps.TryGetValue(this._runtimeData.MapID, out gameMap))
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosY;
				int defaultBirthRadius = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].BirthRadius;
				Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this._runtimeData.MapID, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
				client.ClientData.CurrentLifeV = client.ClientData.LifeV;
				client.ClientData.CurrentMagicV = client.ClientData.MagicV;
				client.ClientData.MoveAndActionNum = 0;
				GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, (int)newPos.X, (int)newPos.Y, -1);
				Global.ClientRealive(client, (int)newPos.X, (int)newPos.Y, -1);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public int GetScore(int monsterScore, int second, int life)
		{
			int timeScore = this._runtimeData.ScoreRateTime * second;
			int lifeScore = this._runtimeData.ScoreRateLife * life;
			return Math.Max(0, monsterScore + timeScore + lifeScore);
		}

		
		public int AwardExp(int baseValue, int score)
		{
			return (int)((double)baseValue * (1.0 + Math.Pow((double)Math.Min(score, 1000000), 0.34) / 100.0));
		}

		
		public int AwardGoldBind(int baseValue, int score)
		{
			return (int)((double)baseValue * (1.0 + Math.Pow((double)Math.Min(score, 500000), 0.34) / 100.0));
		}

		
		public int AwardWolfMoney(int baseValue, int score)
		{
			return 200 + Math.Min(score, 100000) / 100;
		}

		
		public const GameTypes _gameType = GameTypes.CopyWolf;

		
		private static long _nextHeartBeatTicks = 0L;

		
		public static object _mutex = new object();

		
		public CopyWolfInfo _runtimeData = new CopyWolfInfo();

		
		private static CopyWolfManager instance = new CopyWolfManager();
	}
}
