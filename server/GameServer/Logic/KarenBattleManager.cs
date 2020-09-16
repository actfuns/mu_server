using System;
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
	
	public class KarenBattleManager : IManager, IManager2, ICmdProcessorEx, ICmdProcessor, IEventListenerEx
	{
		
		public static KarenBattleManager getInstance()
		{
			return KarenBattleManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KarenBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1210, 2, 2, KarenBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1212, 1, 1, KarenBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, KarenBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(25, 10000, KarenBattleManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, KarenBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(25, 10000, KarenBattleManager.getInstance());
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
			case 1210:
				return this.ProcessKarenBattleEnterCmd(client, nID, bytes, cmdParams);
			case 1212:
				return this.ProcessGetKarenBattleStateCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 24:
			{
				PreBangHuiRemoveMemberEventObject e = eventObject as PreBangHuiRemoveMemberEventObject;
				if (null != e)
				{
					eventObject.Handled = this.OnPreBangHuiRemoveMember(e);
				}
				break;
			}
			case 25:
			{
				PreBangHuiChangeZhiWuEventObject e2 = eventObject as PreBangHuiChangeZhiWuEventObject;
				if (null != e2)
				{
					eventObject.Handled = this.OnPreBangHuiChangeZhiWu(e2);
				}
				break;
			}
			}
		}

		
		public bool LoadKarenPublicConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.Mutex)
			{
				try
				{
					this.SceneDataDict.Clear();
					fileName = "Config/LegionsWar.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KarenBattleSceneInfo sceneItem = new KarenBattleSceneInfo();
						int id = (int)Global.GetSafeAttributeLong(node, "ID");
						int mapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						sceneItem.Id = id;
						sceneItem.MapCode = mapCode;
						sceneItem.MaxLegions = (int)Global.GetSafeAttributeLong(node, "LegionsMax");
						sceneItem.MaxEnterNum = (int)Global.GetSafeAttributeLong(node, "MaxEnterNum");
						sceneItem.EnterCD = (int)Global.GetSafeAttributeLong(node, "EnterCD");
						sceneItem.PrepareSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
						sceneItem.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
						sceneItem.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
						sceneItem.Exp = Global.GetSafeAttributeLong(node, "Exp");
						sceneItem.BandJinBi = (int)Global.GetSafeAttributeLong(node, "BandJinBi");
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
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "WinGoods"), ref sceneItem.WinAwardsItemList, '|', ',');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "LoseGoods"), ref sceneItem.LoseAwardsItemList, '|', ',');
						this.SceneDataDict[mapCode] = sceneItem;
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

		
		public bool InitConfig()
		{
			lock (this.Mutex)
			{
				this.KarenBattleEnterMap.Clear();
				List<string> enterMapList = GameManager.systemParamsList.GetParamValueStringListByName("LegionsWarEnterMap", ',');
				if (null != enterMapList)
				{
					foreach (string item in enterMapList)
					{
						this.KarenBattleEnterMap.Add(Global.SafeConvertToInt32(item));
					}
				}
			}
			return this.LoadKarenPublicConfig();
		}

		
		public bool ProcessKarenBattleEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd<int>(nID, result, false);
					return true;
				}
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int mapCode = Global.SafeConvertToInt32(cmdParams[1]);
				KarenBattleSceneInfo sceneItem = null;
				KarenGameStates state = KarenGameStates.None;
				int eastcount = 0;
				int westcount = 0;
				JunTuanRankData rankData = this.GetJunTuanRankDataByClient(client);
				if (rankData == null || !this.CheckCanEnterKarenBattle(client))
				{
					result = -5;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						0,
						westcount,
						eastcount
					}), false);
					return true;
				}
				if (!this.CheckMap(client))
				{
					result = -21;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						0,
						westcount,
						eastcount
					}), false);
					return true;
				}
				result = this.CheckTimeCondition(ref state);
				if (state != KarenGameStates.Start)
				{
					result = -2001;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						0,
						westcount,
						eastcount
					}), false);
					return true;
				}
				lock (this.Mutex)
				{
					if (!this.SceneDataDict.TryGetValue(mapCode, out sceneItem))
					{
						result = -5;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							0,
							westcount,
							eastcount
						}), false);
						return true;
					}
					foreach (KeyValuePair<int, KarenBattleSceneInfo> item in this.SceneDataDict)
					{
						KarenFuBenData fbData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(item.Key);
						if (null != fbData)
						{
							SceneUIClasses sType = Global.GetMapSceneType(item.Value.MapCode);
							if (sType == SceneUIClasses.KarenWest)
							{
								westcount = fbData.GetRoleCountWithEnter(rankData.Rank);
							}
							else
							{
								eastcount = fbData.GetRoleCountWithEnter(rankData.Rank);
							}
						}
					}
					DateTime lastEnterTime = Global.GetRoleParamsDateTimeFromDB(client, "20019");
					if (!this.GMTest && TimeUtil.NowDateTime().Ticks - lastEnterTime.Ticks < 10000000L * (long)sceneItem.EnterCD)
					{
						GameManager.ClientMgr.NotifyImportantMsg(client, string.Format(GLang.GetLang(2615, new object[0]), sceneItem.EnterCD), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -2007;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							0,
							westcount,
							eastcount
						}), false);
						return true;
					}
					KuaFuServerInfo kfserverInfo = null;
					KarenFuBenData fubenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(mapCode);
					if (fubenData == null || !KuaFuManager.getInstance().TryGetValue(fubenData.ServerId, out kfserverInfo))
					{
						result = -11000;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							0,
							westcount,
							eastcount
						}), false);
						return true;
					}
					if (fubenData.GetRoleCountWithEnter(rankData.Rank) >= sceneItem.MaxEnterNum)
					{
						result = -22;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							0,
							westcount,
							eastcount
						}), false);
						return true;
					}
					SceneUIClasses sceneType = Global.GetMapSceneType(sceneItem.MapCode);
					KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
					if (null != clientKuaFuServerLoginData)
					{
						clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
						clientKuaFuServerLoginData.GameId = (long)fubenData.GameId;
						clientKuaFuServerLoginData.GameType = fubenData.GameType;
						clientKuaFuServerLoginData.EndTicks = fubenData.EndTime.Ticks;
						clientKuaFuServerLoginData.ServerId = client.ServerId;
						clientKuaFuServerLoginData.ServerIp = kfserverInfo.Ip;
						clientKuaFuServerLoginData.ServerPort = kfserverInfo.Port;
						clientKuaFuServerLoginData.FuBenSeqId = 0;
					}
					if (result >= 0)
					{
						result = JunTuanClient.getInstance().GameFuBenRoleChangeState(client.ServerId, client.ClientData.RoleID, (int)clientKuaFuServerLoginData.GameId, rankData.Rank, 4);
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
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					0,
					westcount,
					eastcount
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGetKarenBattleStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int westcount = 0;
				int eastcount = 0;
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						westcount,
						eastcount,
						"",
						""
					}), false);
					return true;
				}
				KarenGameStates timeState = KarenGameStates.None;
				this.CheckTimeCondition(ref timeState);
				if (this.CheckCanEnterKarenBattle(client))
				{
					if (timeState == KarenGameStates.Wait)
					{
						result = 1;
					}
					else if (timeState == KarenGameStates.Start)
					{
						lock (this.Mutex)
						{
							foreach (KeyValuePair<int, KarenBattleSceneInfo> item in this.SceneDataDict)
							{
								KarenFuBenData fubenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(item.Key);
								if (null != fubenData)
								{
									JunTuanRankData rankData = this.GetJunTuanRankDataByClient(client);
									if (null != rankData)
									{
										SceneUIClasses sceneType = Global.GetMapSceneType(item.Value.MapCode);
										if (sceneType == SceneUIClasses.KarenWest)
										{
											westcount = fubenData.GetRoleCountWithEnter(rankData.Rank);
										}
										else
										{
											eastcount = fubenData.GetRoleCountWithEnter(rankData.Rank);
										}
									}
								}
							}
						}
						result = 2;
					}
				}
				else if (timeState == KarenGameStates.Wait || timeState == KarenGameStates.Start)
				{
					JunTuanRankData RankData = this.GetJunTuanRankDataByClient(client);
					if (null != RankData)
					{
						result = 3;
					}
					else
					{
						result = 4;
					}
				}
				string eastjtname = "";
				string westjtname = "";
				List<LingDiData> LingDiList = JunTuanClient.getInstance().GetLingDiData();
				if (null != LingDiList)
				{
					foreach (LingDiData item2 in LingDiList)
					{
						SceneUIClasses mapType = this.ConvertCaiJiLingDiTypeToMapSceneType(item2.LingDiType);
						if (mapType == SceneUIClasses.KarenWest)
						{
							westjtname = item2.JunTuanName;
						}
						else if (mapType == SceneUIClasses.KarenEast)
						{
							eastjtname = item2.JunTuanName;
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					result,
					westcount,
					eastcount,
					westjtname,
					eastjtname
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private SceneUIClasses ConvertCaiJiLingDiTypeToMapSceneType(int lingdiType)
		{
			SceneUIClasses result;
			if (lingdiType == 0)
			{
				result = SceneUIClasses.KarenWest;
			}
			else if (lingdiType == 1)
			{
				result = SceneUIClasses.KarenEast;
			}
			else
			{
				result = SceneUIClasses.Normal;
			}
			return result;
		}

		
		private int ConvertMapSceneTypeToCaiJiLingDiType(SceneUIClasses mapsceneType)
		{
			int result;
			if (mapsceneType == SceneUIClasses.KarenWest)
			{
				result = 0;
			}
			else if (mapsceneType == SceneUIClasses.KarenEast)
			{
				result = 1;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		
		public JunTuanRankData GetJunTuanRankDataBySide(int Side)
		{
			List<JunTuanRankData> RankDataList = JunTuanClient.getInstance().GetJunTuanRankingData();
			JunTuanRankData result;
			if (null == RankDataList)
			{
				result = null;
			}
			else
			{
				KarenBattleSceneInfo sceneItem = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
				if (Side <= 0 || Side > RankDataList.Count || Side > sceneItem.MaxLegions)
				{
					result = null;
				}
				else
				{
					result = RankDataList[Side - 1];
				}
			}
			return result;
		}

		
		public JunTuanRankData GetJunTuanRankDataByClient(GameClient client)
		{
			List<JunTuanRankData> RankDataList = JunTuanClient.getInstance().GetJunTuanRankingData();
			JunTuanRankData result;
			if (null == RankDataList)
			{
				result = null;
			}
			else
			{
				KarenBattleSceneInfo sceneItem = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
				if (RankDataList.Count > sceneItem.MaxLegions)
				{
					RankDataList = new List<JunTuanRankData>(RankDataList.GetRange(0, sceneItem.MaxLegions));
				}
				result = RankDataList.Find((JunTuanRankData x) => x.JunTuanId == client.ClientData.JunTuanId);
			}
			return result;
		}

		
		private bool CheckCanEnterKarenBattle(GameClient client)
		{
			bool result;
			if (client == null || client.ClientData.Faction == 0 || 0 == client.ClientData.JunTuanId)
			{
				result = false;
			}
			else if (client.ClientData.JunTuanZhiWu == 0 || 4 == client.ClientData.JunTuanZhiWu)
			{
				result = false;
			}
			else
			{
				JunTuanRankData RankData = this.GetJunTuanRankDataByClient(client);
				result = (null != RankData);
			}
			return result;
		}

		
		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			KarenFuBenRoleData kroleData = JunTuanClient.getInstance().GetKarenFuBenRoleData((int)kuaFuServerLoginData.GameId, kuaFuServerLoginData.RoleId);
			bool result;
			if (kroleData == null || (long)kroleData.KuaFuMapCode != kuaFuServerLoginData.GameId || kroleData.KuaFuServerId != GameManager.ServerId)
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

		
		public void OnInitGame(SceneUIClasses ManagerType, GameClient client)
		{
			lock (this.Mutex)
			{
				this.FactionIDVsServerIDDict[client.ClientData.Faction] = client.ServerId;
			}
			Global.SaveRoleParamsDateTimeToDB(client, "20019", TimeUtil.NowDateTime(), true);
			EventLogManager.AddKarenBattleEnterEvent(this.ConvertMapSceneTypeToCaiJiLingDiType(ManagerType), client);
		}

		
		private void TimerProc(object sender, EventArgs e)
		{
			bool notifyPrepareGame = false;
			bool notifyEnterGame = false;
			bool notifyEndGame = false;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.Mutex)
			{
				KarenBattleSceneInfo sceneItem = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - 120.0 && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1] + 120.0)
					{
						double secsPrepare = sceneItem.SecondsOfDay[i] - now.TimeOfDay.TotalSeconds;
						double secsEnd = sceneItem.SecondsOfDay[i + 1] + 120.0 - now.TimeOfDay.TotalSeconds;
						if (!this.PrepareGame)
						{
							if (secsPrepare > 0.0 && secsPrepare < 120.0)
							{
								this.PrepareGame = true;
								notifyPrepareGame = true;
								break;
							}
						}
						else if (secsPrepare < 0.0)
						{
							notifyEnterGame = true;
							this.PrepareGame = false;
							break;
						}
						if (!this.EndGame)
						{
							if (secsEnd > 0.0 && secsEnd < 120.0)
							{
								this.EndGame = true;
								notifyEndGame = true;
								break;
							}
						}
						else if (secsEnd < 0.0)
						{
							this.EndGame = false;
							break;
						}
					}
				}
			}
			if (notifyPrepareGame)
			{
				LogManager.WriteLog(LogTypes.Error, "阿卡伦战场活动即将开始,准备通知军团角色进入!", null, true);
				lock (this.Mutex)
				{
					this.FactionIDVsServerIDDict.Clear();
				}
			}
			if (notifyEnterGame)
			{
				int westcount = 0;
				int eastcount = 0;
				lock (this.Mutex)
				{
					LogManager.WriteLog(LogTypes.Error, "阿卡伦战场开启,可以通知已分配到场次的玩家进入游戏了", null, true);
					KarenBattleSceneInfo sceneItem = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
					foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
					{
						if (this.CheckCanEnterKarenBattle(client) && this.CheckMap(client) && this.IsGongNengOpened(client, false))
						{
							if (null != client)
							{
								client.sendCmd(1210, string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									1,
									1,
									westcount,
									eastcount
								}), false);
							}
						}
					}
				}
			}
			if (notifyEndGame)
			{
				lock (this.Mutex)
				{
					foreach (KarenBattleSceneInfo item in this.SceneDataDict.Values)
					{
						KarenFuBenData fubenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(item.MapCode);
						if (fubenData != null && fubenData.ServerId == GameManager.ServerId)
						{
							SceneUIClasses mapType = Global.GetMapSceneType(item.MapCode);
							if (mapType != SceneUIClasses.KarenEast || KarenBattleManager_MapEast.getInstance().SceneDict.Count == 0)
							{
								if (mapType != SceneUIClasses.KarenWest || KarenBattleManager_MapWest.getInstance().SceneDict.Count == 0)
								{
									int lingDiType = this.ConvertMapSceneTypeToCaiJiLingDiType(mapType);
									LingDiData oldLingDiData = null;
									List<LingDiData> LingDiList = JunTuanClient.getInstance().GetLingDiData();
									if (null != LingDiList)
									{
										oldLingDiData = LingDiList.Find((LingDiData x) => x.LingDiType == lingDiType);
									}
									RoleData4Selector oldLeader = (oldLingDiData != null && oldLingDiData.RoleData != null) ? DataHelper.BytesToObject<RoleData4Selector>(oldLingDiData.RoleData, 0, oldLingDiData.RoleData.Length) : null;
									LingDiCaiJiManager.getInstance().SetLingZhu(lingDiType, 0, 0, "", null);
									EventLogManager.AddKarenBattleEvent(lingDiType, oldLeader, 0, 0, 0);
								}
							}
						}
					}
				}
			}
			this.UpdateKuaFuMapClientCount(KarenBattleManager_MapWest.getInstance().SceneDict.Values.FirstOrDefault<KarenBattleScene>());
			this.UpdateKuaFuMapClientCount(KarenBattleManager_MapEast.getInstance().SceneDict.Values.FirstOrDefault<KarenBattleScene>());
		}

		
		private void UpdateKuaFuMapClientCount(KarenBattleScene scene)
		{
			if (null != scene)
			{
				CopyMap copyMap = scene.CopyMap;
				if (null != copyMap)
				{
					KarenBattleSceneInfo sceneItem = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
					List<int> mapClientCountList = new List<int>(new int[sceneItem.MaxLegions]);
					List<GameClient> objsList = copyMap.GetClientsList();
					if (objsList != null && objsList.Count > 0)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient c = objsList[i];
							if (c != null)
							{
								int side = c.ClientData.BattleWhichSide;
								if (side > 0 && side < mapClientCountList.Count)
								{
									List<int> list;
									int index;
									(list = mapClientCountList)[index = side - 1] = list[index] + 1;
								}
							}
						}
					}
					JunTuanClient.getInstance().UpdateKuaFuMapClientCount(scene.GameId, mapClientCountList);
				}
			}
		}

		
		public bool InActivityTime()
		{
			KarenGameStates timeState = KarenGameStates.None;
			this.CheckTimeCondition(ref timeState);
			return timeState == KarenGameStates.Start;
		}

		
		private int CheckTimeCondition(ref KarenGameStates state)
		{
			int result = 0;
			KarenBattleSceneInfo sceneItem = null;
			lock (this.Mutex)
			{
				sceneItem = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
				if (null == sceneItem)
				{
					return -12;
				}
			}
			result = -2001;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.Mutex)
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days)
					{
						if (now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
						{
							state = KarenGameStates.Start;
							result = 1;
						}
						else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i])
						{
							state = KarenGameStates.Wait;
							result = 1;
						}
						else
						{
							state = KarenGameStates.None;
							result = -2001;
						}
						break;
					}
				}
			}
			return result;
		}

		
		public KarenBattleSceneInfo TryGetKarenBattleSceneInfoBySceneType(SceneUIClasses SceneType)
		{
			foreach (KeyValuePair<int, KarenBattleSceneInfo> item in this.SceneDataDict)
			{
				if (Global.GetMapSceneType(item.Value.MapCode) == SceneType)
				{
					return item.Value;
				}
			}
			return null;
		}

		
		public KarenBattleSceneInfo TryGetKarenBattleSceneInfo(int MapCode)
		{
			KarenBattleSceneInfo sceneItem = null;
			this.SceneDataDict.TryGetValue(MapCode, out sceneItem);
			return sceneItem;
		}

		
		public KarenBattleSceneInfo TryGetKarenBattleSceneInfoByBattleID(int BattleID)
		{
			foreach (KeyValuePair<int, KarenBattleSceneInfo> item in this.SceneDataDict)
			{
				if (item.Value.Id == BattleID)
				{
					return item.Value;
				}
			}
			return null;
		}

		
		public TimeSpan GetStartTime(int sceneId)
		{
			KarenBattleSceneInfo sceneItem = null;
			TimeSpan startTime = TimeSpan.MinValue;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.Mutex)
			{
				if (!this.SceneDataDict.TryGetValue(sceneId, out sceneItem))
				{
					goto IL_13C;
				}
			}
			lock (this.Mutex)
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
			IL_13C:
			if (startTime < TimeSpan.Zero)
			{
				startTime = now.TimeOfDay;
			}
			return startTime;
		}

		
		public void GiveAwards(KarenBattleScene scene)
		{
			try
			{
				foreach (KarenBattleClientContextData contextData in scene.ClientContextDataDict.Values)
				{
					int success;
					if (contextData.BattleWhichSide == scene.SuccessSide)
					{
						success = 1;
					}
					else
					{
						success = 0;
					}
					GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleId);
					string awardsInfo = string.Format("{0},{1}", scene.SceneInfo.Id, success);
					if (client != null && client.ClientData.MapCode == scene.m_nMapCode)
					{
						this.NtfCanGetAward(client, success, scene.SceneInfo);
						this.GiveRoleAwards(client, success, scene.SceneInfo);
					}
				}
				this.PushGameResultData(scene);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "阿卡伦系统清场调度异常");
			}
		}

		
		public void PushGameResultData(KarenBattleScene scene)
		{
			JunTuanRankData rankData = this.GetJunTuanRankDataBySide(scene.SuccessSide);
			if (null != rankData)
			{
				JunTuanBaseData baseData = JunTuanManager.getInstance().GetJunTuanBaseDataByJunTuanID(rankData.JunTuanId);
				if (null == baseData)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("无法获取军团基本信息 JunTuanId={0}", rankData.JunTuanId), null, true);
				}
				else if (baseData.BhList == null || baseData.BhList.Count == 0)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("军团基本信息BhList为空 JunTuanId={0}", rankData.JunTuanId), null, true);
				}
				else
				{
					int leaderBangHui = baseData.BhList[0];
					int leaderServerID = 0;
					SceneUIClasses mapType = Global.GetMapSceneType(scene.m_nMapCode);
					int lingDiType = this.ConvertMapSceneTypeToCaiJiLingDiType(mapType);
					LingDiData oldLingDiData = null;
					List<LingDiData> LingDiList = JunTuanClient.getInstance().GetLingDiData();
					if (null != LingDiList)
					{
						oldLingDiData = LingDiList.Find((LingDiData x) => x.LingDiType == lingDiType);
					}
					RoleData4Selector oldLeader = (oldLingDiData != null && oldLingDiData.RoleData != null) ? DataHelper.BytesToObject<RoleData4Selector>(oldLingDiData.RoleData, 0, oldLingDiData.RoleData.Length) : null;
					lock (this.Mutex)
					{
						if (!this.FactionIDVsServerIDDict.TryGetValue(leaderBangHui, out leaderServerID))
						{
							JunTuanData data = JunTuanClient.getInstance().GetJunTuanData(leaderBangHui, rankData.JunTuanId, true);
							if (null == data)
							{
								LogManager.WriteLog(LogTypes.Fatal, string.Format("无法获取JunTuanData BangHuiID={0} JunTuanId={1}", leaderBangHui, rankData.JunTuanId), null, true);
								return;
							}
							LingDiCaiJiManager.getInstance().SetLingZhu(lingDiType, data.LeaderRoleId, rankData.JunTuanId, rankData.JunTuanName, null);
							EventLogManager.AddKarenBattleEvent(lingDiType, oldLeader, data.LeaderZoneId, rankData.JunTuanId, data.LeaderRoleId);
							return;
						}
					}
					BangHuiDetailData bhData = Global.GetBangHuiDetailData(-1, leaderBangHui, leaderServerID);
					if (null == bhData)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("无法获取帮会详细信息 BangHuiID={0} ServerID={1}", leaderBangHui, leaderServerID), null, true);
					}
					else
					{
						RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, bhData.BZRoleID), leaderServerID);
						if (dbRd == null || dbRd.RoleID <= 0)
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("无法获取帮主详细信息 BangHuiID={0} BZRoleID={1} ServerID={2}", leaderBangHui, bhData.BZRoleID, leaderServerID), null, true);
						}
						else
						{
							JunTuanManager.getInstance().OnInitGame(dbRd);
							RoleData4Selector leaderShowInfo = Global.RoleDataEx2RoleData4Selector(dbRd);
							LingDiCaiJiManager.getInstance().SetLingZhu(lingDiType, dbRd.RoleID, rankData.JunTuanId, rankData.JunTuanName, leaderShowInfo);
							EventLogManager.AddKarenBattleEvent(lingDiType, oldLeader, dbRd.ZoneID, rankData.JunTuanId, dbRd.RoleID);
						}
					}
				}
			}
		}

		
		public void NtfKarenNotifyMsg(KarenBattleScene scene, KarenNotifyMsgType index, int LegionID, string param1, string param2)
		{
			KarenNotifyMsg msg = new KarenNotifyMsg();
			msg.index = (int)index;
			msg.LegionID = LegionID;
			msg.param1 = param1;
			msg.param2 = param2;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<KarenNotifyMsg>(1214, msg, scene.CopyMap);
		}

		
		private void NtfCanGetAward(GameClient client, int success, KarenBattleSceneInfo sceneInfo)
		{
			long addExp = Global.GetExpMultiByZhuanShengExpXiShu(client, sceneInfo.Exp);
			int addBindJinBi = sceneInfo.BandJinBi;
			List<AwardsItemData> awardsItemDataList;
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
			client.sendCmd<KarenBattleAwardsData>(1211, new KarenBattleAwardsData
			{
				Exp = addExp,
				BindJinBi = addBindJinBi,
				Success = success,
				AwardGoodsDataList = Global.ConvertToGoodsDataList(awardsItemDataList, -1)
			}, false);
		}

		
		private int GiveRoleAwards(GameClient client, int success, KarenBattleSceneInfo sceneInfo)
		{
			long addExp = 0L;
			int addBindJinBi = 0;
			addExp = Global.GetExpMultiByZhuanShengExpXiShu(client, sceneInfo.Exp);
			addBindJinBi = sceneInfo.BandJinBi;
			List<AwardsItemData> awardsItemDataList;
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
			string sSubject = "阿卡伦战场奖励";
			SceneUIClasses mapType = Global.GetMapSceneType(sceneInfo.MapCode);
			string sContent;
			if (SceneUIClasses.KarenWest == mapType)
			{
				sContent = GLang.GetLang(2617, new object[0]);
			}
			else
			{
				sContent = GLang.GetLang(2618, new object[0]);
			}
			if (awardsItemDataList != null && !Global.CanAddGoodsNum(client, awardsItemDataList.Count))
			{
				Global.UseMailGivePlayerAward2(client, awardsItemDataList, GLang.GetLang(2616, new object[0]), sContent, 0, 0, 0);
			}
			else if (awardsItemDataList != null)
			{
				foreach (AwardsItemData item in awardsItemDataList)
				{
					Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, sSubject, "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
				}
			}
			if (addExp > 0L)
			{
				GameManager.ClientMgr.ProcessRoleExperience(client, addExp, true, true, false, "none");
			}
			if (addBindJinBi > 0)
			{
				GameManager.ClientMgr.AddMoney1(client, addBindJinBi, sSubject, true);
			}
			return 1;
		}

		
		private bool CheckMap(GameClient client)
		{
			lock (this.Mutex)
			{
				if (!this.KarenBattleEnterMap.Contains(client.ClientData.MapCode))
				{
					return false;
				}
			}
			return true;
		}

		
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7) && !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8) && JunTuanManager.getInstance().IsGongNengOpened(client, hint);
		}

		
		public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
		{
			bool result;
			if (e.Player.ClientData.JunTuanId > 0 && this.InActivityTime())
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(2619, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool OnPreBangHuiChangeZhiWu(PreBangHuiChangeZhiWuEventObject e)
		{
			bool result;
			if (e.Player.ClientData.JunTuanId > 0 && this.InActivityTime() && e.TargetZhiWu == 1)
			{
				e.ErrorCode = -201;
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(2620, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public DateTime GetLastStartTime(int sceneId)
		{
			DateTime ret = DateTime.MaxValue;
			KarenBattleSceneInfo sceneItem = null;
			TimeSpan startTime = TimeSpan.MinValue;
			DateTime now = TimeUtil.NowDateTime();
			int minusDay = 0;
			lock (this.Mutex)
			{
				if (!this.SceneDataDict.TryGetValue(sceneId, out sceneItem))
				{
					return DateTime.MaxValue;
				}
			}
			lock (this.Mutex)
			{
				int i = 0;
				if (i < sceneItem.TimePoints.Count - 1)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days)
					{
						if (now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
						{
							minusDay = 7;
						}
					}
					else
					{
						minusDay = now.DayOfWeek - (DayOfWeek)sceneItem.TimePoints[i].Days;
						if (minusDay < 0)
						{
							minusDay += 7;
						}
					}
					ret = now.AddDays((double)(-(double)minusDay)).Date.Add(TimeSpan.FromSeconds(sceneItem.SecondsOfDay[i + 1]));
				}
			}
			return ret;
		}

		
		public const SceneUIClasses ManagerType = SceneUIClasses.Normal;

		
		public bool GMTest = false;

		
		public object Mutex = new object();

		
		private bool PrepareGame = false;

		
		private bool EndGame = false;

		
		private static KarenBattleManager instance = new KarenBattleManager();

		
		public Dictionary<int, KarenBattleSceneInfo> SceneDataDict = new Dictionary<int, KarenBattleSceneInfo>();

		
		public HashSet<int> KarenBattleEnterMap = new HashSet<int>();

		
		public Dictionary<int, int> FactionIDVsServerIDDict = new Dictionary<int, int>();
	}
}
