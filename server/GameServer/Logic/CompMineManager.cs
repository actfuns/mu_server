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
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class CompMineManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		
		public static CompMineManager getInstance()
		{
			return CompMineManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CompMineManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 2000);
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2010, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2011, 2, 2, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2012, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2013, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2015, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2018, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(34, 53, CompMineManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 53, CompMineManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(29, 53, CompMineManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, CompMineManager.getInstance());
			GlobalEventSource.getInstance().registerListener(12, CompMineManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, CompMineManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, CompMineManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(34, 53, CompMineManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 53, CompMineManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(29, 53, CompMineManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, CompMineManager.getInstance());
			GlobalEventSource.getInstance().removeListener(12, CompMineManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, CompMineManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, CompMineManager.getInstance());
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

		
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.Comp, hint) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen(120403);
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 2010:
					return this.ProcessGetCompMineBaseDataCmd(client, nID, bytes, cmdParams);
				case 2011:
					return this.ProcessCompMineEnterCmd(client, nID, bytes, cmdParams);
				case 2012:
					return this.ProcessGetCompMineAwardInfoCmd(client, nID, bytes, cmdParams);
				case 2013:
					return this.ProcessGetCompMineStateCmd(client, nID, bytes, cmdParams);
				case 2015:
					return this.ProcessGetCompMineSelfScoreCmd(client, nID, bytes, cmdParams);
				case 2018:
					return this.ProcessGetCompMineAwardCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		
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
			else if (eventObject.getEventType() == 12)
			{
				PlayerLogoutEventObject eventObj = (PlayerLogoutEventObject)eventObject;
				this.OnLogout(eventObj.getPlayer());
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			switch (num)
			{
			case 29:
			{
				OnClientChangeMapEventObject e = eventObject as OnClientChangeMapEventObject;
				if (null != e)
				{
					e.Result = this.ClientChangeMap(e.Client, e.TeleportID, ref e.ToMapCode, ref e.ToPosX, ref e.ToPosY);
					e.Handled = true;
				}
				break;
			}
			case 30:
			{
				OnCreateMonsterEventObject e2 = eventObject as OnCreateMonsterEventObject;
				if (null != e2)
				{
					CompMineScene scene = null;
					CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(e2.Monster.CopyMapID);
					if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out scene))
					{
						CompMineTruckConfig truckConfig = e2.Monster.Tag as CompMineTruckConfig;
						if (null != truckConfig)
						{
							e2.Monster.Camp = scene.GameId;
							e2.Result = true;
							e2.Handled = true;
						}
					}
				}
				break;
			}
			default:
				if (num == 34)
				{
					AfterMonsterInjureEventObject obj = eventObject as AfterMonsterInjureEventObject;
					if (obj != null && obj.SceneType == 53)
					{
						Monster injureMonster = obj.Monster;
						if (injureMonster != null)
						{
							CompMineTruckConfig truckConfig = obj.Monster.Tag as CompMineTruckConfig;
							if (null != truckConfig)
							{
								bool setTruckDamage = false;
								CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(obj.Monster.CopyMapID);
								CompMineScene scene = null;
								if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out scene) && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
								{
									setTruckDamage = true;
								}
								if (setTruckDamage)
								{
									obj.Injure = Math.Min(obj.Injure + obj.MerlinInjure, truckConfig.MaxHurt);
									obj.MerlinInjure = 0;
								}
								else
								{
									obj.Injure = 0;
									obj.MerlinInjure = 0;
								}
								eventObject.Handled = true;
								eventObject.Result = true;
							}
						}
					}
				}
				break;
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
					this.RuntimeData.CompMineConfigDict.Clear();
					fileName = "Config/CompMineWar.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompMineConfig item = new CompMineConfig();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.Name = Global.GetSafeAttributeStr(node, "Name");
						item.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						item.MaxEnterNum = (int)Global.GetSafeAttributeLong(node, "MaxEnterNum");
						item.EnterCD = (int)Global.GetSafeAttributeLong(node, "EnterCD");
						item.PrepareSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
						item.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
						item.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
						item.ExpNum = (int)Global.GetSafeAttributeLong(node, "Exp");
						item.BandJinBiNum = (int)Global.GetSafeAttributeLong(node, "BandJinBi");
						if (!ConfigParser.ParserTimeRangeListWithDay(item.TimePoints, Global.GetSafeAttributeStr(node, "TimePoints"), true, '|', '-', ','))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, string.Format("读取{0}时间配置(TimePoints)出错", fileName), null, true);
						}
						for (int i = 0; i < item.TimePoints.Count; i++)
						{
							TimeSpan ts = new TimeSpan(item.TimePoints[i].Hours, item.TimePoints[i].Minutes, item.TimePoints[i].Seconds);
							item.SecondsOfDay.Add(ts.TotalSeconds);
						}
						this.RuntimeData.CompMineConfigDict[item.ID] = item;
					}
					this.RuntimeData.CompMineTruckConfigList.Clear();
					fileName = "Config/CompMineTruck.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompMineTruckConfig item2 = new CompMineTruckConfig();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonstersID");
						item2.TimePoints = (int)Global.GetSafeAttributeLong(node, "TimePoints");
						item2.Speed = Global.GetSafeAttributeDouble(node, "Speed");
						item2.MaxHurt = (int)Global.GetSafeAttributeLong(node, "MaxHurt");
						item2.AddLIfe = Global.GetSafeAttributeIntArray(node, "AddLIfe", -1, '|');
						item2.FinishNum = Global.GetSafeAttributeDoubleArray(node, "FinishNum", -1, '|');
						item2.BrokenNum = Global.GetSafeAttributeDoubleArray(node, "BrokenNum", -1, '|');
						item2.CompBoomNum = (int)Global.GetSafeAttributeLong(node, "CompNum");
						item2.CompMineNum = (int)Global.GetSafeAttributeLong(node, "CompMineNum");
						this.RuntimeData.CompMineTruckConfigList.Add(item2);
					}
					this.RuntimeData.CompMineLinkConfigList.Clear();
					fileName = "Config/CompMineLink.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompMineLinkConfig item3 = new CompMineLinkConfig();
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						string[] strFields = Global.GetSafeAttributeStr(node, "TargetSide").Split(new char[]
						{
							'|'
						});
						if (strFields.Length == 2)
						{
							item3.PosX = Global.SafeConvertToInt32(strFields[0]);
							item3.PosY = Global.SafeConvertToInt32(strFields[1]);
						}
						item3.Supplies = (Global.GetSafeAttributeLong(node, "Supplies") > 0L);
						item3.End = (Global.GetSafeAttributeLong(node, "End") > 0L);
						this.RuntimeData.CompMineLinkConfigList.Add(item3);
					}
					this.RuntimeData.CompMineBirthConfigDict.Clear();
					fileName = "Config/CompMineBirthPoint.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompMineBirthConfig item4 = new CompMineBirthConfig();
						item4.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item4.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						string[] strFields = Global.GetSafeAttributeStr(node, "JiaoTuanBirth").Split(new char[]
						{
							'|'
						});
						if (strFields.Length == 3)
						{
							CMBirthPoint pt = new CMBirthPoint();
							pt.PosX = Global.SafeConvertToInt32(strFields[0]);
							pt.PosY = Global.SafeConvertToInt32(strFields[1]);
							pt.BirthRadius = Global.SafeConvertToInt32(strFields[2]);
							item4.BirthPoints[0] = pt;
						}
						strFields = Global.GetSafeAttributeStr(node, "MengJunBirth").Split(new char[]
						{
							'|'
						});
						if (strFields.Length == 3)
						{
							CMBirthPoint pt = new CMBirthPoint();
							pt.PosX = Global.SafeConvertToInt32(strFields[0]);
							pt.PosY = Global.SafeConvertToInt32(strFields[1]);
							pt.BirthRadius = Global.SafeConvertToInt32(strFields[2]);
							item4.BirthPoints[1] = pt;
						}
						strFields = Global.GetSafeAttributeStr(node, "XieHuiBirth").Split(new char[]
						{
							'|'
						});
						if (strFields.Length == 3)
						{
							CMBirthPoint pt = new CMBirthPoint();
							pt.PosX = Global.SafeConvertToInt32(strFields[0]);
							pt.PosY = Global.SafeConvertToInt32(strFields[1]);
							pt.BirthRadius = Global.SafeConvertToInt32(strFields[2]);
							item4.BirthPoints[2] = pt;
						}
						this.RuntimeData.CompMineBirthConfigDict[item4.MapCode] = item4;
					}
					this.RuntimeData.CompMineRewardConfigList.Clear();
					fileName = "Config/CompMineAward.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompMineRewardConfig item5 = new CompMineRewardConfig();
						item5.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item5.Rank = (int)Global.GetSafeAttributeLong(node, "Rank");
						item5.RankRate = Global.GetSafeAttributeDouble(node, "RankRate");
						item5.Grade = (int)Global.GetSafeAttributeLong(node, "CompFeast");
						item5.Contribution = (int)Global.GetSafeAttributeLong(node, "CompHonor");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "GoodsOne"), ref item5.AwardsItemListOne, '|', ',');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "GoodsTwo"), ref item5.AwardsItemListTwo, '|', ',');
						this.RuntimeData.CompMineRewardConfigList.Add(item5);
					}
					this.RuntimeData.CompMineAttackKill = GameManager.systemParamsList.GetParamValueIntArrayByName("CompMineAttackKill", ',');
					this.RuntimeData.CompMineAttackShutDown = GameManager.systemParamsList.GetParamValueIntArrayByName("CompMineAttackShutDown", ',');
					this.RuntimeData.CompMineDie = (int)GameManager.systemParamsList.GetParamValueIntByName("CompMineDie", -1);
					this.RuntimeData.CompMineAwardNum = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CompMineAwardNum", '|');
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
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				CompBattleGameStates state = CompBattleGameStates.None;
				this.CheckCondition(null, ref state);
				if (CompBattleGameStates.None != state)
				{
					foreach (CompMineScene scene in this.SceneDict.Values)
					{
						CompFuBenData fubenItem = null;
						if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fubenItem))
						{
							fubenItem.Init();
							List<GameClient> objsList = scene.CopyMap.GetClientsList();
							if (objsList != null && objsList.Count > 0)
							{
								for (int i = 0; i < objsList.Count; i++)
								{
									GameClient c = objsList[i];
									if (c != null)
									{
										int side = c.ClientData.BattleWhichSide;
										if (side > 0 && side <= fubenItem.RoleCountSideList.Count)
										{
											List<int> roleCountSideList;
											int index;
											(roleCountSideList = fubenItem.RoleCountSideList)[index = side - 1] = roleCountSideList[index] + 1;
											if (c.ClientData.CompZhiWu > 0)
											{
												fubenItem.ZhuJiangRoleDict[c.ClientData.CompType].Add(c.ClientData.RoleID);
											}
										}
									}
								}
							}
							fubenItem.MineTruckGo = scene.ScoreData.MineTruckGo;
							fubenItem.MineSafeArrived = scene.ScoreData.SafeArrived;
							TianTiClient.getInstance().Comp_UpdateKuaFuMapClientCount(31, fubenItem);
						}
					}
				}
			}
		}

		
		public bool ProcessGetCompMineBaseDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleGameStates timeState = CompBattleGameStates.None;
				this.CheckCondition(client, ref timeState);
				List<CompMineBaseData> baseDataList = new List<CompMineBaseData>();
				lock (this.RuntimeData.Mutex)
				{
					for (int compLoop = 1; compLoop <= 3; compLoop++)
					{
						CompMineBaseData baseItem = new CompMineBaseData();
						if (CompBattleGameStates.None != timeState)
						{
							CompFuBenData newFuBenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(31, compLoop);
							if (null != newFuBenData)
							{
								baseItem.MineTruckGo = newFuBenData.MineTruckGo;
								baseItem.SafeArrived = newFuBenData.MineSafeArrived;
							}
						}
						baseDataList.Add(baseItem);
					}
				}
				client.sendCmd<List<CompMineBaseData>>(nID, baseDataList, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessCompMineEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int cityID = Global.SafeConvertToInt32(cmdParams[1]);
				int result = 0;
				CompBattleGameStates state = CompBattleGameStates.None;
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				if (!this.CheckMap(client))
				{
					result = -21;
				}
				else
				{
					result = this.CheckCondition(client, ref state);
					if (state != CompBattleGameStates.Start)
					{
						result = -2001;
						client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
						return true;
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					CompMineConfig compMineConfig = null;
					if (!this.RuntimeData.CompMineConfigDict.TryGetValue(cityID, out compMineConfig))
					{
						result = -3;
						client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
						return true;
					}
					DateTime lastEnterTime = Global.GetRoleParamsDateTimeFromDB(client, "20022");
					if (TimeUtil.NowDateTime().Ticks - lastEnterTime.Ticks < 10000000L * (long)compMineConfig.EnterCD)
					{
						GameManager.ClientMgr.NotifyImportantMsg(client, string.Format(GLang.GetLang(2615, new object[0]), compMineConfig.EnterCD), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -2007;
						client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
						return true;
					}
					KuaFuServerInfo kfserverInfo = null;
					CompFuBenData fubenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(31, cityID);
					if (fubenData == null || !KuaFuManager.getInstance().TryGetValue(fubenData.ServerId, out kfserverInfo))
					{
						result = -11000;
						client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
						return true;
					}
					if (fubenData.GetRoleCountWithEnter(client.ClientData.CompType) >= compMineConfig.MaxEnterNum)
					{
						result = -22;
						client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
						return true;
					}
					KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
					if (null != clientKuaFuServerLoginData)
					{
						clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
						clientKuaFuServerLoginData.GameId = (long)fubenData.GameId;
						clientKuaFuServerLoginData.GameType = 31;
						clientKuaFuServerLoginData.EndTicks = fubenData.EndTime.Ticks;
						clientKuaFuServerLoginData.ServerId = client.ServerId;
						clientKuaFuServerLoginData.ServerIp = kfserverInfo.Ip;
						clientKuaFuServerLoginData.ServerPort = kfserverInfo.Port;
						clientKuaFuServerLoginData.FuBenSeqId = 0;
					}
				}
				if (result >= 0)
				{
					result = TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(31, client.ServerId, cityID, client.ClientData.RoleID, (int)client.ClientData.CompZhiWu, 4);
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
				client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessGetCompMineAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == compRoleData)
				{
					return true;
				}
				CompMineAwardsData awardsData = new CompMineAwardsData();
				if (compRoleData.MineJiFen > 0)
				{
					awardsData.RankNum = compRoleData.MineRankNum;
					CompMineRewardConfig awardConfig = this.CalMineRewardConfig(client, compRoleData);
					awardsData.AwardID = ((awardConfig != null) ? awardConfig.ID : 0);
				}
				client.sendCmd<CompMineAwardsData>(nID, awardsData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessGetCompMineStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleGameStates timeState = CompBattleGameStates.None;
				this.CheckCondition(client, ref timeState);
				int result = (int)timeState;
				KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (result == 0 && compRoleData != null && compRoleData.CompType == compRoleData.CompTypeMine)
				{
					string awardsInfo = Global.GetRoleParamByName(client, "150");
					if (!string.IsNullOrEmpty(awardsInfo))
					{
						long LastStartTimeTicks = Global.SafeConvertToInt64(awardsInfo);
						if (TimeUtil.NOW() - LastStartTimeTicks < 604800000L)
						{
							client.sendCmd<int>(nID, 2, false);
							return true;
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
			return true;
		}

		
		public bool ProcessGetCompMineSelfScoreCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				CompMineScene scene = client.SceneObject as CompMineScene;
				if (null == scene)
				{
					return true;
				}
				KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == compRoleData)
				{
					return true;
				}
				CompMineSelfScore selfScore = new CompMineSelfScore();
				CompMineRewardConfig awardConfig = this.CalMineRewardConfig(client, compRoleData);
				selfScore.RankNum = compRoleData.MineRankNum;
				selfScore.AwardID = ((awardConfig != null) ? awardConfig.ID : 0);
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					CompManager.getInstance().CompSyncDataCache.CompRankMineJiFenDict.V.TryGetValue(client.ClientData.CompType, out selfScore.rankInfo2Client);
				}
				client.sendCmd<CompMineSelfScore>(nID, selfScore, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessGetCompMineAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleGameStates timeState = CompBattleGameStates.None;
				this.CheckCondition(client, ref timeState);
				if (CompBattleGameStates.None != timeState)
				{
					int err = -2001;
					client.sendCmd<int>(nID, err, false);
					return true;
				}
				string awardsInfo = Global.GetRoleParamByName(client, "150");
				if (!string.IsNullOrEmpty(awardsInfo))
				{
					bool clear = true;
					long LastStartTimeTicks = Global.SafeConvertToInt64(awardsInfo);
					int err;
					if (TimeUtil.NOW() - LastStartTimeTicks < 604800000L)
					{
						err = this.GiveRoleAwards(client, LastStartTimeTicks);
						if (err < 0)
						{
							clear = false;
						}
					}
					else
					{
						err = -5;
					}
					if (clear)
					{
						Global.SaveRoleParamsStringToDB(client, "150", "", true);
					}
					client.sendCmd<int>(nID, err, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public void OnCompMineReset()
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				if (client != null && client.ClientData.CompType > 0)
				{
					int MineJiFen = GameManager.ClientMgr.GetCompMineJiFenValue(client);
					if (MineJiFen > 0)
					{
						GameManager.ClientMgr.ModifyCompMineJiFenValue(client, -MineJiFen, "势力矿洞KF", true, true, false);
					}
				}
			}
		}

		
		public bool IsCompMineMap(int mapCodeID)
		{
			bool result;
			lock (this.RuntimeData.Mutex)
			{
				foreach (KeyValuePair<int, CompMineConfig> item in this.RuntimeData.CompMineConfigDict)
				{
					if (item.Value.MapCode == mapCodeID)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		
		private bool CheckMap(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return sceneType == SceneUIClasses.Normal || sceneType == SceneUIClasses.Comp;
		}

		
		public int CheckCondition(GameClient client, ref CompBattleGameStates state)
		{
			int result = 0;
			CompMineConfig sceneItem = null;
			if (client != null && !this.IsGongNengOpened(client, true))
			{
				result = -13;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					sceneItem = this.RuntimeData.CompMineConfigDict.Values.FirstOrDefault<CompMineConfig>();
					if (null == sceneItem)
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
						if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
						{
							if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1] - (double)sceneItem.ClearRolesSecs)
							{
								state = CompBattleGameStates.Start;
								result = 1;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
							{
								state = CompBattleGameStates.Analysis;
								result = 1;
							}
							else
							{
								state = CompBattleGameStates.None;
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
			CompMineConfig sceneItem = null;
			TimeSpan startTime = TimeSpan.MinValue;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CompMineConfigDict.TryGetValue(sceneId, out sceneItem))
				{
					goto IL_14B;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						startTime = TimeSpan.FromSeconds(sceneItem.SecondsOfDay[i]);
						break;
					}
				}
			}
			IL_14B:
			if (startTime < TimeSpan.Zero)
			{
				startTime = now.TimeOfDay;
			}
			return startTime;
		}

		
		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			CompFuBenData kfubenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(31, (int)kuaFuServerLoginData.GameId);
			bool result;
			if (kfubenData == null || kfubenData.ServerId != GameManager.ServerId)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					CompMineConfig sceneInfo;
					if (!this.RuntimeData.CompMineConfigDict.TryGetValue(kfubenData.GameId, out sceneInfo) || (long)sceneInfo.ID != kuaFuServerLoginData.GameId)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		
		public bool OnInitGameKuaFu(GameClient client)
		{
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			CompFuBenData fuBenData;
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
				CompFuBenData newFuBenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(31, (int)kuaFuServerLoginData.GameId);
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
						fuBenData.Init();
						this.RuntimeData.FuBenItemData[fuBenData.GameId] = fuBenData;
					}
				}
			}
			CompMineConfig sceneInfo;
			lock (this.RuntimeData.Mutex)
			{
				kuaFuServerLoginData.FuBenSeqId = fuBenData.SequenceId;
				if (!this.RuntimeData.CompMineConfigDict.TryGetValue(fuBenData.GameId, out sceneInfo))
				{
					return false;
				}
			}
			client.ClientData.BattleWhichSide = client.ClientData.CompType;
			int posX;
			int posY;
			int side = this.GetBirthPoint(sceneInfo.MapCode, client, out posX, out posY);
			bool result;
			if (side <= 0)
			{
				LogManager.WriteLog(LogTypes.Error, "无法获取有效的阵营和出生点,进入跨服失败,side=" + side, null, true);
				result = false;
			}
			else
			{
				client.ClientData.MapCode = sceneInfo.MapCode;
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
				Global.SaveRoleParamsDateTimeToDB(client, "20022", TimeUtil.NowDateTime(), true);
				result = true;
			}
			return result;
		}

		
		private void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		
		public bool ClientRelive(GameClient client)
		{
			int toPosX;
			int toPosY;
			int side = this.GetBirthPoint(client.ClientData.MapCode, client, out toPosX, out toPosY);
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

		
		public int GetBirthPoint(int mapCode, GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				CompMineBirthConfig birthPoint = null;
				if (this.RuntimeData.CompMineBirthConfigDict.TryGetValue(mapCode, out birthPoint))
				{
					CMBirthPoint pt = birthPoint.BirthPoints[side - 1];
					CompMineScene scene = client.SceneObject as CompMineScene;
					if (null != scene)
					{
						Point BirthPoint = Global.GetMapPointByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, pt.PosX / scene.MapGridWidth, pt.PosY / scene.MapGridHeight, pt.BirthRadius / scene.MapGridWidth, 0, false);
						posX = (int)BirthPoint.X;
						posY = (int)BirthPoint.Y;
						return side;
					}
					posX = pt.PosX;
					posY = pt.PosY;
					return side;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		
		public void UpdateCompMineResourceData(Dictionary<int, KFCompData> CompDataDict)
		{
			if (null != CompDataDict)
			{
				lock (this.RuntimeData.Mutex)
				{
					foreach (CompMineScene scene in this.SceneDict.Values)
					{
						for (int i = 0; i < scene.ScoreData.ResJiFenList.Count; i++)
						{
							CompMineResData resData = scene.ScoreData.ResJiFenList[i];
							KFCompData compData;
							if (CompDataDict.TryGetValue(resData.CompType, out compData))
							{
								resData.MineRes = compData.MineRes;
								resData.Rank = compData.MineRank;
							}
						}
						this.UpdateBattleSideScoreRank(scene, true);
					}
				}
			}
		}

		
		public void UpdateBattleSideScoreRank(CompMineScene scene, bool sync = true)
		{
			try
			{
				if (scene.ScoreData.ResJiFenList.Count != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("势力矿洞分数信息异常 CityID={0}", scene.GameId), null, true);
				}
				else
				{
					scene.ScoreData.ResJiFenList.Sort(delegate(CompMineResData left, CompMineResData right)
					{
						int result;
						if (left.MineRes > right.MineRes)
						{
							result = -1;
						}
						else if (left.MineRes < right.MineRes)
						{
							result = 1;
						}
						else if (left.Rank < right.Rank)
						{
							result = -1;
						}
						else if (left.Rank > right.Rank)
						{
							result = 1;
						}
						else if (left.CompType < right.CompType)
						{
							result = -1;
						}
						else if (left.CompType > right.CompType)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					for (int loop = 0; loop < scene.ScoreData.ResJiFenList.Count; loop++)
					{
						CompMineResData ssData = scene.ScoreData.ResJiFenList[loop];
						ssData.Rank = loop + 1;
					}
					if (sync)
					{
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<CompMineSideScore>(2014, scene.ScoreData, scene.CopyMap);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		private void InitScene(CompMineScene scene, Dictionary<int, KFCompData> compDataDict)
		{
			for (int compLoop = 1; compLoop <= 3; compLoop++)
			{
				CompMineResData scoreData = new CompMineResData();
				scoreData.CompType = compLoop;
				KFCompData compData = null;
				if (compDataDict.TryGetValue(compLoop, out compData))
				{
					scoreData.Rank = compData.MineRank;
					scoreData.MineRes = compData.MineRes;
				}
				scene.ScoreData.ResJiFenList.Add(scoreData);
			}
			this.UpdateBattleSideScoreRank(scene, false);
		}

		
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.CompMine)
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
				{
					result = false;
				}
				else
				{
					Dictionary<int, KFCompData> tempCompDataDict = null;
					lock (CompManager.getInstance().RuntimeData.Mutex)
					{
						tempCompDataDict = CompManager.getInstance().CompSyncDataCache.CompDataDict.V;
					}
					int fuBenSeqId = copyMap.FuBenSeqID;
					int mapCode = copyMap.MapCode;
					int roleId = client.ClientData.RoleID;
					int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime now = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						CompMineScene scene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
						{
							CompMineConfig sceneInfo = null;
							CompFuBenData fuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
							{
								LogManager.WriteLog(LogTypes.Error, "势力矿洞没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							if (!this.RuntimeData.CompMineConfigDict.TryGetValue(fuBenData.GameId, out sceneInfo))
							{
								LogManager.WriteLog(LogTypes.Error, "势力矿洞没有为副本找到对应的档位数据,ID:" + fuBenData.GameId, null, true);
							}
							scene = new CompMineScene();
							scene.CopyMap = copyMap;
							scene.CleanAllInfo();
							scene.GameId = gameId;
							scene.m_nMapCode = mapCode;
							scene.CopyMapId = copyMap.CopyMapID;
							scene.FuBenSeqId = fuBenSeqId;
							scene.SceneInfo = sceneInfo;
							scene.MapGridWidth = gameMap.MapGridWidth;
							scene.MapGridHeight = gameMap.MapGridHeight;
							DateTime startTime = now.Date.Add(this.GetStartTime(sceneInfo.ID));
							scene.StartTimeTicks = startTime.Ticks / 10000L;
							this.InitScene(scene, tempCompDataDict);
							this.SceneDict[fuBenSeqId] = scene;
						}
						CompMineClientContextData clientContextData;
						if (!scene.ClientContextDataDict.TryGetValue(roleId, out clientContextData))
						{
							clientContextData = new CompMineClientContextData
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
						client.SceneObject = scene;
						client.SceneGameId = (long)scene.GameId;
						client.SceneContextData2 = clientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(scene.SceneInfo.TotalSecs * 1000));
						TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(31, client.ServerId, scene.SceneInfo.ID, roleId, (int)client.ClientData.CompZhiWu, 5);
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
			if (sceneType == SceneUIClasses.CompMine)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompMineScene CompMineScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out CompMineScene);
					this.RuntimeData.FuBenItemData.Remove(CompMineScene.GameId);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			CompMineTruckConfig tagInfo = monster.Tag as CompMineTruckConfig;
			if (null != tagInfo)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompMineScene scene = client.SceneObject as CompMineScene;
					if (scene != null && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						CompMineClientContextData clientContextData = client.SceneContextData2 as CompMineClientContextData;
						if (null != clientContextData)
						{
							clientContextData.TruckInjure += injure;
						}
					}
				}
			}
		}

		
		private void ProcessEnd(CompMineScene scene, long nowTicks)
		{
			scene.m_eStatus = GameSceneStatuses.STATUS_END;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 31;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		
		public void GiveAwards(CompMineScene scene)
		{
			try
			{
				foreach (CompMineClientContextData contextData in scene.ClientContextDataDict.Values)
				{
					GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleId);
					if (null != client)
					{
						long addExp = Global.GetExpMultiByZhuanShengExpXiShu(client, (long)scene.SceneInfo.ExpNum);
						int addBindJinBi = scene.SceneInfo.BandJinBiNum;
						if (addExp > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, addExp, true, true, false, "none");
						}
						if (addBindJinBi > 0)
						{
							GameManager.ClientMgr.AddMoney1(client, addBindJinBi, "势力矿洞基础奖励", true);
						}
					}
					if (contextData.BattleJiFen > 0)
					{
						string awardsInfo = string.Format("{0}", scene.StartTimeTicks);
						if (client != null)
						{
							Global.SaveRoleParamsStringToDB(client, "150", awardsInfo, true);
						}
						else
						{
							Global.UpdateRoleParamByNameOffline(contextData.RoleId, "150", awardsInfo, contextData.ServerId);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "势力矿洞系统奖励异常");
			}
		}

		
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<CompMineSideScore>(2014, scene.ScoreData, false);
					}
				}
			}
		}

		
		public void OnStartPlayGame(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					CompFuBenData fuBenData;
					if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
					{
						if (client.ClientData.CompZhiWu > 0)
						{
							CompMineClientContextData clientContextData = client.SceneContextData2 as CompMineClientContextData;
							fuBenData.ZhuJiangRoleDict[client.ClientData.CompType].Add(client.ClientData.RoleID);
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<CompMineSideScore>(2014, scene.ScoreData, scene.CopyMap);
						}
						List<int> roleCountSideList;
						int index;
						(roleCountSideList = fuBenData.RoleCountSideList)[index = client.ClientData.CompType - 1] = roleCountSideList[index] + 1;
					}
				}
			}
		}

		
		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					CompFuBenData fuBenData;
					if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
					{
						List<int> roleCountSideList;
						int index;
						(roleCountSideList = fuBenData.RoleCountSideList)[index = client.ClientData.CompType - 1] = roleCountSideList[index] - 1;
						TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(31, client.ServerId, scene.SceneInfo.ID, client.ClientData.RoleID, (int)client.ClientData.CompZhiWu, 7);
					}
				}
			}
		}

		
		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						int addScore = 0;
						int addScoreDie = this.RuntimeData.CompMineDie;
						CompMineClientContextData clientLianShaContextData = client.SceneContextData2 as CompMineClientContextData;
						CompMineClientContextData otherLianShaContextData = other.SceneContextData2 as CompMineClientContextData;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						if (scene.GameId == clientLianShaContextData.BattleWhichSide || scene.GameId == otherLianShaContextData.BattleWhichSide)
						{
							huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
							huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
							huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
							huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
							if (null != clientLianShaContextData)
							{
								clientLianShaContextData.KillNum++;
								if (scene.GameId == clientLianShaContextData.BattleWhichSide)
								{
									clientLianShaContextData.KillNumLocal++;
								}
								int lianShaScore = this.RuntimeData.CompMineAttackKill[0] + clientLianShaContextData.KillNum * this.RuntimeData.CompMineAttackKill[1];
								lianShaScore = Math.Min(this.RuntimeData.CompMineAttackKill[3], Math.Max(this.RuntimeData.CompMineAttackKill[2], lianShaScore));
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
								int overScore = this.RuntimeData.CompMineAttackShutDown[0] + otherLianShaContextData.KillNum * this.RuntimeData.CompMineAttackShutDown[1];
								overScore = Math.Min(this.RuntimeData.CompMineAttackShutDown[3], Math.Max(this.RuntimeData.CompMineAttackShutDown[2], overScore));
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
						}
						if (addScore > 0)
						{
							clientLianShaContextData.BattleJiFen += addScore;
							TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 10, client.ClientData.RoleID, addScore);
							GameManager.ClientMgr.ModifyCompMineJiFenValue(client, addScore, "杀人", true, true, false);
						}
						if (addScoreDie > 0)
						{
							otherLianShaContextData.BattleJiFen += addScoreDie;
							TianTiClient.getInstance().Comp_CompOpt(other.ClientData.CompType, 10, other.ClientData.RoleID, addScoreDie);
							GameManager.ClientMgr.ModifyCompMineJiFenValue(other, addScoreDie, "被杀", true, true, false);
						}
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(2016, huanYingSiYuanLianSha, scene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(2017, huanYingSiYuanLianshaOver, scene.CopyMap);
						}
					}
				}
			}
		}

		
		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			CompMineScene scene = client.SceneObject as CompMineScene;
			CompMineTruckConfig tagInfo = monster.Tag as CompMineTruckConfig;
			if (tagInfo != null && null != scene)
			{
				Dictionary<int, CompConfig> tempCompConfigDict = null;
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					tempCompConfigDict = CompManager.getInstance().RuntimeData.CompConfigDict;
				}
				lock (this.RuntimeData.Mutex)
				{
					long TotalInjure = 0L;
					long[] TotalCompInjure = new long[3];
					foreach (KeyValuePair<int, CompMineClientContextData> kvp in scene.ClientContextDataDict)
					{
						CompMineClientContextData contex = kvp.Value;
						GameClient otherClient = GameManager.ClientMgr.FindClient(contex.RoleId);
						if (null != otherClient)
						{
							if (contex.BattleWhichSide != scene.GameId)
							{
								int addJiFen = (int)((double)contex.TruckInjure / tagInfo.BrokenNum[0]);
								addJiFen = Math.Max(addJiFen, (int)tagInfo.BrokenNum[1]);
								addJiFen = Math.Min(addJiFen, (int)tagInfo.BrokenNum[2]);
								if (addJiFen > 0)
								{
									contex.BattleJiFen += addJiFen;
									TianTiClient.getInstance().Comp_CompOpt(otherClient.ClientData.CompType, 10, otherClient.ClientData.RoleID, addJiFen);
									GameManager.ClientMgr.ModifyCompMineJiFenValue(otherClient, addJiFen, "矿车击碎", true, true, false);
								}
							}
						}
					}
					foreach (KeyValuePair<int, CompMineClientContextData> kvp in scene.ClientContextDataDict)
					{
						CompMineClientContextData contex = kvp.Value;
						TotalInjure += contex.TruckInjure;
						TotalCompInjure[contex.BattleWhichSide - 1] += contex.TruckInjure;
						contex.TruckInjure = 0L;
					}
					for (int compLoop = 1; compLoop <= 3; compLoop++)
					{
						if (compLoop != scene.GameId && TotalCompInjure[compLoop - 1] > 0L)
						{
							int addMineRes = (int)((double)tagInfo.CompMineNum * (double)TotalCompInjure[compLoop - 1] / (double)TotalInjure);
							if (addMineRes > 0)
							{
								TianTiClient.getInstance().Comp_CompOpt(compLoop, 9, addMineRes, 0);
								CompConfig compConfig;
								if (tempCompConfigDict.TryGetValue(compLoop, out compConfig))
								{
									string broadMsg = string.Format(GLang.GetLang(6002, new object[0]), compConfig.CompName, addMineRes);
									this.BroadCastImportantMsg(scene, 0, broadMsg);
								}
							}
							int addCompBoom = (int)((double)tagInfo.CompBoomNum * (double)TotalCompInjure[compLoop - 1] / (double)TotalInjure);
							if (addCompBoom > 0)
							{
								TianTiClient.getInstance().Comp_CompOpt(compLoop, 0, addCompBoom, 0);
								string broadMsg = string.Format(GLang.GetLang(4018, new object[0]), addCompBoom);
								this.BroadCastImportantMsg(scene, compLoop, broadMsg);
							}
						}
					}
					scene.ScoreData.MineTruckProcess = 0;
					scene.ScoreData.SuppliesNum = 0;
					scene.ScoreData.SuppliesStep = 0;
					this.UpdateBattleSideScoreRank(scene, true);
					if (scene.ScoreData.MineTruckGo < this.RuntimeData.CompMineTruckConfigList.Count)
					{
						CompMineTruckConfig truck = this.RuntimeData.CompMineTruckConfigList[scene.ScoreData.MineTruckGo];
						long addTicks = TimeUtil.NOW() + (long)(truck.TimePoints * 1000);
						if (addTicks < scene.m_lEndTime)
						{
							this.AddDelayCreateMonster(scene, addTicks, truck);
						}
					}
				}
			}
		}

		
		public void AddDelayCreateMonster(CompMineScene scene, long ticks, object monster)
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

		
		public void CheckCreateDynamicMonster(CompMineScene scene, long nowMs)
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
							if (obj is CompMineTruckConfig)
							{
								CompMineLinkConfig point = this.RuntimeData.CompMineLinkConfigList[0];
								CompMineTruckConfig item = obj as CompMineTruckConfig;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item.MonsterID, scene.CopyMapId, 1, point.PosX / 100, point.PosY / 100, 0, 0, SceneUIClasses.CompMine, item, null);
								scene.ScoreData.MineTruckProcess = 0;
								scene.ScoreData.MineTruckGo++;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<CompMineSideScore>(2014, scene.ScoreData, scene.CopyMap);
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

		
		private void InitCreateDynamicMonster(CompMineScene scene, long nowTicks)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineTruckConfig truck = this.RuntimeData.CompMineTruckConfigList[0];
				long addTicks = scene.m_lBeginTime + (long)(truck.TimePoints * 1000);
				this.AddDelayCreateMonster(scene, addTicks, truck);
			}
		}

		
		public bool ClientChangeMap(GameClient client, int teleportID, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
		{
			CompMineScene scene = client.SceneObject as CompMineScene;
			bool result;
			if (null == scene)
			{
				result = false;
			}
			else if (scene.GameId != client.ClientData.CompType)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(6004, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(6000, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		public void MonsterMoveStep(Monster monster)
		{
			long ticks = TimeUtil.NOW();
			CompMineTruckConfig tagInfo = monster.Tag as CompMineTruckConfig;
			if (null != tagInfo)
			{
				if ((double)(ticks - monster.MoveTime) >= tagInfo.Speed * 500.0)
				{
					Dictionary<int, CompConfig> tempCompConfigDict = null;
					lock (CompManager.getInstance().RuntimeData.Mutex)
					{
						tempCompConfigDict = CompManager.getInstance().RuntimeData.CompConfigDict;
					}
					lock (this.RuntimeData.Mutex)
					{
						CompMineScene scene = null;
						CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(monster.CopyMapID);
						if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out scene))
						{
							int nStep = monster.Step;
							int nNumStep = this.RuntimeData.CompMineLinkConfigList.Count<CompMineLinkConfig>() - 1;
							int nNextStep = nStep + 1;
							int oldMineTruckProcess = scene.ScoreData.MineTruckProcess;
							scene.ScoreData.MineTruckProcess = (int)((double)nNextStep / (double)nNumStep * 100.0);
							if (nNextStep >= nNumStep)
							{
								scene.ScoreData.SafeArrived++;
								scene.ScoreData.MineTruckProcess = 0;
								scene.ScoreData.SuppliesNum = 0;
								scene.ScoreData.SuppliesStep = 0;
								foreach (KeyValuePair<int, CompMineClientContextData> kvp in scene.ClientContextDataDict)
								{
									CompMineClientContextData contex = kvp.Value;
									GameClient client = GameManager.ClientMgr.FindClient(contex.RoleId);
									if (null != client)
									{
										if (contex.BattleWhichSide == scene.GameId && client.ClientData.MapCode == scene.m_nMapCode)
										{
											int addJiFen = (int)((double)contex.KillNumLocal * tagInfo.FinishNum[0]);
											addJiFen = Math.Max(addJiFen, (int)tagInfo.FinishNum[1]);
											addJiFen = Math.Min(addJiFen, (int)tagInfo.FinishNum[2]);
											if (addJiFen > 0)
											{
												contex.BattleJiFen += addJiFen;
												TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 10, client.ClientData.RoleID, addJiFen);
												GameManager.ClientMgr.ModifyCompMineJiFenValue(client, addJiFen, "矿车到达", true, true, false);
											}
										}
									}
								}
								foreach (KeyValuePair<int, CompMineClientContextData> kvp in scene.ClientContextDataDict)
								{
									CompMineClientContextData contex = kvp.Value;
									contex.TruckInjure = 0L;
								}
								TianTiClient.getInstance().Comp_CompOpt(scene.GameId, 9, tagInfo.CompMineNum, 0);
								CompConfig compConfig;
								if (tempCompConfigDict.TryGetValue(scene.GameId, out compConfig))
								{
									string broadMsg = string.Format(GLang.GetLang(6001, new object[0]), compConfig.CompName, tagInfo.CompMineNum);
									this.BroadCastImportantMsg(scene, 0, broadMsg);
								}
								if (tagInfo.CompBoomNum > 0)
								{
									TianTiClient.getInstance().Comp_CompOpt(scene.GameId, 0, tagInfo.CompBoomNum, 0);
									string broadMsg = string.Format(GLang.GetLang(4018, new object[0]), tagInfo.CompBoomNum);
									this.BroadCastImportantMsg(scene, scene.GameId, broadMsg);
								}
								this.UpdateBattleSideScoreRank(scene, true);
								GameManager.MonsterMgr.AddDelayDeadMonster(monster);
								monster.Tag = null;
								if (scene.ScoreData.MineTruckGo < this.RuntimeData.CompMineTruckConfigList.Count)
								{
									CompMineTruckConfig truck = this.RuntimeData.CompMineTruckConfigList[scene.ScoreData.MineTruckGo];
									long addTicks = ticks + (long)(truck.TimePoints * 1000);
									if (addTicks < scene.m_lEndTime)
									{
										this.AddDelayCreateMonster(scene, addTicks, truck);
									}
								}
							}
							else
							{
								int nNextX = this.RuntimeData.CompMineLinkConfigList[nNextStep].PosX;
								int nNextY = this.RuntimeData.CompMineLinkConfigList[nNextStep].PosY;
								int gridX = nNextX / scene.MapGridWidth;
								int gridY = nNextY / scene.MapGridHeight;
								Point ToGrid = new Point((double)gridX, (double)gridY);
								Point grid = monster.CurrentGrid;
								int nCurrX = (int)grid.X;
								int nCurrY = (int)grid.Y;
								double Direction = Global.GetDirectionByAspect(gridX, gridY, nCurrX, nCurrY);
								ChuanQiUtils.WalkTo(monster, (Dircetions)Direction);
								monster.MoveTime = ticks;
								if (Global.GetTwoPointDistance(ToGrid, grid) < 2.0)
								{
									if (nNextStep < this.RuntimeData.CompMineLinkConfigList.Count && this.RuntimeData.CompMineLinkConfigList[nNextStep].Supplies && scene.ScoreData.SuppliesNum < tagInfo.AddLIfe.Length && nNextStep > scene.ScoreData.SuppliesStep)
									{
										scene.ScoreData.SuppliesStep = nNextStep;
										double AddLIfe = (double)tagInfo.AddLIfe[scene.ScoreData.SuppliesNum];
										GameManager.MonsterMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, AddLIfe);
										LogManager.WriteLog(LogTypes.Analysis, string.Format("势力矿洞跨服副本GameID={0}, 矿车Step={1}, 次数={2}, 回血={3}", new object[]
										{
											scene.GameId,
											nNextStep,
											scene.ScoreData.SuppliesNum + 1,
											AddLIfe
										}), null, true);
										scene.ScoreData.SuppliesNum++;
									}
									monster.Step = nNextStep;
									LogManager.WriteLog(LogTypes.Analysis, string.Format("势力矿洞跨服副本GameID={0}, 矿车Step={1}", scene.GameId, nNextStep), null, true);
								}
								if (oldMineTruckProcess != scene.ScoreData.MineTruckProcess)
								{
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<CompMineSideScore>(2014, scene.ScoreData, scene.CopyMap);
								}
							}
						}
					}
				}
			}
		}

		
		private CompMineRewardConfig CalMineRewardConfig(GameClient client, KFCompRoleData compRoleData)
		{
			CompMineRewardConfig awardConfig = null;
			double rankPct = 0.0;
			lock (CompManager.getInstance().RuntimeData.Mutex)
			{
				if (CompManager.getInstance().CompSyncDataCache.CompMineJoinRoleNum.Length != 0)
				{
					int joinNum = CompManager.getInstance().CompSyncDataCache.CompMineJoinRoleNum[client.ClientData.CompType - 1];
					rankPct = ((joinNum > 0) ? ((double)compRoleData.MineRankNum / (double)joinNum) : 1.0);
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				foreach (CompMineRewardConfig item in this.RuntimeData.CompMineRewardConfigList)
				{
					if (item.Rank > 0 && item.Rank == compRoleData.MineRankNum)
					{
						awardConfig = item;
						break;
					}
					if (item.RankRate > 0.0 && rankPct <= item.RankRate)
					{
						awardConfig = item;
						break;
					}
				}
			}
			return awardConfig;
		}

		
		private int GiveRoleAwards(GameClient client, long LastStartTimeTicks)
		{
			KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
			int result;
			if (null == compRoleData)
			{
				result = -11003;
			}
			else
			{
				CompMineRewardConfig awardConfig = this.CalMineRewardConfig(client, compRoleData);
				if (null == awardConfig)
				{
					result = -3;
				}
				else
				{
					Dictionary<int, KFCompData> tempCompDataDict = null;
					lock (CompManager.getInstance().RuntimeData.Mutex)
					{
						tempCompDataDict = CompManager.getInstance().CompSyncDataCache.CompDataDict.V;
					}
					KFCompData compData;
					if (!tempCompDataDict.TryGetValue(client.ClientData.CompType, out compData))
					{
						result = -3;
					}
					else
					{
						double rewardRate = 1.0;
						if (compData.MineRank > 0 && compData.MineRank <= this.RuntimeData.CompMineAwardNum.Length)
						{
							lock (this.RuntimeData.Mutex)
							{
								rewardRate = this.RuntimeData.CompMineAwardNum[compData.MineRank - 1];
							}
						}
						int addGrade = (int)((double)awardConfig.Grade * rewardRate);
						int addContribution = (int)((double)awardConfig.Contribution * rewardRate);
						List<AwardsItemData> awardsItemDataListOne = awardConfig.AwardsItemListOne.Items;
						List<AwardsItemData> awardsItemDataListTwo = awardConfig.AwardsItemListTwo.Items;
						int awardCnt = 0;
						if (awardsItemDataListOne != null)
						{
							awardCnt += awardsItemDataListOne.Count;
						}
						if (awardsItemDataListTwo != null)
						{
							awardCnt += awardsItemDataListTwo.Count((AwardsItemData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
						}
						if (!Global.CanAddGoodsNum(client, awardCnt))
						{
							result = -100;
						}
						else
						{
							if (addGrade > 0 && CompManager.getInstance().CheckCanAddJunXian(LastStartTimeTicks))
							{
								TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 1, client.ClientData.RoleID, addGrade);
								string broadMsg = string.Format(GLang.GetLang(4017, new object[0]), addGrade);
								GameManager.ClientMgr.NotifyImportantMsg(client, broadMsg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
							if (addContribution > 0)
							{
								GameManager.ClientMgr.ModifyCompDonateValue(client, addContribution, "势力矿洞奖励", true, true, false);
							}
							if (awardsItemDataListOne != null)
							{
								foreach (AwardsItemData item in awardsItemDataListOne)
								{
									int GoodsNum = (int)((double)item.GoodsNum * rewardRate);
									if (GoodsNum > 0)
									{
										Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "势力矿洞奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
									}
								}
							}
							if (awardsItemDataListTwo != null)
							{
								foreach (AwardsItemData item in awardsItemDataListTwo)
								{
									if (Global.IsCanGiveRewardByOccupation(client, item.GoodsID))
									{
										int GoodsNum = (int)((double)item.GoodsNum * rewardRate);
										if (GoodsNum > 0)
										{
											Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "势力矿洞奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
										}
									}
								}
							}
							result = 1;
						}
					}
				}
			}
			return result;
		}

		
		private void BroadCastImportantMsg(CompMineScene scene, int compType, string msg)
		{
			List<GameClient> objsList = scene.CopyMap.GetClientsList();
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i];
					if (c != null && (compType <= 0 || c.ClientData.CompType == compType))
					{
						GameManager.ClientMgr.NotifyImportantMsg(c, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
			}
		}

		
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= CompMineManager.NextHeartBeatTicks)
			{
				CompMineManager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (CompMineScene scene in this.SceneDict.Values)
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
									scene.StateTimeData.GameType = 31;
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
									scene.StateTimeData.GameType = 31;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									for (int guangMuId = 1; guangMuId <= 3; guangMuId++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, guangMuId, 0);
									}
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime)
								{
									this.ProcessEnd(scene, ticks);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
								scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(31, -1, scene.SceneInfo.ID, -1, -1, 6);
								this.GiveAwards(scene);
								CompFuBenData fuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
								{
									fuBenData.State = GameFuBenState.End;
									LogManager.WriteLog(LogTypes.Error, string.Format("势力矿洞跨服副本GameID={0},战斗结束", fuBenData.GameId), null, true);
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
										DataHelper.WriteExceptionLogEx(ex, "势力矿洞系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		
		public const SceneUIClasses ManagerType = SceneUIClasses.CompMine;

		
		private static CompMineManager instance = new CompMineManager();

		
		public CompMineRuntimeData RuntimeData = new CompMineRuntimeData();

		
		public ConcurrentDictionary<int, CompMineScene> SceneDict = new ConcurrentDictionary<int, CompMineScene>();

		
		private static long NextHeartBeatTicks = 0L;
	}
}
