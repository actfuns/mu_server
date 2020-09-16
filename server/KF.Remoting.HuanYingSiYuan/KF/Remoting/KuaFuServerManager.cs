using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract.Data;
using KF.Remoting.Data;
using KF.Remoting.IPStatistics;
using KF.Remoting.KFBoCai;
using KF.TcpCall;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Remoting;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	
	public static class KuaFuServerManager
	{
		
		public static bool CheckConfig()
		{
			KuaFuServerManager.ResourcePath = ConfigurationManager.AppSettings.Get("ResourcePath");
			KuaFuServerManager.ConfigPathStructType = 2;
			string fileName = KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes);
			if (!Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.Map)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.MapConfig)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.Isolate)))
			{
				KuaFuServerManager.ConfigPathStructType = 1;
				fileName = KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes);
				if (!Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.Map)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.MapConfig)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.Isolate)))
				{
					KuaFuServerManager.ConfigPathStructType = 0;
					if (!Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.Map)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.MapConfig)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.Isolate)))
					{
						LogManager.WriteLog(LogTypes.Fatal, "配置文件目录结构不正确", null, true);
					}
				}
			}
			string filePath = KuaFuServerManager.GetResourcePath("Config/Settings.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
			bool result;
			if (!File.Exists(filePath))
			{
				Console.WriteLine("错误:文件未找到.{0}", filePath);
				result = false;
			}
			else
			{
				filePath = KuaFuServerManager.GetResourcePath("Config/SystemTasks.xml", KuaFuServerManager.ResourcePathTypes.Isolate);
				if (!File.Exists(filePath))
				{
					Console.WriteLine("错误:文件未找到.{0}", filePath);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		
		public static bool LoadConfig()
		{
			lock (KuaFuServerManager.Mutex)
			{
				try
				{
					KuaFuServerManager.LoadConfigSuccess = true;
					ConfigurationManager.RefreshSection("appSettings");
					KuaFuServerManager.ServerListUrl = ConfigurationManager.AppSettings.Get("ServerListUrl");
					KuaFuServerManager.KuaFuServerListUrl = ConfigurationManager.AppSettings.Get("KuaFuServerListUrl");
					string InputKing = ConfigurationManager.AppSettings.Get("PlatChargeKingUrl");
					if (!string.IsNullOrEmpty(InputKing))
					{
						KuaFuServerManager.GetPlatChargeKingUrl = InputKing.Split(new char[]
						{
							','
						});
					}
					string InputKingEvery = ConfigurationManager.AppSettings.Get("PlatChargeKingUrl_EveryDay");
					if (!string.IsNullOrEmpty(InputKingEvery))
					{
						KuaFuServerManager.GetPlatChargeKingUrl_EveryDay = InputKingEvery.Split(new char[]
						{
							','
						});
					}
					ConstData.HTTP_MD5_KEY = ConfigurationManager.AppSettings.Get("MD5Key");
					KuaFuServerManager.ResourcePath = ConfigurationManager.AppSettings.Get("ResourcePath");
					KuaFuServerManager.LimitIP = !string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("LimitIP"));
					KuaFuServerManager.UseLanIp = (ConfigurationManager.AppSettings.Get("UseLanIp") == "1");
					KuaFuServerManager.PingTaiKuaFu = (ConfigurationManager.AppSettings.Get("PingTaiKuaFu") == "1");
					KuaFuServerManager.PingTaiKuaFuTestMode = (ConfigurationManager.AppSettings.Get("PingTaiKuaFuTestMode") == "1");
					string platformStr = ConfigurationManager.AppSettings.Get("Platform");
					for (PlatformTypes i = PlatformTypes.Tmsk; i < PlatformTypes.Max; i++)
					{
						if (0 == string.Compare(platformStr, i.ToString(), true))
						{
							KuaFuServerManager.platformType = i;
							break;
						}
					}
					if (KuaFuServerManager.platformType == PlatformTypes.Max)
					{
						LogManager.WriteLog(LogTypes.Fatal, "必须在配置文件中设置有效的平台类型: Platform", null, true);
						KuaFuServerManager.LoadConfigSuccess = false;
						return false;
					}
					string ptidStr = ConfigurationManager.AppSettings.Get("PTID");
					if (string.IsNullOrEmpty(ptidStr) || !int.TryParse(ptidStr, out KuaFuServerManager.PTID) || KuaFuServerManager.PTID <= 0)
					{
						KuaFuServerManager.PTID = (int)KuaFuServerManager.platformType;
					}
					int baseKuaFuServerID = 0;
					if (KuaFuServerManager.PingTaiKuaFu)
					{
						baseKuaFuServerID = ConstData.ConvertToKuaFuServerID(0, KuaFuServerManager.PTID);
					}
					string servicePortStr = ConfigurationManager.AppSettings.Get("ServicePort");
					if (string.IsNullOrEmpty(servicePortStr) || !int.TryParse(servicePortStr, out KuaFuServerManager.ServicePort))
					{
						KuaFuServerManager.ServicePort = 0;
					}
					string globalUseLanIP = KuaFuServerManager.UseLanIp ? "1" : "0";
					List<Tuple<int, string, string, string>> kuaFuWorldPlatformServerListUrls = new List<Tuple<int, string, string, string>>();
					string kuafuWorldPlatforms = ConfigurationManager.AppSettings.Get("PlatfromAll");
					if (!string.IsNullOrEmpty(kuafuWorldPlatforms))
					{
						foreach (string pstr in kuafuWorldPlatforms.Split(KuaFuServerManager.SpliteChars, StringSplitOptions.RemoveEmptyEntries))
						{
							PlatformTypes p;
							if (!Enum.TryParse<PlatformTypes>(pstr, true, out p))
							{
								LogManager.WriteLog(LogTypes.Fatal, ".config配置错误,无法解析出程序已知的平台类型,key=PlatfromAll,value=" + kuafuWorldPlatforms, null, true);
							}
							else
							{
								string serverListUrl = ConfigurationManager.AppSettings.Get("ServerListUrl_" + pstr);
								string kuaFuServerListUrl = ConfigurationManager.AppSettings.Get("KuaFuServerListUrl_" + pstr);
								string useLanIp = ConfigurationManager.AppSettings.Get("UseLanIp_" + pstr);
								if (!string.IsNullOrEmpty(serverListUrl) || !string.IsNullOrEmpty(kuaFuServerListUrl))
								{
									kuaFuWorldPlatformServerListUrls.Add(new Tuple<int, string, string, string>((int)p, serverListUrl, kuaFuServerListUrl, useLanIp ?? globalUseLanIP));
								}
							}
						}
					}
					if (!kuaFuWorldPlatformServerListUrls.Exists((Tuple<int, string, string, string> x) => x.Item1 == KuaFuServerManager.PTID))
					{
						kuaFuWorldPlatformServerListUrls.Add(new Tuple<int, string, string, string>(KuaFuServerManager.PTID, KuaFuServerManager.ServerListUrl, KuaFuServerManager.KuaFuServerListUrl, globalUseLanIP));
					}
					Interlocked.Exchange<List<Tuple<int, string, string, string>>>(ref KuaFuServerManager.KuaFuWorldPlatformServerListUrls, kuaFuWorldPlatformServerListUrls);
					string kuaFuMapLineStr = ConfigurationManager.AppSettings.Get("KuaFuMapLine");
					if (!string.IsNullOrEmpty(kuaFuMapLineStr))
					{
						KuaFuServerManager.KuaFuMapLineDict.Clear();
						string[] mapLineStrs = kuaFuMapLineStr.Split(new char[]
						{
							'|'
						});
						foreach (string mapLineStr in mapLineStrs)
						{
							string[] mapLineParams = mapLineStr.Split(new char[]
							{
								','
							});
							int line;
							int serverId;
							if (mapLineParams.Length == 2 && int.TryParse(mapLineParams[0], out line) && int.TryParse(mapLineParams[1], out serverId))
							{
								KuaFuServerManager.KuaFuMapLineDict[line] = serverId + baseKuaFuServerID;
							}
						}
					}
					string specialLineStr = ConfigurationManager.AppSettings.Get("SpecialLine");
					if (!string.IsNullOrEmpty(specialLineStr))
					{
						KuaFuServerManager.SpecialLineDict.Clear();
						string[] mapLineStrs = specialLineStr.Split(new char[]
						{
							'|'
						});
						foreach (string mapLineStr in mapLineStrs)
						{
							string[] mapLineParams = mapLineStr.Split(new char[]
							{
								','
							});
							int line;
							int serverId;
							if (mapLineParams.Length == 2 && int.TryParse(mapLineParams[0], out line) && int.TryParse(mapLineParams[1], out serverId))
							{
								KuaFuServerManager.SpecialLineDict[line] = serverId + baseKuaFuServerID;
							}
						}
					}
					if (KuaFuServerManager.PingTaiKuaFu)
					{
						KuaFuServerManager.KuaFuMapLineDict.Clear();
						KuaFuServerManager.SpecialLineDict.Clear();
						string pingTaiKuaFuServerLineStr = ConfigurationManager.AppSettings.Get("PingTaiKuaFuServerLine");
						if (!string.IsNullOrEmpty(pingTaiKuaFuServerLineStr))
						{
							KuaFuServerManager.PingTaiKuaFuServerLineDict.Clear();
							string[] mapLineStrs = pingTaiKuaFuServerLineStr.Split(new char[]
							{
								'|'
							});
							foreach (string mapLineStr in mapLineStrs)
							{
								string[] mapLineParams = mapLineStr.Split(new char[]
								{
									','
								});
								int line;
								int serverId;
								if (mapLineParams.Length == 2 && int.TryParse(mapLineParams[0], out line) && int.TryParse(mapLineParams[1], out serverId))
								{
									KuaFuServerManager.PingTaiKuaFuServerLineDict[line] = serverId + baseKuaFuServerID;
								}
							}
						}
					}
					KuaFuServerManager.systemParamsList.LoadParamsList();
					string fileName = KuaFuServerManager.GetResourcePath("Config/VersionSystemOpen.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
					if (!KuaFuServerManager.VersionSystemOpenMgr.LoadVersionSystemOpenData(fileName))
					{
						KuaFuServerManager.LoadConfigSuccess = false;
					}
					fileName = KuaFuServerManager.GetResourcePath("Config/GameFuncControl.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
					if (!GameFuncControlManager.LoadConfig(fileName))
					{
						KuaFuServerManager.LoadConfigSuccess = false;
					}
					fileName = KuaFuServerManager.GetResourcePath("Config/ThemeActivityOpen.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement themeActivityOpenFile = ConfigHelper.Load(fileName);
					if (null != themeActivityOpenFile)
					{
						XElement xmlItem = themeActivityOpenFile.Element("ThemeActivityOpen");
						if (null != xmlItem)
						{
							KuaFuServerManager.ThemeActivityState = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "Open", 0L);
						}
					}
					string fullFileName = KuaFuServerManager.GetResourcePath("Config/MapLine.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement xmlFile = ConfigHelper.Load(fullFileName);
					KuaFuServerManager.LineMap2KuaFuLineDataDict.Clear();
					KuaFuServerManager.ServerMap2KuaFuLineDataDict.Clear();
					KuaFuServerManager.KuaFuMapServerIdDict.Clear();
					KuaFuServerManager.MapCode2KuaFuLineDataDict.Clear();
					IEnumerable<XElement> xmls = ConfigHelper.GetXElements(xmlFile, "MapLine");
					string str;
					foreach (XElement node in xmls)
					{
						KuaFuServerManager.MapMaxOnlineCount = (int)ConfigHelper.GetElementAttributeValueLong(node, "MaxNum", 500L);
						int mapType = (int)ConfigHelper.GetElementAttributeValueLong(node, "Type", 0L);
						str = ConfigHelper.GetElementAttributeValue(node, "Line", "");
						if (!string.IsNullOrEmpty(str))
						{
							string[] mapLineStrs = str.Split(new char[]
							{
								'|'
							});
							foreach (string mapLineStr in mapLineStrs)
							{
								KuaFuLineData kuaFuLineData = new KuaFuLineData();
								string[] mapLineParams = mapLineStr.Split(new char[]
								{
									','
								});
								kuaFuLineData.Line = int.Parse(mapLineParams[0]);
								kuaFuLineData.MapCode = int.Parse(mapLineParams[1]);
								if (mapType == 1)
								{
									KuaFuServerManager.PingTaiKuaFuServerLineDict.TryGetValue(kuaFuLineData.Line, out kuaFuLineData.ServerId);
								}
								else if (mapType == 0)
								{
									KuaFuServerManager.KuaFuMapLineDict.TryGetValue(kuaFuLineData.Line, out kuaFuLineData.ServerId);
								}
								kuaFuLineData.MapType = mapType;
								kuaFuLineData.MaxOnlineCount = KuaFuServerManager.MapMaxOnlineCount;
								KuaFuServerManager.LineMap2KuaFuLineDataDict.TryAdd(new IntPairKey(kuaFuLineData.Line, kuaFuLineData.MapCode), kuaFuLineData);
								if (kuaFuLineData.ServerId > 0)
								{
									if (KuaFuServerManager.ServerMap2KuaFuLineDataDict.TryAdd(new IntPairKey(kuaFuLineData.ServerId, kuaFuLineData.MapCode), kuaFuLineData))
									{
										List<KuaFuLineData> list = null;
										if (!KuaFuServerManager.KuaFuMapServerIdDict.TryGetValue(kuaFuLineData.ServerId, out list))
										{
											list = new List<KuaFuLineData>();
											KuaFuServerManager.KuaFuMapServerIdDict.TryAdd(kuaFuLineData.ServerId, list);
										}
										list.Add(kuaFuLineData);
										if (!KuaFuServerManager.MapCode2KuaFuLineDataDict.TryGetValue(kuaFuLineData.MapCode, out list))
										{
											list = new List<KuaFuLineData>();
											KuaFuServerManager.MapCode2KuaFuLineDataDict.TryAdd(kuaFuLineData.MapCode, list);
										}
										list.Add(kuaFuLineData);
									}
								}
							}
						}
					}
					xmlFile = ConfigHelper.Load("config.xml");
					KuaFuServerManager.WritePerformanceLogMs = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "WritePerformanceLogMs", "value", 10L);
					str = ConfigHelper.GetElementAttributeValue(xmlFile, "add", "key", "LoadConfigFromServer", "value", "true");
					if (!bool.TryParse(str, out KuaFuServerManager.LoadConfigFromServer))
					{
						KuaFuServerManager.LoadConfigFromServer = false;
					}
					try
					{
						DbHelperMySQL.ExecuteSql("select 1");
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Error, "数据库连接失败", null, true);
						LogManager.WriteException(ex.ToString());
						return false;
					}
					HuanYingSiYuanPersistence.Instance.InitConfig();
					TianTiPersistence.Instance.InitConfig();
					YongZheZhanChangPersistence.Instance.InitConfig();
					KuaFuCopyDbMgr.Instance.InitConfig();
					SpreadPersistence.Instance.InitConfig();
					AllyPersistence.Instance.InitConfig();
					ZhengBaManagerK.Instance().InitConfig();
					RankPersistence.Instance.InitConfig();
					zhengDuoService.Instance().InitConfig();
					LingDiCaiJiService.Instance().InitConfig();
					BangHuiMatchService.Instance().InitConfig();
					KuaFuLueDuoService.Instance().InitConfig();
					KuaFuServerManager.LoadConfigSuccess &= HongBaoManager_K.getInstance().LoadConfig();
					JunTuanEraService.Instance().InitConfig();
					CompService.Instance().InitConfig();
					TSingleton<KuaFuWorldManager>.getInstance().LoadConfig(false);
					RebornService.Instance().InitConfig();
					SpecPriorityActivityMgr.Instance().InitConfig();
					TianTi5v5Service.InitConfig();
					KuaFuServerManager.LoadConfigSuccess &= ZhanDuiZhengBa_K.InitConfig();
					KuaFuServerManager.LoadConfigSuccess &= EscapeBattle_K.InitConfig();
					KFBoCaiConfigManager.LoadConfig(false);
					Zork5v5Service.Instance().InitConfig();
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
					return false;
				}
			}
			return KuaFuServerManager.LoadConfigSuccess;
		}

		
		public static void InitServer()
		{
			DateTime lastRunTime = TimeUtil.NowDateTime();
			TianTiPersistence.Instance.LoadTianTiRankData(lastRunTime);
			ZhengBaManagerK.Instance().ReloadSyncData(lastRunTime);
			CoupleArenaService.getInstance().StartUp();
			CoupleWishService.getInstance().StartUp();
			RankService.getInstance().StartUp();
			IPStatisticsPersistence.Instance.LoadConfig();
			YaoSaiService.Instance().InitConfig();
			JunTuanPersistence.Instance.InitConfig();
			JunTuanPersistence.Instance.LoadDatabase();
			BangHuiMatchService.Instance().LoadDatabase(lastRunTime);
			CompService.Instance().LoadDatabase(lastRunTime);
			RebornService.Instance().LoadDatabase(lastRunTime);
			SpecPriorityActivityMgr.Instance().LoadDatabase(lastRunTime);
			ZhanDuiZhengBa_K.LoadSyncData(lastRunTime, false);
			EscapeBattle_K.LoadSyncData(lastRunTime, false);
			KFBoCaiManager.GetInstance().StartUp();
			Zork5v5Service.Instance().LoadDatabase(lastRunTime);
		}

		
		public static void StartServerConfigThread()
		{
			if (KuaFuServerManager.UpdateServerConfigThread == null)
			{
				KuaFuServerManager.UpdateServerConfigThread = new Thread(delegate()
				{
					for (;;)
					{
						try
						{
							KuaFuServerManager.AsyncFromDataBase();
							KuaFuServerManager.UpdateDataFromServer();
							Thread.Sleep(20000);
						}
						catch (Exception ex)
						{
							LogManager.WriteExceptionUseCache(ex.ToString());
						}
					}
				});
				KuaFuServerManager.UpdateServerConfigThread.IsBackground = true;
				KuaFuServerManager.UpdateServerConfigThread.Start();
			}
			if (KuaFuServerManager.CheckServerLoadThread == null)
			{
				KuaFuServerManager.CheckServerLoadThread = new Thread(delegate()
				{
					for (;;)
					{
						try
						{
							KuaFuServerManager.UpdateServerLoad();
							Thread.Sleep(8000);
						}
						catch (Exception ex)
						{
							LogManager.WriteExceptionUseCache(ex.ToString());
						}
					}
				});
				KuaFuServerManager.CheckServerLoadThread.IsBackground = true;
				KuaFuServerManager.CheckServerLoadThread.Start();
			}
			if (KuaFuServerManager.WorkThread == null)
			{
				KuaFuServerManager.WorkThread = new Thread(delegate()
				{
					for (;;)
					{
						long startTicks = TimeUtil.NOW();
						try
						{
							HongBaoManager_K.getInstance().ThreadProc(null);
							ZhanDuiZhengBa_K.Update();
							EscapeBattle_K.Update();
						}
						catch (Exception ex)
						{
							LogManager.WriteExceptionUseCache(ex.ToString());
						}
						long endTicks = TimeUtil.NOW();
						long sleepTicks = Math.Min(1000L + startTicks - endTicks, 1000L);
						if (sleepTicks > 0L)
						{
							Thread.Sleep((int)sleepTicks);
							long sleepEndTicks = TimeUtil.NOW();
							if (sleepEndTicks - endTicks > 2000L)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("休眠时间异常,Thread.Sleep({0})休眠了{1}ms", sleepTicks, sleepEndTicks - endTicks), null, true);
							}
						}
					}
				});
				KuaFuServerManager.WorkThread.IsBackground = true;
				KuaFuServerManager.WorkThread.Start();
			}
			if (KuaFuServerManager.FastWorkThread == null)
			{
				KuaFuServerManager.FastWorkThread = new Thread(delegate()
				{
					for (;;)
					{
						long startTicks = TimeUtil.NOW();
						try
						{
							KFServiceBase.TimerProc();
						}
						catch (Exception ex)
						{
							LogManager.WriteExceptionUseCache(ex.ToString());
						}
						long endTicks = TimeUtil.NOW();
						long sleepTicks = Math.Min(1000L + startTicks - endTicks, 200L);
						if (sleepTicks > 0L)
						{
							Thread.Sleep((int)sleepTicks);
							long sleepEndTicks = TimeUtil.NOW();
							if (sleepEndTicks - endTicks > 1000L)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("休眠时间异常,Thread.Sleep({0})休眠了{1}ms", sleepTicks, sleepEndTicks - endTicks), null, true);
							}
						}
					}
				});
				KuaFuServerManager.FastWorkThread.IsBackground = true;
				KuaFuServerManager.FastWorkThread.Start();
			}
		}

		
		private static void UpdateServerLoad()
		{
			foreach (KuaFuServerInfo srv in KuaFuServerManager._ServerIdServerInfoDict.Values)
			{
				int load = 0;
				int state = 0;
				ClientAgentManager.Instance().GetServerState(srv.ServerId, out state, out load);
				if (load != srv.Load || state != srv.State)
				{
					try
					{
						DbHelperMySQL.ExecuteSql(string.Format("update ignore t_server_info set `load`={0},`state`={1} where `serverid`={2}", load, state, srv.ServerId));
						srv.Load = load;
						srv.State = state;
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
					}
				}
			}
			Dictionary<int, GameTypeStaticsData> statics = ClientAgentManager.Instance().GetGameTypeStatics();
			StringBuilder sb = new StringBuilder();
			if (statics != null)
			{
				foreach (KeyValuePair<int, GameTypeStaticsData> kvp in statics)
				{
					sb.AppendFormat("REPLACE INTO t_server_load(gametype, server_alived, fuben_alived, role_signup_count, role_start_game_count,tip) VALUES({0}, {1}, {2}, {3}, {4},'{5}');", new object[]
					{
						kvp.Key,
						kvp.Value.ServerAlived,
						kvp.Value.FuBenAlived,
						kvp.Value.SingUpRoleCount,
						kvp.Value.StartGameRoleCount,
						((GameTypes)kvp.Key).ToString()
					});
					sb.AppendLine();
				}
			}
			if (sb.Length > 0)
			{
				try
				{
					DbHelperMySQL.ExecuteSql(sb.ToString());
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		
		private static void AsyncFromDataBase()
		{
			try
			{
				object ageObj = DbHelperMySQL.GetSingle("select value from t_async where id = 1");
				if (null != ageObj)
				{
					int age = (int)ageObj;
					if (age > KuaFuServerManager._ServerListAge)
					{
						HashSet<int> existAllIds = new HashSet<int>();
						HashSet<int> existKfIds = new HashSet<int>();
						MySqlDataReader sdr = DbHelperMySQL.ExecuteReader("select * from t_server_info", false);
						while (sdr.Read())
						{
							try
							{
								KuaFuServerInfo serverInfo = new KuaFuServerInfo
								{
									ServerId = Convert.ToInt32(sdr["serverid"]),
									Ip = sdr["ip"].ToString(),
									Port = Convert.ToInt32(sdr["port"]),
									DbIp = sdr["dbip"].ToString(),
									DbPort = Convert.ToInt32(sdr["dbport"]),
									LogDbIp = sdr["logdbip"].ToString(),
									LogDbPort = Convert.ToInt32(sdr["logdbport"]),
									State = Convert.ToInt32(sdr["state"]),
									Flags = Convert.ToInt32(sdr["flags"]),
									strServerName = sdr["strservername"].ToString(),
									PTID = Convert.ToInt32(sdr["ptid"]),
									Age = age
								};
								serverInfo.LanIp = serverInfo.LogDbIp;
								KuaFuServerManager._ServerIdServerInfoDict[serverInfo.ServerId] = serverInfo;
								existAllIds.Add(serverInfo.ServerId);
								if ((serverInfo.Flags & 1) != 0)
								{
									existKfIds.Add(serverInfo.ServerId);
								}
							}
							catch (Exception ex)
							{
								LogManager.WriteExceptionUseCache(ex.ToString());
							}
						}
						sdr.Close();
						int nextServerListAge = DataHelper2.UnixSecondsNow();
						lock (KuaFuServerManager.MutexServerList)
						{
							foreach (int id in KuaFuServerManager._ServerIdServerInfoDict.Keys.ToList<int>())
							{
								if (!existAllIds.Contains(id))
								{
									KuaFuServerInfo tmp;
									KuaFuServerManager._ServerIdServerInfoDict.TryRemove(id, out tmp);
								}
							}
							KuaFuServerManager._ServerListAge = nextServerListAge;
						}
						DbHelperMySQL.ExecuteSql(string.Format("update t_async set value={0} where id = 1", nextServerListAge));
						ClientAgentManager.Instance().SetAllKfServerId(existKfIds);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public static string GetResourcePath(string fileName, KuaFuServerManager.ResourcePathTypes resType)
		{
			if (KuaFuServerManager.ConfigPathStructType == 1)
			{
				switch (resType)
				{
				case KuaFuServerManager.ResourcePathTypes.Application:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, DataHelper2.CurrentDirectory, fileName);
				case KuaFuServerManager.ResourcePathTypes.GameRes:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Isolate:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/ServerRes/1/IsolateRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Map:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/Map", fileName);
				case KuaFuServerManager.ResourcePathTypes.MapConfig:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/MapConfig", fileName);
				}
			}
			else if (KuaFuServerManager.ConfigPathStructType == 2)
			{
				switch (resType)
				{
				case KuaFuServerManager.ResourcePathTypes.Application:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, DataHelper2.CurrentDirectory, fileName);
				case KuaFuServerManager.ResourcePathTypes.GameRes:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Isolate:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/ServerRes/1/IsolateRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Map:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/Map", fileName);
				case KuaFuServerManager.ResourcePathTypes.MapConfig:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/../MapConfig", fileName);
				}
			}
			else
			{
				switch (resType)
				{
				case KuaFuServerManager.ResourcePathTypes.Application:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, DataHelper2.CurrentDirectory, fileName);
				case KuaFuServerManager.ResourcePathTypes.GameRes:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Isolate:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/IsolateRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Map:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes/Map", fileName);
				case KuaFuServerManager.ResourcePathTypes.MapConfig:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes/MapConfig", fileName);
				}
			}
			return KuaFuServerManager.ResourcePath + "\\" + fileName;
		}

		
		public static int GetSpecialLineId(GameTypes gameType)
		{
			lock (KuaFuServerManager.Mutex)
			{
				int serverId;
				if (KuaFuServerManager.SpecialLineDict.TryGetValue((int)gameType, out serverId))
				{
					return serverId;
				}
			}
			return 0;
		}

		
		public static bool IsGongNengOpened(int gongNengID)
		{
			int versionGongNengId = gongNengID;
			if (versionGongNengId >= 100000 && versionGongNengId < 120000)
			{
				versionGongNengId -= 100000;
			}
			lock (KuaFuServerManager.Mutex)
			{
				if (!KuaFuServerManager.VersionSystemOpenMgr.IsVersionSystemOpen(versionGongNengId))
				{
					return false;
				}
			}
			return true;
		}

		
		public static List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			List<KuaFuServerInfo> result = null;
			lock (KuaFuServerManager.MutexServerList)
			{
				if (age < KuaFuServerManager._ServerListAge)
				{
					if (!KuaFuServerManager.OptimizationServerList)
					{
						return KuaFuServerManager._ServerIdServerInfoDict.Values.ToList<KuaFuServerInfo>();
					}
					result = new List<KuaFuServerInfo>(500);
					foreach (KuaFuServerInfo item in KuaFuServerManager._ServerIdServerInfoDict.Values)
					{
						if (item.ServerId % 10000 == item.Port % 10000)
						{
							result.Add(item);
						}
					}
				}
			}
			return result;
		}

		
		private static BuffServerListData RequestServerListData(string serverListUrl)
		{
			BuffServerListData result;
			try
			{
				ClientServerListData clientListData = new ClientServerListData();
				clientListData.lTime = TimeUtil.NOW();
				clientListData.strMD5 = MD5Helper.get_md5_string(ConstData.HTTP_MD5_KEY + clientListData.lTime.ToString());
				byte[] clientBytes = DataHelper2.ObjectToBytes<ClientServerListData>(clientListData);
				byte[] responseData = WebHelper.RequestByPost(serverListUrl, clientBytes, 10000, 30000);
				if (responseData == null)
				{
					result = null;
				}
				else
				{
					BuffServerListData serverListResponseData = DataHelper2.BytesToObject<BuffServerListData>(responseData, 0, responseData.Length);
					if (serverListResponseData == null || serverListResponseData.listServerData == null || serverListResponseData.listServerData.Count == 0)
					{
						result = null;
					}
					else
					{
						result = serverListResponseData;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			return result;
		}

		
		public static bool UpdateDataFromServer()
		{
			int nextServerListAge = DataHelper2.UnixSecondsNow();
			int nextServerGameConfigAge = DataHelper2.UnixSecondsNow();
			bool result;
			if (Math.Abs(nextServerListAge - KuaFuServerManager._ServerListAge) < 3)
			{
				result = false;
			}
			else if (!KuaFuServerManager.LoadConfigFromServer)
			{
				result = false;
			}
			else
			{
				if (Monitor.TryEnter(KuaFuServerManager.MutexServerList))
				{
					bool serverInfoChanged = false;
					HashSet<int> existAllIds = new HashSet<int>();
					HashSet<int> existKfIds = new HashSet<int>();
					List<BuffServerListData> ptServerList = new List<BuffServerListData>();
					try
					{
						List<Tuple<int, string, string, string>> kuaFuWorldPlatformServerListUrls = KuaFuServerManager.KuaFuWorldPlatformServerListUrls;
						foreach (Tuple<int, string, string, string> tuple in kuaFuWorldPlatformServerListUrls)
						{
							int ptid = tuple.Item1;
							string serverListUrl = tuple.Item2;
							string kuaFuServerListUrl = tuple.Item3;
							bool useLanIp = tuple.Item4 == "1";
							if (ptid == KuaFuServerManager.PTID || KuaFuServerManager.PingTaiKuaFu)
							{
								if (!string.IsNullOrEmpty(serverListUrl))
								{
									BuffServerListData serverListResponseData = KuaFuServerManager.RequestServerListData(serverListUrl);
									ptServerList.Add(serverListResponseData);
									if (null != serverListResponseData)
									{
										serverListResponseData.PTID = ptid;
										serverListResponseData.UseLanIP = useLanIp;
									}
								}
								if (!string.IsNullOrEmpty(kuaFuServerListUrl))
								{
									BuffServerListData kuaFuServerListResponseData = KuaFuServerManager.RequestServerListData(KuaFuServerManager.KuaFuServerListUrl);
									ptServerList.Add(kuaFuServerListResponseData);
									if (null != kuaFuServerListResponseData)
									{
										kuaFuServerListResponseData.PTID = ptid;
									}
									if (null != kuaFuServerListResponseData)
									{
										kuaFuServerListResponseData.PTID = ptid;
										kuaFuServerListResponseData.UseLanIP = useLanIp;
									}
								}
							}
						}
						if (ptServerList.Exists((BuffServerListData x) => x == null))
						{
							return false;
						}
						foreach (BuffServerListData list in ptServerList)
						{
							int baseKuaFuServerID = 0;
							if (KuaFuServerManager.PingTaiKuaFu)
							{
								baseKuaFuServerID = ConstData.ConvertToKuaFuServerID(0, list.PTID);
							}
							foreach (BuffServerInfo item in list.listServerData)
							{
								int localServerID = item.nServerID % 10000;
								item.nServerID = localServerID + baseKuaFuServerID;
								existAllIds.Add(item.nServerID);
								int flags;
								if (localServerID >= 9000)
								{
									flags = 1;
								}
								else
								{
									flags = 2;
								}
								KuaFuServerInfo data;
								if (KuaFuServerManager.UpdateServerInfo(item, KuaFuServerManager._ServerListAge, flags, out data, KuaFuServerManager._ServerIdServerInfoDict, list))
								{
									serverInfoChanged = true;
								}
							}
						}
						foreach (int id in KuaFuServerManager._ServerIdServerInfoDict.Keys)
						{
							if (!existAllIds.Contains(id))
							{
								KuaFuServerManager.RemoveServerInfo(id, KuaFuServerManager._ServerIdServerInfoDict);
								serverInfoChanged = true;
							}
						}
						if (serverInfoChanged)
						{
							KuaFuServerManager._ServerListAge = nextServerListAge;
						}
						foreach (KuaFuServerInfo item2 in KuaFuServerManager._ServerIdServerInfoDict.Values)
						{
							if ((item2.Flags & 1) > 0)
							{
								existKfIds.Add(item2.ServerId);
							}
							item2.Age = KuaFuServerManager._ServerListAge;
						}
						ClientAgentManager.Instance().SetAllKfServerId(existKfIds);
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
						return false;
					}
					finally
					{
						Monitor.Exit(KuaFuServerManager.MutexServerList);
					}
				}
				result = true;
			}
			return result;
		}

		
		public static void UpdateServerListAge()
		{
			lock (KuaFuServerManager.MutexServerList)
			{
				KuaFuServerManager._ServerListAge = TimeUtil.AgeByUnixTime(KuaFuServerManager._ServerListAge);
				long age = (long)DataHelper2.UnixSecondsNow();
				long now = TimeUtil.NOW();
				long now2 = TimeUtil.Before1970Ticks + age * 1000L;
				long c = (long)KuaFuServerManager._ServerListAge - age;
				long d = now2 - now;
			}
		}

		
		public static bool UpdateServerInfo(BuffServerInfo item, int ServerListAge, int serverFlags, out KuaFuServerInfo data, ConcurrentDictionary<int, KuaFuServerInfo> ServerIdServerInfoDict, BuffServerListData list)
		{
			bool serverInfoChanged = false;
			if (!ServerIdServerInfoDict.TryGetValue(item.nServerID, out data))
			{
				data = new KuaFuServerInfo
				{
					ServerId = item.nServerID,
					Age = ServerListAge,
					Flags = serverFlags,
					strServerName = item.strServerName,
					PTID = list.PTID
				};
				ServerIdServerInfoDict[item.nServerID] = data;
				serverInfoChanged = true;
			}
			string dbip = item.strURL;
			if (list.UseLanIP)
			{
				dbip = item.strLanIp;
			}
			if (data.Ip != item.strURL || data.Port != item.nServerPort || data.LanIp != item.strLanIp || data.strServerName != item.strServerName || data.PTID != list.PTID || data.DbIp != dbip)
			{
				data.Ip = item.strURL;
				data.Port = item.nServerPort;
				data.DbPort = item.nServerPort + 10000;
				data.LogDbPort = item.nServerPort + 20000;
				data.LanIp = item.strLanIp;
				data.strServerName = item.strServerName;
				data.Flags = serverFlags;
				data.Age = ServerListAge;
				data.PTID = list.PTID;
				data.DbIp = dbip;
				data.LogDbIp = dbip;
				serverInfoChanged = true;
			}
			if (serverInfoChanged)
			{
				try
				{
					DbHelperMySQL.ExecuteSql(string.Format("INSERT INTO t_server_info(serverid,ip,port,dbip,dbport,logdbip,logdbport,state,age,flags,strservername,ptid) VALUES({0},'{1}',{2},'{9}',{3},'{6}',{4},0,0,{5},'{7}','{8}') ON DUPLICATE KEY UPDATE `ip`='{1}',port={2},dbip='{9}',dbport={3},logdbip='{6}',logdbport={4},flags={5},strservername='{7}',ptid='{8}'", new object[]
					{
						data.ServerId,
						data.Ip,
						data.Port,
						data.DbPort,
						data.LogDbPort,
						data.Flags,
						data.LanIp,
						data.strServerName,
						data.PTID,
						data.DbIp
					}));
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.Message);
				}
			}
			return serverInfoChanged;
		}

		
		public static void RemoveServerInfo(int serverId, ConcurrentDictionary<int, KuaFuServerInfo> ServerIdServerInfoDict)
		{
			KuaFuServerInfo data;
			ServerIdServerInfoDict.TryRemove(serverId, out data);
			try
			{
				DbHelperMySQL.ExecuteSql(string.Format("delete from t_server_info where `serverid`={0}", serverId));
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		
		public static KuaFuServerInfo GetKuaFuServerInfo(int serverId)
		{
			lock (KuaFuServerManager.MutexServerList)
			{
				KuaFuServerInfo serverInfo;
				if (KuaFuServerManager._ServerIdServerInfoDict.TryGetValue(serverId, out serverInfo))
				{
					return serverInfo;
				}
			}
			return null;
		}

		
		public static KuaFuLoginInfo GetKuaFuLoginInfo(int serverId, int kfServerID)
		{
			KuaFuLoginInfo info = new KuaFuLoginInfo
			{
				ServerId = serverId,
				KuaFuServerId = kfServerID
			};
			lock (KuaFuServerManager.MutexServerList)
			{
				KuaFuServerInfo serverInfo;
				if (KuaFuServerManager._ServerIdServerInfoDict.TryGetValue(serverId, out serverInfo))
				{
					info.LocalIPs = new string[]
					{
						serverInfo.DbIp,
						serverInfo.DbIp
					};
					info.LocalPorts = new int[]
					{
						serverInfo.DbPort,
						serverInfo.LogDbPort
					};
				}
				if (KuaFuServerManager._ServerIdServerInfoDict.TryGetValue(kfServerID, out serverInfo))
				{
					info.KuaFuIP = serverInfo.Ip;
					info.KuaFuPort = serverInfo.Port;
				}
			}
			return info;
		}

		
		public static bool UpdateServerGameConfig(int serverId, int gameType, int capacity, ConcurrentDictionary<int, KuaFuServerGameConfig> KuaFuServerIdGameConfigDict)
		{
			bool serverGameConfigChanged = false;
			KuaFuServerGameConfig data;
			if (!KuaFuServerIdGameConfigDict.TryGetValue(serverId, out data))
			{
				data = new KuaFuServerGameConfig
				{
					ServerId = serverId,
					GameType = gameType
				};
				KuaFuServerIdGameConfigDict[serverId] = data;
				serverGameConfigChanged = true;
			}
			else if (data.Capacity != capacity)
			{
				data.Capacity = capacity;
				serverGameConfigChanged = true;
			}
			return serverGameConfigChanged;
		}

		
		public static int GetUniqueClientId()
		{
			int uniqueClientId = DataHelper2.UnixSecondsNow();
			lock (KuaFuServerManager.UniqueClientIdMutex)
			{
				KuaFuServerManager.UniqueClientId++;
				if (KuaFuServerManager.UniqueClientId < uniqueClientId)
				{
					KuaFuServerManager.UniqueClientId = uniqueClientId;
				}
				uniqueClientId = KuaFuServerManager.UniqueClientId;
			}
			return uniqueClientId;
		}

		
		public static int GetServerIDFromZoneID(int zoneID)
		{
			KuaFuServerInfo serverInfo;
			if (KuaFuServerManager._ServerIdServerInfoDict.TryGetValue(zoneID, out serverInfo))
			{
				int serverID = serverInfo.ServerId / 10000 * 10000 + serverInfo.Port % 10000;
				if (KuaFuServerManager._ServerIdServerInfoDict.ContainsKey(serverID))
				{
					return serverID;
				}
			}
			return zoneID;
		}

		
		public static string FormatName(int serverID, string name)
		{
			return KuaFuServerManager.FormatName(name, serverID);
		}

		
		public static string FormatName(string name, int serverID)
		{
			string result;
			if (KuaFuServerManager.ThemeActivityState == 0)
			{
				result = string.Format("S{0}·{1}", ConstData.ConvertToNormalServerID(serverID), name);
			}
			else
			{
				string zoneName;
				KuaFuServerInfo kuaFuServerInfo;
				for (;;)
				{
					lock (KuaFuServerManager.ZoneID2ZoneNameDict)
					{
						if (KuaFuServerManager.ZoneID2ZoneNameDict.TryGetValue(serverID, out zoneName))
						{
							goto IL_E7;
						}
					}
					kuaFuServerInfo = KuaFuServerManager.GetKuaFuServerInfo(serverID);
					if (kuaFuServerInfo != null && !string.IsNullOrEmpty(kuaFuServerInfo.strServerName))
					{
						break;
					}
					Thread.Sleep(100);
				}
				zoneName = kuaFuServerInfo.strServerName;
				lock (KuaFuServerManager.ZoneID2ZoneNameDict)
				{
					KuaFuServerManager.ZoneID2ZoneNameDict[serverID] = zoneName;
				}
				IL_E7:
				result = string.Format("[{0}]{1}", zoneName, name);
			}
			return result;
		}

		
		public static int EnterKuaFuMapLine(int line, int mapCode)
		{
			lock (KuaFuServerManager.Mutex)
			{
				KuaFuLineData kuaFuLineData;
				if (KuaFuServerManager.LineMap2KuaFuLineDataDict.TryGetValue(new IntPairKey(line, mapCode), out kuaFuLineData))
				{
					if (kuaFuLineData.OnlineCount < kuaFuLineData.MaxOnlineCount)
					{
						kuaFuLineData.OnlineCount++;
						return kuaFuLineData.ServerId;
					}
				}
			}
			return 0;
		}

		
		public static void UpdateKuaFuLineData(int serverId, Dictionary<int, int> mapClientCountDict)
		{
			if (null != mapClientCountDict)
			{
				lock (KuaFuServerManager.Mutex)
				{
					foreach (KeyValuePair<int, int> kv in mapClientCountDict)
					{
						KuaFuLineData kuaFuLineData;
						if (KuaFuServerManager.ServerMap2KuaFuLineDataDict.TryGetValue(new IntPairKey(serverId, kv.Key), out kuaFuLineData))
						{
							kuaFuLineData.OnlineCount = kv.Value;
							kuaFuLineData.State = 1;
						}
					}
				}
			}
		}

		
		public static void UpdateKuaFuMapLineState(int serverId, int state)
		{
			List<KuaFuLineData> list = null;
			lock (KuaFuServerManager.Mutex)
			{
				if (KuaFuServerManager.KuaFuMapServerIdDict.TryGetValue(serverId, out list))
				{
					foreach (KuaFuLineData kuaFuLineData in list)
					{
						kuaFuLineData.State = state;
					}
				}
			}
		}

		
		public static List<KuaFuLineData> GetKuaFuLineDataList(int mapCode)
		{
			lock (KuaFuServerManager.Mutex)
			{
				List<KuaFuLineData> list;
				if (KuaFuServerManager.MapCode2KuaFuLineDataDict.TryGetValue(mapCode, out list))
				{
					return list;
				}
			}
			return null;
		}

		
		public static bool WaitStop(int millisecondsTimeout = 0)
		{
			return KuaFuServerManager.StopEvent.Wait(millisecondsTimeout);
		}

		
		public static void OnStartServer()
		{
			try
			{
				KuaFuServerManager.FileWatcher.Path = Environment.CurrentDirectory;
				KuaFuServerManager.FileWatcher.Filter = "Server*.txt";
				KuaFuServerManager.FileWatcher.Changed += KuaFuServerManager.OnFileChanged;
				KuaFuServerManager.FileWatcher.NotifyFilter = NotifyFilters.LastWrite;
				KuaFuServerManager.FileWatcher.EnableRaisingEvents = true;
				KuaFuServerManager.FileWatcher.IncludeSubdirectories = false;
				KuaFuServerManager.WorkerThread.Start();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		
		public static void OnStopServer()
		{
			try
			{
				bool stop = false;
				lock (KuaFuServerManager.Mutex)
				{
					if (!KuaFuServerManager.ServerStop)
					{
						KuaFuServerManager.ServerStop = true;
						stop = true;
					}
				}
				if (stop)
				{
					CoupleArenaService.getInstance().OnStopServer();
					CoupleWishService.getInstance().OnStopServer();
					KuaFuLueDuoService.Instance().OnStopServer();
					CompService.Instance().OnStopServer();
					RebornService.Instance().OnStopServer();
					TianTiPersistence.Instance.OnStopServer();
					KuaFuServerManager.StopEvent.Signal();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		
		private static void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			lock (KuaFuServerManager.Mutex)
			{
				if (KuaFuServerManager.ServerStop)
				{
					return;
				}
			}
			if (e.ChangeType == WatcherChangeTypes.Changed)
			{
				if (e.Name.EndsWith("ServerStop.txt"))
				{
					string pidStr = File.ReadAllText(e.Name);
					if (pidStr.Trim().StartsWith(Process.GetCurrentProcess().Id.ToString()))
					{
						KuaFuServerManager.OnStopServer();
					}
				}
			}
		}

		
		public static void TimerThreadProc()
		{
			for (;;)
			{
				try
				{
					if (KuaFuServerManager.ServerStop)
					{
						break;
					}
					DateTime now = TimeUtil.NowDateTime();
					KuaFuLueDuoService.Instance().Update(now);
					Thread.Sleep(500);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
		}

		
		private const int ThreadInterval = 500;

		
		public static object Mutex = new object();

		
		private static object MutexServerList = new object();

		
		public static bool LoadConfigFromServer = false;

		
		public static int WritePerformanceLogMs = 1;

		
		public static int MapMaxOnlineCount = 500;

		
		public static int MaxGetAsyncItemDataCount = 10000;

		
		private static string ServerListUrl;

		
		private static string KuaFuServerListUrl;

		
		public static string[] GetPlatChargeKingUrl;

		
		public static string[] GetPlatChargeKingUrl_EveryDay;

		
		private static int _ServerListAge;

		
		private static Thread UpdateServerConfigThread;

		
		private static Thread CheckServerLoadThread;

		
		private static Thread WorkThread;

		
		private static Thread FastWorkThread;

		
		private static object UniqueClientIdMutex = new object();

		
		private static int UniqueClientId = 0;

		
		public static string ResourcePath;

		
		public static Dictionary<int, int> KuaFuMapLineDict = new Dictionary<int, int>();

		
		public static Dictionary<int, int> SpecialLineDict = new Dictionary<int, int>();

		
		public static Dictionary<int, int> PingTaiKuaFuServerLineDict = new Dictionary<int, int>();

		
		public static PlatformTypes platformType = PlatformTypes.Max;

		
		public static bool LoadConfigSuccess = true;

		
		public static bool LimitIP = true;

		
		public static bool UseLanIp = false;

		
		public static bool EnableGMSetAllServerTime;

		
		public static int ThemeActivityState;

		
		public static bool PingTaiKuaFu = false;

		
		public static bool PingTaiKuaFuTestMode = false;

		
		public static int PTID;

		
		public static int ServicePort;

		
		public static List<Tuple<int, string, string, string>> KuaFuWorldPlatformServerListUrls = new List<Tuple<int, string, string, string>>();

		
		public static ConcurrentDictionary<IntPairKey, KuaFuLineData> LineMap2KuaFuLineDataDict = new ConcurrentDictionary<IntPairKey, KuaFuLineData>();

		
		public static ConcurrentDictionary<IntPairKey, KuaFuLineData> ServerMap2KuaFuLineDataDict = new ConcurrentDictionary<IntPairKey, KuaFuLineData>();

		
		public static ConcurrentDictionary<int, List<KuaFuLineData>> KuaFuMapServerIdDict = new ConcurrentDictionary<int, List<KuaFuLineData>>();

		
		public static ConcurrentDictionary<int, List<KuaFuLineData>> MapCode2KuaFuLineDataDict = new ConcurrentDictionary<int, List<KuaFuLineData>>();

		
		public static VersionSystemOpenManager VersionSystemOpenMgr = new VersionSystemOpenManager();

		
		public static SystemParamsListKF systemParamsList = new SystemParamsListKF();

		
		private static int ConfigPathStructType = 1;

		
		public static char[] SpliteChars = new char[]
		{
			'-',
			',',
			'|',
			';'
		};

		
		private static GetKuaFuServerListRequestData KuaFuServerListRequestData = new GetKuaFuServerListRequestData();

		
		private static ConcurrentDictionary<int, KuaFuServerInfo> _ServerIdServerInfoDict = new ConcurrentDictionary<int, KuaFuServerInfo>();

		
		public static bool OptimizationServerList = false;

		
		private static Dictionary<int, string> ZoneID2ZoneNameDict = new Dictionary<int, string>();

		
		private static FileSystemWatcher FileWatcher = new FileSystemWatcher();

		
		private static bool ServerStop;

		
		private static CountdownEvent StopEvent = new CountdownEvent(1);

		
		private static Thread WorkerThread = new Thread(new ThreadStart(KuaFuServerManager.TimerThreadProc));

		
		public enum ResourcePathTypes
		{
			
			Application,
			
			GameRes,
			
			Isolate,
			
			Map,
			
			MapConfig
		}
	}
}
