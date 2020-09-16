using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class ZhengDuoManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx, IEventListener, ICopySceneManager
	{
		
		public static ZhengDuoManager getInstance()
		{
			return ZhengDuoManager.instance;
		}

		
		public bool initialize()
		{
			bool result;
			if (!this.ConfigData.Load(Global.GameResPath("Config\\PlunderLands.xml"), Global.GameResPath("Config\\PlunderLandsMonster.xml"), Global.GameResPath("Config\\PlunderLandsRebirth.xml")))
			{
				result = false;
			}
			else
			{
				this.ConfigData.AwardHurtMin = (int)GameManager.systemParamsList.GetParamValueIntByName("PlunderLandsLowest", -1);
				this.ConfigData.ZhengDuoMapList = StringUtil.StringToIntList(GameManager.systemParamsList.GetParamValueByName("PlunderLandsEnterMap"), ',');
				result = true;
			}
			return result;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1070, 1, 1, ZhengDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1071, 1, 1, ZhengDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1073, 1, 1, ZhengDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1074, 1, 1, ZhengDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(10, ZhengDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, ZhengDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(17, ZhengDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(23, 10000, ZhengDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, ZhengDuoManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, ZhengDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, ZhengDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(17, ZhengDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(23, 10000, ZhengDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, ZhengDuoManager.getInstance());
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
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1070:
					return this.HandleZhengDuoData(client, nID, bytes, cmdParams);
				case 1071:
					return this.HandleZhengDuoSign(client, nID, bytes, cmdParams);
				case 1073:
					return this.HandleZhengDuoEnter(client, nID, bytes, cmdParams);
				case 1074:
					return this.HandleZhengDuoRankList(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		
		private bool HandleZhengDuoData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			ZhengDuoData zhengDuoData = new ZhengDuoData();
			int bhid = client.ClientData.Faction;
			long nowTicks = TimeUtil.NOW();
			if (bhid > 0)
			{
				lock (this.RuntimeData.Mutex)
				{
					zhengDuoData.Step = this.RuntimeData.ZhengDuoStep;
					zhengDuoData.State = this.RuntimeData.State;
					client.ClientData.ZhengDuoDataAge = this.RuntimeData.Age;
					ZhengDuoRankData self;
					if (this.RuntimeData.ZhengDuoStep == 1 && this.RuntimeData.State == 1)
					{
						ZhengDuoSignUpData signUpData;
						if (this.RuntimeData.FightDataDict.TryGetValue(client.ClientData.Faction, out signUpData) && signUpData.Week == this.RuntimeData.WeekDay)
						{
							zhengDuoData.SignUp = 1;
							if (nowTicks >= signUpData.StartTicks + (long)signUpData.UsedTicks)
							{
								zhengDuoData.State = 0;
							}
						}
					}
					else if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(bhid, out self))
					{
						zhengDuoData.SignUp = 1;
						zhengDuoData.Lose = self.Lose;
						if (this.RuntimeData.ZhengDuoStep >= 1 && (this.RuntimeData.ZhengDuoStep < 5 || this.RuntimeData.State > 0))
						{
							zhengDuoData.State = (this.RuntimeData.State & self.State);
							if (self.Enemy > 0)
							{
								ZhengDuoRankData enemy;
								if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(self.Enemy, out enemy))
								{
									zhengDuoData.OtherName = enemy.BhName;
									zhengDuoData.OtherZoneId = enemy.ZoneId;
								}
							}
						}
						else if (this.RuntimeData.Rank1Bhid > 0)
						{
							ZhengDuoRankData enemy;
							if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(this.RuntimeData.Rank1Bhid, out enemy))
							{
								zhengDuoData.OtherName = enemy.BhName;
								zhengDuoData.OtherZoneId = enemy.ZoneId;
							}
						}
					}
					else
					{
						int[] args = Global.sendToDB<int[], int>(10224, bhid, client.ServerId);
						if (args != null && args.Length >= 2 && args[0] == this.RuntimeData.WeekDay)
						{
							zhengDuoData.SignUp = 1;
							zhengDuoData.Lose = 1;
						}
						if (this.RuntimeData.Rank1Bhid > 0)
						{
							ZhengDuoRankData enemy;
							if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(this.RuntimeData.Rank1Bhid, out enemy))
							{
								zhengDuoData.OtherName = enemy.BhName;
								zhengDuoData.OtherZoneId = enemy.ZoneId;
							}
						}
					}
				}
			}
			client.sendCmd<ZhengDuoData>(nID, zhengDuoData, false);
			return true;
		}

		
		private bool HandleZhengDuoSign(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = -4007;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.ZhengDuo, false))
			{
				result = -4007;
			}
			else
			{
				int bhid = client.ClientData.Faction;
				if (bhid <= 0 || client.ClientData.BHZhiWu != 1)
				{
					result = -1002;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.ZhengDuoStep == 1)
						{
							int weekDay = TimeUtil.GetWeekStartDayIdNow();
							ZhengDuoSignUpData data;
							if (this.RuntimeData.FightDataDict.TryGetValue(bhid, out data) && data.Week == weekDay)
							{
								result = -4005;
							}
							else
							{
								data = new ZhengDuoSignUpData();
								data.Bhid = bhid;
								data.StartTicks = TimeUtil.NOW();
								data.UsedTicks = 1800000;
								data.Week = weekDay;
								this.RuntimeData.FightDataDict[bhid] = data;
								result = 1;
								GameManager.ClientMgr.SendBangHuiCmd<int>(data.Bhid, 1072, this.RuntimeData.ZhengDuoStep, true, true);
								EventLogManager.AddGameEvent(LogRecordType.ZhengDuoZhiDi, new object[]
								{
									0,
									weekDay,
									GameManager.ServerId,
									data.Bhid,
									"争夺之地报名"
								});
							}
						}
					}
				}
			}
			client.sendCmd<int>(nID, result, false);
			return true;
		}

		
		private bool HandleZhengDuoEnter(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 1;
			int bhid = client.ClientData.Faction;
			lock (this.RuntimeData.Mutex)
			{
				if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.ZhengDuo, false))
				{
					result = -4007;
				}
				else if (this.RuntimeData.ZhengDuoStep == 0 || this.RuntimeData.State == 0)
				{
					result = -4007;
				}
				else
				{
					SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
					if (sceneType != SceneUIClasses.Normal)
					{
						result = -21;
					}
					else
					{
						long nowTicks = TimeUtil.NOW();
						int weekDay = TimeUtil.GetWeekStartDayIdNow();
						if (this.RuntimeData.ZhengDuoStep == 1)
						{
							ZhengDuoSignUpData data;
							ZhengDuoSceneInfo sceneInfo;
							if (!this.RuntimeData.FightDataDict.TryGetValue(bhid, out data))
							{
								result = -4008;
							}
							else if (data.Week == weekDay && nowTicks >= data.StartTicks + (long)data.UsedTicks)
							{
								result = -4006;
							}
							else if (this.ConfigData.SceneDataDict.TryGetValue(1, out sceneInfo))
							{
								result = 1;
								if (data.FuBenSeqId == 0)
								{
									data.FuBenSeqId = FuBenManager.GetFuBenSeqId(0);
								}
								client.ClientData.BattleWhichSide = 1;
								int x;
								int y;
								this.GetBirthPoint(client, out x, out y);
								client.SceneInfoObject = sceneInfo;
								client.ClientData.FuBenSeqID = data.FuBenSeqId;
								FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, data.FuBenSeqId, 0, 0);
								client.ClientData.WaitingChangeMapToMapCode = sceneInfo.MapCode;
								GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, sceneInfo.MapCode, x, y, -1, 0);
							}
							else
							{
								result = -3;
							}
						}
						else
						{
							ZhengDuoFuBenData fuBenData = this.GetKuaFuFuBenDataByBhid(bhid, weekDay);
							if (fuBenData != null && fuBenData.State == GameFuBenState.Start)
							{
								KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
								if (null != clientKuaFuServerLoginData)
								{
									clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
									clientKuaFuServerLoginData.GameId = fuBenData.GameId;
									clientKuaFuServerLoginData.GameType = 17;
									clientKuaFuServerLoginData.EndTicks = fuBenData.EndTime.Ticks;
									clientKuaFuServerLoginData.ServerId = GameManager.ServerId;
									KuaFuServerInfo serverInfo;
									if (!KuaFuManager.getInstance().TryGetValue(fuBenData.ServerId, out serverInfo))
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("进入争夺之地时找不到跨服服务器的IP和端口信息#serverid={0}", fuBenData.ServerId), null, true);
										result = -11000;
									}
									else
									{
										clientKuaFuServerLoginData.ServerIp = serverInfo.Ip;
										clientKuaFuServerLoginData.ServerPort = serverInfo.Port;
										GlobalNew.RecordSwitchKuaFuServerLog(client);
										client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
									}
								}
							}
							else
							{
								result = -4006;
							}
						}
					}
				}
			}
			client.sendCmd<int>(nID, result, false);
			return true;
		}

		
		private bool HandleZhengDuoRankList(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			ZhengDuoRankList rankList = new ZhengDuoRankList();
			int bhid = client.ClientData.Faction;
			if (bhid > 0)
			{
				int[] args = Global.sendToDB<int[], int>(10224, bhid, client.ServerId);
				lock (this.RuntimeData.Mutex)
				{
					if (args != null && args.Length >= 2 && args[0] == this.RuntimeData.WeekDay)
					{
						rankList.UsedMillisecond = args[1];
					}
					rankList.RankList = new List<ZhengDuoRankData>();
					foreach (ZhengDuoRankData item in this.RuntimeData.ZhengDuoRankDatas)
					{
						if (null != item)
						{
							rankList.RankList.Add(item);
						}
					}
				}
			}
			client.sendCmd<ZhengDuoRankList>(nID, rankList, false);
			return true;
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 23:
			{
				PreBangHuiAddMemberEventObject e = eventObject as PreBangHuiAddMemberEventObject;
				if (null != e)
				{
					eventObject.Handled = this.OnPreBangHuiAddMember(e);
				}
				break;
			}
			case 24:
			{
				PreBangHuiRemoveMemberEventObject e2 = eventObject as PreBangHuiRemoveMemberEventObject;
				if (null != e2)
				{
					eventObject.Handled = this.OnPreBangHuiRemoveMember(e2);
				}
				break;
			}
			}
		}

		
		public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
		{
			bool result;
			if (this.IsActivityState(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(549, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
		{
			bool result;
			if (this.IsActivityState(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(550, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType != 11)
			{
				if (eventType == 17)
				{
					this.HandleMonsterInjured(((MonsterInjuredEventObject)eventObject).getAttacker(), ((MonsterInjuredEventObject)eventObject).getMonster(), ((MonsterInjuredEventObject)eventObject).injure);
				}
			}
			else
			{
				this.HandleMonsterDead(((MonsterDeadEventObject)eventObject).getAttacker(), ((MonsterDeadEventObject)eventObject).getMonster());
			}
		}

		
		private void HandleMonsterDead(GameClient player, Monster monster)
		{
			ZhengDuoScene scene = player.SceneObject as ZhengDuoScene;
			if (scene != null)
			{
				if (monster.Tag is ZhengDuoMonsterInfo)
				{
					scene.KillerId = player.ClientData.RoleID;
					scene.KillUsedTicks = (int)(TimeUtil.NOW() - scene.StartTimeTicks);
					scene.m_lEndTime = TimeUtil.NOW();
				}
			}
		}

		
		private void HandleMonsterInjured(GameClient client, Monster monster, int injure)
		{
			ZhengDuoScene scene = client.SceneObject as ZhengDuoScene;
			if (scene != null)
			{
				if (monster.Tag is ZhengDuoMonsterInfo)
				{
					lock (scene.ClientContextDataDict)
					{
						ZhengDuoScoreData scoreData;
						if (!scene.ClientContextDataDict.TryGetValue(client.ClientData.RoleID, out scoreData))
						{
							scoreData = new ZhengDuoScoreData(client.ClientData.RoleID, client.ClientData.RoleName, 0L);
						}
						scoreData.Score += (long)injure;
						if (scene.PreliminarisesMode)
						{
							bool find = false;
							for (int i = 0; i < scene.ScoreData.Length; i++)
							{
								ZhengDuoScoreData scoreData2 = scene.ScoreData[i];
								if (scoreData2 != null && scoreData2.Id == client.ClientData.RoleID)
								{
									scoreData2.Score = scoreData.Score;
									find = true;
								}
							}
							if (!find)
							{
								if (ZhengDuoScoreData.Compare_static(scene.ScoreData[2], scoreData) > 0)
								{
									scene.ScoreData[3] = new ZhengDuoScoreData(scoreData.Id, client.ClientData.RoleName, scoreData.Score);
									find = true;
								}
							}
							if (find)
							{
								Array.Sort<ZhengDuoScoreData>(scene.ScoreData, new Comparison<ZhengDuoScoreData>(ZhengDuoScoreData.Compare_static));
							}
						}
						else if (scene.ScoreData[0].Id == client.ClientData.Faction)
						{
							scene.ScoreData[0].Score += (long)injure;
						}
						else
						{
							scene.ScoreData[1].Score += (long)injure;
						}
					}
				}
			}
		}

		
		private bool IsActivityState(int bhid)
		{
			bool state = false;
			if (bhid > 0)
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.State == 0)
					{
						return false;
					}
					ZhengDuoSignUpData data;
					if (this.RuntimeData.FightDataDict.TryGetValue(bhid, out data) && data.Week == this.RuntimeData.WeekDay && TimeUtil.NOW() < data.StartTicks + (long)data.UsedTicks)
					{
						state = true;
					}
					ZhengDuoRankData rankData;
					if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(bhid, out rankData))
					{
						if (rankData.Week == this.RuntimeData.WeekDay && rankData.State > 0)
						{
							state = true;
						}
					}
				}
			}
			return state;
		}

		
		public void CheckTipsIconState(GameClient client)
		{
			bool state = this.IsActivityState(client.ClientData.Faction);
			if (client._IconStateMgr.AddFlushIconState(15004, state))
			{
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.ZhengDuo)
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
					DateTime now = TimeUtil.NowDateTime();
					int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					ZhengDuoRankData rankData = null;
					ZhengDuoScoreData scoreData = null;
					lock (this.RuntimeData.Mutex)
					{
						ZhengDuoScene scene = null;
						ZhengDuoFuBenData fuBenData = null;
						if (!this.RuntimeData.SceneDict.TryGetValue(fuBenSeqId, out scene))
						{
							ZhengDuoSceneInfo sceneInfo = client.SceneInfoObject as ZhengDuoSceneInfo;
							scene = new ZhengDuoScene();
							scene.CopyMap = copyMap;
							scene.CleanAllInfo();
							scene.GameId = gameId;
							scene.m_nMapCode = mapCode;
							scene.CopyMapId = copyMap.CopyMapID;
							scene.FuBenSeqId = fuBenSeqId;
							scene.SceneInfo = sceneInfo;
							scene.MapGridWidth = gameMap.MapGridWidth;
							scene.MapGridHeight = gameMap.MapGridHeight;
							DateTime startTime = this.GetStartTime(sceneInfo.Id);
							scene.StartTimeTicks = startTime.Ticks / 10000L;
							scene.m_lEndTime = scene.StartTimeTicks + (long)((sceneInfo.SecondWait + sceneInfo.SecondFight) * 1000);
							scene.PreliminarisesMode = (sceneInfo.Id == 1);
							this.RuntimeData.SceneDict[fuBenSeqId] = scene;
							if (sceneInfo.Id > 1)
							{
								if (!this.RuntimeData.FuBenItemData.TryGetValue((long)gameId, out fuBenData))
								{
									LogManager.WriteLog(LogTypes.Error, "没有为跨服副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
									return false;
								}
								if (fuBenData.State == GameFuBenState.End)
								{
									LogManager.WriteLog(LogTypes.Error, "跨服副本已经结束#GameID=" + gameId, null, true);
									return false;
								}
								foreach (KeyValuePair<int, int> kv in fuBenData.PlayerDict)
								{
									scene.ScoreData[kv.Value - 1] = new ZhengDuoScoreData(kv.Key, null, 0L);
								}
							}
							else
							{
								ZhengDuoSignUpData signUpData;
								if (!this.RuntimeData.FightDataDict.TryGetValue(client.ClientData.Faction, out signUpData))
								{
									LogManager.WriteLog(LogTypes.Error, "没有为跨服副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
									return false;
								}
								if (signUpData.StartTicks + (long)signUpData.UsedTicks <= TimeUtil.NOW())
								{
									LogManager.WriteLog(LogTypes.Error, "跨服副本已经结束#GameID=" + gameId, null, true);
									return false;
								}
								scene.StartTimeTicks = signUpData.StartTicks;
								client.ClientData.BattleWhichSide = 1;
							}
							scene.Start();
							this.InitCreateDynamicMonster(scene);
						}
						ZhengDuoScoreData clientContextData;
						if (!scene.ClientContextDataDict.TryGetValue(roleId, out clientContextData))
						{
							clientContextData = new ZhengDuoScoreData(roleId, Global.FormatRoleName4(client), 0L);
							scene.ClientContextDataDict[roleId] = clientContextData;
						}
						client.SceneObject = scene;
						client.SceneContextData2 = clientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)((scene.SceneInfo.TotalSecs + 120) * 1000));
						for (int i = 0; i < scene.RankDatas.Length; i++)
						{
							if (null == scene.RankDatas[i])
							{
								rankData = (scene.RankDatas[i] = new ZhengDuoRankData());
								rankData.Bhid = client.ClientData.Faction;
								break;
							}
							if (scene.RankDatas[i].Bhid == client.ClientData.Faction)
							{
								break;
							}
						}
						scoreData = scene.ScoreData[client.ClientData.BattleWhichSide - 1];
					}
					if (rankData != null && string.IsNullOrEmpty(rankData.BhName))
					{
						BangHuiDetailData bangHuiData = Global.GetBangHuiDetailData(client.ClientData.RoleID, client.ClientData.Faction, client.ServerId);
						if (bangHuiData != null)
						{
							rankData.Bhid = bangHuiData.BHID;
							rankData.BhName = bangHuiData.BHName;
							rankData.ZoneId = bangHuiData.ZoneID;
							rankData.BhLevel = bangHuiData.QiLevel;
							rankData.ServerID = GameManager.ServerId;
							rankData.ZhanLi = bangHuiData.TotalCombatForce;
							rankData.Week = TimeUtil.GetWeekStartDayIdNow();
							if (null != scoreData)
							{
								scoreData.Name = Global.FormatBangHuiNameWithZone(bangHuiData.ZoneID, bangHuiData.BHName);
							}
						}
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

		
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.ZhengDuo)
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhengDuoScene scene;
					this.RuntimeData.SceneDict.TryRemove(copyMap.FuBenSeqID, out scene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		private DateTime GetStartTime(int sceneId)
		{
			ZhengDuoSceneInfo sceneItem = null;
			TimeSpan startTime = TimeUtil.GetTimeOfWeekNow();
			DateTime now = TimeUtil.GetWeekStartTimeNow();
			lock (this.RuntimeData.Mutex)
			{
				if (this.ConfigData.SceneDataDict.TryGetValue(sceneId, out sceneItem))
				{
					if (startTime >= sceneItem.TimeBegin && startTime < sceneItem.TimeEnd)
					{
						startTime = sceneItem.TimeBegin;
					}
				}
			}
			return now.Add(startTime);
		}

		
		private void SyncData()
		{
			long zhengDuoAge = 0L;
			ZhengDuoSyncData syncData = TianTiClient.getInstance().ZhengDuoSync(this.RuntimeData.Age);
			lock (this.RuntimeData.Mutex)
			{
				if (syncData != null && this.RuntimeData.Age != syncData.Age)
				{
					zhengDuoAge = syncData.Age;
					this.RuntimeData.Age = syncData.Age;
					Array.Clear(this.RuntimeData.ZhengDuoRankDatas, 0, this.RuntimeData.ZhengDuoRankDatas.Length);
					this.RuntimeData.bhid2ZhengDuoRankDataDict.Clear();
					this.RuntimeData.Rank1Bhid = 0;
					bool notify = this.RuntimeData.ZhengDuoStep != syncData.ZhengDuoStep && syncData.State > this.RuntimeData.State;
					this.RuntimeData.ZhengDuoStep = syncData.ZhengDuoStep;
					this.RuntimeData.State = syncData.State;
					this.RuntimeData.WeekDay = syncData.WeekDay;
					if (null != syncData.RankDatas)
					{
						foreach (ZhengDuoRankData data in syncData.RankDatas)
						{
							if (data != null && data.Bhid > 0)
							{
								this.RuntimeData.bhid2ZhengDuoRankDataDict[data.Bhid] = data;
								this.RuntimeData.ZhengDuoRankDatas[data.Rank1] = data;
								if (notify && data.Lose == 0 && data.State > 0)
								{
									GameManager.ClientMgr.SendBangHuiCmd<int>(data.Bhid, 1072, syncData.ZhengDuoStep, true, true);
								}
								if (data.Rank2 == 1)
								{
									this.RuntimeData.Rank1Bhid = data.Bhid;
								}
							}
						}
					}
				}
			}
			if (zhengDuoAge > 0L)
			{
				int index = 0;
				GameClient client;
				while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
				{
					if (client.ClientData.ZhengDuoDataAge > 0L && client.ClientData.ZhengDuoDataAge != zhengDuoAge)
					{
						this.HandleZhengDuoData(client, 1070, null, null);
					}
				}
			}
		}

		
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= this.RuntimeData.NextHeartBeatTicks)
			{
				this.RuntimeData.NextHeartBeatTicks = nowTicks + 1020L;
				this.SyncData();
				foreach (ZhengDuoScene scene in this.RuntimeData.SceneDict.Values)
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
							scene.Age++;
							if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (ticks >= scene.m_lBeginTime)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.StateTimeData.GameType = 17;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									scene.CopyMap.AddGuangMuEvent(1, 0);
									scene.CopyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(scene.CopyMap.MapCode, scene.CopyMap.CopyMapID, 1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(scene.CopyMap.MapCode, scene.CopyMap.CopyMapID, 2, 0);
								}
								else if (scene.Age % 3 == 0)
								{
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<ZhengDuoScoreInfo>(1075, this.MakeZhengDuoScoreInfo(scene), scene.CopyMap);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime || scene.KillerId > 0)
								{
									this.CompleteScene(scene);
									scene.m_eStatus = GameSceneStatuses.STATUS_END;
									scene.m_lLeaveTime = scene.m_lEndTime + (long)(scene.SceneInfo.SecondClear * 1000);
									scene.StateTimeData.GameType = 17;
									scene.StateTimeData.State = 5;
									scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<ZhengDuoScoreInfo>(1075, this.MakeZhengDuoScoreInfo(scene), scene.CopyMap);
								}
								else
								{
									this.CheckCreateDynamicMonster(scene, nowTicks);
									if (scene.Age % 3 == 0)
									{
										GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
										GameManager.ClientMgr.BroadSpecialCopyMapMessage<ZhengDuoScoreInfo>(1075, this.MakeZhengDuoScoreInfo(scene), scene.CopyMap);
									}
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
								scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								if (scene.PreliminarisesMode)
								{
									ZhengDuoRankData data = scene.RankDatas[0];
									data.UsedMillisecond = scene.KillUsedTicks;
									ZhengDuoSignUpData signUpData;
									if (this.RuntimeData.FightDataDict.TryGetValue(data.Bhid, out signUpData))
									{
										signUpData.UsedTicks = scene.KillUsedTicks;
										EventLogManager.AddGameEvent(LogRecordType.ZhengDuoZhiDi, new object[]
										{
											1,
											this.RuntimeData.WeekDay,
											GameManager.ServerId,
											signUpData.Bhid,
											scene.KillUsedTicks,
											"争夺之地海选战斗结果"
										});
									}
									TianTiClient.getInstance().ZhengDuoSign(data.Bhid, data.UsedMillisecond, data.ZoneId, data.BhName, data.BhLevel, data.ZhanLi);
									this.GiveAwards(scene);
									int[] args = new int[]
									{
										data.Bhid,
										this.RuntimeData.WeekDay,
										scene.KillUsedTicks
									};
									Global.sendToDB<int, int[]>(10223, args, data.ServerID);
								}
								else
								{
									int[] bhids = new int[2];
									if (scene.ScoreData[0] != null)
									{
										bhids[0] = scene.ScoreData[0].Id;
									}
									if (scene.ScoreData[1] != null)
									{
										bhids[1] = scene.ScoreData[1].Id;
									}
									TianTiClient.getInstance().ZhengDuoResult(scene.SuccessSide, bhids);
									this.GiveAwards(scene);
									EventLogManager.AddGameEvent(LogRecordType.ZhengDuoZhiDi, new object[]
									{
										2,
										this.RuntimeData.WeekDay,
										GameManager.ServerId,
										scene.SuccessSide,
										bhids[0],
										bhids[1],
										"争夺之地淘汰赛战斗结果"
									});
								}
								ZhengDuoFuBenData fuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue((long)scene.GameId, out fuBenData))
								{
									fuBenData.State = GameFuBenState.End;
									LogManager.WriteLog(LogTypes.Error, string.Format("争夺之地跨服副本GameID={0},战斗结束", fuBenData.GameId), null, true);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= scene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(scene.m_lLeaveTime + 120000L);
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
													if (scene.PreliminarisesMode)
													{
														Global.GotoLastMap(c, 1);
													}
													else
													{
														KuaFuManager.getInstance().GotoLastMap(c);
													}
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "争夺之地系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void CompleteScene(ZhengDuoScene scene)
		{
			int successSide = 0;
			if (scene.PreliminarisesMode)
			{
				if (scene.KillerId > 0)
				{
					successSide = scene.RankDatas[0].Bhid;
				}
			}
			else if (scene.ScoreData[0] != null && scene.ScoreData[1] != null)
			{
				if (scene.ScoreData[0].Score > scene.ScoreData[1].Score)
				{
					successSide = scene.ScoreData[0].Id;
				}
				else if (scene.ScoreData[1].Score > scene.ScoreData[0].Score)
				{
					successSide = scene.ScoreData[1].Id;
				}
			}
			else if (scene.ScoreData[0] != null)
			{
				successSide = scene.ScoreData[0].Id;
			}
			scene.SuccessSide = successSide;
		}

		
		public void CreateMonster(ZhengDuoScene scene, ZhengDuoMonsterInfo monsterInfo)
		{
			GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, monsterInfo.MonsterID, scene.CopyMapId, 1, monsterInfo.PosX / scene.MapGridWidth, monsterInfo.PosY / scene.MapGridHeight, 0, 0, SceneUIClasses.ZhengDuo, monsterInfo, null);
		}

		
		private void AddDelayCreateMonster(ZhengDuoScene scene, long ticks, object monster)
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

		
		private void InitCreateDynamicMonster(ZhengDuoScene scene)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZhengDuoMonsterInfo monsterInfo = this.ConfigData.MonsterInfo;
				this.AddDelayCreateMonster(scene, scene.m_lBeginTime + (long)(monsterInfo.RefreshSecond * 1000), this.ConfigData.MonsterInfo);
			}
		}

		
		public void CheckCreateDynamicMonster(ZhengDuoScene scene, long nowMs)
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
							if (obj is ZhengDuoMonsterInfo)
							{
								ZhengDuoMonsterInfo monsterInfo = obj as ZhengDuoMonsterInfo;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, monsterInfo.MonsterID, scene.CopyMapId, 1, monsterInfo.PosX / scene.MapGridWidth, monsterInfo.PosY / scene.MapGridHeight, 0, 0, SceneUIClasses.ZhengDuo, monsterInfo, MonsterFlags.AllFlags);
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

		
		public void GiveAwards(ZhengDuoScene scene)
		{
			try
			{
				int[] bhids = new int[2];
				List<GameClient> list = scene.CopyMap.GetClientsList();
				foreach (GameClient client in list)
				{
					try
					{
						long score = 0L;
						ZhengDuoAwardData awardData = new ZhengDuoAwardData();
						if (scene.KillerId > 0)
						{
							awardData.Second = scene.KillUsedTicks / 1000;
						}
						if (scene.SuccessSide == client.ClientData.Faction)
						{
							awardData.State = 1;
						}
						AwardsItemList awardsItemList = new AwardsItemList();
						ZhengDuoScoreData scoreData;
						if (this.ConfigData.AwardHurtMin == 0 || (scene.ClientContextDataDict.TryGetValue(client.ClientData.RoleID, out scoreData) && (score = scoreData.Score) >= (long)this.ConfigData.AwardHurtMin))
						{
							if (client.ClientData.Faction == scene.SuccessSide)
							{
								long exp = Global.GetExpMultiByZhuanShengExpXiShu(client, scene.SceneInfo.RateExp);
								int money = scene.SceneInfo.RateBindJinBi;
								awardData.Exp = exp;
								awardData.Money = money;
								ConfigParser.ParseAwardsItemList(scene.SceneInfo.AwardWin, ref awardsItemList, '|', ',');
							}
							else
							{
								long exp = (long)((double)Global.GetExpMultiByZhuanShengExpXiShu(client, scene.SceneInfo.RateExp) * 0.8);
								int money = (int)((double)scene.SceneInfo.RateBindJinBi * 0.8);
								awardData.Exp = exp;
								awardData.Money = money;
								ConfigParser.ParseAwardsItemList(scene.SceneInfo.AwardLost, ref awardsItemList, '|', ',');
							}
							if (client.ClientData.RoleID == scene.KillerId)
							{
								awardsItemList.Add(scene.SceneInfo.AwardKiller);
							}
						}
						else
						{
							LogManager.WriteLog(LogTypes.Data, string.Format("争夺之地积分未达到最低奖励要求#rid={0},rname={1},score={2}", client.ClientData.RoleID, client.ClientData.RoleName, score), null, true);
						}
						awardData.GoodsList = Global.ConvertToGoodsDataList(awardsItemList.Items, -1);
						if (awardData.Money > 0)
						{
							GameManager.ClientMgr.AddMoney1(client, awardData.Money, "争夺之地奖励", true);
						}
						if (awardData.Exp > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, awardData.Exp, true, true, false, "争夺之地奖励");
						}
						if (awardData.GoodsList != null && awardData.GoodsList.Count > 0)
						{
							if (!Global.CanAddGoodsDataList(client, awardData.GoodsList))
							{
								GameManager.ClientMgr.SendMailWhenPacketFull(client, awardData.GoodsList, GLang.GetLang(551, new object[0]), GLang.GetLang(552, new object[0]));
							}
							else
							{
								for (int i = 0; i < awardData.GoodsList.Count; i++)
								{
									GoodsData goodsData = awardData.GoodsList[i];
									if (null != goodsData)
									{
										goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "争夺之地奖励", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, 0, 0, 0, null, null, 0, true);
									}
								}
							}
						}
						client.sendCmd<ZhengDuoAwardData>(1076, awardData, false);
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState, bool scoreInfo = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZhengDuoScene scene;
				if (this.RuntimeData.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (scoreInfo)
					{
						client.sendCmd<ZhengDuoScoreInfo>(1075, this.MakeZhengDuoScoreInfo(scene), false);
					}
				}
			}
		}

		
		private ZhengDuoScoreInfo MakeZhengDuoScoreInfo(ZhengDuoScene scene)
		{
			ZhengDuoScoreInfo scoreInfo = new ZhengDuoScoreInfo
			{
				Step = scene.SceneInfo.Id
			};
			scoreInfo.ScoreRank = new List<ZhengDuoScoreData>();
			int i = 0;
			while (i < scene.ScoreData.Length && i < 3)
			{
				if (null != scene.ScoreData[i])
				{
					scoreInfo.ScoreRank.Add(scene.ScoreData[i]);
				}
				i++;
			}
			return scoreInfo;
		}

		
		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				ZhengDuoBirthInfo birthPoint;
				if (this.ConfigData.BirthInfoList.TryGetValue(side, out birthPoint))
				{
					posX = birthPoint.X;
					posY = birthPoint.Y;
					return side;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		
		private ZhengDuoFuBenData GetKuaFuFuBenDataByBhid(int bhid, int weekDay)
		{
			ZhengDuoFuBenData fuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.FuBenItemDataByBhid.TryGetValue(bhid, out fuBenData) && fuBenData.WeekDay == weekDay && fuBenData.GroupIndex == this.RuntimeData.ZhengDuoStep)
				{
					return fuBenData;
				}
			}
			fuBenData = TianTiClient.getInstance().GetZhengDuoFuBenDataByBhid(bhid);
			if (fuBenData == null || fuBenData.WeekDay != weekDay || fuBenData.GroupIndex != this.RuntimeData.ZhengDuoStep)
			{
				LogManager.WriteLog(LogTypes.Error, "获取不到有效的争夺之地副本数据,fuBenData == null", null, true);
				fuBenData = null;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhengDuoFuBenData fuBenData2;
					if (this.RuntimeData.FuBenItemDataByBhid.TryGetValue(bhid, out fuBenData2) && fuBenData2.WeekDay == weekDay && fuBenData2.GroupIndex == this.RuntimeData.ZhengDuoStep)
					{
						fuBenData = fuBenData2;
					}
					else
					{
						this.RuntimeData.FuBenItemDataByBhid[bhid] = fuBenData;
					}
				}
			}
			return fuBenData;
		}

		
		private ZhengDuoFuBenData GetKuaFuFuBenData(long gameId, int weekDay)
		{
			ZhengDuoFuBenData fuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData) && fuBenData.WeekDay == weekDay && fuBenData.GroupIndex == this.RuntimeData.ZhengDuoStep)
				{
					return fuBenData;
				}
			}
			fuBenData = TianTiClient.getInstance().GetZhengDuoFuBenData(gameId);
			if (fuBenData == null || fuBenData.WeekDay != weekDay)
			{
				LogManager.WriteLog(LogTypes.Error, "获取不到有效的争夺之地副本数据,fuBenData == null", null, true);
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhengDuoFuBenData fuBenData2;
					if (this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData2) && fuBenData2.WeekDay == weekDay && fuBenData2.GroupIndex == this.RuntimeData.ZhengDuoStep)
					{
						fuBenData = fuBenData2;
					}
					else
					{
						this.RuntimeData.FuBenItemData[gameId] = fuBenData;
						fuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
					}
				}
			}
			return fuBenData;
		}

		
		public bool OnInitGame(GameClient client)
		{
			int side = 0;
			bool result = false;
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			ZhengDuoFuBenData fuBenData = this.GetKuaFuFuBenData(kuaFuServerLoginData.GameId, this.RuntimeData.WeekDay);
			if (fuBenData != null && fuBenData.State < GameFuBenState.End)
			{
				if (fuBenData.ServerId == GameManager.ServerId)
				{
					if (fuBenData.PlayerDict.TryGetValue(client.ClientData.Faction, out side))
					{
						result = true;
					}
				}
			}
			bool result2;
			if (!result)
			{
				result2 = false;
			}
			else
			{
				client.ClientData.BattleWhichSide = side;
				int posX;
				int posY;
				side = this.GetBirthPoint(client, out posX, out posY);
				if (side <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, "无法获取有效的阵营和出生点,进入跨服失败,side=" + side, null, true);
					result2 = false;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						kuaFuServerLoginData.FuBenSeqId = fuBenData.SequenceId;
						ZhengDuoSceneInfo sceneInfo;
						if (!this.ConfigData.SceneDataDict.TryGetValue(fuBenData.GroupIndex, out sceneInfo))
						{
							return false;
						}
						client.ClientData.MapCode = sceneInfo.MapCode;
						client.SceneInfoObject = sceneInfo;
					}
					client.ClientData.PosX = posX;
					client.ClientData.PosY = posY;
					client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
					result2 = true;
				}
			}
			return result2;
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

		
		public const SceneUIClasses _sceneType = SceneUIClasses.ZhengDuo;

		
		public const GameTypes _gameType = GameTypes.ZhengDuo;

		
		private static ZhengDuoManager instance = new ZhengDuoManager();

		
		private KFZhengDuoConfig ConfigData = new KFZhengDuoConfig();

		
		private ZhengDuoRuntimeData RuntimeData = new ZhengDuoRuntimeData();
	}
}
