using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic.Copy;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.MoRi;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using KF.TcpCall;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class KuaFuManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		
		static KuaFuManager()
		{
			AsyncDataItem.InitKnownTypes();
		}

		
		public static KuaFuManager getInstance()
		{
			return KuaFuManager.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			try
			{
				this.CoreInterface = coreInterface;
				if (!this.InitConfig())
				{
					return false;
				}
				RemotingConfiguration.Configure(Process.GetCurrentProcess().MainModule.FileName + ".config", false);
				if (!HuanYingSiYuanClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!TianTiClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!YongZheZhanChangClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!KFCopyRpcClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!SpreadClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!AllyClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!IPStatisticsClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!JunTuanClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!KuaFuWorldClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				GlobalEventSource.getInstance().registerListener(12, KuaFuManager.getInstance());
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		
		public bool startup()
		{
			try
			{
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("HuanYingSiYuanClient.TimerProc", new EventHandler(HuanYingSiYuanClient.getInstance().TimerProc)), 2000, 2857);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("TianTiClient.TimerProc", new EventHandler(TianTiClient.getInstance().TimerProc)), 2000, 2857);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("YongZheZhanChangClient.TimerProc", new EventHandler(YongZheZhanChangClient.getInstance().TimerProc)), 2000, 3389);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KFCopyRpcClient.TimerProc", new EventHandler(KFCopyRpcClient.getInstance().TimerProc)), 2000, 2732);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("SpreadClient.TimerProc", new EventHandler(SpreadClient.getInstance().TimerProc)), 2000, 4285);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("AllyClient.TimerProc", new EventHandler(AllyClient.getInstance().TimerProc)), 2000, 5714);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("IPStatisticsClient.TimerProc", new EventHandler(IPStatisticsClient.getInstance().TimerProc)), 2000, 5000);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("JunTuanClient.TimerProc", new EventHandler(JunTuanClient.getInstance().TimerProc)), 2000, 2500);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KuaFuWorldClient.TimerProc", new EventHandler(KuaFuWorldClient.getInstance().TimerProc)), 2000, 3389);
				lock (this.RuntimeData.Mutex)
				{
					if (null == this.RuntimeData.BackGroundThread)
					{
						this.RuntimeData.BackGroundThread = new Thread(new ThreadStart(this.BackGroudThreadProc));
						this.RuntimeData.BackGroundThread.IsBackground = true;
						this.RuntimeData.BackGroundThread.Start();
					}
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		
		public bool showdown()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.BackGroundThread.Abort();
					this.RuntimeData.BackGroundThread = null;
				}
				GlobalEventSource.getInstance().removeListener(12, KuaFuManager.getInstance());
				if (!HuanYingSiYuanClient.getInstance().showdown())
				{
					return false;
				}
				if (!SpreadClient.getInstance().showdown())
				{
					return false;
				}
				if (!AllyClient.getInstance().showdown())
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		
		public bool destroy()
		{
			try
			{
				if (!HuanYingSiYuanClient.getInstance().destroy())
				{
					return false;
				}
				if (!SpreadClient.getInstance().destroy())
				{
					return false;
				}
				if (!AllyClient.getInstance().destroy())
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
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
			int nID = eventObject.getEventType();
			int num = nID;
			if (num != 12)
			{
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
		}

		
		public bool InitConfig()
		{
			bool success = true;
			try
			{
				ConfigurationManager.RefreshSection("appSettings");
				KuaFuManager.KuaFuWorldKuaFuGameServer = (ConfigurationManager.AppSettings.Get("KuaFuWorldKuaFuGameServer") == "1");
				foreach (string name in RuntimeVariableNames.KuaFuUriNames)
				{
					string uri = ConfigurationManager.AppSettings.Get(name);
					this.CoreInterface.SetRuntimeVariable(name, uri);
				}
				KFCallManager.Start();
			}
			catch (Exception ex)
			{
				success = false;
				LogManager.WriteLog(LogTypes.Fatal, "加载跨服中心服务地址配置失败。", ex, true);
			}
			return success;
		}

		
		public bool OnUserLogin2(TMSKSocket socket, int verSign, string userID, string userName, string lastTime, string isadult, string signCode)
		{
			WebLoginToken webLoginToken = new WebLoginToken
			{
				VerSign = verSign,
				UserID = userID,
				UserName = userName,
				LastTime = lastTime,
				Isadult = isadult,
				SignCode = signCode
			};
			socket.ClientKuaFuServerLoginData.WebLoginToken = webLoginToken;
			return true;
		}

		
		public bool OnUserLogin(TMSKSocket socket, int verSign, string userID, string userName, string lastTime, string userToken, string isadult, string signCode, int serverId = 0, string ip = null, int port = 0, int roleId = 0, int gameType = 0, long gameId = 0L)
		{
			KuaFuServerLoginData kuaFuServerLoginData = socket.ClientKuaFuServerLoginData;
			if (serverId > 0 && ip != null)
			{
				kuaFuServerLoginData.ServerId = serverId;
				kuaFuServerLoginData.ServerIp = ip;
				kuaFuServerLoginData.ServerPort = port;
				kuaFuServerLoginData.RoleId = roleId;
				kuaFuServerLoginData.GameType = gameType;
				kuaFuServerLoginData.GameId = gameId;
			}
			if (kuaFuServerLoginData.WebLoginToken == null)
			{
				kuaFuServerLoginData.WebLoginToken = new WebLoginToken
				{
					VerSign = verSign,
					UserID = userID,
					UserName = userName,
					LastTime = lastTime,
					Isadult = isadult,
					SignCode = signCode
				};
			}
			if (roleId > 0 && serverId > 0 && gameType > 0)
			{
				if (GameManager.ServerLineID != GameManager.ServerId)
				{
					LogManager.WriteLog(LogTypes.Error, "GameManager.ServerLineID未配置,禁止跨服登录", null, true);
					return false;
				}
				if (string.IsNullOrEmpty(ip) || port <= 0 || gameType <= 0 || gameId <= 0L)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色{0}未能在服务器列表中找本服务器，作为跨服服务器", kuaFuServerLoginData.RoleId), null, true);
					return false;
				}
				if (!KuaFuManager.getInstance().CanKuaFuLogin())
				{
					return false;
				}
				socket.ServerId = serverId;
				switch (gameType)
				{
				case 1:
					socket.IsKuaFuLogin = HuanYingSiYuanClient.getInstance().KuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 2:
					socket.IsKuaFuLogin = TianTiClient.getInstance().KuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 5:
				case 6:
				case 15:
					socket.IsKuaFuLogin = YongZheZhanChangClient.getInstance().KuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 7:
					socket.IsKuaFuLogin = YongZheZhanChangClient.getInstance().CanEnterKuaFuMap(kuaFuServerLoginData);
					goto IL_399;
				case 8:
					socket.IsKuaFuLogin = SingletonTemplate<CopyTeamManager>.Instance().HandleKuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 10:
					socket.IsKuaFuLogin = LangHunLingYuManager.getInstance().CanEnterKuaFuMap(kuaFuServerLoginData);
					goto IL_399;
				case 12:
					socket.IsKuaFuLogin = SingletonTemplate<ZhengBaManager>.Instance().CanKuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 13:
					socket.IsKuaFuLogin = SingletonTemplate<CoupleArenaManager>.Instance().CanKuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 17:
					socket.IsKuaFuLogin = true;
					goto IL_399;
				case 19:
				case 20:
					socket.IsKuaFuLogin = KarenBattleManager.getInstance().KuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 22:
					socket.IsKuaFuLogin = true;
					goto IL_399;
				case 24:
					socket.IsKuaFuLogin = BangHuiMatchManager.getInstance().KuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 25:
					socket.IsKuaFuLogin = KuaFuLueDuoManager.getInstance().KuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 27:
				case 28:
				case 29:
					socket.IsKuaFuLogin = true;
					goto IL_399;
				case 30:
					socket.IsKuaFuLogin = CompBattleManager.getInstance().KuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 31:
					socket.IsKuaFuLogin = CompMineManager.getInstance().KuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 32:
					socket.IsKuaFuLogin = true;
					goto IL_399;
				case 34:
					socket.IsKuaFuLogin = TianTi5v5Manager.getInstance().CanKuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				case 36:
					socket.IsKuaFuLogin = ZorkBattleManager.getInstance().KuaFuLogin(kuaFuServerLoginData);
					goto IL_399;
				}
				EventObjectEx_I1 eventObject = new EventObjectEx_I1(kuaFuServerLoginData, 61, gameType);
				if (GlobalEventSource4Scene.getInstance().fireEvent(eventObject, 10007))
				{
					socket.IsKuaFuLogin = eventObject.Result;
				}
				IL_399:
				string dbIp = "";
				int dbPort = 0;
				string logDbIp = "";
				int logDbPort = 0;
				if (kuaFuServerLoginData.ips != null && kuaFuServerLoginData.ports != null)
				{
					dbIp = kuaFuServerLoginData.ips[0];
					logDbIp = kuaFuServerLoginData.ips[1];
					dbPort = kuaFuServerLoginData.ports[0];
					logDbPort = kuaFuServerLoginData.ports[1];
				}
				else if (!KuaFuManager.getInstance().GetKuaFuDbServerInfo(serverId, out dbIp, out dbPort, out logDbIp, out logDbPort))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("GameType{0}未能找到角色{1}的原服务器的服务器IP和端口", gameType, kuaFuServerLoginData.RoleId), null, true);
					return false;
				}
				if (socket.IsKuaFuLogin && serverId != 0)
				{
					if (serverId != 0)
					{
						if (!this.InitGameDbConnection(serverId, dbIp, dbPort, logDbIp, logDbPort))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("连接角色{0}的原服务器的GameDBServer和LogDBServer失败", kuaFuServerLoginData.RoleId), null, true);
							return false;
						}
					}
					return socket.IsKuaFuLogin;
				}
			}
			else
			{
				if (GameManager.IsKuaFuServer)
				{
					LogManager.WriteLog(LogTypes.Error, "跨服服务器禁止非跨服登录,请检查是否将LineID配置,原服应当为1", null, true);
					return false;
				}
				if (KuaFuManager.getInstance().LocalLogin(userID))
				{
					kuaFuServerLoginData.RoleId = 0;
					kuaFuServerLoginData.GameId = 0L;
					kuaFuServerLoginData.GameType = 0;
					kuaFuServerLoginData.ServerId = 0;
					socket.ServerId = 0;
					socket.IsKuaFuLogin = false;
					return true;
				}
			}
			LogManager.WriteLog(LogTypes.Error, string.Format("未能找到角色{0}的跨服活动或副本信息", kuaFuServerLoginData.RoleId), null, true);
			return false;
		}

		
		public bool OnInitGame(GameClient client)
		{
			int gameType = Global.GetClientKuaFuServerLoginData(client).GameType;
			bool result;
			if (KuaFuManager.KuaFuWorldKuaFuGameServer && !KuaFuManager.KuaFuWorldGameTypes.Contains(gameType))
			{
				result = true;
			}
			else
			{
				switch (gameType)
				{
				case 1:
					return HuanYingSiYuanManager.getInstance().OnInitGame(client);
				case 2:
					return TianTiManager.getInstance().OnInitGame(client);
				case 3:
					return SingletonTemplate<MoRiJudgeManager>.Instance().OnInitGame(client);
				case 4:
					return ElementWarManager.getInstance().OnInitGame(client);
				case 5:
					return YongZheZhanChangManager.getInstance().OnInitGame(client);
				case 6:
					return KuaFuBossManager.getInstance().OnInitGame(client);
				case 7:
				case 32:
					return KuaFuMapManager.getInstance().OnInitGame(client);
				case 8:
					return SingletonTemplate<CopyTeamManager>.Instance().HandleKuaFuInitGame(client);
				case 10:
					return LangHunLingYuManager.getInstance().OnInitGameKuaFu(client);
				case 12:
					return SingletonTemplate<ZhengBaManager>.Instance().KuaFuInitGame(client);
				case 13:
					return SingletonTemplate<CoupleArenaManager>.Instance().KuaFuInitGame(client);
				case 15:
					return KingOfBattleManager.getInstance().OnInitGame(client);
				case 17:
					return ZhengDuoManager.getInstance().OnInitGame(client);
				case 19:
					return KarenBattleManager_MapWest.getInstance().OnInitGame(client);
				case 20:
					return KarenBattleManager_MapEast.getInstance().OnInitGame(client);
				case 22:
					return LingDiCaiJiManager.getInstance().KuaFuInitGame(client);
				case 24:
					return BangHuiMatchManager.getInstance().OnInitGameKuaFu(client);
				case 25:
					return KuaFuLueDuoManager.getInstance().OnInitGameKuaFu(client);
				case 26:
					return WanMoXiaGuManager.getInstance().OnInitGame(client);
				case 27:
				case 28:
				case 29:
					return CompManager.getInstance().OnInitGameKuaFu(client);
				case 30:
					return CompBattleManager.getInstance().OnInitGameKuaFu(client);
				case 31:
					return CompMineManager.getInstance().OnInitGameKuaFu(client);
				case 34:
					return TianTi5v5Manager.getInstance().OnInitGame(client);
				case 36:
					return ZorkBattleManager.getInstance().OnInitGameKuaFu(client);
				}
				EventObjectEx_I1 eventObject = new EventObjectEx_I1(client, 62, gameType);
				result = (GlobalEventSource4Scene.getInstance().fireEvent(eventObject, 10007) && eventObject.Result);
			}
			return result;
		}

		
		public void OnStartPlayGame(GameClient client)
		{
			int gameType = Global.GetClientKuaFuServerLoginData(client).GameType;
			int num = gameType;
			if (num != 7)
			{
				switch (num)
				{
				case 27:
				case 28:
				case 29:
					CompManager.getInstance().OnStartPlayGame(client);
					return;
				case 30:
				case 31:
					return;
				case 32:
					break;
				default:
					return;
				}
			}
			KuaFuMapManager.getInstance().OnStartPlayGame(client);
		}

		
		public void OnLeaveScene(GameClient client, SceneUIClasses sceneType, bool logout = false)
		{
			if (client.ClientSocket.IsKuaFuLogin)
			{
				switch (sceneType)
				{
				case SceneUIClasses.HuanYingSiYuan:
					HuanYingSiYuanManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.TianTi:
					TianTiManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.YongZheZhanChang:
					YongZheZhanChangManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.ElementWar:
					ElementWarManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.MoRiJudge:
					SingletonTemplate<MoRiJudgeManager>.Instance().OnLogOut(client);
					goto IL_1E7;
				case SceneUIClasses.CopyWolf:
					CopyWolfManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.LangHunLingYu:
					LangHunLingYuManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.KFZhengBa:
					SingletonTemplate<ZhengBaManager>.Instance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.CoupleArena:
					SingletonTemplate<CoupleArenaManager>.Instance().OnLeaveFuBen(client);
					goto IL_1E7;
				case SceneUIClasses.KingOfBattle:
					KingOfBattleManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.KarenWest:
					KarenBattleManager_MapWest.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.KarenEast:
					KarenBattleManager_MapEast.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.LingDiCaiJi:
					LingDiCaiJiManager.getInstance().OnLeaveFuBen(client);
					goto IL_1E7;
				case SceneUIClasses.BangHuiMatch:
					BangHuiMatchManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.KuaFuLueDuo:
					KuaFuLueDuoManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.WanMoXiaGu:
					WanMoXiaGuManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case SceneUIClasses.ZorkBattle:
					ZorkBattleManager.getInstance().OnLogout(client);
					goto IL_1E7;
				}
				if (SingletonTemplate<CopyTeamManager>.Instance().IsKuaFuCopy(client.ClientData.FuBenID))
				{
					SingletonTemplate<CopyTeamManager>.Instance().OnLeaveFuBen(client, sceneType);
				}
				IL_1E7:
				if (!logout)
				{
					this.GotoLastMap(client);
				}
			}
		}

		
		public void OnLogout(GameClient client)
		{
			switch (client.ClientData.SignUpGameType)
			{
			case 1:
				HuanYingSiYuanClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.None, true);
				break;
			case 2:
				TianTiClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.None, true);
				break;
			}
		}

		
		public void GotoLastMap(GameClient client)
		{
			if (!client.CheckCheatData.DisableAutoKuaFu)
			{
				client.sendCmd<KuaFuServerLoginData>(14000, new KuaFuServerLoginData
				{
					RoleId = 0
				}, false);
			}
		}

		
		public void ClearCopyMapClients(CopyMap copyMap)
		{
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
				DataHelper.WriteExceptionLogEx(ex, "跨服组队竞技清场调度异常");
			}
		}

		
		public int GetServerInfoAsyncAge()
		{
			int serverInfoAsyncAge;
			lock (this.RuntimeData.Mutex)
			{
				serverInfoAsyncAge = this.RuntimeData.ServerInfoAsyncAge;
			}
			return serverInfoAsyncAge;
		}

		
		public bool LocalLogin(string userId)
		{
			return this.LocalServerFlags == 0 || (2 & this.LocalServerFlags) == 0 || true;
		}

		
		public bool CanKuaFuLogin()
		{
			return GameManager.ServerId >= 9000;
		}

		
		public void UpdateServerInfoList(List<KuaFuServerInfo> list)
		{
			if (list != null && list.Count > 0)
			{
				lock (this.RuntimeData.Mutex)
				{
					int age = list[0].Age;
					if (age > this.RuntimeData.ServerInfoAsyncAge || this.RuntimeData.ServerInfoAsyncAge - age > 120000)
					{
						this.RuntimeData.ServerInfoAsyncAge = age;
						this.RuntimeData.ServerIdServerInfoDict.Clear();
						foreach (KuaFuServerInfo item in list)
						{
							this.RuntimeData.ServerIdServerInfoDict[item.ServerId] = item;
							if (GameManager.ServerId == item.ServerId || GameManager.KuaFuServerId == item.ServerId)
							{
								this.LocalServerFlags = item.Flags;
							}
						}
					}
				}
			}
		}

		
		public bool TryGetValue(int serverId, out KuaFuServerInfo kuaFuServerInfo)
		{
			bool result;
			lock (this.RuntimeData.Mutex)
			{
				result = this.RuntimeData.ServerIdServerInfoDict.TryGetValue(serverId, out kuaFuServerInfo);
			}
			return result;
		}

		
		public bool GetKuaFuDbServerInfo(int serverId, out string dbIp, out int dbPort, out string logIp, out int logPort)
		{
			KuaFuServerInfo kuaFuServerInfo;
			bool result;
			if (KuaFuManager.getInstance().TryGetValue(serverId, out kuaFuServerInfo))
			{
				dbIp = kuaFuServerInfo.DbIp;
				dbPort = kuaFuServerInfo.DbPort;
				logIp = kuaFuServerInfo.DbIp;
				logPort = kuaFuServerInfo.LogDbPort;
				result = true;
			}
			else
			{
				dbIp = null;
				dbPort = 0;
				logIp = null;
				logPort = 0;
				result = false;
			}
			return result;
		}

		
		public bool GetKuaFuDbServerInfo(int serverId, out string dbIp, out int dbPort, out string logIp, out int logPort, out string gsIp, out int gsPort)
		{
			KuaFuServerInfo kuaFuServerInfo;
			bool result;
			if (KuaFuManager.getInstance().TryGetValue(serverId, out kuaFuServerInfo))
			{
				dbIp = kuaFuServerInfo.DbIp;
				dbPort = kuaFuServerInfo.DbPort;
				logIp = kuaFuServerInfo.DbIp;
				logPort = kuaFuServerInfo.LogDbPort;
				gsIp = kuaFuServerInfo.Ip;
				gsPort = kuaFuServerInfo.Port;
				result = true;
			}
			else
			{
				dbIp = null;
				dbPort = 0;
				logIp = null;
				logPort = 0;
				gsIp = null;
				gsPort = 0;
				result = false;
			}
			return result;
		}

		
		public void KuaFuSwitchServer(GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= client.KuaFuSwitchServerTicks + 5000L)
			{
				client.KuaFuSwitchServerTicks = nowTicks;
				GlobalNew.RecordSwitchKuaFuServerLog(client);
				client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
			}
		}

		
		private void BackGroudThreadProc()
		{
			for (;;)
			{
				try
				{
					this.HandleTransferChatMsg();
				}
				catch
				{
				}
				Thread.Sleep(1800);
			}
		}

		
		public bool InitGameDbConnection(int serverId, string ip, int port, string logIp, int logPort)
		{
			bool init = false;
			KuaFuDbConnection pool;
			lock (this.DbMutex)
			{
				if (!this.GameDbConnectPoolDict.TryGetValue(serverId, out pool))
				{
					pool = new KuaFuDbConnection(serverId);
					pool.LastHeartTicks = TimeUtil.NOW();
					this.GameDbConnectPoolDict[serverId] = pool;
					init = true;
				}
				else
				{
					pool.Pool[0].ChangeIpPort(ip, port);
					pool.Pool[1].ChangeIpPort(logIp, logPort);
				}
			}
			bool result;
			if (init)
			{
				result = (pool.Pool[0].Init(3, ip, port, string.Format("server_db_{0}", serverId)) && pool.Pool[1].Init(3, logIp, logPort, string.Format("server_log_{0}", serverId)));
			}
			else
			{
				result = (pool.Pool[0].Supply() && pool.Pool[1].Supply());
			}
			return result;
		}

		
		public TCPClient PopGameDbClient(int serverId, int poolId)
		{
			try
			{
				KuaFuDbConnection pool;
				lock (this.DbMutex)
				{
					if (!this.GameDbConnectPoolDict.TryGetValue(serverId, out pool))
					{
						return null;
					}
				}
				return pool.Pool[poolId].Pop();
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		
		public void PushGameDbClient(int serverId, TCPClient tcpClient, int poolId)
		{
			try
			{
				KuaFuDbConnection pool;
				lock (this.DbMutex)
				{
					if (!this.GameDbConnectPoolDict.TryGetValue(serverId, out pool))
					{
						return;
					}
					if (tcpClient.LastCmdID == 10025)
					{
						pool.LastHeartTicks = TimeUtil.NOW();
					}
				}
				pool.Pool[poolId].Push(tcpClient);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		private void HandleTransferChatMsg()
		{
			long ticks = TimeUtil.NOW();
			if (ticks - this.LastTransferTicks >= 1000L)
			{
				this.LastTransferTicks = ticks;
				string strcmd = "";
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					GameManager.ServerLineID,
					0,
					this.SendServerHeartCount,
					""
				});
				this.SendServerHeartCount++;
				this.ActiveServerIdList.Clear();
				lock (this.DbMutex)
				{
					foreach (KuaFuDbConnection connection in this.GameDbConnectPoolDict.Values)
					{
						if (connection.ServerId % 3 == this.SendServerHeartCount % 3)
						{
							this.ActiveServerIdList.Add(connection);
						}
					}
				}
				List<int> serverIdList = new List<int>();
				foreach (KuaFuDbConnection conn in this.ActiveServerIdList)
				{
					if (ticks - conn.LastHeartTicks > 300000L)
					{
						lock (this.DbMutex)
						{
							this.GameDbConnectPoolDict.Remove(conn.ServerId);
							conn.Dispose();
						}
					}
					else if (conn.Pool[0].ErrCount == 0)
					{
						serverIdList.Add(conn.ServerId);
					}
				}
				foreach (int serverId in serverIdList)
				{
					try
					{
						List<string> chatMsgList = Global.sendToDB<List<string>, string>(10018, strcmd, serverId);
						if (chatMsgList != null && chatMsgList.Count > 0)
						{
							for (int i = 0; i < chatMsgList.Count; i++)
							{
								GameManager.ClientMgr.TransferChatMsg(chatMsgList[i]);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
						lock (this.DbMutex)
						{
							this.GameDbConnectPoolDict.Remove(serverId);
						}
					}
				}
			}
		}

		
		public bool IsKuaFuMap(int mapCode)
		{
			switch (Global.GetMapSceneType(mapCode))
			{
			case SceneUIClasses.HuanYingSiYuan:
			case SceneUIClasses.TianTi:
			case SceneUIClasses.YongZheZhanChang:
			case SceneUIClasses.ElementWar:
			case SceneUIClasses.MoRiJudge:
			case SceneUIClasses.KuaFuBoss:
			case SceneUIClasses.KaLunTe:
			case SceneUIClasses.CopyWolf:
			case SceneUIClasses.LangHunLingYu:
			case SceneUIClasses.KFZhengBa:
			case SceneUIClasses.HuanShuYuan:
			case SceneUIClasses.CoupleArena:
			case SceneUIClasses.KingOfBattle:
			case SceneUIClasses.ZhengDuo:
			case SceneUIClasses.KarenWest:
			case SceneUIClasses.KarenEast:
			case SceneUIClasses.LingDiCaiJi:
			case SceneUIClasses.BangHuiMatch:
			case SceneUIClasses.KuaFuLueDuo:
			case SceneUIClasses.Comp:
			case SceneUIClasses.WanMoXiaGu:
			case SceneUIClasses.CompBattle:
			case SceneUIClasses.CompMine:
			case SceneUIClasses.ChongShengMap:
			case SceneUIClasses.TianTi5v5:
			case SceneUIClasses.ZhanDuiZhengBa:
			case SceneUIClasses.ZorkBattle:
			case SceneUIClasses.EscapeBattle:
				return true;
			}
			return false;
		}

		
		
		
		public int SingUpMaxSeconds { get; private set; }

		
		
		
		public int AutoCancelMaxSeconds { get; private set; }

		
		
		
		public int CannotJoinCopyMaxSeconds { get; private set; }

		
		public void InitCopyTime()
		{
			int[] arr = GameManager.systemParamsList.GetParamValueIntArrayByName("KuaFuFuBenTime", ',');
			if (arr != null && arr.Length >= 3)
			{
				this.SingUpMaxSeconds = arr[0];
				this.AutoCancelMaxSeconds = arr[1];
				this.CannotJoinCopyMaxSeconds = arr[2];
			}
		}

		
		public void SetCannotJoinKuaFu_UseAutoEndTicks(GameClient client)
		{
			if (this.CannotJoinCopyMaxSeconds > 0)
			{
				this.SetCannotJoinKuaFuCopyEndTicks(client, TimeUtil.NowDateTime().AddSeconds((double)this.CannotJoinCopyMaxSeconds).Ticks);
			}
		}

		
		public void SetCannotJoinKuaFuCopyEndTicks(GameClient client, long endTicks)
		{
			if (client != null)
			{
				Global.SaveRoleParamsInt64ValueToDB(client, "CannotJoinKFCopyEndTicks", endTicks, true);
			}
		}

		
		public bool IsInCannotJoinKuaFuCopyTime(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = true;
			}
			else
			{
				long endTicks = Global.GetRoleParamsInt64FromDB(client, "CannotJoinKFCopyEndTicks");
				result = (TimeUtil.NowDateTime().Ticks < endTicks);
			}
			return result;
		}

		
		public void NotifyClientCannotJoinKuaFuCopyEndTicks(GameClient client)
		{
			long endTicks = Global.GetRoleParamsInt64FromDB(client, "CannotJoinKFCopyEndTicks");
		}

		
		public string GetZoneName(int serverID)
		{
			string zoneName;
			lock (this.ZoneID2ZoneNameDict)
			{
				if (this.ZoneID2ZoneNameDict.TryGetValue(serverID, out zoneName))
				{
					return zoneName;
				}
			}
			KuaFuServerInfo kuaFuServerInfo;
			if (KuaFuManager.getInstance().TryGetValue(serverID, out kuaFuServerInfo))
			{
				zoneName = kuaFuServerInfo.strServerName;
				lock (this.ZoneID2ZoneNameDict)
				{
					this.ZoneID2ZoneNameDict[serverID] = zoneName;
				}
			}
			return zoneName;
		}

		
		public bool ClientCmdCheckFaild(int cmdID, GameClient client, ref int roleID)
		{
			bool result;
			if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ClientCmdCheckFaild,cmd={0},client=null", cmdID), null, true);
				result = true;
			}
			else if (client.ClientSocket.IsKuaFuLogin)
			{
				if (client.ClientSocket.ClientKuaFuServerLoginData.GameType == 32)
				{
					if (!Data.KuaFuWorldCmdEnabled(cmdID))
					{
						LogManager.WriteLog(LogTypes.Fatal, "KuaFuWorldCmd " + (TCPGameServerCmds)cmdID, null, true);
						return true;
					}
				}
				if (client.ClientData.RoleID != roleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("RoleIDCheckFaild,cmd={0},NeedRoleID={1},CmdRoleID={2}", (TCPGameServerCmds)cmdID, client.ClientData.RoleID, roleID), null, true);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else if (client.ClientData.RoleID != roleID)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("RoleIDCheckFaild,cmd={0},NeedRoleID={1},CmdRoleID={2}", (TCPGameServerCmds)cmdID, client.ClientData.RoleID, roleID), null, true);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		private ICoreInterface CoreInterface = null;

		
		private static KuaFuManager instance = new KuaFuManager();

		
		public static bool KuaFuWorldKuaFuGameServer = false;

		
		public KuaFuDataData RuntimeData = new KuaFuDataData();

		
		private static int[] KuaFuWorldGameTypes = new int[]
		{
			32
		};

		
		private int LocalServerFlags = 0;

		
		private object DbMutex = new object();

		
		private Dictionary<int, KuaFuDbConnection> GameDbConnectPoolDict = new Dictionary<int, KuaFuDbConnection>();

		
		public long LastTransferTicks = 0L;

		
		public int SendServerHeartCount = 0;

		
		private List<KuaFuDbConnection> ActiveServerIdList = new List<KuaFuDbConnection>();

		
		private Dictionary<int, string> ZoneID2ZoneNameDict = new Dictionary<int, string>();
	}
}
