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
	// Token: 0x0200003F RID: 63
	public static class KuaFuServerManager
	{
		// Token: 0x060002B5 RID: 693 RVA: 0x000269A4 File Offset: 0x00024BA4
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

		// Token: 0x060002B6 RID: 694 RVA: 0x00026B78 File Offset: 0x00024D78
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

		// Token: 0x060002B7 RID: 695 RVA: 0x000277A8 File Offset: 0x000259A8
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

		// Token: 0x060002B8 RID: 696 RVA: 0x00027AB0 File Offset: 0x00025CB0
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

		// Token: 0x060002B9 RID: 697 RVA: 0x00027C04 File Offset: 0x00025E04
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

		// Token: 0x060002BA RID: 698 RVA: 0x00027E40 File Offset: 0x00026040
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

		// Token: 0x060002BB RID: 699 RVA: 0x00028180 File Offset: 0x00026380
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

		// Token: 0x060002BC RID: 700 RVA: 0x000283BC File Offset: 0x000265BC
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

		// Token: 0x060002BD RID: 701 RVA: 0x00028424 File Offset: 0x00026624
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

		// Token: 0x060002BE RID: 702 RVA: 0x000284AC File Offset: 0x000266AC
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

		// Token: 0x060002BF RID: 703 RVA: 0x000285C0 File Offset: 0x000267C0
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

		// Token: 0x060002C0 RID: 704 RVA: 0x000286A8 File Offset: 0x000268A8
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

		// Token: 0x060002C1 RID: 705 RVA: 0x00028B64 File Offset: 0x00026D64
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

		// Token: 0x060002C2 RID: 706 RVA: 0x00028BE8 File Offset: 0x00026DE8
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

		// Token: 0x060002C3 RID: 707 RVA: 0x00028E64 File Offset: 0x00027064
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

		// Token: 0x060002C4 RID: 708 RVA: 0x00028EB8 File Offset: 0x000270B8
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

		// Token: 0x060002C5 RID: 709 RVA: 0x00028F20 File Offset: 0x00027120
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

		// Token: 0x060002C6 RID: 710 RVA: 0x00029018 File Offset: 0x00027218
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

		// Token: 0x060002C7 RID: 711 RVA: 0x0002907C File Offset: 0x0002727C
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

		// Token: 0x060002C8 RID: 712 RVA: 0x000290F8 File Offset: 0x000272F8
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

		// Token: 0x060002C9 RID: 713 RVA: 0x00029158 File Offset: 0x00027358
		public static string FormatName(int serverID, string name)
		{
			return KuaFuServerManager.FormatName(name, serverID);
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00029174 File Offset: 0x00027374
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

		// Token: 0x060002CB RID: 715 RVA: 0x00029298 File Offset: 0x00027498
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

		// Token: 0x060002CC RID: 716 RVA: 0x00029334 File Offset: 0x00027534
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

		// Token: 0x060002CD RID: 717 RVA: 0x000293FC File Offset: 0x000275FC
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

		// Token: 0x060002CE RID: 718 RVA: 0x000294A4 File Offset: 0x000276A4
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

		// Token: 0x060002CF RID: 719 RVA: 0x0002950C File Offset: 0x0002770C
		public static bool WaitStop(int millisecondsTimeout = 0)
		{
			return KuaFuServerManager.StopEvent.Wait(millisecondsTimeout);
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0002952C File Offset: 0x0002772C
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

		// Token: 0x060002D1 RID: 721 RVA: 0x000295C8 File Offset: 0x000277C8
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

		// Token: 0x060002D2 RID: 722 RVA: 0x000296A0 File Offset: 0x000278A0
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

		// Token: 0x060002D3 RID: 723 RVA: 0x00029758 File Offset: 0x00027958
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

		// Token: 0x04000173 RID: 371
		private const int ThreadInterval = 500;

		// Token: 0x04000174 RID: 372
		public static object Mutex = new object();

		// Token: 0x04000175 RID: 373
		private static object MutexServerList = new object();

		// Token: 0x04000176 RID: 374
		public static bool LoadConfigFromServer = false;

		// Token: 0x04000177 RID: 375
		public static int WritePerformanceLogMs = 1;

		// Token: 0x04000178 RID: 376
		public static int MapMaxOnlineCount = 500;

		// Token: 0x04000179 RID: 377
		public static int MaxGetAsyncItemDataCount = 10000;

		// Token: 0x0400017A RID: 378
		private static string ServerListUrl;

		// Token: 0x0400017B RID: 379
		private static string KuaFuServerListUrl;

		// Token: 0x0400017C RID: 380
		public static string[] GetPlatChargeKingUrl;

		// Token: 0x0400017D RID: 381
		public static string[] GetPlatChargeKingUrl_EveryDay;

		// Token: 0x0400017E RID: 382
		private static int _ServerListAge;

		// Token: 0x0400017F RID: 383
		private static Thread UpdateServerConfigThread;

		// Token: 0x04000180 RID: 384
		private static Thread CheckServerLoadThread;

		// Token: 0x04000181 RID: 385
		private static Thread WorkThread;

		// Token: 0x04000182 RID: 386
		private static Thread FastWorkThread;

		// Token: 0x04000183 RID: 387
		private static object UniqueClientIdMutex = new object();

		// Token: 0x04000184 RID: 388
		private static int UniqueClientId = 0;

		// Token: 0x04000185 RID: 389
		public static string ResourcePath;

		// Token: 0x04000186 RID: 390
		public static Dictionary<int, int> KuaFuMapLineDict = new Dictionary<int, int>();

		// Token: 0x04000187 RID: 391
		public static Dictionary<int, int> SpecialLineDict = new Dictionary<int, int>();

		// Token: 0x04000188 RID: 392
		public static Dictionary<int, int> PingTaiKuaFuServerLineDict = new Dictionary<int, int>();

		// Token: 0x04000189 RID: 393
		public static PlatformTypes platformType = PlatformTypes.Max;

		// Token: 0x0400018A RID: 394
		public static bool LoadConfigSuccess = true;

		// Token: 0x0400018B RID: 395
		public static bool LimitIP = true;

		// Token: 0x0400018C RID: 396
		public static bool UseLanIp = false;

		// Token: 0x0400018D RID: 397
		public static bool EnableGMSetAllServerTime;

		// Token: 0x0400018E RID: 398
		public static int ThemeActivityState;

		// Token: 0x0400018F RID: 399
		public static bool PingTaiKuaFu = false;

		// Token: 0x04000190 RID: 400
		public static bool PingTaiKuaFuTestMode = false;

		// Token: 0x04000191 RID: 401
		public static int PTID;

		// Token: 0x04000192 RID: 402
		public static int ServicePort;

		// Token: 0x04000193 RID: 403
		public static List<Tuple<int, string, string, string>> KuaFuWorldPlatformServerListUrls = new List<Tuple<int, string, string, string>>();

		// Token: 0x04000194 RID: 404
		public static ConcurrentDictionary<IntPairKey, KuaFuLineData> LineMap2KuaFuLineDataDict = new ConcurrentDictionary<IntPairKey, KuaFuLineData>();

		// Token: 0x04000195 RID: 405
		public static ConcurrentDictionary<IntPairKey, KuaFuLineData> ServerMap2KuaFuLineDataDict = new ConcurrentDictionary<IntPairKey, KuaFuLineData>();

		// Token: 0x04000196 RID: 406
		public static ConcurrentDictionary<int, List<KuaFuLineData>> KuaFuMapServerIdDict = new ConcurrentDictionary<int, List<KuaFuLineData>>();

		// Token: 0x04000197 RID: 407
		public static ConcurrentDictionary<int, List<KuaFuLineData>> MapCode2KuaFuLineDataDict = new ConcurrentDictionary<int, List<KuaFuLineData>>();

		// Token: 0x04000198 RID: 408
		public static VersionSystemOpenManager VersionSystemOpenMgr = new VersionSystemOpenManager();

		// Token: 0x04000199 RID: 409
		public static SystemParamsListKF systemParamsList = new SystemParamsListKF();

		// Token: 0x0400019A RID: 410
		private static int ConfigPathStructType = 1;

		// Token: 0x0400019B RID: 411
		public static char[] SpliteChars = new char[]
		{
			'-',
			',',
			'|',
			';'
		};

		// Token: 0x0400019C RID: 412
		private static GetKuaFuServerListRequestData KuaFuServerListRequestData = new GetKuaFuServerListRequestData();

		// Token: 0x0400019D RID: 413
		private static ConcurrentDictionary<int, KuaFuServerInfo> _ServerIdServerInfoDict = new ConcurrentDictionary<int, KuaFuServerInfo>();

		// Token: 0x0400019E RID: 414
		public static bool OptimizationServerList = false;

		// Token: 0x0400019F RID: 415
		private static Dictionary<int, string> ZoneID2ZoneNameDict = new Dictionary<int, string>();

		// Token: 0x040001A0 RID: 416
		private static FileSystemWatcher FileWatcher = new FileSystemWatcher();

		// Token: 0x040001A1 RID: 417
		private static bool ServerStop;

		// Token: 0x040001A2 RID: 418
		private static CountdownEvent StopEvent = new CountdownEvent(1);

		// Token: 0x040001A3 RID: 419
		private static Thread WorkerThread = new Thread(new ThreadStart(KuaFuServerManager.TimerThreadProc));

		// Token: 0x02000040 RID: 64
		public enum ResourcePathTypes
		{
			// Token: 0x040001AB RID: 427
			Application,
			// Token: 0x040001AC RID: 428
			GameRes,
			// Token: 0x040001AD RID: 429
			Isolate,
			// Token: 0x040001AE RID: 430
			Map,
			// Token: 0x040001AF RID: 431
			MapConfig
		}
	}
}
