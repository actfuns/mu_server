using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class WanMoXiaGuManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2, ICopySceneManager
	{
		
		public static WanMoXiaGuManager getInstance()
		{
			return WanMoXiaGuManager.instance;
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
			GlobalEventSource4Scene.getInstance().registerListener(10001, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10000, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10004, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10005, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, WanMoXiaGuManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10000, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10004, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10005, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, WanMoXiaGuManager.getInstance());
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
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject deadEv = eventObject as MonsterDeadEventObject;
				Monster monster = deadEv.getMonster();
				GameClient client = deadEv.getAttacker();
				if (client != null && null != monster)
				{
					this.OnInjureMonster(client, monster, 0L);
				}
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 10000:
			{
				KuaFuFuBenRoleCountEvent e = eventObject as KuaFuFuBenRoleCountEvent;
				if (null != e)
				{
					GameClient client = GameManager.ClientMgr.FindClient(e.RoleId);
					if (null != client)
					{
						client.sendCmd<int>(1264, e.RoleCount, false);
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10001:
			{
				KuaFuNotifyEnterGameEvent e2 = eventObject as KuaFuNotifyEnterGameEvent;
				if (null != e2)
				{
					KuaFuServerLoginData kuaFuServerLoginData = e2.Arg as KuaFuServerLoginData;
					if (null != kuaFuServerLoginData)
					{
						GameClient client = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
						if (null != client)
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
								client.sendCmd(1263, string.Format("{0}:{1}", kuaFuServerLoginData.GameId, e2.TeamCombatAvg), false);
							}
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10004:
			{
				KuaFuNotifyCopyCancelEvent e3 = eventObject as KuaFuNotifyCopyCancelEvent;
				GameClient client = GameManager.ClientMgr.FindClient(e3.RoleId);
				if (client != null)
				{
					client.ClientData.SignUpGameType = 0;
					client.sendCmd(1265, string.Format("{0}:{1}", e3.GameId, e3.Reason), false);
				}
				eventObject.Handled = true;
				break;
			}
			case 10005:
			{
				KuaFuNotifyRealEnterGameEvent e4 = eventObject as KuaFuNotifyRealEnterGameEvent;
				if (e4 != null)
				{
					GameClient client = GameManager.ClientMgr.FindClient(e4.RoleId);
					if (client != null)
					{
						client.ClientData.SignUpGameType = 0;
						GlobalNew.RecordSwitchKuaFuServerLog(client);
						client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
					}
				}
				eventObject.Handled = true;
				break;
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
					Dictionary<int, WanMoXiaGuMonsterConfigInfo> dict = new Dictionary<int, WanMoXiaGuMonsterConfigInfo>();
					fileName = Global.GameResPath("Config/WanMoXiaGu.xml");
					XElement xml = CheckHelper.LoadXml(fileName, true);
					if (null == xml)
					{
						return false;
					}
					foreach (XElement xmlItem in xml.Elements())
					{
						if (xmlItem != null)
						{
							WanMoXiaGuMonsterConfigInfo config = new WanMoXiaGuMonsterConfigInfo();
							config.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							config.MonsterID = ConfigHelper.GetElementAttributeValueIntArray(xmlItem, "MonstersID", null);
							config.MonstersNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MonstersNum", "0"));
							config.BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0"));
							config.EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"));
							this.RuntimeData.BeginNum = Math.Min(this.RuntimeData.BeginNum, config.BeginNum);
							this.RuntimeData.EndNum = Math.Max(this.RuntimeData.EndNum, config.EndNum);
							string[] pts = Global.GetDefAttributeStr(xmlItem, "Site", "0,0").Split(new char[]
							{
								'|'
							});
							config.Site = new List<int>();
							foreach (string pt in pts)
							{
								config.Site.AddRange(ConfigHelper.String2IntList(pt, ','));
							}
							config.Props = Global.GetDefAttributeStr(xmlItem, "Props", "");
							config.Intro = Global.GetDefAttributeStr(xmlItem, "Intro", "");
							config.RecoverTime = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "RecoverTime", "0"));
							config.RecoverNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "RecoverNum", "0"));
							config.Decorations = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Decorations", "0"));
							dict.Add(config.ID, config);
						}
					}
					this.RuntimeData.MonsterOrderConfigList = dict;
					this.RuntimeData.AwardList = ConfigHelper.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("WanMoXiaGuAward"), true, '|', ',');
					double[] arr = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WanMoXiaGuCall", ',');
					this.RuntimeData.BossMonsterID = (int)arr[0];
					this.RuntimeData.WanMoXiaGuCall = arr[1];
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		
		public bool ProcessJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				if (sceneType != SceneUIClasses.Normal)
				{
					client.sendCmd(nID, "-21".ToString(), false);
					return true;
				}
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd(nID, "-2001".ToString(), false);
					return true;
				}
				if (client.ClientData.SignUpGameType != 0)
				{
					client.sendCmd(nID, "-2002".ToString(), false);
					return true;
				}
				if (KuaFuManager.getInstance().IsInCannotJoinKuaFuCopyTime(client))
				{
					client.sendCmd(nID, "-2004".ToString(), false);
					return true;
				}
				SystemXmlItem systemFuBenItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(this.RuntimeData.CopyID, out systemFuBenItem))
				{
					client.sendCmd(nID, "-3".ToString(), false);
					return true;
				}
				int minLevel = systemFuBenItem.GetIntValue("MinLevel", -1);
				int minZhuanSheng = systemFuBenItem.GetIntValue("MinZhuanSheng", -1);
				int levelLimit = minZhuanSheng * 100 + minLevel;
				if (client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level < levelLimit)
				{
					client.sendCmd(nID, "-19".ToString(), false);
					return true;
				}
				int oldCount = this.GetWanMoXiaGuCount(client);
				if (oldCount >= systemFuBenItem.GetIntValue("FinishNumber", -1))
				{
					client.sendCmd(nID, "-16".ToString(), false);
					return true;
				}
				int result = 0;
				if (result > 0)
				{
					client.ClientData.SignUpGameType = 8;
					GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 8);
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

		
		public bool ProcessQuitCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd(nID, 0.ToString(), false);
					return true;
				}
				client.ClientData.SignUpGameType = 0;
				int result = 0;
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd(nID, 0.ToString(), false);
					return true;
				}
				client.ClientData.SignUpGameType = 0;
				int flag = Global.SafeConvertToInt32(cmdParams[1]);
				if (flag > 0)
				{
					int result = 0;
					if (result < 0)
					{
						flag = 0;
					}
				}
				else
				{
					KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
				}
				if (flag <= 0)
				{
					Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
					client.sendCmd(1262, 0.ToString(), false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool OnInitGame(GameClient client)
		{
			GameMap gameMap = null;
			bool result;
			if (GameManager.MapMgr.DictMaps.TryGetValue(this.RuntimeData.MapID, out gameMap))
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].DefaultBirthPosY;
				int defaultBirthRadius = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].BirthRadius;
				Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this.RuntimeData.MapID, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
				client.ClientData.MapCode = this.RuntimeData.MapID;
				client.ClientData.PosX = (int)newPos.X;
				client.ClientData.PosY = (int)newPos.Y;
				client.ClientData.FuBenSeqID = Global.GetClientKuaFuServerLoginData(client).FuBenSeqId;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool ClientRelive(GameClient client)
		{
			GameMap gameMap = null;
			bool result;
			if (GameManager.MapMgr.DictMaps.TryGetValue(this.RuntimeData.MapID, out gameMap))
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].DefaultBirthPosY;
				int defaultBirthRadius = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].BirthRadius;
				Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this.RuntimeData.MapID, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
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

		
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.WanMoXiaGu, hint);
		}

		
		public int GetWanMoXiaGuCount(GameClient client)
		{
			int count = 0;
			foreach (int id in this.RuntimeData.FuBenIds)
			{
				FuBenData fuBenData = Global.GetFuBenData(client, id);
				int c;
				Global.GetFuBenEnterNum(fuBenData, out c);
				count += c;
			}
			return count;
		}

		
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (copyMap.MapCode == this.RuntimeData.MapID)
			{
				int fuBenSeqId = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.RuntimeData.Mutex)
				{
					WanMoXiaGuScene scene = null;
					if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
					{
						scene = new WanMoXiaGuScene();
						scene.CopyMapInfo = copyMap;
						scene.CleanAllInfo();
						scene.GameId = Global.GetClientKuaFuServerLoginData(client).GameId;
						scene.MapID = mapCode;
						scene.CopyMapID = copyMap.CopyMapID;
						scene.FuBenSeqId = fuBenSeqId;
						scene.PlayerCount = 1;
						scene.BossLifePercent = 1.0;
						this.SceneDict[fuBenSeqId] = scene;
					}
					else
					{
						scene.PlayerCount++;
					}
					client.ClientData.BattleWhichSide = 1;
					client.SceneObject = scene;
					copyMap.IsKuaFuCopy = true;
					copyMap.CustomPassAwards = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, scene.ScoreData, scene.CopyMapInfo);
				}
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
			return true;
		}

		
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= WanMoXiaGuManager._nextHeartBeatTicks)
			{
				WanMoXiaGuManager._nextHeartBeatTicks = nowTicks + 1020L;
				long nowSecond = nowTicks / 1000L;
				List<WanMoXiaGuScene> removeList = new List<WanMoXiaGuScene>();
				lock (this.RuntimeData.Mutex)
				{
					foreach (WanMoXiaGuScene scene in this.SceneDict.Values)
					{
						int nID = scene.FuBenSeqId;
						int nCopyID = scene.CopyMapID;
						int nMapID = scene.MapID;
						if (nID >= 0 && nCopyID >= 0 && nMapID >= 0)
						{
							CopyMap copyMap = scene.CopyMapInfo;
							if (scene.SceneStatus == GameSceneStatuses.STATUS_NULL)
							{
								scene.PrepareTime = nowSecond;
								scene.BeginTime = nowSecond + (long)this.RuntimeData.PrepareSecs;
								scene.SceneStatus = GameSceneStatuses.STATUS_PREPARE;
								scene.StateTimeData.GameType = 8;
								scene.StateTimeData.State = (int)scene.SceneStatus;
								scene.StateTimeData.EndTicks = nowTicks + (long)(this.RuntimeData.PrepareSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMapInfo);
							}
							else if (scene.SceneStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (nowSecond >= scene.BeginTime)
								{
									scene.SceneStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.EndTime = nowSecond + (long)this.RuntimeData.FightingSecs;
									scene.StateTimeData.GameType = 8;
									scene.StateTimeData.State = (int)scene.SceneStatus;
									scene.StateTimeData.EndTicks = nowTicks + (long)(this.RuntimeData.FightingSecs * 1000);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMapInfo);
								}
							}
							else if (scene.SceneStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (nowSecond >= scene.EndTime)
								{
									scene.SceneStatus = GameSceneStatuses.STATUS_END;
								}
								else if (null == scene.Boss)
								{
									scene.Boss = (GameManager.MonsterMgr.FindMonsterByExtensionID(copyMap.CopyMapID, this.RuntimeData.BossMonsterID).FirstOrDefault<object>() as Monster);
									if (null != scene.Boss)
									{
										scene.ScoreData.MonsterID = scene.Boss.RoleID;
										scene.ScoreData.BossLifePercent = 1.0;
									}
								}
								else if (scene.Boss != null && scene.Boss.Alive)
								{
									if (!scene.MonsterCreated && scene.BossLifePercent < this.RuntimeData.WanMoXiaGuCall)
									{
										int rnd = Global.GetRandomNumber(this.RuntimeData.BeginNum, this.RuntimeData.EndNum);
										foreach (WanMoXiaGuMonsterConfigInfo item in this.RuntimeData.MonsterOrderConfigList.Values)
										{
											if (rnd >= item.BeginNum && rnd <= item.EndNum)
											{
												scene.ScoreData.Intro = item.Intro;
												scene.ScoreData.Decorations = item.Decorations;
												scene.ZuoQiInfo = item;
												this.CreateMonster(scene, scene.ZuoQiInfo);
												scene.MonsterCreated = true;
												scene.NextRelifeTicks = nowTicks + (long)(scene.ZuoQiInfo.RecoverTime * 1000);
											}
										}
									}
									if (scene.ZuoQiInfo != null)
									{
										if (scene.MonsterCount != scene.ScoreData.MonsterCount)
										{
											scene.MonsterCount = scene.ScoreData.MonsterCount;
											foreach (string pstr in scene.ZuoQiInfo.Props.Split(new char[]
											{
												'|'
											}))
											{
												string[] strs = pstr.Split(new char[]
												{
													','
												});
												ExtPropIndexes propIdx;
												double propValue;
												if (strs.Length == 2 && Enum.TryParse<ExtPropIndexes>(strs[0], out propIdx) && double.TryParse(strs[1], out propValue))
												{
													scene.Boss.TempPropsBuffer.AddTempExtProp((int)propIdx, propValue * (double)scene.MonsterCount, long.MaxValue);
												}
											}
										}
										if (nowTicks >= scene.NextRelifeTicks)
										{
											scene.NextRelifeTicks = nowTicks + (long)(scene.ZuoQiInfo.RecoverTime * 1000);
											if (scene.MonsterCount > 0)
											{
												Monster monster = scene.Boss;
												monster.AddLife((long)(scene.ZuoQiInfo.RecoverNum * scene.MonsterCount));
												List<object> listObjs = Global.GetAll9Clients(monster);
												GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.SafeDirection, monster.VLife, monster.VMana, 120, listObjs, 0);
											}
											scene.BossLifePercent = (scene.ScoreData.BossLifePercent = scene.Boss.VLife / scene.Boss.MonsterInfo.VLifeMax);
											GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, scene.ScoreData, scene.CopyMapInfo);
										}
									}
								}
								else
								{
									scene.Success = 1;
									scene.SceneStatus = GameSceneStatuses.STATUS_END;
								}
							}
							else if (scene.SceneStatus == GameSceneStatuses.STATUS_END)
							{
								scene.SceneStatus = GameSceneStatuses.STATUS_AWARD;
								scene.EndTime = nowSecond;
								scene.LeaveTime = scene.EndTime + (long)this.RuntimeData.ClearRolesSecs;
								if (scene.Success > 0)
								{
									this.GiveAwards(scene);
								}
								scene.StateTimeData.GameType = 8;
								scene.StateTimeData.State = 3;
								scene.StateTimeData.EndTicks = nowTicks + (long)(this.RuntimeData.ClearRolesSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMapInfo);
							}
							else if (scene.SceneStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (nowSecond >= scene.LeaveTime)
								{
									removeList.Add(scene);
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
										DataHelper.WriteExceptionLogEx(ex, "【万魔峡谷】清场调度异常");
									}
								}
							}
						}
					}
				}
				if (removeList.Count > 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						foreach (WanMoXiaGuScene scene in removeList)
						{
							WanMoXiaGuScene item2;
							this.SceneDict.TryRemove(scene.FuBenSeqId, out item2);
						}
					}
				}
			}
		}

		
		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (monster.VLife <= 0.0)
			{
			}
			lock (this.RuntimeData.Mutex)
			{
				WanMoXiaGuScene scene = client.SceneObject as WanMoXiaGuScene;
				if (scene != null && scene.SceneStatus == GameSceneStatuses.STATUS_BEGIN)
				{
					if (monster.GetMonsterData().ExtensionID == this.RuntimeData.BossMonsterID)
					{
						scene.BossLifePercent = monster.VLife / monster.MonsterInfo.VLifeMax;
						if (scene.BossLifePercent <= 0.0)
						{
							scene.Success = 1;
							scene.SceneStatus = GameSceneStatuses.STATUS_END;
						}
						if (scene.BossLifePercent <= 0.0 || Math.Round(scene.BossLifePercent, 2) != Math.Round(scene.ScoreData.BossLifePercent, 2))
						{
							scene.ScoreData.BossLifePercent = scene.BossLifePercent;
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, scene.ScoreData, scene.CopyMapInfo);
						}
						else
						{
							scene.ScoreData.BossLifePercent = scene.BossLifePercent;
						}
					}
					else if (monster.VLife <= 0.0 && scene.AddKilledMonster(monster))
					{
						scene.ScoreData.MonsterCount--;
						scene.ScoreData.MonsterCount = ((scene.ScoreData.MonsterCount < 0) ? 0 : scene.ScoreData.MonsterCount);
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, scene.ScoreData, scene.CopyMapInfo);
					}
				}
			}
		}

		
		public void CreateMonster(WanMoXiaGuScene scene, object tag)
		{
			CopyMap copyMap = scene.CopyMapInfo;
			GameMap gameMap = null;
			if (GameManager.MapMgr.DictMaps.TryGetValue(scene.MapID, out gameMap))
			{
				WanMoXiaGuMonsterConfigInfo waveConfig = tag as WanMoXiaGuMonsterConfigInfo;
				if (waveConfig != null)
				{
					for (int i = 0; i < waveConfig.MonstersNum; i++)
					{
						int monsterID = waveConfig.MonsterID[i];
						int gridX = waveConfig.Site[i * 2] / gameMap.MapGridWidth;
						int gridY = waveConfig.Site[i * 2 + 1] / gameMap.MapGridHeight;
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapID, monsterID, scene.CopyMapInfo.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.WanMoXiaGu, waveConfig, null);
					}
					scene.ScoreData.MonsterCount = waveConfig.MonstersNum;
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, scene.ScoreData, scene.CopyMapInfo);
				}
			}
		}

		
		public void GiveAwards(WanMoXiaGuScene scene)
		{
			try
			{
				FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(scene.CopyMapInfo.FubenMapID, scene.MapID);
				if (fuBenMapItem != null)
				{
					int usedSecs = (int)(scene.EndTime - scene.BeginTime);
					int zhanLi = 0;
					List<GameClient> objsList = scene.CopyMapInfo.GetClientsList().Distinct<GameClient>().ToList<GameClient>();
					if (objsList != null && objsList.Count > 0)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient client = objsList[i];
							if (client != null && client == GameManager.ClientMgr.FindClient(client.ClientData.RoleID))
							{
								zhanLi += client.ClientData.CombatForce;
								long nExp = (long)fuBenMapItem.Experience;
								int money = fuBenMapItem.Money1;
								int idx = this.RuntimeData.AwardList.Count - 1;
								int mul = this.RuntimeData.AwardList[idx][1];
								for (int j = 0; j <= idx; j++)
								{
									if (usedSecs <= this.RuntimeData.AwardList[j][0])
									{
										mul = this.RuntimeData.AwardList[j][1];
										break;
									}
								}
								if (nExp > 0L)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, nExp, false, true, false, "万魔峡谷通关奖励");
								}
								if (money > 0)
								{
									GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money, "万魔峡谷通关奖励", true);
								}
								List<GoodsData> goodsList = new List<GoodsData>();
								if (null != fuBenMapItem.GoodsDataList)
								{
									for (int k = 0; k < fuBenMapItem.GoodsDataList.Count; k++)
									{
										GoodsData goodsData = new GoodsData(fuBenMapItem.GoodsDataList[k]);
										goodsData.GCount *= mul;
										goodsList.Add(goodsData);
									}
								}
								if (goodsList.Count > 0)
								{
									if (Global.CanAddGoodsDataList(client, goodsList))
									{
										foreach (GoodsData goodsData in goodsList)
										{
											GoodsUtil.AddGoodsDBCommand(client, goodsData, true, 1, "万魔峡谷通关奖励", true);
										}
									}
									else
									{
										Global.UseMailGivePlayerAward2(client, goodsList, GLang.GetLang(4000, new object[0]), GLang.GetLang(4001, new object[0]), 0, 0, 0);
									}
								}
								WanMoXiaGuAwardsData awardsData = new WanMoXiaGuAwardsData
								{
									Success = scene.Success,
									UsedSecs = usedSecs,
									Exp = nExp,
									Money = money,
									AwardsGoods = goodsList
								};
								client.sendCmd<WanMoXiaGuAwardsData>(1267, awardsData, false);
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
				DataHelper.WriteExceptionLogEx(ex, "【万魔峡谷】清场调度异常");
			}
		}

		
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool scoreInfo = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				WanMoXiaGuScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (scoreInfo)
					{
						client.sendCmd<WanMoXiaGuScoreData>(1266, scene.ScoreData, false);
					}
				}
			}
		}

		
		public void LeaveFuBen(GameClient client)
		{
			WanMoXiaGuScene scene = null;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					return;
				}
			}
			lock (this.RuntimeData.Mutex)
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

		
		public const SceneUIClasses _sceneType = SceneUIClasses.WanMoXiaGu;

		
		public const GameTypes GameType = GameTypes.KuaFuCopy;

		
		public ConcurrentDictionary<int, WanMoXiaGuScene> SceneDict = new ConcurrentDictionary<int, WanMoXiaGuScene>();

		
		private static long _nextHeartBeatTicks = 0L;

		
		public WanMoXiaGuData RuntimeData = new WanMoXiaGuData();

		
		private static WanMoXiaGuManager instance = new WanMoXiaGuManager();
	}
}
