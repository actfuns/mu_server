using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.AoYunDaTi;
using GameServer.Logic.BocaiSys;
using GameServer.Logic.BossAI;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.Damon;
using GameServer.Logic.ExtensionProps;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.GoldAuction;
using GameServer.Logic.Goods;
using GameServer.Logic.KuaFuIPStatistics;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.MoRi;
using GameServer.Logic.MUWings;
using GameServer.Logic.Name;
using GameServer.Logic.Olympics;
using GameServer.Logic.RefreshIconState;
using GameServer.Logic.SecondPassword;
using GameServer.Logic.TuJian;
using GameServer.Logic.UserMoneyCharge;
using GameServer.Logic.UserReturn;
using GameServer.Logic.YueKa;
using GameServer.Logic.ZhuanPan;
using GameServer.Server;
using GameServer.Tools;
using GameServer.Tools.CheckSysValue;
using KF.Contract.Data;
using KF.TcpCall;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer
{
	// Token: 0x02000860 RID: 2144
	public class Program : IConnectInfoContainer
	{
		// Token: 0x06003C1F RID: 15391
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleCtrlHandler(Program.ControlCtrlDelegate HandlerRoutine, bool Add);

		// Token: 0x06003C20 RID: 15392 RVA: 0x003365D8 File Offset: 0x003347D8
		public static bool HandlerRoutine(int CtrlType)
		{
			switch (CtrlType)
			{
			}
			return true;
		}

		// Token: 0x06003C21 RID: 15393
		[DllImport("user32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		// Token: 0x06003C22 RID: 15394
		[DllImport("user32.dll")]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

		// Token: 0x06003C23 RID: 15395
		[DllImport("user32.dll")]
		private static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

		// Token: 0x06003C24 RID: 15396 RVA: 0x00336608 File Offset: 0x00334808
		private static void HideCloseBtn()
		{
			Console.Title = "Server_" + Global.GetRandomNumber(0, 100000);
			IntPtr windowHandle = Program.FindWindow(null, Console.Title);
			IntPtr closeMenu = Program.GetSystemMenu(windowHandle, IntPtr.Zero);
			uint SC_CLOSE = 61536U;
			Program.RemoveMenu(closeMenu, SC_CLOSE, 0U);
		}

		// Token: 0x06003C25 RID: 15397 RVA: 0x00336660 File Offset: 0x00334860
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				Exception exception = e.ExceptionObject as Exception;
				DataHelper.WriteFormatExceptionLog(exception, "CurrentDomain_UnhandledException", UnhandedException.ShowErrMsgBox, true);
				if (Program.bDumpAndExit_ServerRunOk)
				{
					if (!Directory.Exists(Program.DumpBaseDir))
					{
						Directory.CreateDirectory(Program.DumpBaseDir);
					}
					SysConOut.WriteLine("");
					SysConOut.WriteLine("I had a problem, and i'm writting `dump` now, please wait for a moment...");
					Process process = Process.Start("C:\\Program Files\\Debugging Tools for Windows (x64)\\adplus.exe", "-hang -o " + Program.DumpBaseDir + " -p " + Process.GetCurrentProcess().Id.ToString());
					process.WaitForExit();
					Thread.Sleep(5000);
				}
			}
			catch
			{
			}
			finally
			{
				if (Program.bDumpAndExit_ServerRunOk)
				{
					Process.GetCurrentProcess().Kill();
					Process.GetCurrentProcess().WaitForExit();
				}
			}
		}

		// Token: 0x06003C26 RID: 15398 RVA: 0x00336760 File Offset: 0x00334960
		private static void ExceptionHook()
		{
			AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
		}

		// Token: 0x06003C27 RID: 15399 RVA: 0x0033677C File Offset: 0x0033497C
		public static void DeleteFile(string strFileName)
		{
			string strFullFileName = Directory.GetCurrentDirectory() + "\\" + strFileName;
			if (File.Exists(strFullFileName))
			{
				FileInfo fi = new FileInfo(strFullFileName);
				if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
				{
					fi.Attributes = FileAttributes.Normal;
				}
				File.Delete(strFullFileName);
			}
		}

		// Token: 0x06003C28 RID: 15400 RVA: 0x003367E8 File Offset: 0x003349E8
		public static void WritePIDToFile(string strFile)
		{
			string strFileName = Directory.GetCurrentDirectory() + "\\" + strFile;
			Process processes = Process.GetCurrentProcess();
			int nPID = processes.Id;
			File.WriteAllText(strFileName, string.Concat(nPID));
		}

		// Token: 0x06003C29 RID: 15401 RVA: 0x00336828 File Offset: 0x00334A28
		public static int GetServerPIDFromFile()
		{
			string strFileName = Directory.GetCurrentDirectory() + "\\GameServerStop.txt";
			int result;
			if (File.Exists(strFileName))
			{
				string str = File.ReadAllText(strFileName);
				result = int.Parse(str);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06003C2A RID: 15402 RVA: 0x0033686C File Offset: 0x00334A6C
		private static void Main(string[] args)
		{
			Program.DeleteFile("Start.txt");
			Program.DeleteFile("Stop.txt");
			Program.DeleteFile("GameServerStop.txt");
			args = Environment.GetCommandLineArgs();
			if (args.Contains("-gmsettime"))
			{
				GMCommands.EnableGMSetAllServerTime = true;
			}
			if (args.Contains("-testmode"))
			{
				Consts.TestMode = true;
			}
			if (!GCSettings.IsServerGC && Environment.ProcessorCount > 2)
			{
				SysConOut.WriteLine(string.Format("服务器GC运行在:{0}, {1}", GCSettings.IsServerGC ? "服务器模式" : "工作站模式", GCSettings.LatencyMode));
				Console.WriteLine("GC模式不正确,禁止启动,尝试自动设置为正确模式");
				string configFile = Process.GetCurrentProcess().MainModule.FileName + ".config";
				XElement xml = XElement.Load(configFile);
				XElement xml2 = xml.Element("runtime");
				if (null == xml2)
				{
					xml.SetElementValue("runtime", "");
					xml2 = xml.Element("runtime");
				}
				xml2.SetElementValue("gcServer", "");
				xml2.Element("gcServer").SetAttributeValue("enabled", "true");
				xml.Save(configFile);
				Console.WriteLine("自动设置为服务器模式,重新启动即可");
				Console.Read();
			}
			else
			{
				Program.HideCloseBtn();
				Program.SetConsoleCtrlHandler(Program.newDelegate, true);
				if (Console.WindowWidth < 88)
				{
					Console.BufferWidth = 88;
					Console.WindowWidth = 88;
				}
				Program.ExceptionHook();
				TimeUtil.Init();
				Program.InitCommonCmd();
				Global.CheckCodes();
				Program.OnStartServer();
				Program.ShowCmdHelpInfo(null);
				Program.WritePIDToFile("Start.txt");
				Program.bDumpAndExit_ServerRunOk = false;
				Thread thread = new Thread(new ParameterizedThreadStart(Program.ConsoleInputThread));
				thread.IsBackground = true;
				thread.Start();
				while (!Program.NeedExitServer || !Program.ServerConsole.MustCloseNow || Program.ServerConsole.MainDispatcherWorker.IsBusy)
				{
					Thread.Sleep(1000);
				}
				thread.Abort();
				Process.GetCurrentProcess().Kill();
			}
		}

		// Token: 0x06003C2B RID: 15403 RVA: 0x00336AD0 File Offset: 0x00334CD0
		public static void ConsoleInputThread(object obj)
		{
			while (!Program.NeedExitServer)
			{
				string cmd = Console.ReadLine();
				if (!string.IsNullOrEmpty(cmd))
				{
					if (cmd != null && 0 == cmd.CompareTo("exit"))
					{
						SysConOut.WriteLine("确认退出吗(输入 y 将立即退出)？");
						cmd = Console.ReadLine();
						if (0 == cmd.CompareTo("y"))
						{
							break;
						}
					}
					Program.ParseInputCmd(cmd);
				}
			}
			Program.OnExitServer();
		}

		// Token: 0x06003C2C RID: 15404 RVA: 0x00336B58 File Offset: 0x00334D58
		private static void ParseInputCmd(string cmd)
		{
			Program.CmdCallback cb = null;
			int index = cmd.IndexOf('/');
			string cmd2 = cmd;
			if (index > 0)
			{
				cmd2 = cmd.Substring(0, index - 1).TrimEnd(new char[0]);
			}
			if (Program.CmdDict.TryGetValue(cmd2, out cb) && null != cb)
			{
				cb(cmd);
			}
			else
			{
				SysConOut.WriteLine("未知命令,输入 help 查看具体命令信息");
			}
		}

		// Token: 0x06003C2D RID: 15405 RVA: 0x00336BCA File Offset: 0x00334DCA
		private static void OnStartServer()
		{
			Program.ServerConsole.InitServer();
			Console.Title = string.Format("游戏服务器{0}线@{1}@{2}", GameManager.ServerLineID, Program.GetVersionDateTime(), Program.ProgramExtName);
		}

		// Token: 0x06003C2E RID: 15406 RVA: 0x00336BFC File Offset: 0x00334DFC
		private static void OnExitServer()
		{
			Program.ServerConsole.ExitServer();
		}

		// Token: 0x06003C2F RID: 15407 RVA: 0x00336C0A File Offset: 0x00334E0A
		public static void Exit()
		{
			Program.NeedExitServer = true;
		}

		// Token: 0x06003C30 RID: 15408 RVA: 0x00336C34 File Offset: 0x00334E34
		private static void InitCommonCmd()
		{
			Program.CmdDict.Add("help", new Program.CmdCallback(Program.ShowCmdHelpInfo));
			Program.CmdDict.Add("gc", new Program.CmdCallback(Program.GarbageCollect));
			Program.CmdDict.Add("show dbconnect", new Program.CmdCallback(Program.ShowDBConnectInfo));
			Program.CmdDict.Add("show baseinfo", new Program.CmdCallback(Program.ShowServerBaseInfo));
			Program.CmdDict.Add("show tcpinfo", new Program.CmdCallback(Program.ShowServerTCPInfo));
			Program.CmdDict.Add("show copymapinfo", new Program.CmdCallback(Program.ShowCopyMapInfo));
			Program.CmdDict.Add("show gcinfo", new Program.CmdCallback(Program.ShowGCInfo));
			Program.CmdDict.Add("show roleinfo", new Program.CmdCallback(Program.ShowRoleInfo));
			Program.CmdDict.Add("list copymap", new Program.CmdCallback(Program.ListCopyMap));
			Program.CmdDict.Add("write map", new Program.CmdCallback(CheckSysValueHelper.WriteMap));
			Program.CmdDict.Add("check val", new Program.CmdCallback(CheckSysValueHelper.GetValue));
			Program.CmdDict.Add("testmode 5", new Program.CmdCallback(Program.SetTestMode));
			Program.CmdDict.Add("testmode 1", new Program.CmdCallback(Program.SetTestMode));
			Program.CmdDict.Add("testmode 0", new Program.CmdCallback(Program.SetTestMode));
			Program.CmdDict.Add("testkf 0", new Program.CmdCallback(S2KFCommunication.stop));
			Program.CmdDict.Add("testkf 1", new Program.CmdCallback(S2KFCommunication.start));
			Program.CmdDict.Add("patch", new Program.CmdCallback(Program.RunPatchFromConsole));
			Program.CmdDict.Add("show objinfo", new Program.CmdCallback(Program.ShowObjectInfo));
			Program.CmdDict.Add("clear", delegate(string x)
			{
				Console.Clear();
			});
			Program.CmdDict.Add("show magicactions", delegate(string x)
			{
				SystemMagicAction.PrintMaigcActionUsage();
			});
			Program.CmdDict.Add("report", delegate(string x)
			{
				GameManager.ServerMonitor.CheckReport();
			});
		}

		// Token: 0x06003C31 RID: 15409 RVA: 0x00336EC8 File Offset: 0x003350C8
		public static void LoadIPList(string strCmd)
		{
			try
			{
				if (string.IsNullOrEmpty(strCmd))
				{
					strCmd = GameManager.GameConfigMgr.GetGameConfigItemStr("whiteiplist", "");
				}
				LogManager.WriteLog(LogTypes.Error, string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", strCmd), null, true);
				bool enabeld = true;
				string[] ipList = strCmd.Split(new char[]
				{
					','
				});
				List<string> resultList = Global._TCPManager.MySocketListener.InitIPWhiteList(ipList, enabeld);
				if (resultList.Count > 0)
				{
					Console.WriteLine("IP白名单列表内容如下:");
					foreach (string ip in resultList)
					{
						Console.WriteLine(ip);
					}
				}
				else
				{
					Console.WriteLine("IP白名单为空,不限制IP登录");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("读取IP白名单异常,所以不限制IP登录.异常信息:\n" + ex.ToString());
			}
		}

		// Token: 0x06003C32 RID: 15410 RVA: 0x00336FE4 File Offset: 0x003351E4
		public static void CalcGCInfo()
		{
			long ticks = TimeUtil.NOW();
			for (int i = 0; i < 3; i++)
			{
				Program.GCCollectionCounts1[i] = GC.CollectionCount(i);
				if (Program.GCCollectionCounts[i] != 0)
				{
					int count = Program.GCCollectionCounts1[i] - Program.GCCollectionCounts[i];
					if (ticks >= Program.MaxGCCollectionCounts1sTicks[i] + 1000L)
					{
						if (count > Program.MaxGCCollectionCounts1s[i])
						{
							Program.MaxGCCollectionCounts1s[i] = count;
						}
						Program.MaxGCCollectionCounts1sTicks[i] = ticks;
					}
					if (ticks >= Program.MaxGCCollectionCounts5sTicks[i] + 5000L)
					{
						if (Program.GCCollectionCounts5[i] != 0)
						{
							int count5s = Program.GCCollectionCounts1[i] - Program.GCCollectionCounts5[i];
							if (count5s > Program.MaxGCCollectionCounts5s[i])
							{
								Program.MaxGCCollectionCounts5s[i] = count5s;
							}
						}
						Program.MaxGCCollectionCounts5sTicks[i] = ticks;
						Program.GCCollectionCounts5[i] = Program.GCCollectionCounts1[i];
					}
					Program.GCCollectionCountsNow[i] = count;
				}
				Program.GCCollectionCounts[i] = Program.GCCollectionCounts1[i];
			}
		}

		// Token: 0x06003C33 RID: 15411 RVA: 0x00337108 File Offset: 0x00335308
		private static void ShowGCInfo(string cmd = null)
		{
			try
			{
				Console.WriteLine(string.Format("GC计数类别    {0,-10} {1,-10} {2,-10}", "0 gen", "1 gen", "2 gen"));
				Console.WriteLine(string.Format("总计GC计数    {0,-10} {1,-10} {2,-10}", Program.GCCollectionCounts[0], Program.GCCollectionCounts[1], Program.GCCollectionCounts[2]));
				Console.WriteLine(string.Format("每秒GC计数    {0,-10} {1,-10} {2,-10}", Program.GCCollectionCountsNow[0], Program.GCCollectionCountsNow[1], Program.GCCollectionCountsNow[2]));
				Console.WriteLine(string.Format("1秒GC最大     {0,-10} {1,-10} {2,-10}", Program.MaxGCCollectionCounts1s[0], Program.MaxGCCollectionCounts1s[1], Program.MaxGCCollectionCounts1s[2]));
				Console.WriteLine(string.Format("5秒GC最大     {0,-10} {1,-10} {2,-10}", Program.MaxGCCollectionCounts5s[0], Program.MaxGCCollectionCounts5s[1], Program.MaxGCCollectionCounts5s[2]));
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ShowGCInfo()", false, false);
			}
		}

		// Token: 0x06003C34 RID: 15412 RVA: 0x00337230 File Offset: 0x00335430
		private static void ShowCmdHelpInfo(string cmd = null)
		{
			SysConOut.WriteLine(string.Format("游戏服务器{0}:", GameManager.ServerLineID));
			SysConOut.WriteLine("输入 help， 显示帮助信息");
			SysConOut.WriteLine("输入 exit， 然后输入y退出？");
			SysConOut.WriteLine("输入 gc， 执行垃圾回收");
			SysConOut.WriteLine("输入 show dbconnect， 查看数据库链接信息");
			SysConOut.WriteLine("输入 show baseinfo， 查看基础运行信息");
			SysConOut.WriteLine("输入 show tcpinfo， 查看通讯相关信息");
			SysConOut.WriteLine("输入 show copymapinfo， 查看副本相关信息");
			SysConOut.WriteLine("输入 show gcinfo， 查看GC相关信息");
			SysConOut.WriteLine("输入 write map 绘画静态成员关系图,如果本地无缓存或者调试新加的点击，不然无法模糊查询");
			SysConOut.WriteLine("输入 check val 查询系统变量值");
			SysConOut.WriteLine("输入 testkf 1 开启中心压力测试");
			SysConOut.WriteLine("输入 testkf 0 关闭中心压力测试");
		}

		// Token: 0x06003C35 RID: 15413 RVA: 0x003372DC File Offset: 0x003354DC
		private static void GarbageCollect(string cmd = null)
		{
			try
			{
				GC.Collect();
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "GarbageCollect()", false, false);
			}
		}

		// Token: 0x06003C36 RID: 15414 RVA: 0x00337318 File Offset: 0x00335518
		private static string ReadPasswd()
		{
			StringBuilder sb = new StringBuilder();
			for (;;)
			{
				ConsoleKeyInfo i = Console.ReadKey();
				if (i.Key == ConsoleKey.Enter)
				{
					break;
				}
				if (Console.CursorLeft > 0)
				{
					Console.CursorLeft--;
					Console.Write("*");
					sb.Append(i.KeyChar);
				}
			}
			return sb.ToString();
		}

		// Token: 0x06003C37 RID: 15415 RVA: 0x00337390 File Offset: 0x00335590
		private static void SetTestMode(string cmd = null)
		{
			if (!string.IsNullOrEmpty(cmd))
			{
				if ("tmsk201405" == Program.ReadPasswd())
				{
					if (cmd.IndexOf("testmode 5") == 0)
					{
						GameManager.TestGamePerformanceMode = true;
						GameManager.TestGamePerformanceAllPK = true;
						Console.WriteLine("开启压测模式,全体PK");
					}
					else if (cmd.IndexOf("testmode 1") == 0)
					{
						GameManager.TestGamePerformanceMode = true;
						GameManager.TestGamePerformanceAllPK = false;
						Console.WriteLine("开启压测模式,和平模式");
					}
					else
					{
						GameManager.TestGamePerformanceMode = false;
						GameManager.TestGamePerformanceAllPK = false;
						Console.WriteLine("关闭压测模式");
					}
				}
			}
		}

		// Token: 0x06003C38 RID: 15416 RVA: 0x00337444 File Offset: 0x00335644
		public static void RunPatchFromConsole(string cmd)
		{
			try
			{
				if (!string.IsNullOrEmpty(cmd))
				{
					if (!("tmsk201405" != Program.ReadPasswd()))
					{
						Console.WriteLine("输入补丁信息:");
						string arg = Console.ReadLine();
						Program.RunPatch(arg, true);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "执行修补程序异常");
			}
		}

		// Token: 0x06003C39 RID: 15417 RVA: 0x003374C0 File Offset: 0x003356C0
		public static void RunPatch(string arg, bool console = true)
		{
			try
			{
				if (!string.IsNullOrEmpty(arg))
				{
					if (!string.IsNullOrEmpty(arg))
					{
						char[] spliteChars = new char[]
						{
							' '
						};
						string[] args = arg.Split(spliteChars, StringSplitOptions.RemoveEmptyEntries);
						if (args != null && args.Length >= 3 && !string.IsNullOrEmpty(args[0]) && !string.IsNullOrEmpty(args[1]) && !string.IsNullOrEmpty(args[2]))
						{
							string assemblyName = DataHelper.CurrentDirectory + args[0];
							if (File.Exists(assemblyName))
							{
								Assembly t = Assembly.LoadFrom(assemblyName);
								if (null != t)
								{
									Type a = t.GetType(args[1]);
									if (null != a)
									{
										MethodInfo mi = a.GetMethod(args[2], BindingFlags.Static | BindingFlags.NonPublic);
										if (null != mi)
										{
											object[] param = new object[]
											{
												args
											};
											string s2 = (string)mi.Invoke(null, param);
											LogManager.WriteLog(LogTypes.SQL, "执行修补程序" + arg + ",结果:" + s2, null, true);
											if (console && s2 != null && s2.Length < 4096)
											{
												Console.WriteLine(s2);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "执行修补程序异常");
			}
		}

		// Token: 0x06003C3A RID: 15418 RVA: 0x00337660 File Offset: 0x00335860
		public static void ShowObjectInfo(string cmd)
		{
			try
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("在线玩家数{0}\n", GameManager.ClientMgr.GetClientCountFromDict());
				sb.AppendFormat("各地图人数\n{0}", GameManager.ClientMgr.GetAllMapRoleNumStr());
				sb.AppendFormat("地图对象引用的角色对象数\n{0}", GameManager.MapGridMgr.GetAllMapClientCountForConsole());
				sb.AppendLine("命令执行结束\n");
				Console.WriteLine(sb.ToString());
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "执行ShowGameClientInfo异常");
			}
		}

		// Token: 0x06003C3B RID: 15419 RVA: 0x003376FC File Offset: 0x003358FC
		private static void ShowDBConnectInfo(string cmd = null)
		{
			try
			{
				foreach (KeyValuePair<int, string> item in Program.ServerConsole.DBServerConnectDict)
				{
					SysConOut.WriteLine(item.Value);
				}
				foreach (KeyValuePair<int, string> item in Program.ServerConsole.LogDBServerConnectDict)
				{
					SysConOut.WriteLine(item.Value);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ShowDBConnectInfo()", false, false);
			}
		}

		// Token: 0x06003C3C RID: 15420 RVA: 0x003377D8 File Offset: 0x003359D8
		private static void ShowServerBaseInfo(string cmd = null)
		{
			SysConOut.WriteLine(string.Format("在线数量 {0}/{1}", GameManager.ClientMgr.GetClientCount(), Global._TCPManager.MySocketListener.ConnectedSocketsCount));
			int workerThreads = 0;
			int completionPortThreads = 0;
			ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
			SysConOut.WriteLine(string.Format("线程池信息 workerThreads={0}, completionPortThreads={1}", workerThreads, completionPortThreads));
			SysConOut.WriteLine(string.Format("TCP事件读写缓存数量 readPool={0}/{2}, writePool={1}/{2}", Global._TCPManager.MySocketListener.ReadPoolCount, Global._TCPManager.MySocketListener.WritePoolCount, Global._TCPManager.MySocketListener.numConnections * 3));
			SysConOut.WriteLine(string.Format("数据库指令数量 {0}", GameManager.DBCmdMgr.GetDBCmdCount()));
			SysConOut.WriteLine(string.Format("与DbServer的连接数量{0}/{1}", Global._TCPManager.tcpClientPool.GetPoolCount(), Global._TCPManager.tcpClientPool.InitCount));
			SysConOut.WriteLine(string.Format("TcpOutPacketPool个数:{0}, 实例: {1}, TcpInPacketPool个数:{2}, 实例: {3}, TCPCmdWrapper个数: {4}, SendCmdWrapper: {5}", new object[]
			{
				Global._TCPManager.TcpOutPacketPool.Count,
				TCPOutPacket.GetInstanceCount(),
				Global._TCPManager.TcpInPacketPool.Count,
				TCPInPacket.GetInstanceCount(),
				TCPCmdWrapper.GetTotalCount(),
				SendCmdWrapper.GetInstanceCount()
			}));
			string info = Global._MemoryManager.GetCacheInfoStr();
			SysConOut.WriteLine(info);
			info = Global._FullBufferManager.GetFullBufferInfoStr();
			SysConOut.WriteLine(info);
			info = Global._TCPManager.GetAllCacheCmdPacketInfo();
			SysConOut.WriteLine(info);
		}

		// Token: 0x06003C3D RID: 15421 RVA: 0x003379BC File Offset: 0x00335BBC
		private static void ShowServerTCPInfo(string cmd = null)
		{
			bool clear = cmd.Contains("/c");
			bool detail = cmd.Contains("/d");
			DateTime now = TimeUtil.NowDateTime();
			SysConOut.WriteLine(string.Format("当前时间:{0},统计时长:{1}", now.ToString("yyyy-MM-dd HH:mm:ss"), (now - ProcessSessionTask.StartTime).ToString()));
			if (clear)
			{
				detail = true;
				ProcessSessionTask.StartTime = now;
			}
			SysConOut.WriteLine(string.Format("总接收字节: {0:0.00} MB", (double)Global._TCPManager.MySocketListener.TotalBytesReadSize / 1048576.0));
			SysConOut.WriteLine(string.Format("总发送字节: {0:0.00} MB", (double)Global._TCPManager.MySocketListener.TotalBytesWriteSize / 1048576.0));
			SysConOut.WriteLine(string.Format("总处理指令个数 {0}", TCPCmdHandler.TotalHandledCmdsNum));
			SysConOut.WriteLine(string.Format("当前正在处理指令的线程数 {0}", TCPCmdHandler.GetHandlingCmdCount()));
			SysConOut.WriteLine(string.Format("单个指令消耗的最大时间 {0}", TCPCmdHandler.MaxUsedTicksByCmdID));
			SysConOut.WriteLine(string.Format("消耗的最大时间指令ID {0}", (TCPGameServerCmds)TCPCmdHandler.MaxUsedTicksCmdID));
			SysConOut.WriteLine(string.Format("发送调用总次数 {0}", Global._TCPManager.MySocketListener.GTotalSendCount));
			SysConOut.WriteLine(string.Format("发送的最大包的大小 {0}", Global._SendBufferManager.MaxOutPacketSize));
			SysConOut.WriteLine(string.Format("发送的最大包的指令ID {0}", (TCPGameServerCmds)Global._SendBufferManager.MaxOutPacketSizeCmdID));
			SysConOut.WriteLine(string.Format("指令处理平均耗时（毫秒）{0}", (ProcessSessionTask.processCmdNum != 0L) ? TimeUtil.TimeMS(ProcessSessionTask.processTotalTime / ProcessSessionTask.processCmdNum, 2) : 0.0));
			SysConOut.WriteLine(string.Format("指令处理耗时详情", new object[0]));
			try
			{
				if (detail)
				{
					if (Console.WindowWidth < 160)
					{
						Console.WindowWidth = 160;
					}
				}
				else if (Console.WindowWidth >= 88)
				{
					Console.WindowWidth = 88;
				}
			}
			catch
			{
			}
			int count = 0;
			lock (ProcessSessionTask.cmdMoniter)
			{
				List<Tuple<string, string>> outputList = new List<Tuple<string, string>>();
				foreach (PorcessCmdMoniter i in ProcessSessionTask.cmdMoniter.Values)
				{
					if (i.processNum > 0 || i.SendNum > 0L)
					{
						if (detail)
						{
							string info;
							if (count++ == 0)
							{
								info = string.Format("{0, -48}{1, 6}{2, 7}{3, 7} {4, 7} {5, 4} {6, 4} {7, 5}", new object[]
								{
									"消息",
									"已处理次数",
									"平均处理时长",
									"总计消耗时长",
									"总计字节数",
									"发送次数",
									"发送字节数",
									"失败/成功/数据"
								});
								outputList.Add(new Tuple<string, string>("", info));
							}
							info = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##} {4, 13:0.##} {5, 8} {6, 12} {7, 4}/{8}/{9}", new object[]
							{
								(TCPGameServerCmds)i.cmd,
								i.processNum,
								TimeUtil.TimeMS(i.avgProcessTime(), 2),
								TimeUtil.TimeMS(i.processTotalTime, 2),
								i.GetTotalBytes(),
								i.SendNum,
								i.OutPutBytes,
								i.Num_Faild,
								i.Num_OK,
								i.Num_WithData
							});
							outputList.Add(new Tuple<string, string>(((TCPGameServerCmds)i.cmd).ToString(), info));
						}
						else
						{
							string info;
							if (count++ == 0)
							{
								info = string.Format("{0, -48}{1, 6}{2, 7}{3, 7}", new object[]
								{
									"消息",
									"已处理次数",
									"平均处理时长",
									"总计消耗时长"
								});
								outputList.Add(new Tuple<string, string>("", info));
							}
							info = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##}", new object[]
							{
								(TCPGameServerCmds)i.cmd,
								i.processNum,
								TimeUtil.TimeMS(i.avgProcessTime(), 2),
								TimeUtil.TimeMS(i.processTotalTime, 2)
							});
							outputList.Add(new Tuple<string, string>(((TCPGameServerCmds)i.cmd).ToString(), info));
						}
					}
					if (clear)
					{
						i.Reset();
					}
				}
				outputList.Sort((Tuple<string, string> x, Tuple<string, string> y) => string.Compare(x.Item1, y.Item1));
				foreach (Tuple<string, string> item in outputList)
				{
					Console.ForegroundColor = count++ / 2 % 2 + ConsoleColor.Yellow;
					SysConOut.WriteLine(item.Item2);
				}
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		// Token: 0x06003C3E RID: 15422 RVA: 0x00338008 File Offset: 0x00336208
		private static void ShowCopyMapInfo(string cmd = null)
		{
			string info = GameManager.CopyMapMgr.GetCopyMapStrInfo();
			SysConOut.WriteLine(info);
		}

		// Token: 0x06003C3F RID: 15423 RVA: 0x00338028 File Offset: 0x00336228
		private static void ListCopyMap(string cmd = null)
		{
			string info = GameManager.CopyMapMgr.ListCopyMapStrInfo();
			SysConOut.WriteLine(info);
		}

		// Token: 0x06003C40 RID: 15424 RVA: 0x00338048 File Offset: 0x00336248
		private static void ShowRoleInfo(string cmd = null)
		{
			StringBuilder sb = new StringBuilder();
			int count = GameManager.ClientMgr.GetMaxClientCount();
			for (int i = 0; i < count; i++)
			{
				GameClient client = GameManager.ClientMgr.FindClientByNid(i);
				if (null != client)
				{
					sb.AppendFormat("{0, -12} : {4, -3} : {5, -8} : {6, -8} : {7, -8} : {1}({2},{3})\n", new object[]
					{
						client.ClientData.RoleName,
						client.ClientData.MapCode,
						client.ClientData.PosX,
						client.ClientData.PosY,
						client.CodeRevision,
						client.MainExeVer,
						client.ResVer,
						client.ClientSocket.ClientCmdSecs
					});
				}
			}
			if (sb.Length == 0)
			{
				SysConOut.WriteLine("没有玩家在线");
			}
			else
			{
				SysConOut.WriteLine(sb.ToString());
			}
		}

		// Token: 0x06003C41 RID: 15425 RVA: 0x00338170 File Offset: 0x00336370
		public void AddDBConnectInfo(int index, string info)
		{
			lock (this.DBServerConnectDict)
			{
				if (this.DBServerConnectDict.ContainsKey(index))
				{
					this.DBServerConnectDict[index] = info;
				}
				else
				{
					this.DBServerConnectDict.Add(index, info);
				}
			}
		}

		// Token: 0x06003C42 RID: 15426 RVA: 0x003381EC File Offset: 0x003363EC
		public void AddLogDBConnectInfo(int index, string info)
		{
			lock (this.LogDBServerConnectDict)
			{
				if (this.LogDBServerConnectDict.ContainsKey(index))
				{
					this.LogDBServerConnectDict[index] = info;
				}
				else
				{
					this.LogDBServerConnectDict.Add(index, info);
				}
			}
		}

		// Token: 0x06003C43 RID: 15427 RVA: 0x00338268 File Offset: 0x00336468
		private static void InitProgramExtName()
		{
			Program.ProgramExtName = DataHelper.CurrentDirectory;
		}

		// Token: 0x06003C44 RID: 15428 RVA: 0x003382B4 File Offset: 0x003364B4
		public void InitServer()
		{
			Program.InitProgramExtName();
			int workerThreads = 0;
			int completionPortThreads = 0;
			ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
			ThreadPool.SetMinThreads(Math.Max(workerThreads, 1), Math.Max(completionPortThreads, 64));
			ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
			ThreadPool.SetMaxThreads(Math.Min(workerThreads, 1), Math.Min(completionPortThreads, 360));
			if (!File.Exists("Policy.xml"))
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", "Policy.xml"));
			}
			TCPPolicy.LoadPolicyServerFile("Policy.xml");
			Global.LoadLangDict();
			SearchTable.Init(14);
			SysConOut.WriteLine("正在初始化游戏资源目录");
			XElement xml = this.InitGameResPath();
			try
			{
				SysConOut.WriteLine("正在初始化数据库连接");
				this.InitTCPManager(xml, true);
				SysConOut.WriteLine("从数据库中获取配置参数");
				GameManager.GameConfigMgr.LoadGameConfigFromDBServer();
				this.InitGameConfigWithDB();
				KFCallManager.Start();
				SysConOut.WriteLine("正在初始化GameRes压缩资源");
				this.InitGameRes();
				SysConOut.WriteLine("载入世界等级");
				WorldLevelManager.getInstance().InitConfig();
				WorldLevelManager.getInstance().ResetWorldLevel();
				SysConOut.WriteLine("正在初始化游戏管理对象");
				this.InitGameManager(xml);
				LuaExeManager.getInstance().InitLuaEnv();
				SysConOut.WriteLine("正在初始化游戏的所有地图和地图中的怪物");
				this.InitGameMapsAndMonsters();
				Data.LoadConfig();
				SingletonTemplate<CreateRoleLimitManager>.Instance().LoadConfig();
				SysConOut.WriteLine("正在初始化活动管理器");
				GlobalServiceManager.initialize();
				GlobalServiceManager.startup();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Process.GetCurrentProcess().Kill();
			}
			SysConOut.WriteLine("正在设置后台工作者线程");
			this.eventWorker = new BackgroundWorker();
			this.eventWorker.DoWork += this.eventWorker_DoWork;
			this.dbCommandWorker = new BackgroundWorker();
			this.dbCommandWorker.DoWork += this.dbCommandWorker_DoWork;
			this.logDBCommandWorker = new BackgroundWorker();
			this.logDBCommandWorker.DoWork += this.logDBCommandWorker_DoWork;
			this.clientsWorker = new BackgroundWorker();
			this.clientsWorker.DoWork += new DoWorkEventHandler(this.clientsWorker_DoWork);
			this.buffersWorker = new BackgroundWorker();
			this.buffersWorker.DoWork += new DoWorkEventHandler(this.buffersWorker_DoWork);
			this.spriteDBWorker = new BackgroundWorker();
			this.spriteDBWorker.DoWork += new DoWorkEventHandler(this.spriteDBWorker_DoWork);
			this.othersWorker = new BackgroundWorker();
			this.othersWorker.DoWork += new DoWorkEventHandler(this.othersWorker_DoWork);
			this.FightingWorker = new BackgroundWorker();
			this.FightingWorker.DoWork += new DoWorkEventHandler(this.FightingWorker_DoWork);
			this.chatMsgWorker = new BackgroundWorker();
			this.chatMsgWorker.DoWork += new DoWorkEventHandler(this.chatMsgWorker_DoWork);
			this.fuBenWorker = new BackgroundWorker();
			this.fuBenWorker.DoWork += new DoWorkEventHandler(this.fuBenWorker_DoWork);
			this.dbWriterWorker = new BackgroundWorker();
			this.dbWriterWorker.DoWork += new DoWorkEventHandler(this.dbWriterWorker_DoWork);
			this.SocketSendCacheDataWorker = new BackgroundWorker();
			this.SocketSendCacheDataWorker.DoWork += new DoWorkEventHandler(this.SocketSendCacheDataWorker_DoWork);
			this.ShengXiaoGuessWorker = new BackgroundWorker();
			this.ShengXiaoGuessWorker.DoWork += new DoWorkEventHandler(this.ShengXiaoGuessWorker_DoWork);
			this.MainDispatcherWorker = new BackgroundWorker();
			this.MainDispatcherWorker.DoWork += new DoWorkEventHandler(this.MainDispatcherWorker_DoWork);
			this.socketCheckWorker = new BackgroundWorker();
			this.socketCheckWorker.DoWork += new DoWorkEventHandler(this.SocketCheckWorker_DoWork);
			this.dynamicMonstersWorker = new BackgroundWorker();
			this.dynamicMonstersWorker.DoWork += new DoWorkEventHandler(this.DynamicMonstersWorker_DoWork);
			this.BanWorker = new BackgroundWorker();
			this.BanWorker.DoWork += new DoWorkEventHandler(this.LoadBanWorker_DoWork);
			this.TwLogWorker = new BackgroundWorker();
			this.TwLogWorker.DoWork += this.TwLogWorker_DoWork;
			this.IPStatisticsWorker = new BackgroundWorker();
			this.IPStatisticsWorker.DoWork += this.IPStatisticsWorker_DoWork;
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("MapGridMagicHelper.ExecuteAllItemsEx()", delegate(object s, EventArgs e)
			{
				GameManager.GridMagicHelperMgrEx.ExecuteAllItemsEx();
			}), 1000, 200);
			for (int i = 0; i < GameManager.MapMgr.DictMaps.Values.Count; i++)
			{
				int mapCode = GameManager.MapMgr.DictMaps.Values.ElementAt(i).MapCode;
				if (mapCode == 6090)
				{
					for (int subMapCode = 0; subMapCode < 25; subMapCode++)
					{
					}
				}
				else
				{
					ScheduleExecutor2.Instance.scheduleExecute(new MonsterTask(mapCode, -1), 0, 80);
				}
			}
			this.Gird9UpdateWorkers = new BackgroundWorker[Program.MaxGird9UpdateWorkersNum];
			for (int nThread = 0; nThread < Program.MaxGird9UpdateWorkersNum; nThread++)
			{
				this.Gird9UpdateWorkers[nThread] = new BackgroundWorker();
				this.Gird9UpdateWorkers[nThread].DoWork += new DoWorkEventHandler(this.Gird9UpdateWorker_DoWork);
			}
			if (GameManager.IsKuaFuServer)
			{
				for (int i = 0; i < 5; i++)
				{
					int index = i;
					ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("DoSpriteExtensionWork", delegate(object s, EventArgs e)
					{
						GameManager.ClientMgr.DoSpriteExtensionWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, index, 5);
					}), 30000 + i, 100);
				}
			}
			this.RoleStroyboardDispatcherWorker = new BackgroundWorker();
			this.RoleStroyboardDispatcherWorker.DoWork += this.RoleStroyboardDispatcherWorker_DoWork;
			UnhandedException.ShowErrMsgBox = false;
			Global._TCPManager.MySocketListener.DontAccept = false;
			if (!this.MainDispatcherWorker.IsBusy)
			{
				this.MainDispatcherWorker.RunWorkerAsync();
			}
			if (!this.dynamicMonstersWorker.IsBusy)
			{
				this.dynamicMonstersWorker.RunWorkerAsync();
			}
			for (int nThread = 0; nThread < Program.MaxGird9UpdateWorkersNum; nThread++)
			{
				if (!this.Gird9UpdateWorkers[nThread].IsBusy)
				{
					this.Gird9UpdateWorkers[nThread].RunWorkerAsync(nThread);
				}
			}
			if (!this.RoleStroyboardDispatcherWorker.IsBusy)
			{
				this.RoleStroyboardDispatcherWorker.RunWorkerAsync();
			}
			Program.StartThreadPoolDriverTimer();
			GameManager.GameConfigMgr.SetGameConfigItem("gameserver_version", Program.GetVersionDateTime());
			Global.UpdateDBGameConfigg("gameserver_version", Program.GetVersionDateTime());
			GameManager.ArenaBattleMgr.ReShowPKKing();
			LuoLanChengZhanManager.getInstance().ReShowLuolanKing(0);
			SysConOut.WriteLine("正在初始化通信监听");
			Thread.Sleep(3000);
			this.InitTCPManager(xml, false);
			GroupMailManager.RequestNewGroupMailList();
			SysConOut.WriteLine(string.Format("服务器GC运行在:{0}, {1}", GCSettings.IsServerGC ? "服务器模式" : "工作站模式", GCSettings.LatencyMode));
			SysConOut.WriteLine("服务器已经正常启动");
			GameManager.ServerStarting = false;
		}

		// Token: 0x06003C45 RID: 15429 RVA: 0x00338A04 File Offset: 0x00336C04
		public void ExitServer()
		{
			if (!Program.NeedExitServer)
			{
				GlobalServiceManager.showdown();
				GlobalServiceManager.destroy();
				Global._TCPManager.Stop();
				this.Window_Closing();
				BoCaiBuy2DBList.getInstance().SoptServer();
				SysConOut.WriteLine("正在尝试关闭服务器,看到服务器关闭完毕提示后回车退出系统");
				if (0 == Program.GetServerPIDFromFile())
				{
					string cmd = Console.ReadLine();
					while (this.MainDispatcherWorker.IsBusy)
					{
						SysConOut.WriteLine("正在尝试关闭服务器");
						cmd = Console.ReadLine();
					}
					Program.StopThreadPoolDriverTimer();
				}
			}
		}

		// Token: 0x06003C46 RID: 15430 RVA: 0x00338AA8 File Offset: 0x00336CA8
		private XElement InitGameResPath()
		{
			XElement xml = null;
			try
			{
				xml = XElement.Load("AppConfig.xml");
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", "AppConfig.xml"));
			}
			Global.AbsoluteGameResPath = Global.GetSafeAttributeStr(xml, "Resource", "Path");
			string appPath = DataHelper.CurrentDirectory;
			if (Global.AbsoluteGameResPath.IndexOf("$SERVER$") >= 0)
			{
				Global.AbsoluteGameResPath = Global.AbsoluteGameResPath.Replace("$SERVER$", appPath);
			}
			if (!string.IsNullOrEmpty(Global.AbsoluteGameResPath))
			{
				Global.AbsoluteGameResPath = Global.AbsoluteGameResPath.Replace("\\", "/");
				Global.AbsoluteGameResPath = Global.AbsoluteGameResPath.TrimEnd(new char[]
				{
					'/'
				});
			}
			Global.CheckConfigPathType();
			return xml;
		}

		// Token: 0x06003C47 RID: 15431 RVA: 0x00338BCC File Offset: 0x00336DCC
		private void InitGameRes()
		{
			try
			{
				Global.AddXElement("ConfigSettings", Global.GetGameResXml("Config/Settings.xml"));
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败 错误信息:{1}", "Config/Settings.xml", ex.Message));
			}
			try
			{
				Global.AddXElement("ConfigLevelUp", Global.GetGameResXml("Config/LevelUp.xml"));
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", "Config/LevelUp.xml"));
			}
			try
			{
				Global.AddXElement("Configgoods", Global.GetGameResXml("Config/Goods.xml"));
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", "Config/Goods.xml"));
			}
			Data.WalkUnitCost = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpeedConfig"), "WalkUnitCost");
			Data.RunUnitCost = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpeedConfig"), "RunUnitCost");
			string[] ticks = Global.GetSafeAttribute(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpeedConfig"), "Tick").Value.Split(new char[]
			{
				','
			});
			Data.SpeedTickList = new int[ticks.Length];
			for (int j = 0; j < ticks.Length; j++)
			{
				Data.SpeedTickList[j] = Convert.ToInt32(ticks[j]);
			}
			XElement distConfig = Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "DistanceConfig");
			Data.WalkStepWidth = (int)Global.GetSafeAttributeLong(distConfig, "WalkStepWidth");
			Data.RunStepWidth = (int)Global.GetSafeAttributeLong(distConfig, "RunStepWidth");
			Data.MaxAttackDistance = (int)Global.GetSafeAttributeLong(distConfig, "MaxAttackDistance");
			Data.MinAttackDistance = (int)Global.GetSafeAttributeLong(distConfig, "MinAttackDistance");
			Data.MaxMagicDistance = (int)Global.GetSafeAttributeLong(distConfig, "MaxMagicDistance");
			Data.MaxAttackSlotTick = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpeedConfig"), "MaxAttackSlotTick");
			XElement SpriteConfig = Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpriteConfig");
			Data.LifeTotalWidth = (int)Global.GetSafeAttributeLong(SpriteConfig, "LifeTotalWidth");
			Data.HoldWidth = (int)Global.GetSafeAttributeLong(SpriteConfig, "HoldWidth");
			Data.HoldHeight = (int)Global.GetSafeAttributeLong(SpriteConfig, "HoldHeight");
			Data.GoodsPackOvertimeTick = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "GoodsPack"), "MaxOvertimeTick");
			Data.PackDestroyTimeTick = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "GoodsPack"), "PackDestroyTimeTick");
			Data.TaskMaxFocusCount = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Task"), "MaxFocusNum");
			Data.AliveGoodsID = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Alive"), "GoodsID");
			Data.AliveMaxLevel = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Alive"), "MaxLevel");
			Data.AutoGetThing = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Bag"), "AutoGetThing");
			int maxLevel = 0;
			IEnumerable<XElement> levelList = Global.XmlInfo["ConfigLevelUp"].Elements("Experience");
			if (null != levelList)
			{
				int listCount = levelList.Count<XElement>();
				maxLevel = listCount;
				Data.LevelUpExperienceList = new long[listCount];
				int i;
				for (i = 0; i < listCount; i++)
				{
					Data.LevelUpExperienceList[i] = Convert.ToInt64(levelList.Single((XElement X) => X.Attribute("Level").Value == i.ToString()).Attribute("Value").Value);
				}
			}
			this.LoadRoleSitExpList(maxLevel);
			this.LoadRoleBasePropItems(maxLevel);
			this.LoadRoleZhuanZhiInfo();
			GameManager.ChangeLifeMgr.LoadRoleZhuanShengInfo();
			this.LoadRoleOccupationAddPointInfo();
			this.LoadRoleChangeLifeAddPointInfo();
			WeaponAdornManager.LoadWeaponAdornInfo();
			this.LoadBloodCastleDataInfo();
			this.LoadMoBaiDataInfo();
			this.InitMapStallPosList();
			this.InitMapNameDictionary();
		}

		// Token: 0x06003C48 RID: 15432 RVA: 0x00339058 File Offset: 0x00337258
		private void LoadRoleSitExpList(int maxLevel)
		{
			Data.RoleSitExpList = new RoleSitExpItem[maxLevel];
			XElement xmlFile = null;
			try
			{
				xmlFile = Global.GetGameResXml(string.Format("Config/RoleSiteExp.xml", new object[0]));
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/RoleSiteExp.xml", new object[0])));
			}
			IEnumerable<XElement> xmlItems = xmlFile.Elements();
			foreach (XElement xmlItem in xmlItems)
			{
				int level = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
				if (level >= Data.RoleSitExpList.Length)
				{
					break;
				}
				Data.RoleSitExpList[level] = new RoleSitExpItem
				{
					Level = level,
					Experience = (int)Global.GetSafeAttributeLong(xmlItem, "Experience"),
					InterPower = (int)Global.GetSafeAttributeLong(xmlItem, "InterPower"),
					SkilledDegrees = (int)Global.GetSafeAttributeLong(xmlItem, "SkilledDegrees"),
					PKPoint = (int)Global.GetSafeAttributeLong(xmlItem, "PkPoints")
				};
			}
		}

		// Token: 0x06003C49 RID: 15433 RVA: 0x0033919C File Offset: 0x0033739C
		private void LoadRoleBasePropItems(int maxLevel)
		{
			int i = 0;
			while (i < 6)
			{
				RoleBasePropItem[] roleBasePropItems = new RoleBasePropItem[maxLevel];
				XElement xmlFile = null;
				try
				{
					xmlFile = Global.GetGameResXml(string.Format("Config/Roles/{0}.xml", i));
				}
				catch (Exception ex)
				{
					if (i != 4)
					{
						throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/{0}.xml", i)));
					}
					Data.RoleBasePropList.Add(roleBasePropItems);
					goto IL_38B;
				}
				goto IL_69;
				IL_38B:
				i++;
				continue;
				IL_69:
				int j = 0;
				IEnumerable<XElement> propLevels = xmlFile.Elements("Levels").Elements<XElement>();
				foreach (XElement xmlItem in propLevels)
				{
					double[] arrRoleExtProp = new double[177];
					for (int k = 0; k < 177; k++)
					{
						arrRoleExtProp[k] = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, ((ExtPropIndexes)k).ToString(), "0.0"));
					}
					arrRoleExtProp[13] = Global.GetSafeAttributeDouble(xmlItem, "LifeV");
					arrRoleExtProp[15] = Global.GetSafeAttributeDouble(xmlItem, "MagicV");
					arrRoleExtProp[3] = Global.GetSafeAttributeDouble(xmlItem, "MinDefenseV");
					arrRoleExtProp[4] = Global.GetSafeAttributeDouble(xmlItem, "MaxDefenseV");
					arrRoleExtProp[5] = Global.GetSafeAttributeDouble(xmlItem, "MinMDefenseV");
					arrRoleExtProp[6] = Global.GetSafeAttributeDouble(xmlItem, "MaxMDefenseV");
					arrRoleExtProp[7] = Global.GetSafeAttributeDouble(xmlItem, "MinAttackV");
					arrRoleExtProp[8] = Global.GetSafeAttributeDouble(xmlItem, "MaxAttackV");
					arrRoleExtProp[9] = Global.GetSafeAttributeDouble(xmlItem, "MinMAttackV");
					arrRoleExtProp[10] = Global.GetSafeAttributeDouble(xmlItem, "MaxMAttackV");
					arrRoleExtProp[22] = Global.GetSafeAttributeDouble(xmlItem, "RecoverLifeV");
					arrRoleExtProp[23] = Global.GetSafeAttributeDouble(xmlItem, "RecoverMagicV");
					arrRoleExtProp[88] = 0.0;
					arrRoleExtProp[89] = 0.0;
					arrRoleExtProp[2] = 1.0;
					roleBasePropItems[j] = new RoleBasePropItem
					{
						arrRoleExtProp = arrRoleExtProp,
						LifeV = Global.GetSafeAttributeDouble(xmlItem, "LifeV"),
						MagicV = Global.GetSafeAttributeDouble(xmlItem, "MagicV"),
						MinDefenseV = Global.GetSafeAttributeDouble(xmlItem, "MinDefenseV"),
						MaxDefenseV = Global.GetSafeAttributeDouble(xmlItem, "MaxDefenseV"),
						MinMDefenseV = Global.GetSafeAttributeDouble(xmlItem, "MinMDefenseV"),
						MaxMDefenseV = Global.GetSafeAttributeDouble(xmlItem, "MaxMDefenseV"),
						MinAttackV = Global.GetSafeAttributeDouble(xmlItem, "MinAttackV"),
						MaxAttackV = Global.GetSafeAttributeDouble(xmlItem, "MaxAttackV"),
						MinMAttackV = Global.GetSafeAttributeDouble(xmlItem, "MinMAttackV"),
						MaxMAttackV = Global.GetSafeAttributeDouble(xmlItem, "MaxMAttackV"),
						RecoverLifeV = Global.GetSafeAttributeDouble(xmlItem, "RecoverLifeV"),
						RecoverMagicV = Global.GetSafeAttributeDouble(xmlItem, "RecoverMagicV"),
						Dodge = Global.GetSafeAttributeDouble(xmlItem, "Dodge"),
						HitV = Global.GetSafeAttributeDouble(xmlItem, "HitV"),
						PhySkillIncreasePercent = Global.GetSafeAttributeDouble(xmlItem, "PhySkillIncreasePercent"),
						MagicSkillIncreasePercent = Global.GetSafeAttributeDouble(xmlItem, "MagicSkillIncreasePercent"),
						AttackSpeed = Global.GetSafeAttributeDouble(xmlItem, "AttackSpeed")
					};
					j++;
					if (j >= roleBasePropItems.Length)
					{
						break;
					}
				}
				Data.RoleBasePropList.Add(roleBasePropItems);
				goto IL_38B;
			}
		}

		// Token: 0x06003C4A RID: 15434 RVA: 0x0033957C File Offset: 0x0033777C
		private void LoadRoleZhuanZhiInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/Roles/ZhuanZhi.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> ChgOccpXEle = xmlFile.Elements("ZhuanZhis").Elements<XElement>();
					foreach (XElement xmlItem in ChgOccpXEle)
					{
						if (null != xmlItem)
						{
							ChangeOccupInfo tmpChgOccuInfo = new ChangeOccupInfo();
							int nOccupationID = (int)Global.GetSafeAttributeLong(xmlItem, "OccupationID");
							tmpChgOccuInfo.OccupationID = (int)Global.GetSafeAttributeLong(xmlItem, "OccupationID");
							tmpChgOccuInfo.NeedLevel = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
							tmpChgOccuInfo.NeedMoney = (int)Global.GetSafeAttributeLong(xmlItem, "NeedJinBi");
							tmpChgOccuInfo.AwardPropPoint = (int)Global.GetSafeAttributeLong(xmlItem, "AwardShuXing");
							string sGoodsID = Global.GetSafeAttributeStr(xmlItem, "NeedGoods");
							if (string.IsNullOrEmpty(sGoodsID))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("转职文件NeedGoods为空", new object[0]), null, true);
							}
							else
							{
								string[] fields = sGoodsID.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("转职文件NeedGoods为空", new object[0]), null, true);
								}
								else
								{
									tmpChgOccuInfo.NeedGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(sGoodsID, "转职文件");
								}
							}
							string sGoodsID2 = Global.GetSafeAttributeStr(xmlItem, "AwardGoods");
							if (string.IsNullOrEmpty(sGoodsID2))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("转职文件NeedGoods为空", new object[0]), null, true);
							}
							else
							{
								string[] fields2 = sGoodsID2.Split(new char[]
								{
									'|'
								});
								if (fields2.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("转职文件NeedGoods为空", new object[0]), null, true);
								}
								else
								{
									tmpChgOccuInfo.AwardGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(sGoodsID2, "转职文件");
								}
							}
							Data.ChangeOccupInfoList.Add(nOccupationID, tmpChgOccuInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanZhi.xml", new object[0])));
			}
		}

		// Token: 0x06003C4B RID: 15435 RVA: 0x00339800 File Offset: 0x00337A00
		private void LoadRoleOccupationAddPointInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/Roles/OccupationAddPoint.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> ChgOccpXEle = xmlFile.Elements("ShuXings").Elements<XElement>();
					foreach (XElement xmlItem in ChgOccpXEle)
					{
						if (null != xmlItem)
						{
							OccupationAddPointInfo tmpInfo = new OccupationAddPointInfo();
							int nOccupationID = (int)Global.GetSafeAttributeLong(xmlItem, "OccupationID");
							tmpInfo.OccupationID = (int)Global.GetSafeAttributeLong(xmlItem, "OccupationID");
							tmpInfo.AddPoint = (int)Global.GetSafeAttributeLong(xmlItem, "JiaDian");
							Data.OccupationAddPointInfoList.Add(nOccupationID, tmpInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanZhi.xml", new object[0])));
			}
		}

		// Token: 0x06003C4C RID: 15436 RVA: 0x00339920 File Offset: 0x00337B20
		private void LoadRoleChangeLifeAddPointInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/Roles/ZhuanShengAddPoint.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> ChgOccpXEle = xmlFile.Elements("ShuXings").Elements<XElement>();
					foreach (XElement xmlItem in ChgOccpXEle)
					{
						if (null != xmlItem)
						{
							ChangeLifeAddPointInfo tmpInfo = new ChangeLifeAddPointInfo();
							int nChangeLev = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuanShengLevel");
							tmpInfo.ChangeLevel = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuanShengLevel");
							tmpInfo.AddPoint = (int)Global.GetSafeAttributeLong(xmlItem, "JiaDian");
							tmpInfo.nStrLimit = (int)Global.GetSafeAttributeLong(xmlItem, "Strength");
							tmpInfo.nDexLimit = (int)Global.GetSafeAttributeLong(xmlItem, "Dexterity");
							tmpInfo.nIntLimit = (int)Global.GetSafeAttributeLong(xmlItem, "Intelligence");
							tmpInfo.nConLimit = (int)Global.GetSafeAttributeLong(xmlItem, "Constitution");
							Data.ChangeLifeAddPointInfoList.Add(nChangeLev, tmpInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanShengAddPoint.xml", new object[0])));
			}
		}

		// Token: 0x06003C4D RID: 15437 RVA: 0x00339AAC File Offset: 0x00337CAC
		private void LoadMoBaiDataInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/MoBai.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> xmlItems = xmlFile.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							MoBaiData mobaiData = new MoBaiData();
							mobaiData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							mobaiData.AdrationMaxLimit = (int)Global.GetSafeAttributeLong(xmlItem, "Number");
							mobaiData.NeedJinBi = (int)Global.GetSafeAttributeLong(xmlItem, "NeedJinBi");
							mobaiData.JinBiExpAward = (int)Global.GetSafeAttributeLong(xmlItem, "JinBiExpAward");
							mobaiData.JinBiZhanGongAward = (int)Global.GetSafeAttributeLong(xmlItem, "JinBiZhanGongAward");
							mobaiData.NeedZuanShi = (int)Global.GetSafeAttributeLong(xmlItem, "NeedZuanShi");
							mobaiData.ZuanShiExpAward = (int)Global.GetSafeAttributeLong(xmlItem, "ZuanShiExpAward");
							mobaiData.ZuanShiZhanGongAward = (int)Global.GetSafeAttributeLong(xmlItem, "ZuanShiZhanGongAward");
							mobaiData.ExtraNumber = (int)Global.GetSafeAttributeLong(xmlItem, "ExtraNumber");
							mobaiData.LingJingAwardByJinBi = (int)Global.GetSafeAttributeLong(xmlItem, "JinBiLingJing");
							mobaiData.LingJingAwardByZuanShi = (int)Global.GetSafeAttributeLong(xmlItem, "ZuanShiLingJing");
							mobaiData.ShenLiJingHuaByJinBi = (int)Global.GetSafeAttributeLong(xmlItem, "JinBishenlijinghua");
							mobaiData.ShenLiJingHuaByZuanShi = (int)Global.GetSafeAttributeLong(xmlItem, "ZuanShishenlijinghua");
							string LevLimit = Global.GetSafeAttributeStr(xmlItem, "MinLevel");
							string[] fields = LevLimit.Split(new char[]
							{
								','
							});
							int[] nArray = Global.StringArray2IntArray(fields);
							mobaiData.MinZhuanSheng = nArray[0];
							mobaiData.MinLevel = nArray[1];
							Data.MoBaiDataInfoList.Add(mobaiData.ID, mobaiData);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/MoBai.xml", new object[0])));
			}
		}

		// Token: 0x06003C4E RID: 15438 RVA: 0x00339CDC File Offset: 0x00337EDC
		private void LoadBloodCastleDataInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/BloodCastleInfo.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> BloodCastleXEle = xmlFile.Elements("BloodCastleInfos").Elements<XElement>();
					foreach (XElement xmlItem in BloodCastleXEle)
					{
						if (null != xmlItem)
						{
							BloodCastleDataInfo tmpInfo = new BloodCastleDataInfo();
							int nMapCodeID = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
							tmpInfo.MapCode = nMapCodeID;
							tmpInfo.MinChangeLifeNum = (int)Global.GetSafeAttributeLong(xmlItem, "MinChangeLife");
							tmpInfo.MaxChangeLifeNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxChangeLife");
							tmpInfo.MaxEnterNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxEnter");
							tmpInfo.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
							tmpInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							string[] fields = goodsIDs.Split(new char[]
							{
								','
							});
							int[] nArray = Global.StringArray2IntArray(fields);
							tmpInfo.NeedGoodsID = nArray[0];
							tmpInfo.NeedGoodsNum = nArray[1];
							tmpInfo.MaxPlayerNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxPlayer");
							tmpInfo.NeedKillMonster1Level = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster1Level");
							tmpInfo.NeedKillMonster1Num = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster1Num");
							tmpInfo.NeedKillMonster2ID = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster2ID");
							tmpInfo.NeedKillMonster2Num = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster2Num");
							tmpInfo.NeedCreateMonster2Num = (int)Global.GetSafeAttributeLong(xmlItem, "NeedCreateMonster2Num");
							tmpInfo.NeedCreateMonster2Pos = Global.GetSafeAttributeStr(xmlItem, "NeedCreateMonster2Pos");
							tmpInfo.NeedCreateMonster2Radius = (int)Global.GetSafeAttributeLong(xmlItem, "NeedCreateMonster2Radius");
							tmpInfo.NeedCreateMonster2PursuitRadius = (int)Global.GetSafeAttributeLong(xmlItem, "PursuitRadius");
							tmpInfo.GateID = (int)Global.GetSafeAttributeLong(xmlItem, "GateID");
							tmpInfo.GatePos = Global.GetSafeAttributeStr(xmlItem, "GatePos");
							tmpInfo.CrystalID = (int)Global.GetSafeAttributeLong(xmlItem, "CrystalID");
							tmpInfo.CrystalPos = Global.GetSafeAttributeStr(xmlItem, "CrystalPos");
							tmpInfo.TimeModulus = (int)Global.GetSafeAttributeLong(xmlItem, "TimeModulus");
							tmpInfo.ExpModulus = (int)Global.GetSafeAttributeLong(xmlItem, "ExpModulus");
							tmpInfo.MoneyModulus = (int)Global.GetSafeAttributeLong(xmlItem, "MoneyModulus");
							string goodsID = Global.GetSafeAttributeStr(xmlItem, "AwardItem1");
							string[] sfields = goodsID.Split(new char[]
							{
								'|'
							});
							tmpInfo.AwardItem1 = sfields;
							goodsID = Global.GetSafeAttributeStr(xmlItem, "AwardItem2");
							sfields = goodsID.Split(new char[]
							{
								'|'
							});
							tmpInfo.AwardItem2 = sfields;
							List<string> timePointsList = new List<string>();
							string timePoints = Global.GetSafeAttributeStr(xmlItem, "BeginTime");
							if (timePoints != null && timePoints != "")
							{
								string[] sField = timePoints.Split(new char[]
								{
									','
								});
								for (int i = 0; i < sField.Length; i++)
								{
									timePointsList.Add(sField[i].Trim());
								}
							}
							tmpInfo.BeginTime = timePointsList;
							tmpInfo.PrepareTime = (int)Global.GetSafeAttributeLong(xmlItem, "PrepareTime");
							tmpInfo.DurationTime = (int)Global.GetSafeAttributeLong(xmlItem, "DurationTime");
							tmpInfo.LeaveTime = (int)Global.GetSafeAttributeLong(xmlItem, "LeaveTime");
							tmpInfo.DiaoXiangID = (int)Global.GetSafeAttributeLong(xmlItem, "DiaoXiangID");
							tmpInfo.DiaoXiangPos = Global.GetSafeAttributeStr(xmlItem, "DiaoXiangPos");
							Data.BloodCastleDataInfoList.Add(nMapCodeID, tmpInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/BloodCastleInfo.xml", new object[0])));
			}
		}

		// Token: 0x06003C4F RID: 15439 RVA: 0x0033A110 File Offset: 0x00338310
		private void LoadCopyScoreDataInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/FuBenPingFen.xml", new object[0]));
				if (null != xmlFile)
				{
					int[] nArray = GameManager.systemParamsList.GetParamValueIntArrayByName("CopyScoreDataMapInfo", ',');
					List<CopyScoreDataInfo> CopyScoreList = new List<CopyScoreDataInfo>();
					IEnumerable<XElement> CopyScoreXEle = xmlFile.Elements("CopyScoreInfos").Elements<XElement>();
					foreach (XElement xmlItem in CopyScoreXEle)
					{
						if (null != xmlItem)
						{
							CopyScoreDataInfo tmpInfo = new CopyScoreDataInfo();
							int nCopyMapID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							tmpInfo.CopyMapID = nCopyMapID;
							tmpInfo.ScoreName = Global.GetSafeAttributeStr(xmlItem, "PingFenName");
							tmpInfo.MinScore = (int)Global.GetSafeAttributeLong(xmlItem, "MinFen");
							tmpInfo.MaxScore = (int)Global.GetSafeAttributeLong(xmlItem, "MaxFen");
							tmpInfo.ExpModulus = Global.GetSafeAttributeDouble(xmlItem, "ExpXiShu");
							tmpInfo.MoneyModulus = Global.GetSafeAttributeDouble(xmlItem, "JinBiXiShu");
							tmpInfo.FallPacketID = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsList");
							tmpInfo.AwardType = (int)Global.GetSafeAttributeLong(xmlItem, "AwardType");
							tmpInfo.MinMoJing = (int)Global.GetSafeAttributeLong(xmlItem, "MinMoJing");
							tmpInfo.MaxMoJing = (int)Global.GetSafeAttributeLong(xmlItem, "MaxMoJing");
							CopyScoreList.Add(tmpInfo);
						}
					}
					foreach (int nID in nArray)
					{
						List<CopyScoreDataInfo> CopyScoreListTmp = new List<CopyScoreDataInfo>();
						for (int nIndex = 0; nIndex < CopyScoreList.Count; nIndex++)
						{
							if (CopyScoreList[nIndex].CopyMapID == nID)
							{
								CopyScoreListTmp.Add(CopyScoreList[nIndex]);
							}
						}
						Data.CopyScoreDataInfoList.Add(nID, CopyScoreListTmp);
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/FuBenPingFen.xml", new object[0])));
			}
		}

		// Token: 0x06003C50 RID: 15440 RVA: 0x0033A380 File Offset: 0x00338580
		private void LoadFreshPlayerCopySceneInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/FreshPlayerCopySceneInfo.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> CopyScoreXEle = xmlFile.Elements("FreshPlayerCopySceneInfos").Elements<XElement>();
					foreach (XElement xmlItem in CopyScoreXEle)
					{
						if (null != xmlItem)
						{
							Data.FreshPlayerSceneInfo = new FreshPlayerCopySceneInfo
							{
								MapCode = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode"),
								NeedKillMonster1Level = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster1Level"),
								NeedKillMonster1Num = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster1Num"),
								NeedKillMonster2ID = (int)Global.GetSafeAttributeLong(xmlItem, "WuShiID"),
								NeedKillMonster2Num = (int)Global.GetSafeAttributeLong(xmlItem, "KillWuShiNum"),
								NeedCreateMonster2Num = (int)Global.GetSafeAttributeLong(xmlItem, "WuShiNum"),
								NeedCreateMonster2Pos = Global.GetSafeAttributeStr(xmlItem, "WuShiPos"),
								NeedCreateMonster2Radius = (int)Global.GetSafeAttributeLong(xmlItem, "WuShiRadius"),
								NeedCreateMonster2PursuitRadius = (int)Global.GetSafeAttributeLong(xmlItem, "PursuitRadius"),
								GateID = (int)Global.GetSafeAttributeLong(xmlItem, "GateID"),
								GatePos = Global.GetSafeAttributeStr(xmlItem, "GatePos"),
								CrystalID = (int)Global.GetSafeAttributeLong(xmlItem, "CrystalID"),
								CrystalPos = Global.GetSafeAttributeStr(xmlItem, "CrystalPos"),
								DiaoXiangID = (int)Global.GetSafeAttributeLong(xmlItem, "DiaoXiangID"),
								DiaoXiangPos = Global.GetSafeAttributeStr(xmlItem, "DiaoXiangPos")
							};
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/FreshPlayerCopySceneInfo.xml", new object[0])));
			}
		}

		// Token: 0x06003C51 RID: 15441 RVA: 0x0033A5A0 File Offset: 0x003387A0
		private void LoadTaskStarDataInfo()
		{
			try
			{
				string fileName = "Config/TaskStarInfos.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null != xml)
				{
					IEnumerable<XElement> TaskStarInfoXEle = xml.Elements("TaskStarInfos").Elements<XElement>();
					foreach (XElement xmlItem in TaskStarInfoXEle)
					{
						if (null != xmlItem)
						{
							TaskStarDataInfo TaskStarInfo = new TaskStarDataInfo();
							TaskStarInfo.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							TaskStarInfo.ExpModulus = Global.GetSafeAttributeDouble(xmlItem, "EXPXiShu");
							TaskStarInfo.BindYuanBaoModulus = Global.GetSafeAttributeDouble(xmlItem, "BindZhuanShiXiShu");
							TaskStarInfo.StarSoulModulus = Global.GetSafeAttributeDouble(xmlItem, "XingHunXiShu");
							TaskStarInfo.Probability = (int)(Global.GetSafeAttributeDouble(xmlItem, "GaiLv") * 10000.0);
							Data.TaskStarInfo.Add(TaskStarInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/TaskStarInfos.xml", new object[0])));
			}
		}

		// Token: 0x06003C52 RID: 15442 RVA: 0x0033A714 File Offset: 0x00338914
		private void LoadDailyCircleTaskAwardInfo()
		{
			try
			{
				string fileName = "Config/DailyCircleTaskAward.xml";
				XElement xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null != xmlFile)
				{
					IEnumerable<XElement> xmlItems = xmlFile.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							DailyCircleTaskAwardInfo DailyCircleTaskAward = new DailyCircleTaskAwardInfo();
							DailyCircleTaskAward.ID = (int)Global.GetSafeAttributeLong(xmlItem, "Id");
							DailyCircleTaskAward.MinChangeLifeLev = (int)Global.GetSafeAttributeLong(xmlItem, "MinzhuanshengLevel");
							DailyCircleTaskAward.MaxChangeLifeLev = (int)Global.GetSafeAttributeLong(xmlItem, "MaxzhuanshengLevel");
							DailyCircleTaskAward.MinLev = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
							DailyCircleTaskAward.MaxLev = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
							DailyCircleTaskAward.Experience = (int)Global.GetSafeAttributeLong(xmlItem, "EXP");
							DailyCircleTaskAward.XingHun = (int)Global.GetSafeAttributeLong(xmlItem, "XingHun");
							string strGoodInfo = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							string[] fields = strGoodInfo.Split(new char[]
							{
								','
							});
							int[] nArray = Global.StringArray2IntArray(fields);
							DailyCircleTaskAward.GoodsID = nArray[0];
							DailyCircleTaskAward.GoodsNum = nArray[1];
							DailyCircleTaskAward.Binding = ((nArray.Length >= 3) ? nArray[2] : 1);
							Data.DailyCircleTaskAward.Add(DailyCircleTaskAward);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/DailyCircleTaskAward.xml", new object[0])));
			}
		}

		// Token: 0x06003C53 RID: 15443 RVA: 0x0033A8F8 File Offset: 0x00338AF8
		private void LoadTaofaTaskAwardInfo()
		{
			try
			{
				int ExBangZuan = (int)GameManager.systemParamsList.GetParamValueIntByName("PriceTaskAward", -1);
				Data.TaofaTaskExAward.BangZuan = ExBangZuan;
				Global.MaxTaofaTaskNumForMU = (int)GameManager.systemParamsList.GetParamValueIntByName("PriceTaskNum", -1);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load PriceTaskAward : {0} fail", string.Format("systemParamsList.PriceTaskAward", new object[0])));
			}
		}

		// Token: 0x06003C54 RID: 15444 RVA: 0x0033A974 File Offset: 0x00338B74
		private void LoadCombatForceInfoInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/Roles/CombatForceInfo.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> xmlItems = xmlFile.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							CombatForceInfo CombatForceData = new CombatForceInfo();
							int nID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							CombatForceData.ID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");
							CombatForceData.MaxHPModulus = Global.GetSafeAttributeDouble(xmlItem, "LifeV");
							CombatForceData.MaxMPModulus = Global.GetSafeAttributeDouble(xmlItem, "MagicV");
							CombatForceData.MinPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MinDefenseV");
							CombatForceData.MaxPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxDefenseV");
							CombatForceData.MinMagicDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MinMDefenseV");
							CombatForceData.MaxMagicDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxMDefenseV");
							CombatForceData.MinPhysicsAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MinAttackV");
							CombatForceData.MaxPhysicsAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxAttackV");
							CombatForceData.MinMagicAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MinMAttackV");
							CombatForceData.MaxMagicAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxMAttackV");
							CombatForceData.HitValueModulus = Global.GetSafeAttributeDouble(xmlItem, "HitV");
							CombatForceData.DodgeModulus = Global.GetSafeAttributeDouble(xmlItem, "Dodge");
							CombatForceData.AddAttackInjureModulus = Global.GetSafeAttributeDouble(xmlItem, "AddAttackInjure");
							CombatForceData.DecreaseInjureModulus = Global.GetSafeAttributeDouble(xmlItem, "DecreaseInjureValue");
							CombatForceData.LifeStealModulus = Global.GetSafeAttributeDouble(xmlItem, "LifeSteal");
							CombatForceData.AddAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "AddAttack");
							CombatForceData.AddDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "AddDefense");
							CombatForceData.FireAttack = Global.GetSafeAttributeDouble(xmlItem, "FireAttack");
							CombatForceData.WaterAttack = Global.GetSafeAttributeDouble(xmlItem, "WaterAttack");
							CombatForceData.LightningAttack = Global.GetSafeAttributeDouble(xmlItem, "LightningAttack");
							CombatForceData.SoilAttack = Global.GetSafeAttributeDouble(xmlItem, "SoilAttack");
							CombatForceData.IceAttack = Global.GetSafeAttributeDouble(xmlItem, "IceAttack");
							CombatForceData.WindAttack = Global.GetSafeAttributeDouble(xmlItem, "WindAttack");
							CombatForceData.ArmorMax = ConfigHelper.GetElementAttributeValueDouble(xmlItem, "ArmorMax", 1.0);
							CombatForceData.HolyAttack = Global.GetSafeAttributeDouble(xmlItem, "HolyAttack");
							CombatForceData.HolyDefense = Global.GetSafeAttributeDouble(xmlItem, "HolyDefense");
							CombatForceData.ShadowAttack = Global.GetSafeAttributeDouble(xmlItem, "ShadowAttack");
							CombatForceData.ShadowDefense = Global.GetSafeAttributeDouble(xmlItem, "ShadowDefense");
							CombatForceData.NatureAttack = Global.GetSafeAttributeDouble(xmlItem, "NatureAttack");
							CombatForceData.NatureDefense = Global.GetSafeAttributeDouble(xmlItem, "NatureDefense");
							CombatForceData.ChaosAttack = Global.GetSafeAttributeDouble(xmlItem, "ChaosAttack");
							CombatForceData.ChaosDefense = Global.GetSafeAttributeDouble(xmlItem, "ChaosDefense");
							CombatForceData.IncubusAttack = Global.GetSafeAttributeDouble(xmlItem, "IncubusAttack");
							CombatForceData.IncubusDefense = Global.GetSafeAttributeDouble(xmlItem, "IncubusDefense");
							Data.CombatForceDataInfo.Add(nID, CombatForceData);
						}
					}
				}
			}
			catch (Exception e)
			{
				e.ToString();
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/CombatForceInfo.xml", new object[0])));
			}
		}

		// Token: 0x06003C55 RID: 15445 RVA: 0x0033ACE8 File Offset: 0x00338EE8
		private void LoadDaimonSquareDataInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/Demon.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> BloodCastleXEle = xmlFile.Elements("DaimonSquareInfos").Elements<XElement>();
					foreach (XElement xmlItem in BloodCastleXEle)
					{
						if (null != xmlItem)
						{
							DaimonSquareDataInfo tmpInfo = new DaimonSquareDataInfo();
							int nMapCodeID = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
							tmpInfo.MapCode = nMapCodeID;
							tmpInfo.MinChangeLifeNum = (int)Global.GetSafeAttributeLong(xmlItem, "MinChangeLife");
							tmpInfo.MaxChangeLifeNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxChangeLife");
							tmpInfo.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
							tmpInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
							tmpInfo.MaxEnterNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxEnter");
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							string[] fields = goodsIDs.Split(new char[]
							{
								','
							});
							int[] nArray = Global.StringArray2IntArray(fields);
							tmpInfo.NeedGoodsID = nArray[0];
							tmpInfo.NeedGoodsNum = nArray[1];
							tmpInfo.MaxPlayerNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxPlayer");
							string sMonsterID = Global.GetSafeAttributeStr(xmlItem, "MonsterID");
							tmpInfo.MonsterID = sMonsterID.Split(new char[]
							{
								'|'
							});
							int nMonsterIDLength = tmpInfo.MonsterID.Length;
							string sMonsterNum = Global.GetSafeAttributeStr(xmlItem, "MonsterNumber");
							tmpInfo.MonsterNum = sMonsterNum.Split(new char[]
							{
								'|'
							});
							int nMonsterNumLength = tmpInfo.MonsterNum.Length;
							string sMonsterPos = Global.GetSafeAttributeStr(xmlItem, "MonsterPos");
							string[] sArraysPos = sMonsterPos.Split(new char[]
							{
								','
							});
							tmpInfo.posX = Global.SafeConvertToInt32(sArraysPos[0]);
							tmpInfo.posZ = Global.SafeConvertToInt32(sArraysPos[1]);
							tmpInfo.Radius = Global.SafeConvertToInt32(sArraysPos[2]);
							tmpInfo.MonsterSum = (int)Global.GetSafeAttributeLong(xmlItem, "MonsterSum");
							string sMonsterCond = Global.GetSafeAttributeStr(xmlItem, "SuccessConditions");
							tmpInfo.CreateNextWaveMonsterCondition = sMonsterCond.Split(new char[]
							{
								'|'
							});
							int nMonsterCondLength = tmpInfo.CreateNextWaveMonsterCondition.Length;
							if (nMonsterIDLength != nMonsterNumLength || nMonsterIDLength != nMonsterCondLength)
							{
							}
							tmpInfo.TimeModulus = (int)Global.GetSafeAttributeLong(xmlItem, "TimeModulus");
							tmpInfo.ExpModulus = (int)Global.GetSafeAttributeLong(xmlItem, "ExpModulus");
							tmpInfo.MoneyModulus = (int)Global.GetSafeAttributeLong(xmlItem, "MoneyModulus");
							string goodsID = Global.GetSafeAttributeStr(xmlItem, "AwardItem1");
							string[] sfields = goodsID.Split(new char[]
							{
								'|'
							});
							tmpInfo.AwardItem = sfields;
							List<string> timePointsList = new List<string>();
							string timePoints = Global.GetSafeAttributeStr(xmlItem, "BeginTime");
							if (timePoints != null && timePoints != "")
							{
								string[] sField = timePoints.Split(new char[]
								{
									','
								});
								for (int i = 0; i < sField.Length; i++)
								{
									timePointsList.Add(sField[i].Trim());
								}
							}
							tmpInfo.BeginTime = timePointsList;
							tmpInfo.PrepareTime = (int)Global.GetSafeAttributeLong(xmlItem, "PrepareTime");
							tmpInfo.DurationTime = (int)Global.GetSafeAttributeLong(xmlItem, "DurationTime");
							tmpInfo.LeaveTime = (int)Global.GetSafeAttributeLong(xmlItem, "LeaveTime");
							Data.DaimonSquareDataInfoList.Add(nMapCodeID, tmpInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/Demon.xml", new object[0])));
			}
		}

		// Token: 0x06003C56 RID: 15446 RVA: 0x0033B108 File Offset: 0x00339308
		private void LoadSystemParamsDataForCache()
		{
			try
			{
				double[] dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingForgeLevelAddShangHaiJiaCheng", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.WingForgeLevelAddShangHaiJiaCheng = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingForgeLevelAddDefenseRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.WingForgeLevelAddDefenseRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingZhuiJiaLevelAddDefenseRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.WingZhuiJiaLevelAddDefenseRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingForgeLevelAddShangHaiXiShou", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.WingForgeLevelAddShangHaiXiShou = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ForgeLevelAddAttackRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.ForgeLevelAddAttackRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuiJiaLevelAddAttackRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.ZhuiJiaLevelAddAttackRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ForgeLevelAddDefenseRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.ForgeLevelAddDefenseRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuiJiaLevelAddDefenseRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.ZhuiJiaLevelAddDefenseRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ForgeLevelAddMaxLifeVRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.ForgeLevelAddMaxLifeVRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueAddAttackRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.ZhuoYueAddAttackRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueAddDefenseRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.ZhuoYueAddDefenseRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("RebornZhuoYueAddRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.RebornZhuoYueAddRates = dValue;
				}
				string str = GameManager.systemParamsList.GetParamValueByName("ShiJieChuanSong");
				if (!string.IsNullOrEmpty(str))
				{
					string[] fields = str.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < fields.Length; i++)
					{
						string[] fields2 = fields[i].Split(new char[]
						{
							','
						});
						int mapCode = Global.SafeConvertToInt32(fields2[0]);
						int needMoney = Global.SafeConvertToInt32(fields2[1]);
						Data.MapTransNeedMoneyDict.Add(mapCode, needMoney);
					}
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EquipZhuanShengAddAttackRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.EquipChangeLifeAddAttackRates = dValue;
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EquipZhuanShengAddDefenseRates", ',');
				if (dValue != null && dValue.Length > 0)
				{
					Data.EquipChangeLifeAddDefenseRates = dValue;
				}
				int nValue = (int)GameManager.systemParamsList.GetParamValueDoubleByName("ZuanshiVIPExp", 0.0);
				if (nValue != 0)
				{
					Data.DiamondToVipExpValue = nValue;
				}
				int[] nValue2 = GameManager.systemParamsList.GetParamValueIntArrayByName("BossStaticDataIDForChengJiu", ',');
				if (nValue2 != null && nValue2.Length > 0)
				{
					Data.KillBossCountForChengJiu = nValue2;
				}
				str = GameManager.systemParamsList.GetParamValueByName("ForgeProtectStoneGoodsIDS");
				if (!string.IsNullOrEmpty(str))
				{
					string[] fields = str.Split(new char[]
					{
						'|'
					});
					Data.ForgeProtectStoneGoodsID = new int[fields.Length];
					Data.ForgeProtectStoneGoodsNum = new int[fields.Length];
					for (int i = 0; i < fields.Length; i++)
					{
						string[] fields2 = fields[i].Split(new char[]
						{
							','
						});
						int nID = Global.SafeConvertToInt32(fields2[0]);
						int nNum = Global.SafeConvertToInt32(fields2[1]);
						Data.ForgeProtectStoneGoodsID[i] = nID;
						Data.ForgeProtectStoneGoodsNum[i] = nNum;
					}
				}
				str = GameManager.systemParamsList.GetParamValueByName("WinCaiLiaoZuanShi");
				if (!string.IsNullOrEmpty(str))
				{
					string[] field = str.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < field.Length; i++)
					{
						string[] field2 = field[i].Split(new char[]
						{
							','
						});
						int goodsId = Global.SafeConvertToInt32(field2[0]);
						int goodsZuanshi = Global.SafeConvertToInt32(field2[1]);
						Data.LingYuMaterialZuanshiDict[goodsId] = goodsZuanshi;
					}
				}
				SecondPasswordManager.ValidSecWhenLogout = GameManager.systemParamsList.GetParamValueIntByName("SecondPasswordTime", -1);
				str = GameManager.systemParamsList.GetParamValueByName("MoBaiNumber");
				if (!string.IsNullOrEmpty(str))
				{
					Data.PKkingadrationData.AdrationMaxLimit = Global.SafeConvertToInt32(str);
				}
				str = GameManager.systemParamsList.GetParamValueByName("JiBiMoBai");
				if (!string.IsNullOrEmpty(str))
				{
					string[] strFelds = str.Split(new char[]
					{
						','
					});
					Data.PKkingadrationData.GoldAdrationSpend = Global.SafeConvertToInt32(strFelds[0]);
					Data.PKkingadrationData.GoldAdrationExpModulus = Global.SafeConvertToInt32(strFelds[1]);
					Data.PKkingadrationData.GoldAdrationShengWangModulus = Global.SafeConvertToInt32(strFelds[2]);
				}
				str = GameManager.systemParamsList.GetParamValueByName("ZuanShiMoBai");
				if (!string.IsNullOrEmpty(str))
				{
					string[] strFelds = str.Split(new char[]
					{
						','
					});
					Data.PKkingadrationData.DiamondAdrationSpend = Global.SafeConvertToInt32(strFelds[0]);
					Data.PKkingadrationData.DiamondAdrationExpModulus = Global.SafeConvertToInt32(strFelds[1]);
					Data.PKkingadrationData.DiamondAdrationShengWangModulus = Global.SafeConvertToInt32(strFelds[2]);
				}
				str = GameManager.systemParamsList.GetParamValueByName("LuoLanMoBaiNumber");
				if (!string.IsNullOrEmpty(str))
				{
					Data.LLCZadrationData.AdrationMaxLimit = Global.SafeConvertToInt32(str);
				}
				str = GameManager.systemParamsList.GetParamValueByName("LuoLanJiBiMoBai");
				if (!string.IsNullOrEmpty(str))
				{
					string[] strFelds = str.Split(new char[]
					{
						','
					});
					Data.LLCZadrationData.GoldAdrationSpend = Global.SafeConvertToInt32(strFelds[0]);
					Data.LLCZadrationData.GoldAdrationExpModulus = Global.SafeConvertToInt32(strFelds[1]);
					Data.LLCZadrationData.GoldAdrationShengWangModulus = Global.SafeConvertToInt32(strFelds[2]);
				}
				str = GameManager.systemParamsList.GetParamValueByName("LuoLanZuanShiMoBai");
				if (!string.IsNullOrEmpty(str))
				{
					string[] strFelds = str.Split(new char[]
					{
						','
					});
					Data.LLCZadrationData.DiamondAdrationSpend = Global.SafeConvertToInt32(strFelds[0]);
					Data.LLCZadrationData.DiamondAdrationExpModulus = Global.SafeConvertToInt32(strFelds[1]);
					Data.LLCZadrationData.DiamondAdrationShengWangModulus = Global.SafeConvertToInt32(strFelds[2]);
				}
				str = GameManager.systemParamsList.GetParamValueByName("CangKuAward");
				if (!string.IsNullOrEmpty(str))
				{
					string[] strFelds = str.Split(new char[]
					{
						'|'
					});
					Data.InsertAwardtPortableBagTaskID = Global.SafeConvertToInt32(strFelds[0]);
					Data.InsertAwardtPortableBagGoodsInfo = strFelds[1];
				}
				dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("HongMingDebuff", ',');
				if (dValue != null)
				{
					Data.RedNameDebuffInfo = dValue;
				}
				str = GameManager.systemParamsList.GetParamValueByName("ForgeNeedGoodsIDs");
				if (str != null && str.Length > 0)
				{
					Data.ForgeNeedGoodsID = Global.String2StringArray(str, '|');
				}
				str = GameManager.systemParamsList.GetParamValueByName("ForgeNeedGoodsNum");
				if (str != null && str.Length > 0)
				{
					Data.ForgeNeedGoodsNum = Global.String2StringArray(str, '|');
				}
				if (Data.ForgeNeedGoodsID.Length != Data.ForgeNeedGoodsNum.Length)
				{
					throw new Exception(string.Format("load file : {0} error", string.Format("LoadSystemParamsDataForCache", new object[0])));
				}
				nValue = (int)GameManager.systemParamsList.GetParamValueDoubleByName("PaiHangChongBai", 0.0);
				if (nValue != 0)
				{
					Data.PaihangbangAdration = nValue;
				}
				nValue2 = GameManager.systemParamsList.GetParamValueIntArrayByName("storycopymapid", ',');
				if (nValue2 != null && nValue2.Length > 0)
				{
					Data.StoryCopyMapID = nValue2;
				}
				nValue = (int)GameManager.systemParamsList.GetParamValueDoubleByName("QiFuTime", 0.0);
				if (nValue != 0)
				{
					Data.FreeImpetrateIntervalTime = nValue * 60;
				}
				SingletonTemplate<GuardStatueManager>.Instance().SuitFactor = GameManager.systemParamsList.GetParamValueDoubleByName("ShouHuSuit", 0.0);
				SingletonTemplate<GuardStatueManager>.Instance().LevelFactor = GameManager.systemParamsList.GetParamValueDoubleByName("ShouHuLevel", 0.0);
				str = GameManager.systemParamsList.GetParamValueByName("ShouHuMax");
				SingletonTemplate<GuardStatueManager>.Instance().InitRecoverPoint_BySysParam(str);
				str = GameManager.systemParamsList.GetParamValueByName("ShouHuDiaoXiang");
				SingletonTemplate<GuardStatueManager>.Instance().InitSoulSlot_BySysParam(str);
				nValue2 = GameManager.systemParamsList.GetParamValueIntArrayByName("ModName", ',');
				SingletonTemplate<NameManager>.Instance().CostZuanShiBase = nValue2[0];
				SingletonTemplate<NameManager>.Instance().CostZuanShiMax = nValue2[1];
				SingletonTemplate<MoRiJudgeManager>.Instance().AwardFactor = GameManager.systemParamsList.GetParamValueDoubleArrayByName("MoRiShenPanAward", ',');
				KuaFuManager.getInstance().InitCopyTime();
				SingletonTemplate<SoulStoneManager>.Instance().LoadJingHuaExpConfig();
				SingletonTemplate<MonsterAttackerLogManager>.Instance().LoadRecordMonsters();
				SingletonTemplate<SpeedUpTickCheck>.Instance().LoadConfig();
				SingletonTemplate<NameManager>.Instance().LoadConfig();
				SingletonTemplate<CoupleArenaManager>.Instance().InitSystenParams();
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("load file : {0} fail, {1}", string.Format("LoadSystemParamsDataForCache", new object[0]), ex.ToString()));
			}
		}

		// Token: 0x06003C57 RID: 15447 RVA: 0x0033BB74 File Offset: 0x00339D74
		public static void LoadTotalLoginDataInfo()
		{
			try
			{
				string fileName = "Config/Gifts/NewHuoDongLoginNumGift.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
				XElement xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null != xmlFile)
				{
					Dictionary<int, TotalLoginDataInfo> tmpTotalLoginInfo = new Dictionary<int, TotalLoginDataInfo>();
					IEnumerable<XElement> TotalLoginXEle = xmlFile.Elements("HuoDongLoginNumGift").Elements<XElement>();
					foreach (XElement xmlItem in TotalLoginXEle)
					{
						if (null != xmlItem)
						{
							TotalLoginDataInfo tmpInfo = new TotalLoginDataInfo();
							int nID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							tmpInfo.TotalLoginDays = (int)Global.GetSafeAttributeLong(xmlItem, "TimeOl");
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsID1");
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length != 0)
							{
								tmpInfo.NormalAward = new List<GoodsData>();
								foreach (string goods in fields)
								{
									string[] goodsProp = goods.Split(new char[]
									{
										','
									});
									if (goodsProp.Length == 7)
									{
										GoodsData goodsdata = new GoodsData();
										goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);
										goodsdata.GCount = Global.SafeConvertToInt32(goodsProp[1]);
										goodsdata.Binding = Global.SafeConvertToInt32(goodsProp[2]);
										goodsdata.Forge_level = Global.SafeConvertToInt32(goodsProp[3]);
										goodsdata.AppendPropLev = Global.SafeConvertToInt32(goodsProp[4]);
										goodsdata.Lucky = Global.SafeConvertToInt32(goodsProp[5]);
										goodsdata.ExcellenceInfo = Global.SafeConvertToInt32(goodsProp[6]);
										tmpInfo.NormalAward.Add(goodsdata);
									}
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsID2");
							fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length != 0)
							{
								tmpInfo.Award0 = new List<GoodsData>();
								tmpInfo.Award1 = new List<GoodsData>();
								tmpInfo.Award2 = new List<GoodsData>();
								tmpInfo.Award3 = new List<GoodsData>();
								tmpInfo.Award5 = new List<GoodsData>();
								foreach (string goods in fields)
								{
									string[] goodsProp = goods.Split(new char[]
									{
										','
									});
									if (goodsProp.Length == 7)
									{
										GoodsData goodsdata = new GoodsData();
										goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);
										goodsdata.GCount = Global.SafeConvertToInt32(goodsProp[1]);
										goodsdata.Binding = Global.SafeConvertToInt32(goodsProp[2]);
										goodsdata.Forge_level = Global.SafeConvertToInt32(goodsProp[3]);
										goodsdata.AppendPropLev = Global.SafeConvertToInt32(goodsProp[4]);
										goodsdata.Lucky = Global.SafeConvertToInt32(goodsProp[5]);
										goodsdata.ExcellenceInfo = Global.SafeConvertToInt32(goodsProp[6]);
										int nOcu = Global.GetMainOccupationByGoodsID(goodsdata.GoodsID);
										if (nOcu == 0)
										{
											tmpInfo.Award0.Add(goodsdata);
										}
										else if (nOcu == 1)
										{
											tmpInfo.Award1.Add(goodsdata);
										}
										else if (nOcu == 2)
										{
											tmpInfo.Award2.Add(goodsdata);
										}
										else if (nOcu == 3)
										{
											tmpInfo.Award3.Add(goodsdata);
										}
										else if (nOcu == 5)
										{
											tmpInfo.Award5.Add(goodsdata);
										}
									}
								}
							}
							tmpTotalLoginInfo.Add(nID, tmpInfo);
						}
					}
					lock (Data.TotalLoginDataInfoListLock)
					{
						Data.TotalLoginDataInfoList = tmpTotalLoginInfo;
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/Gifts/NewHuoDongLoginNumGift.xml", new object[0])));
			}
		}

		// Token: 0x06003C58 RID: 15448 RVA: 0x0033C00C File Offset: 0x0033A20C
		private void LoadVIPDataInfo()
		{
			try
			{
				string fileName = "Config/Gifts/VipDailyAwards.xml";
				XElement xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null != xmlFile)
				{
					IEnumerable<XElement> VIPXEle = xmlFile.Elements();
					foreach (XElement xmlItem in VIPXEle)
					{
						if (null != xmlItem)
						{
							VIPDataInfo tmpInfo = new VIPDataInfo();
							int nAwardID = (int)Global.GetSafeAttributeLong(xmlItem, "AwardID");
							tmpInfo.AwardID = nAwardID;
							tmpInfo.ZuanShi = (int)Global.GetSafeAttributeDouble(xmlItem, "ZuanShi");
							tmpInfo.BindZuanShi = (int)Global.GetSafeAttributeDouble(xmlItem, "BindZuanShi");
							tmpInfo.JinBi = (int)Global.GetSafeAttributeDouble(xmlItem, "JinBi");
							tmpInfo.BindJinBi = (int)Global.GetSafeAttributeDouble(xmlItem, "BindJinBi");
							tmpInfo.VIPlev = (int)Global.GetSafeAttributeDouble(xmlItem, "VIPlev");
							tmpInfo.XiHongMing = (int)Global.GetSafeAttributeDouble(xmlItem, "XiHongMing");
							tmpInfo.XiuLi = (int)Global.GetSafeAttributeDouble(xmlItem, "XiuLi");
							tmpInfo.DailyMaxUseTimes = (int)Global.GetSafeAttributeDouble(xmlItem, "DailyMaxUseTimes");
							string strBuff = Global.GetSafeAttributeStr(xmlItem, "BufferGoods");
							if (strBuff != null)
							{
								string[] strField = strBuff.Split(new char[]
								{
									','
								});
								if (strField.Count<string>() > 0)
								{
									tmpInfo.BufferGoods = new int[strField.Count<string>()];
									for (int i = 0; i < strField.Count<string>(); i++)
									{
										int nValue = Global.SafeConvertToInt32(strField[i]);
										tmpInfo.BufferGoods[i] = nValue;
									}
								}
							}
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							if (!string.IsNullOrEmpty(goodsIDs))
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length != 0)
								{
									tmpInfo.AwardGoods = new List<GoodsData>();
									foreach (string goods in fields)
									{
										string[] goodsProp = goods.Split(new char[]
										{
											','
										});
										GoodsData goodsdata = new GoodsData();
										if (goodsProp.Length == 7)
										{
											goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);
											goodsdata.GCount = Global.SafeConvertToInt32(goodsProp[1]);
											goodsdata.Binding = Global.SafeConvertToInt32(goodsProp[2]);
											goodsdata.Forge_level = Global.SafeConvertToInt32(goodsProp[3]);
											goodsdata.AppendPropLev = Global.SafeConvertToInt32(goodsProp[4]);
											goodsdata.Lucky = Global.SafeConvertToInt32(goodsProp[5]);
											goodsdata.ExcellenceInfo = Global.SafeConvertToInt32(goodsProp[6]);
										}
										else
										{
											goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);
										}
										tmpInfo.AwardGoods.Add(goodsdata);
									}
								}
							}
							Data.VIPDataInfoList.Add(nAwardID, tmpInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/Gifts/VipDailyAwards.xml", new object[0])));
			}
		}

		// Token: 0x06003C59 RID: 15449 RVA: 0x0033C39C File Offset: 0x0033A59C
		private void LoadVIPLevAwardAndExpInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/MuVip.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> VIPXEle = xmlFile.Elements();
					foreach (XElement xmlItem in VIPXEle)
					{
						if (null != xmlItem)
						{
							VIPLevAwardAndExpInfo tmpInfo = new VIPLevAwardAndExpInfo();
							int nVipLev = (int)Global.GetSafeAttributeLong(xmlItem, "VIPLevel");
							tmpInfo.VipLev = nVipLev;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "LiBaoAward");
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length != 0)
							{
								tmpInfo.AwardList = new List<GoodsData>();
								foreach (string goods in fields)
								{
									string[] goodsProp = goods.Split(new char[]
									{
										','
									});
									GoodsData goodsdata = new GoodsData();
									if (goodsProp.Length == 7)
									{
										goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);
										goodsdata.GCount = Global.SafeConvertToInt32(goodsProp[1]);
										goodsdata.Binding = Global.SafeConvertToInt32(goodsProp[2]);
										goodsdata.Forge_level = Global.SafeConvertToInt32(goodsProp[3]);
										goodsdata.AppendPropLev = Global.SafeConvertToInt32(goodsProp[4]);
										goodsdata.Lucky = Global.SafeConvertToInt32(goodsProp[5]);
										goodsdata.ExcellenceInfo = Global.SafeConvertToInt32(goodsProp[6]);
									}
									else
									{
										goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);
									}
									tmpInfo.AwardList.Add(goodsdata);
								}
							}
							tmpInfo.NeedExp = (int)Global.GetSafeAttributeLong(xmlItem, "NeedExp");
							Data.VIPLevAwardAndExpInfoList.Add(nVipLev, tmpInfo);
							VIPEumValue.VIPENUMVALUE_MAXLEVEL = Math.Max(nVipLev, VIPEumValue.VIPENUMVALUE_MAXLEVEL);
							if (VIPEumValue.VIP_MIN_NEED_EXP <= 0)
							{
								VIPEumValue.VIP_MIN_NEED_EXP = tmpInfo.NeedExp;
							}
							VIPEumValue.VIP_MIN_NEED_EXP = Math.Min(tmpInfo.NeedExp, VIPEumValue.VIP_MIN_NEED_EXP);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/MuVip.xml", new object[0])));
			}
		}

		// Token: 0x06003C5A RID: 15450 RVA: 0x0033C634 File Offset: 0x0033A834
		private void LoadMeditateInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/MingXiang.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> MeditateXEle = xmlFile.Elements();
					foreach (XElement xmlItem in MeditateXEle)
					{
						if (null != xmlItem)
						{
							MeditateData tmpInfo = new MeditateData();
							int ID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");
							tmpInfo.MeditateID = ID;
							tmpInfo.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xmlItem, "MinZhuanSheng");
							tmpInfo.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(xmlItem, "MaxZhuanSheng");
							tmpInfo.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
							tmpInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
							tmpInfo.Experience = (int)Global.GetSafeAttributeLong(xmlItem, "Experience");
							tmpInfo.StarSoul = (int)Global.GetSafeAttributeLong(xmlItem, "Xinghun");
							tmpInfo.MediateRewardTuple = ConfigHelper.GetElementAttributeValueIntArray(xmlItem, "RewardID", null);
							tmpInfo.GetRewardTime = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "GetRewardTime", 600L) * 1000;
							Data.MeditateInfoList.Add(ID, tmpInfo);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("load xml file : {0}", string.Format("Config/VIPExp.xml", new object[0])), ex, true);
			}
		}

		// Token: 0x06003C5B RID: 15451 RVA: 0x0033C7F8 File Offset: 0x0033A9F8
		private void LoadExperienceCopyMapDataInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/JinYanFuBen.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> ExperienceXEle = xmlFile.Elements("JinYanFuBen").Elements<XElement>();
					foreach (XElement xmlItem in ExperienceXEle)
					{
						if (null != xmlItem)
						{
							ExperienceCopyMapDataInfo tmpInfo = new ExperienceCopyMapDataInfo();
							int nMapCodeID = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
							tmpInfo.CopyMapID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							tmpInfo.MapCodeID = nMapCodeID;
							tmpInfo.MonsterIDList = new Dictionary<int, List<int>>();
							string sMonsterID = Global.GetSafeAttributeStr(xmlItem, "MonsterID");
							string[] sID = sMonsterID.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < sID.Length; i++)
							{
								string[] sFildID = sID[i].Split(new char[]
								{
									','
								});
								List<int> tmpIDList = new List<int>();
								for (int j = 0; j < sFildID.Length; j++)
								{
									int nid = Global.SafeConvertToInt32(sFildID[j]);
									tmpIDList.Add(nid);
								}
								tmpInfo.MonsterIDList.Add(i, tmpIDList);
							}
							tmpInfo.MonsterNumList = new Dictionary<int, List<int>>();
							string sMonsterNum = Global.GetSafeAttributeStr(xmlItem, "MonsterNumber");
							string[] sNum = sMonsterNum.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < sNum.Length; i++)
							{
								string[] sFildNum = sNum[i].Split(new char[]
								{
									','
								});
								List<int> tmpNumList = new List<int>();
								for (int j = 0; j < sFildNum.Length; j++)
								{
									int nnum = Global.SafeConvertToInt32(sFildNum[j]);
									tmpNumList.Add(nnum);
								}
								tmpInfo.MonsterNumList.Add(i, tmpNumList);
							}
							string sMonsterPos = Global.GetSafeAttributeStr(xmlItem, "MonsterPos");
							string[] sArraysPos = sMonsterPos.Split(new char[]
							{
								','
							});
							tmpInfo.posX = Global.SafeConvertToInt32(sArraysPos[0]);
							tmpInfo.posZ = Global.SafeConvertToInt32(sArraysPos[1]);
							tmpInfo.Radius = Global.SafeConvertToInt32(sArraysPos[2]);
							tmpInfo.MonsterSum = (int)Global.GetSafeAttributeLong(xmlItem, "MonsterSum");
							string sMonsterCond = Global.GetSafeAttributeStr(xmlItem, "SuccessConditions");
							string[] sCon = sMonsterCond.Split(new char[]
							{
								'|'
							});
							tmpInfo.CreateNextWaveMonsterCondition = new int[sCon.Length];
							for (int i = 0; i < sCon.Length; i++)
							{
								tmpInfo.CreateNextWaveMonsterCondition[i] = Global.SafeConvertToInt32(sCon[i]);
							}
							int nMonsterCondLength = tmpInfo.CreateNextWaveMonsterCondition.Length;
							Data.ExperienceCopyMapDataInfoList.Add(nMapCodeID, tmpInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/JinYanFuBen.xml", new object[0])));
			}
		}

		// Token: 0x06003C5C RID: 15452 RVA: 0x0033CB68 File Offset: 0x0033AD68
		private void LoadBossHomeInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/BossZhiJia.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> BossHomeXEle = xmlFile.Elements("BossZhiJias").Elements<XElement>();
					foreach (XElement xmlItem in BossHomeXEle)
					{
						if (null != xmlItem)
						{
							Data.BosshomeData = new BossHomeData
							{
								MapID = (int)Global.GetSafeAttributeDouble(xmlItem, "MapCode"),
								VIPLevLimit = (int)Global.GetSafeAttributeLong(xmlItem, "KaiQiVipLevel"),
								MinChangeLifeLimit = (int)Global.GetSafeAttributeLong(xmlItem, "MinChangeLife"),
								MaxChangeLifeLimit = (int)Global.GetSafeAttributeLong(xmlItem, "MaxChangeLife"),
								MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel"),
								MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel"),
								EnterNeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "EnterNeedZuanShi"),
								OneMinuteNeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "MapTimeNeedZuanShi")
							};
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/BossZhiJia.xml", new object[0])));
			}
		}

		// Token: 0x06003C5D RID: 15453 RVA: 0x0033CD04 File Offset: 0x0033AF04
		private void LoadGoldTempleInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/HuangJinShengDian.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> BossHomeXEle = xmlFile.Elements("HuangJinShengDians").Elements<XElement>();
					foreach (XElement xmlItem in BossHomeXEle)
					{
						if (null != xmlItem)
						{
							Data.GoldtempleData = new GoldTempleData
							{
								MapID = (int)Global.GetSafeAttributeDouble(xmlItem, "MapCode"),
								VIPLevLimit = (int)Global.GetSafeAttributeLong(xmlItem, "KaiQiVipLevel"),
								MinChangeLifeLimit = (int)Global.GetSafeAttributeLong(xmlItem, "MinChangeLife"),
								MaxChangeLifeLimit = (int)Global.GetSafeAttributeLong(xmlItem, "MaxChangeLife"),
								MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel"),
								MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel"),
								EnterNeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "EnterNeedZuanShi"),
								OneMinuteNeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "MapTimeNeedZuanShi")
							};
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/HuangJinShengDian.xml", new object[0])));
			}
		}

		// Token: 0x06003C5E RID: 15454 RVA: 0x0033CEA0 File Offset: 0x0033B0A0
		private void LoadEquipUpgradeInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/MuEquipUp.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> equipXEle = xmlFile.Elements("Equip");
					foreach (XElement xmlItem in equipXEle)
					{
						IEnumerable<XElement> items = xmlItem.Elements("Item");
						Dictionary<int, MuEquipUpgradeData> tmpData = new Dictionary<int, MuEquipUpgradeData>();
						int nID = (int)Global.GetSafeAttributeLong(xmlItem, "Categoriy");
						foreach (XElement item in items)
						{
							if (null != item)
							{
								MuEquipUpgradeData tmpInfo = new MuEquipUpgradeData();
								int nSuitID = (int)Global.GetSafeAttributeLong(item, "SuitID");
								tmpInfo.CategoriyID = nID;
								tmpInfo.SuitID = nSuitID;
								tmpInfo.NeedMoJing = (int)Global.GetSafeAttributeLong(item, "NeedMoJing");
								tmpData[nSuitID] = tmpInfo;
							}
						}
						Data.EquipUpgradeData.Add(nID, tmpData);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("load xml file : {0} fail" + ex.ToString(), string.Format("Config/MuEquipUp.xml", new object[0])));
			}
		}

		// Token: 0x06003C5F RID: 15455 RVA: 0x0033D070 File Offset: 0x0033B270
		private void LoadFuBenNeedInfo()
		{
			try
			{
				List<string> configItems = GameManager.systemParamsList.GetParamValueStringListByName("FuBenNeed", '|');
				if (configItems != null && configItems.Count > 0)
				{
					foreach (string configItem in configItems)
					{
						int[] configArray = Global.String2IntArray(configItem, ',');
						if (configArray != null && configArray.Length == 2)
						{
							int fuBenTabId = configArray[0];
							int needTaskId = configArray[1];
							Data.FuBenNeedDict[fuBenTabId] = needTaskId;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("load xml file : {0} fail" + ex.ToString(), string.Format("Config/MuEquipUp.xml", new object[0])));
			}
		}

		// Token: 0x06003C60 RID: 15456 RVA: 0x0033D16C File Offset: 0x0033B36C
		private void LoadGoldCopySceneInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/JinBiFuBen.xml", new object[0]));
				if (null != xmlFile)
				{
					GoldCopySceneData tmpGoldCopySceneInfo = new GoldCopySceneData();
					XElement args = xmlFile.Element("PatrolPath");
					if (null != args)
					{
						string sPatorlPathID = Global.GetSafeAttributeStr(args, "Path");
						if (string.IsNullOrEmpty(sPatorlPathID))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪路径为空", new object[0]), null, true);
						}
						else
						{
							string[] fields = sPatorlPathID.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪路径为空", new object[0]), null, true);
							}
							else
							{
								tmpGoldCopySceneInfo.m_MonsterPatorlPathList = new List<int[]>();
								for (int i = 0; i < fields.Length; i++)
								{
									string[] sa = fields[i].Split(new char[]
									{
										','
									});
									if (sa.Length != 2)
									{
										LogManager.WriteLog(LogTypes.Warning, string.Format("解析{0}文件中的奖励项时失败,坐标有误", "Config/JinBiFuBen.xml"), null, true);
									}
									else
									{
										int[] pos = Global.StringArray2IntArray(sa);
										tmpGoldCopySceneInfo.m_MonsterPatorlPathList.Add(pos);
									}
								}
								Data.Goldcopyscenedata.m_MonsterPatorlPathList = tmpGoldCopySceneInfo.m_MonsterPatorlPathList;
							}
						}
					}
					IEnumerable<XElement> MonstersXEle = xmlFile.Elements("Monsters").Elements<XElement>();
					foreach (XElement xmlItem in MonstersXEle)
					{
						if (null != xmlItem)
						{
							GoldCopySceneMonster tmpGoldCopySceneMon = new GoldCopySceneMonster();
							tmpGoldCopySceneMon.m_MonsterID = new List<int>();
							int nWave = (int)Global.GetSafeAttributeLong(xmlItem, "WaveID");
							tmpGoldCopySceneMon.m_Wave = nWave;
							tmpGoldCopySceneMon.m_Num = (int)Global.GetSafeAttributeLong(xmlItem, "Num");
							tmpGoldCopySceneMon.m_Delay1 = (int)Global.GetSafeAttributeLong(xmlItem, "Delay1");
							tmpGoldCopySceneMon.m_Delay2 = (int)Global.GetSafeAttributeLong(xmlItem, "Delay2");
							string sMonstersID = Global.GetSafeAttributeStr(xmlItem, "MonsterList");
							if (string.IsNullOrEmpty(sMonstersID))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪ID为空", new object[0]), null, true);
							}
							else
							{
								string[] fields = sMonstersID.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪ID为空", new object[0]), null, true);
								}
								else
								{
									for (int i = 0; i < fields.Length; i++)
									{
										int Monsters = Global.SafeConvertToInt32(fields[i]);
										tmpGoldCopySceneMon.m_MonsterID.Add(Monsters);
									}
								}
							}
							tmpGoldCopySceneInfo.GoldCopySceneMonsterData.Add(nWave, tmpGoldCopySceneMon);
						}
					}
					Data.Goldcopyscenedata.GoldCopySceneMonsterData = tmpGoldCopySceneInfo.GoldCopySceneMonsterData;
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/JinBiFuBen.xml", new object[0])));
			}
		}

		// Token: 0x06003C61 RID: 15457 RVA: 0x0033D4E4 File Offset: 0x0033B6E4
		private void LoadEquipJuHunInfo()
		{
			Dictionary<int, EquipJuHunXmlData> juHunDataDict = new Dictionary<int, EquipJuHunXmlData>();
			try
			{
				XElement xml = Global.GetGameResXml(string.Format("Config/JuHun.xml", new object[0]));
				if (null != xml)
				{
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							int type = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Type", "0"));
							int level = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Level", "0"));
							double growProportion = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "GrowProportion", "0"));
							Dictionary<int, int> costGoods = new Dictionary<int, int>();
							string[] costGoodsArry = Global.GetDefAttributeStr(xmlItem, "CostGoods", "").Split(new char[]
							{
								'|'
							});
							foreach (string item in costGoodsArry)
							{
								string[] str = item.Split(new char[]
								{
									','
								});
								if (str.Length == 2)
								{
									costGoods.Add(Convert.ToInt32(str[0]), Convert.ToInt32(str[1]));
								}
							}
							Dictionary<int, int> protectGoods = new Dictionary<int, int>();
							string[] protectGoodsArray = Global.GetDefAttributeStr(xmlItem, "ProtectGoods", "").Split(new char[]
							{
								'|'
							});
							foreach (string item in protectGoodsArray)
							{
								string[] str = item.Split(new char[]
								{
									','
								});
								if (str.Length == 2)
								{
									protectGoods.Add(Convert.ToInt32(str[0]), Convert.ToInt32(str[1]));
								}
							}
							int costBandJinBi = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "CostBandJinBi", "0"));
							double successProportion = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "SuccessProportion", "0"));
							EquipJuHunXmlData questionTimeItem = new EquipJuHunXmlData
							{
								ID = id,
								Type = type,
								Level = level,
								GrowProportion = growProportion,
								CostGoods = costGoods,
								ProtectGoods = protectGoods,
								CostBandJinBi = costBandJinBi,
								SuccessProportion = successProportion
							};
							juHunDataDict.Add(id, questionTimeItem);
						}
					}
					Data.EquipJuHunDataDict = juHunDataDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("启动时加载xml文件: {0} 失败\r\n{1}", string.Format("Config/JuHun.xml", new object[0]), ex.ToString()), null, true);
			}
		}

		// Token: 0x06003C62 RID: 15458 RVA: 0x0033D804 File Offset: 0x0033BA04
		private void LoadBagType()
		{
			Dictionary<int, BagTypeXmlData> bagTypeDict = new Dictionary<int, BagTypeXmlData>();
			try
			{
				XElement xml = Global.GetGameResXml(string.Format("Config/BagType.xml", new object[0]));
				if (null != xml)
				{
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							string goodsType = Global.GetDefAttributeStr(xmlItem, "GoodsType", "");
							if (!string.IsNullOrEmpty(goodsType))
							{
								int bagType = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BagType", "0"));
								int bindingType = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BangdingType", "0"));
								foreach (string one in goodsType.Split(new char[]
								{
									','
								}))
								{
									bagTypeDict[Convert.ToInt32(one)] = new BagTypeXmlData
									{
										BagType = bagType,
										BindingType = bindingType
									};
								}
							}
						}
					}
					Data.BagTypeDict = bagTypeDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("启动时加载xml文件: {0} 失败\r\n{1}", string.Format("Config/BagType.xml", new object[0]), ex.ToString()), null, true);
			}
		}

		// Token: 0x06003C63 RID: 15459 RVA: 0x0033D9C8 File Offset: 0x0033BBC8
		private void InitMapStallPosList()
		{
			Data.MapStallList.Clear();
			if (null != Global.XmlInfo["ConfigSettings"])
			{
				IEnumerable<XElement> mapList = null;
				try
				{
					XElement xmlRoot = Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "MapStalls");
					if (null != xmlRoot)
					{
						mapList = xmlRoot.Elements("Stall");
					}
				}
				catch (Exception)
				{
				}
				if (null != mapList)
				{
					foreach (XElement xmlItem in mapList)
					{
						int mapCode = (int)Global.GetSafeAttributeLong(xmlItem, "Code");
						Point toPos = new Point((double)((int)Global.GetSafeAttributeLong(xmlItem, "X")), (double)((int)Global.GetSafeAttributeLong(xmlItem, "Y")));
						int radius = (int)Global.GetSafeAttributeLong(xmlItem, "Radius");
						MapStallItem mapStallItem = new MapStallItem
						{
							MapID = mapCode,
							ToPos = toPos,
							Radius = radius
						};
						if (null != mapStallItem)
						{
							Data.MapStallList.Add(mapStallItem);
						}
					}
				}
			}
		}

		// Token: 0x06003C64 RID: 15460 RVA: 0x0033DB30 File Offset: 0x0033BD30
		private void InitMapNameDictionary()
		{
			Data.MapNamesDict.Clear();
			if (null != Global.XmlInfo["ConfigSettings"])
			{
				IEnumerable<XElement> mapList = null;
				try
				{
					XElement xmlRoot = Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Maps");
					if (null != xmlRoot)
					{
						mapList = xmlRoot.Elements("Map");
					}
				}
				catch (Exception)
				{
				}
				if (null != mapList)
				{
					foreach (XElement xmlItem in mapList)
					{
						int mapCode = (int)Global.GetSafeAttributeLong(xmlItem, "Code");
						string mapName = Global.GetSafeAttributeStr(xmlItem, "Name");
						Data.MapNamesDict[mapCode] = mapName;
					}
				}
			}
		}

		// Token: 0x06003C65 RID: 15461 RVA: 0x0033DC38 File Offset: 0x0033BE38
		private void ExitOnError(string msg, Exception ex)
		{
			LogManager.WriteLog(LogTypes.Fatal, msg + ex.ToString(), null, true);
			Console.ReadLine();
			Process.GetCurrentProcess().Kill();
		}

		// Token: 0x06003C66 RID: 15462 RVA: 0x0033DC68 File Offset: 0x0033BE68
		private void InitGameMapsAndMonsters()
		{
			XElement xml = null;
			try
			{
				xml = Global.GetGameResXml("Config/MapConfig.xml");
			}
			catch (Exception ex)
			{
				this.ExitOnError(string.Format("启动时加载xml文件: {0} 失败", "MapConfig.xml"), ex);
			}
			IEnumerable<XElement> mapItems = xml.Elements();
			GameManager.ClientMgr.initialize(mapItems);
			GameManager.MonsterMgr.initialize(mapItems);
			MonsterStaticInfoMgr.Initialize();
			GameManager.MonsterZoneMgr.LoadAllMonsterXml();
			Stopwatch sw = new Stopwatch();
			sw.Start();
			foreach (XElement mapItem in mapItems)
			{
				int mapPicCode = Global.GetMapPicCodeByCode((int)Global.GetSafeAttributeLong(mapItem, "Code"));
				try
				{
					GameMap gameMap = GameManager.MapMgr.InitAddMap((int)Global.GetSafeAttributeLong(mapItem, "Code"), mapPicCode, 0, 0, (int)Global.GetSafeAttributeLong(mapItem, "BirthPosX"), (int)Global.GetSafeAttributeLong(mapItem, "BirthPosY"), (int)Global.GetSafeAttributeLong(mapItem, "BirthRadius"));
					GameManager.MapGridMgr.InitAddMapGrid((int)Global.GetSafeAttributeLong(mapItem, "Code"), gameMap.MapWidth, gameMap.MapHeight, GameManager.MapGridWidth, GameManager.MapGridHeight, gameMap);
					GameManager.MonsterZoneMgr.AddMapMonsters((int)Global.GetSafeAttributeLong(mapItem, "Code"), gameMap);
					NPCGeneralManager.LoadMapNPCRoles((int)Global.GetSafeAttributeLong(mapItem, "Code"), gameMap);
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载地图配置时出错,xml={0}", mapItem.ToString()), ex, true);
				}
			}
			sw.Stop();
			JunQiManager.InitLingDiJunQi();
			SysConOut.WriteLine(StringUtil.substitute("所有地图的怪物总的个数为:{0}, 耗时:{1}ms", new object[]
			{
				GameManager.MonsterMgr.GetTotalMonstersCount(),
				sw.ElapsedMilliseconds
			}));
		}

		// Token: 0x06003C67 RID: 15463 RVA: 0x0033DE9C File Offset: 0x0033C09C
		private void InitCache(XElement xml)
		{
			Global._FullBufferManager = new FullBufferManager();
			Global._SendBufferManager = new SendBufferManager();
			SendBuffer.SendDataIntervalTicks = (long)Global.GMax(20, Global.GMin(500, (int)Global.GetSafeAttributeLong(xml, "SendDataParam", "SendDataIntervalTicks")));
			SendBuffer.MaxSingleSocketSendBufferSize = Global.GMax(18000, Global.GMin(256000, (int)Global.GetSafeAttributeLong(xml, "SendDataParam", "MaxSingleSocketSendBufferSize")));
			SendBuffer.SendDataTimeOutTicks = (long)Global.GMax(3000, Global.GMin(20000, (int)Global.GetSafeAttributeLong(xml, "SendDataParam", "SendDataTimeOutTicks")));
			SendBuffer.MaxBufferSizeForLargePackge = SendBuffer.MaxSingleSocketSendBufferSize * 2 / 3;
			Global._MemoryManager = new MemoryManager();
			string cacheMemoryBlocks = Global.GetSafeAttributeStr(xml, "CacheMemoryParam", "CacheMemoryBlocks");
			if (string.IsNullOrWhiteSpace(cacheMemoryBlocks))
			{
				Global._MemoryManager.AddBatchBlock(100, 1500);
				Global._MemoryManager.AddBatchBlock(600, 400);
				Global._MemoryManager.AddBatchBlock(600, 50);
				Global._MemoryManager.AddBatchBlock(600, 100);
			}
			else
			{
				string[] items = cacheMemoryBlocks.Split(new char[]
				{
					'|'
				});
				foreach (string item in items)
				{
					string[] pair = item.Split(new char[]
					{
						','
					});
					int blockSize = int.Parse(pair[0]);
					int blockNum = int.Parse(pair[1]);
					blockNum = Global.GMax(blockNum, 80);
					if (blockSize > 0 && blockNum > 0)
					{
						Global._MemoryManager.AddBatchBlock(blockNum, blockSize);
						GameManager.MemoryPoolConfigDict[blockSize] = blockNum;
					}
				}
			}
		}

		// Token: 0x06003C68 RID: 15464 RVA: 0x0033E078 File Offset: 0x0033C278
		private void InitTCPManager(XElement xml, bool bConnectDB)
		{
			if (bConnectDB)
			{
				GameManager.DefaultMapCode = (int)Global.GetSafeAttributeLong(xml, "Map", "Code");
				GameManager.MainMapCode = (int)Global.GetSafeAttributeLong(xml, "Map", "MainCode");
				GameManager.ServerLineID = (int)Global.GetSafeAttributeLong(xml, "Server", "LineID");
				GameManager.IsKuaFuServer = (GameManager.ServerLineID > 1);
				GameManager.AutoGiveGoodsIDList = null;
				LogManager.LogTypeToWrite = (LogTypes)Global.GetSafeAttributeLong(xml, "Server", "LogType");
				GameManager.SystemServerEvents.EventLevel = (EventLevels)Global.GetSafeAttributeLong(xml, "Server", "EventLevel");
				GameManager.SystemRoleTaskEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithTongQianEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithYinLiangEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithYinPiaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithYuanBaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleSaleEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleExchangeEvents1.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleExchangeEvents2.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleExchangeEvents3.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleGoodsEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleHorseEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBangGongEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleJingMaiEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleRefreshQiZhenGeEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleWaBaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleMapEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleFuBenAwardEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleWuXingAwardEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRolePaoHuanOkEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleYaBiaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleLianZhanEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleHuoDongMonsterEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleDigTreasureWithYaoShiEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleAutoSubYuanBaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleAutoSubGoldEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleAutoSubEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithTianDiJingYuanEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleFetchVipAwardEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleFetchMailMoneyEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithGoldEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				int dbLog = Global.GMax(0, (int)Global.GetSafeAttributeLong(xml, "DBLog", "DBLogEnable"));
				this.InitCache(xml);
				try
				{
					Global.Flag_NameServer = true;
					NameServerNamager.Init(xml);
				}
				catch (Exception ex)
				{
					Global.Flag_NameServer = false;
					Console.WriteLine(ex.ToString());
				}
				int nCapacity = (int)Global.GetSafeAttributeLong(xml, "Socket", "capacity") * 3;
				TCPManager.getInstance().initialize(nCapacity);
				Global._TCPManager = TCPManager.getInstance();
				Global._TCPManager.tcpClientPool.RootWindow = this;
				Global._TCPManager.tcpClientPool.Init((int)Global.GetSafeAttributeLong(xml, "DBServer", "pool"), Global.GetSafeAttributeStr(xml, "DBServer", "ip"), (int)Global.GetSafeAttributeLong(xml, "DBServer", "port"), "DBServer");
				Global._TCPManager.tcpLogClientPool.RootWindow = this;
				Global._TCPManager.tcpLogClientPool.Init((int)Global.GetSafeAttributeLong(xml, "LogDBServer", "pool"), Global.GetSafeAttributeStr(xml, "LogDBServer", "ip"), (int)Global.GetSafeAttributeLong(xml, "LogDBServer", "port"), "LogDBServer");
				GameManager.systemGMCommands.InitGMCommands(null);
			}
			else
			{
				TCPCmdHandler.KeySHA1 = Global.GetSafeAttributeStr(xml, "Token", "sha1");
				TCPCmdHandler.KeyData = Global.GetSafeAttributeStr(xml, "Token", "data");
				TCPCmdHandler.WebKey = Global.GetSafeAttributeStr(xml, "Token", "webkey");
				TCPCmdHandler.WebKeyLocal = TCPCmdHandler.WebKey;
				string loginWebKey = GameManager.GameConfigMgr.GetGameConfigItemStr("loginwebkey", TCPCmdHandler.WebKey);
				if (!string.IsNullOrEmpty(loginWebKey) && loginWebKey.Length >= 5)
				{
					TCPCmdHandler.WebKey = loginWebKey;
				}
				Global._TCPManager.tcpRandKey.Init((int)Global.GetSafeAttributeLong(xml, "Token", "count"), (int)Global.GetSafeAttributeLong(xml, "Token", "randseed"));
				Global._TCPManager.RootWindow = this;
				Global._TCPManager.Start(Global.GetSafeAttributeStr(xml, "Socket", "ip"), (int)Global.GetSafeAttributeLong(xml, "Socket", "port"));
			}
		}

		// Token: 0x06003C69 RID: 15465 RVA: 0x0033E5C4 File Offset: 0x0033C7C4
		public static void DyLoadConfig()
		{
		}

		// Token: 0x06003C6A RID: 15466 RVA: 0x0033E5C8 File Offset: 0x0033C7C8
		private void InitGameManager(XElement xml)
		{
			GameManager.AppMainWnd = this;
			GameManager.SystemTasksMgr.LoadFromXMlFile("Config/SystemTasks.xml", "Tasks", "ID", 1);
			ProcessTask.InitBranchTasks(GameManager.SystemTasksMgr.SystemXmlItemDict);
			GameManager.NPCTasksMgr.LoadNPCTasks(GameManager.SystemTasksMgr);
			GameManager.SystemNPCsMgr.LoadFromXMlFile("Config/npcs.xml", "NPCs", "ID", 0);
			GameManager.SystemOperasMgr.LoadFromXMlFile("Config/SystemOperations.xml", "Operations", "ID", 0);
			GameManager.SystemGoods.LoadFromXMlFile("Config/Goods.xml", "Goods", "ID", 0);
			SingletonTemplate<GoodsCanUseManager>.Instance().Init();
			SingletonTemplate<GoodsReplaceManager>.Instance().Init();
			GameManager.NPCSaleListMgr.LoadSaleList();
			GameManager.SystemGoodsNamgMgr.LoadGoodsItemsDict(GameManager.SystemGoods);
			GameManager.SystemMagicActionMgr.ParseGoodsActions(GameManager.SystemGoods);
			GameManager.SystemMagicsMgr.LoadFromXMlFile("Config/Magics.xml", "Magics", "ID", 0);
			GameManager.SystemMagicQuickMgr.LoadMagicItemsDict(GameManager.SystemMagicsMgr);
			GameManager.SystemMagicActionMgr.ParseMagicActions(GameManager.SystemMagicsMgr);
			GameManager.SystemMagicActionMgr.ParseMagicActionRelations(GameManager.SystemMagicsMgr);
			GameManager.SystemMagicActionMgr2.ParseMagicActions2(GameManager.SystemMagicsMgr);
			GameManager.SystemMagicScanTypeMgr.ParseScanTypeActions2(GameManager.SystemMagicsMgr);
			MagicsManyTimeDmageCachingMgr.ParseManyTimeDmageItems(GameManager.SystemMagicsMgr);
			GameManager.SystemPassiveMgr.LoadFromXMlFile("Config/PassiveEffect.xml", "", "ID", 0);
			GameManager.SystemPassiveEffectMgr.ParseMagicActions(GameManager.SystemPassiveMgr);
			GameManager.SystemMonsterGoodsList.LoadFromXMlFile("Config/MonsterGoodsList.xml", "", "ID", 1);
			GameManager.SystemLimitTimeMonsterGoodsList.LoadFromXMlFile("Config/HuoDongMonsterGoodsList.xml", "", "ID", 1);
			GameManager.SystemGoodsQuality.LoadFromXMlFile("Config/GoodsQuality.xml", "", "ID", 1);
			GameManager.SystemGoodsLevel.LoadFromXMlFile("Config/GoodsLevel.xml", "", "ID", 1);
			GameManager.SystemGoodsBornIndex.LoadFromXMlFile("Config/GoodsBorn.xml", "", "ID", 1);
			GameManager.SystemGoodsZhuiJia.LoadFromXMlFile("Config/GoodsZhuiJia.xml", "", "ID", 1);
			GameManager.SystemGoodsExcellenceProperty.LoadFromXMlFile("Config/ExcellencePropertyRandom.xml", "ExcellenceProperty", "ID", 1);
			GameManager.SystemBattle.LoadFromXMlFile("Config/Battle.xml", "", "ID", 0);
			GameManager.SystemBattlePaiMingAwards.LoadFromXMlFile("Config/BattlePaiMingAward.xml", "", "ID", 0);
			GameManager.SystemArenaBattle.LoadFromXMlFile("Config/ArenaBattle.xml", "", "ID", 0);
			GameManager.systemNPCScripts.LoadFromXMlFile("Config/NPCScripts.xml", "Scripts", "ID", 0);
			GameManager.SystemMagicActionMgr.ParseNPCScriptActions(GameManager.systemNPCScripts);
			GameManager.systemPets.LoadFromXMlFile("Config/Pet.xml", "Pets", "ID", 0);
			HorseCachingManager.LoadHorseEnchanceItems();
			GameManager.systemGoodsMergeTypes.LoadFromXMlFile("Config/GoodsMergeType.xml", "Types", "ID", 1);
			GameManager.systemGoodsMergeItems.LoadFromXMlFile("Config/GoodsMergeItems.xml", "Items", "ID", 1);
			GameManager.systemParamsList.LoadParamsList();
			BuffManager.InitConfig();
			GameManager.systemMallMgr.LoadFromXMlFile("Config/Mall.xml", "Mall", "ID", 1);
			GongGaoDataManager.LoadGongGaoData();
			JingMaiCacheManager.LoadJingMaiItems();
			MagicsCacheManager.LoadMagicItems();
			TimerBossManager.getInstance();
			GameManager.systemJingMaiExpMgr.LoadFromXMlFile("Config/JingMaiExp.xml", "", "ID", 0);
			GameManager.systemGoodsBaoGuoMgr.LoadFromXMlFile("Config/GoodsPack.xml", "", "ID", 0);
			GameManager.systemWaBaoMgr.LoadFromXMlFile("Config/Dig.xml", "", "ID", 0);
			GameManager.systemWeekLoginGiftMgr.LoadFromXMlFile("Config/Gifts/LoginNumGift.xml", "", "ID", 1);
			GameManager.systemMOnlineTimeGiftMgr.LoadFromXMlFile("Config/Gifts/OnlieTimeGift.xml", "", "ID", 1);
			GameManager.systemNewRoleGiftMgr.LoadFromXMlFile("Config/Gifts/NewRoleGift.xml", "", "ID", 1);
			GameManager.systemCombatAwardMgr.LoadFromXMlFile("Config/Gifts/ComatEffectivenessGift.xml", "", "ID", 1);
			GameManager.systemUpLevelGiftMgr.LoadFromXMlFile("Config/Gifts/UpLevelGift.xml", "", "ID", 1);
			GameManager.systemFuBenMgr.LoadFromXMlFile("Config/FuBen.xml", "", "ID", 0);
			GameManager.systemYaBiaoMgr.LoadFromXMlFile("Config/Yabiao.xml", "", "ID", 0);
			GameManager.systemSpecialTimeMgr.LoadFromXMlFile("Config/SpecialTimes.xml", "", "ID", 1);
			GameManager.systemHeroConfigMgr.LoadFromXMlFile("Config/Hero.xml", "", "ID", 0);
			GameManager.systemBangHuiFlagUpLevelMgr.LoadFromXMlFile("Config/FlagUpLevel.xml", "Flag", "ID", 0);
			GameManager.systemJunQiMgr.LoadFromXMlFile("Config/JunQi.xml", "", "ID", 0);
			GameManager.systemQiZuoMgr.LoadFromXMlFile("Config/QiZuo.xml", "", "ID", 0);
			GameManager.systemLingQiMapQiZhiMgr.LoadFromXMlFile("Config/LingDiQiZhi.xml", "", "ID", 0);
			GameManager.systemQiZhenGeGoodsMgr.LoadFromXMlFile("Config/QiZhenGeGoods.xml", "Mall", "ID", 0);
			GameManager.systemHuangChengFuHuoMgr.LoadFromXMlFile("Config/HuangCheng.xml", "", "ID", 0);
			GameManager.systemBattleExpMgr.LoadFromXMlFile("Config/BattleExp.xml", "", "ID", 0);
			GameManager.systemBangZhanAwardsMgr.LoadFromXMlFile("Config/BangZhanAward.xml", "", "ID", 0);
			GameManager.systemBattleRebirthMgr.LoadFromXMlFile("Config/Rebirth.xml", "", "ID", 0);
			GameManager.systemBattleAwardMgr.LoadFromXMlFile("Config/BattleAward.xml", "", "ID", 0);
			GameManager.systemEquipBornMgr.LoadFromXMlFile("Config/EquipBorn.xml", "", "ID", 0);
			GameManager.systemBornNameMgr.LoadFromXMlFile("Config/BornName.xml", "", "ID", 0);
			GameManager.systemVipDailyAwardsMgr.LoadFromXMlFile("Config/Gifts/VipDailyAwards.xml", "", "AwardID", 1);
			GameManager.systemActivityTipMgr.LoadFromXMlFile("Config/Activity/ActivityTip.xml", "", "ID", 0);
			GameManager.systemLuckyAwardMgr.LoadFromXMlFile("Config/LuckyAward.xml", "", "ID", 0);
			GameManager.systemLuckyAward2Mgr.LoadFromXMlFile("Config/LuckyAward2.xml", "", "ID", 0);
			GameManager.systemLuckyMgr.LoadFromXMlFile("Config/Lucky.xml", "", "Number", 0);
			GameManager.systemChengJiu.LoadFromXMlFile("Config/ChengJiu.xml", "ChengJiu", "ChengJiuID", 0);
			ChengJiuManager.InitChengJiuConfig();
			GameManager.systemChengJiuBuffer.LoadFromXMlFile("Config/ChengJiuBuff.xml", "", "ID", 0);
			GameManager.systemWeaponTongLing.LoadFromXMlFile("Config/TongLing.xml", "", "ID", 0);
			QianKunManager.LoadImpetrateItemsInfo();
			QianKunManager.LoadImpetrateItemsInfoFree();
			QianKunManager.LoadImpetrateItemsInfoHuodong();
			QianKunManager.LoadImpetrateItemsInfoFreeHuoDong();
			QianKunManager.LoadImpetrateItemsInfoTeQuan();
			QianKunManager.LoadImpetrateItemsInfoFreeTeQuan();
			GameManager.systemImpetrateByLevelMgr.LoadFromXMlFile("Config/DigType.xml", "", "ID", 0);
			GameManager.systemXingYunChouJiangMgr.LoadFromXMlFile("Config/RiChangGifts/NewDig1.xml", "", "ID", 0);
			GameManager.systemYueDuZhuanPanChouJiangMgr.LoadFromXMlFile("Config/RiChangGifts/NewDig2.xml", "GiftList", "ID", 0);
			GameManager.systemEveryDayOnLineAwardMgr.LoadFromXMlFile("Config/Gifts/MUNewRoleGift.xml", "", "ID", 1);
			GameManager.systemSeriesLoginAwardMgr.LoadFromXMlFile("Config/Gifts/MULoginNumGift.xml", "", "ID", 1);
			GameManager.systemMonsterMgr.LoadFromXMlFile("Config/Monsters.xml", "Monsters", "ID", 0);
			GameManager.SystemJingMaiLevel.LoadFromXMlFile("Config/JingMai.xml", "", "ID", 0);
			GameManager.SystemWuXueLevel.LoadFromXMlFile("Config/WuXue.xml", "", "ID", 0);
			GameManager.SystemTaskPlots.LoadFromXMlFile("Config/TaskPlot.xml", "", "ID", 1);
			GameManager.SystemQiangGou.LoadFromXMlFile("Config/QiangGou.xml", "", "ID", 1);
			GameManager.SystemHeFuQiangGou.LoadFromXMlFile("Config/HeFuGifts/HeFuQiangGou.xml", "", "ID", 0);
			GameManager.SystemJieRiQiangGou.LoadFromXMlFile("Config/JieRiGifts/JieRiQiangGou.xml", "", "ID", 0);
			GameManager.SystemZuanHuangLevel.LoadFromXMlFile("Config/ZuanHuang.xml", "", "ID", 0);
			GameManager.SystemSystemOpen.LoadFromXMlFile("Config/SystemOpen.xml", "", "ID", 0);
			GameManager.SystemDropMoney.LoadFromXMlFile("Config/DropMoney.xml", "", "ID", 0);
			GameManager.SystemDengLuDali.LoadFromXMlFile("Config/Gifts/HuoDongLoginNumGift.xml", "GoodsList", "ID", 1);
			GameManager.SystemBuChang.LoadFromXMlFile("Config/BuChang.xml", "", "ID", 0);
			GameManager.SystemZhanHunLevel.LoadFromXMlFile("Config/ZhanHun.xml", "", "ID", 0);
			GameManager.SystemRongYuLevel.LoadFromXMlFile("Config/RongYu.xml", "", "ID", 0);
			GameManager.SystemExchangeMoJingAndQiFu.LoadFromXMlFile("Config/DuiHuanItems.xml", "Items", "ID", 1);
			GameManager.SystemExchangeType.LoadFromXMlFile("Config/DuiHuanType.xml", "DuiHuan", "ID", 1);
			GameManager.systemCaiJiMonsterMgr.LoadFromXMlFile("Config/CrystalMonster.xml", "", "MonsterID", 0);
			GameManager.SystemDamonUpgrade.LoadFromXMlFile("Config/PetLevelUp.xml", "", "ID", 0);
			GameManager.QingGongYanMgr.LoadQingGongYanConfig();
			CopyTargetManager.LoadConfig();
			CallPetManager.LoadCallPetType();
			CallPetManager.LoadCallPetConfig();
			CallPetManager.LoadCallPetSystem();
			WingStarCacheManager.LoadWingStarItems();
			Global.LoadVipLevelAwardList();
			ChengJiuManager.InitFlagIndex();
			ChengJiuManager.initAchievementRune();
			EquipUpgradeCacheMgr.LoadEquipUpgradeItems();
			FuBenManager.LoadFuBenMap();
			GoodsBaoGuoCachingMgr.LoadGoodsBaoGuoDict();
			WuXingMapMgr.LoadXuXingConfig();
			WuXingMapMgr.LoadWuXingAward();
			BroadcastInfoMgr.LoadBroadcastInfoItemList();
			PopupWinMgr.LoadPopupWinItemList();
			MallGoodsMgr.InitMallGoodsPriceDict();
			ChuanQiQianHua.LoadEquipQianHuaProps();
			this.LoadCopyScoreDataInfo();
			this.LoadFreshPlayerCopySceneInfo();
			this.LoadTaskStarDataInfo();
			this.LoadDailyCircleTaskAwardInfo();
			this.LoadTaofaTaskAwardInfo();
			this.LoadCombatForceInfoInfo();
			this.LoadDaimonSquareDataInfo();
			this.LoadSystemParamsDataForCache();
			Program.LoadTotalLoginDataInfo();
			this.LoadVIPDataInfo();
			this.LoadVIPLevAwardAndExpInfo();
			this.LoadMeditateInfo();
			GameManager.systemDailyActiveInfo.LoadFromXMlFile("Config/DailyActiveInfor.xml", "DailyActive", "DailyActiveID", 0);
			GameManager.systemDailyActiveAward.LoadFromXMlFile("Config/DailyActiveAward.xml", "DailyActiveAward", "ID", 0);
			DailyActiveManager.InitDailyActiveFlagIndex();
			this.LoadExperienceCopyMapDataInfo();
			GameManager.systemAngelTempleData.LoadFromXMlFile("Config/AngelTemple.xml", "", "ID", 0);
			GameManager.AngelTempleAward.LoadFromXMlFile("Config/AngelTempleAward.xml", "", "ID", 0);
			GameManager.AngelTempleLuckyAward.LoadFromXMlFile("Config/AngelTempleLuckyAward.xml", "", "ID", 0);
			GameManager.TaskZhangJie.LoadFromXMlFile("Config/TaskZhangJie.xml", "", "ID", 1);
			ReloadXmlManager.InitTaskZhangJieInfo();
			GameManager.JiaoYiTab.LoadFromXMlFile("Config/JiaoYiTab.xml", "", "TabID", 0);
			GameManager.JiaoYiType.LoadFromXMlFile("Config/JiaoYiType.xml", "", "ID", 0);
			GameManager.SystemZhanMengBuild.LoadFromXMlFile("Config/ZhanMengBuild.xml", "", "ID", 0);
			GameManager.SystemWingsUp.LoadFromXMlFile("Config/Wing/WingUp.xml", "", "Level", 0);
			GameManager.SystemBossAI.LoadFromXMlFile("Config/AI.xml", "", "ID", 0);
			GameManager.SystemMagicActionMgr.ParseBossAIActions(GameManager.SystemBossAI);
			BossAICachingMgr.LoadBossAICachingItems(GameManager.SystemBossAI);
			GameManager.SystemExtensionProps.LoadFromXMlFile("Config/TuoZhan.xml", "", "ID", 0);
			GameManager.SystemMagicActionMgr.ParseExtensionPropsActions(GameManager.SystemExtensionProps);
			ExtensionPropsMgr.LoadCachingItems(GameManager.SystemExtensionProps);
			this.LoadBossHomeInfo();
			this.LoadGoldTempleInfo();
			this.LoadFuBenNeedInfo();
			this.LoadEquipUpgradeInfo();
			this.LoadGoldCopySceneInfo();
			GameManager.MagicSwordMgr.LoadMagicSwordData();
			GameManager.SummonerMgr.LoadSummonerData();
			GameManager.MerlinMagicBookMgr.LoadMerlinSystemParamsConfigData();
			GameManager.MerlinMagicBookMgr.LoadMerlinConfigData();
			GameManager.FluorescentGemMgr.LoadFluorescentGemConfigData();
			SingletonTemplate<GetInterestingDataMgr>.Instance().LoadConfig();
			Global.LoadSpecialMachineConfig();
			ElementhrtsManager.LoadRefineType();
			ElementhrtsManager.LoadElementHrtsBase();
			ElementhrtsManager.LoadElementHrtsLevelInfo();
			ElementhrtsManager.LoadSpecialElementHrtsExp();
			this.LoadEquipJuHunInfo();
			this.LoadBagType();
			WeaponMaster.LoadWeaponMaster();
			MoYuLongXue.LoadMoYuXml();
			ZhuanShengShiLian.LoadZhuanShengShiLianXml();
			GameManager.PlatConfigMgr.LoadPlatConfig();
			UserMoneyMgr.getInstance().InitConfig();
			Program.LoadIPList("");
			GameManager.BattleMgr.Init();
			GameManager.ArenaBattleMgr.Init();
			GameManager.ShengXiaoGuessMgr.Init();
			GameManager.BulletinMsgMgr.LoadBulletinMsgFromDBServer();
			JunQiManager.LoadBangHuiJunQiItemsDictFromDBServer();
			JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer();
			JunQiManager.ParseWeekDaysTimes();
			Global.InitBagParams();
			Global.InitGuMuMapCodes();
			Global.InitVipGumuExpMultiple();
			Global.InitMingJieMapCodeList();
			Global.InitDecreaseInjureInfo();
			Global.InitAllForgeLevelInfo();
			Global.LoadItemLogMark();
			Global.LoadLogTradeGoods();
			Global.LoadForgeSystemParams();
			Global.LoadReliveMonsterGongGaoMark();
			ArtifactManager.initArtifact();
			if (!HuodongCachingMgr.LoadActivitiesConfig())
			{
				Process.GetCurrentProcess().Kill();
			}
			if (!HuodongCachingMgr.LoadHeFuActivitiesConfig())
			{
				Process.GetCurrentProcess().Kill();
			}
			if (!HuodongCachingMgr.LoadJieriActivitiesConfig())
			{
				Process.GetCurrentProcess().Kill();
			}
			Global.InitMapSceneTypeDict();
			GameManager.AngelTempleMgr.InitAngelTemple();
			GameManager.BloodCastleCopySceneMgr.InitBloodCastleCopyScene();
			GameManager.DaimonSquareCopySceneMgr.InitDaimonSquareCopyScene();
			GameManager.StarConstellationMgr.LoadStarConstellationTypeInfo();
			GameManager.StarConstellationMgr.LoadStarConstellationDetailInfo();
			CaiJiLogic.LoadConfig();
			GameManager.GuildCopyMapMgr.LoadGuildCopyMapOrder();
			LingYuManager.LoadConfig();
			ZhuLingZhuHunManager.LoadConfig();
			YueKaManager.LoadConfig();
			UpgradeDamon.LoadUpgradeAttr();
			GameManager.VersionSystemOpenMgr.LoadVersionSystemOpenData();
			SingletonTemplate<TuJianManager>.Instance().LoadConfig();
			SingletonTemplate<GuardStatueManager>.Instance().LoadConfig();
			GameManager.loginWaitLogic.LoadConfig();
			GameFuncControlManager.LoadConfig(Global.GameResPath("Config/GameFuncControl.xml"));
			Data.ThemeActivityState = HuodongCachingMgr.GetThemeActivityState();
			if (Data.ThemeActivityState == 0 && Data.ZhuTiID > 0)
			{
				LogManager.WriteLog(LogTypes.Fatal, "主题服开关配置ThemeActivityState和ZhuTiID配置不一致", null, true);
			}
			LogFilterConfig.InitConfig();
			GoldAuctionConfigModel.LoadConfig();
			BoCaiConfigMgr.LoadConfig(false);
		}

		// Token: 0x06003C6B RID: 15467 RVA: 0x0033F3E0 File Offset: 0x0033D5E0
		private void InitGameConfigWithDB()
		{
			GameManager.ServerId = Global.sendToDB<int, string>(11002, "", 0);
			GameManager.PTID = Global.sendToDB<int, string>(11005, "", 0);
			GameManager.KuaFuServerId = ConstData.ConvertToKuaFuServerID(GameManager.ServerId, GameManager.PTID);
			GameManager.Flag_OptimizationBagReset = (GameManager.GameConfigMgr.GetGameConfigItemInt("optimization_bag_reset", 1) > 0);
			GameManager.SetLogFlags((long)GameManager.GameConfigMgr.GetGameConfigItemInt("logflags", int.MaxValue));
			string platformType = GameManager.GameConfigMgr.GetGameConfigItemStr("platformtype", "");
			for (PlatformTypes i = PlatformTypes.Tmsk; i < PlatformTypes.Max; i++)
			{
				if (0 == string.Compare(platformType, i.ToString(), true))
				{
					GameManager.PlatformType = i;
					break;
				}
			}
			if (platformType == "andrid")
			{
				GameManager.PlatformType = PlatformTypes.Android;
			}
			if (PlatformTypes.Tmsk == GameManager.PlatformType)
			{
				throw new Exception(string.Format("t_config platformtype wrong!!!", new object[0]));
			}
			if (GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", -1) <= 0)
			{
				throw new Exception(string.Format("t_config money-to-yuanbao wrong!!!", new object[0]));
			}
			GameManager.LoadGameConfigFlags();
		}

		// Token: 0x06003C6C RID: 15468 RVA: 0x0033F51F File Offset: 0x0033D71F
		private void InitMonsterManager()
		{
		}

		// Token: 0x06003C6D RID: 15469 RVA: 0x0033F524 File Offset: 0x0033D724
		protected static void StartThreadPoolDriverTimer()
		{
			Program.ThreadPoolDriverTimer = new Timer(new TimerCallback(Program.ThreadPoolDriverTimer_Tick), null, 1000, 1000);
			Program.LogThreadPoolDriverTimer = new Timer(new TimerCallback(Program.LogThreadPoolDriverTimer_Tick), null, 500, 500);
		}

		// Token: 0x06003C6E RID: 15470 RVA: 0x0033F574 File Offset: 0x0033D774
		protected static void StopThreadPoolDriverTimer()
		{
			Program.ThreadPoolDriverTimer.Change(-1, -1);
			Program.LogThreadPoolDriverTimer.Change(-1, -1);
		}

		// Token: 0x06003C6F RID: 15471 RVA: 0x0033F594 File Offset: 0x0033D794
		protected static void ThreadPoolDriverTimer_Tick(object sender)
		{
			try
			{
				Program.ServerConsole.ExecuteBackgroundWorkers(null, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ThreadPoolDriverTimer_Tick", false, false);
			}
		}

		// Token: 0x06003C70 RID: 15472 RVA: 0x0033F5E0 File Offset: 0x0033D7E0
		public static void LogThreadPoolDriverTimer_Tick(object sender)
		{
			try
			{
				Program.ServerConsole.ExecuteBackgroundLogWorkers(null, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "LogThreadPoolDriverTimer_Tick", false, false);
			}
		}

		// Token: 0x06003C71 RID: 15473 RVA: 0x0033F628 File Offset: 0x0033D828
		private void ExecuteBackgroundLogWorkers(object sender, EventArgs e)
		{
			try
			{
				if (!this.logDBCommandWorker.IsBusy)
				{
					this.logDBCommandWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "logDBCommandWorker", false, false);
			}
		}

		// Token: 0x06003C72 RID: 15474 RVA: 0x0033F67C File Offset: 0x0033D87C
		private void ExecuteBackgroundWorkers(object sender, EventArgs e)
		{
			try
			{
				if (!this.eventWorker.IsBusy)
				{
					this.eventWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "eventWorker", false, false);
			}
			try
			{
				if (!this.dbCommandWorker.IsBusy)
				{
					this.dbCommandWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "dbCommandWorker", false, false);
			}
			try
			{
				if (!this.clientsWorker.IsBusy)
				{
					this.clientsWorker.RunWorkerAsync(0);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "clientsWorker", false, false);
			}
			try
			{
				if (!this.buffersWorker.IsBusy)
				{
					this.buffersWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "buffersWorker", false, false);
			}
			try
			{
				if (!this.spriteDBWorker.IsBusy)
				{
					this.spriteDBWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "spriteDBWorker", false, false);
			}
			try
			{
				if (!this.othersWorker.IsBusy)
				{
					this.othersWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "othersWorker", false, false);
			}
			try
			{
				if (!this.FightingWorker.IsBusy)
				{
					this.FightingWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "FightingWorker", false, false);
			}
			try
			{
				if (!this.chatMsgWorker.IsBusy)
				{
					this.chatMsgWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "chatMsgWorker", false, false);
			}
			try
			{
				if (!this.fuBenWorker.IsBusy)
				{
					this.fuBenWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "fuBenWorker", false, false);
			}
			try
			{
				if (!this.dbWriterWorker.IsBusy)
				{
					this.dbWriterWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "dbWriterWorker", false, false);
			}
			try
			{
				if (!this.SocketSendCacheDataWorker.IsBusy)
				{
					this.SocketSendCacheDataWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "SocketSendCacheDataWorker", false, false);
			}
			try
			{
				if (!this.ShengXiaoGuessWorker.IsBusy)
				{
					this.ShengXiaoGuessWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ShengXiaoGuessWorker", false, false);
			}
			try
			{
				if (!this.socketCheckWorker.IsBusy)
				{
					this.socketCheckWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "socketCheckWorker", false, false);
			}
			try
			{
				if (!this.BanWorker.IsBusy)
				{
					this.BanWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "BanWorker", false, false);
			}
			try
			{
				if (!this.IPStatisticsWorker.IsBusy)
				{
					this.IPStatisticsWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "BanWorker", false, false);
			}
			try
			{
				if (!this.TwLogWorker.IsBusy)
				{
					this.TwLogWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "TwLogWorker", false, false);
			}
			Program.CalcGCInfo();
		}

		// Token: 0x06003C73 RID: 15475 RVA: 0x0033FAAC File Offset: 0x0033DCAC
		private void closingTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				string title = "";
				GameClient client = GameManager.ClientMgr.GetRandomClient();
				if (null != client)
				{
					title = string.Format("游戏服务器{0}, 关闭中, 剩余{1}个角色", GameManager.ServerLineID, GameManager.ClientMgr.GetClientCount());
					Global.ForceCloseClient(client, "游戏服务器关闭", true);
				}
				else
				{
					this.ClosingCounter -= 200;
					if (this.ClosingCounter <= 0)
					{
						Global._SendBufferManager.Exit = true;
						this.MustCloseNow = true;
					}
					else
					{
						int counter = GameManager.DBCmdMgr.GetDBCmdCount() + this.ClosingCounter / 200;
						title = string.Format("游戏服务器{0}, 关闭中, 倒计时:{1}", GameManager.ServerLineID, counter);
					}
				}
				Console.Title = title;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "closingTimer_Tick", false, false);
			}
		}

		// Token: 0x06003C74 RID: 15476 RVA: 0x0033FBAC File Offset: 0x0033DDAC
		public void auxiliaryTimer_Tick(object sender, EventArgs e)
		{
			long ticksA = TimeUtil.NOW();
			try
			{
				long ticks = TimeUtil.NOW();
				if (ticks - this.LastAuxiliaryTicks > 1000L)
				{
					this.DoLog(string.Format("\r\nauxiliaryTimer_Tick开始执行经过时间:{0}毫秒", ticks - this.LastAuxiliaryTicks));
				}
				this.LastAuxiliaryTicks = ticks;
				ticks = TimeUtil.NOW();
				ticks = TimeUtil.NOW();
				long ticks2 = TimeUtil.NOW();
				ticks = TimeUtil.NOW();
				Global._TCPManager.tcpClientPool.Supply();
				Global._TCPManager.tcpLogClientPool.Supply();
				ticks2 = TimeUtil.NOW();
				if (ticks2 > ticks + 1000L)
				{
					this.DoLog(string.Format("tcpClientPool.Supply 消耗:{0}毫秒", ticks2 - ticks));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "auxiliaryTimer_Tick", false, false);
			}
			long ticksB = TimeUtil.NOW();
			if (ticksB > ticksA + 1000L)
			{
				this.DoLog(string.Format("auxiliaryTimer_Tick 消耗:{0}毫秒", ticksB - ticksA));
			}
		}

		// Token: 0x06003C75 RID: 15477 RVA: 0x0033FCE0 File Offset: 0x0033DEE0
		public void dynamicMonsterTimer_Tick(object sender, EventArgs e)
		{
			long ticksA = TimeUtil.NOW();
			try
			{
				long ticks = TimeUtil.NOW();
				if (ticks - this.LastDynamicMonsterTicks > 1000L)
				{
					this.DoLog(string.Format("\r\ndynamicMonsterTimer_Tick开始执行经过时间:{0}毫秒", ticks - this.LastDynamicMonsterTicks));
				}
				this.LastDynamicMonsterTicks = ticks;
				ticks = TimeUtil.NOW();
				GameManager.MonsterZoneMgr.RunMapMonsters(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				long ticks2 = TimeUtil.NOW();
				if (ticks2 > ticks + 1000L)
				{
					this.DoLog(string.Format("RunMapMonsters 消耗:{0}毫秒", ticks2 - ticks));
				}
				ticks = TimeUtil.NOW();
				GameManager.MonsterZoneMgr.RunMapDynamicMonsters(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				ticks2 = TimeUtil.NOW();
				if (ticks2 > ticks + 1000L)
				{
					this.DoLog(string.Format("RunMapDynamicMonsters 消耗:{0}毫秒", ticks2 - ticks));
				}
				ticks = TimeUtil.NOW();
				GameManager.MonsterMgr.DoMonsterDeadCall();
				ticks2 = TimeUtil.NOW();
				if (ticks2 > ticks + 1000L)
				{
					this.DoLog(string.Format("DoMonsterDeadCall 消耗:{0}毫秒", ticks2 - ticks));
				}
				if (ticks2 > this.LastMonsterUniqueIdProcTicks)
				{
					ticks = ticks2;
					this.LastMonsterUniqueIdProcTicks = ticks2 + 60000L;
					GameManager.MonsterMgr.DoDeadMonsterUniqueIdProc(ticks);
					ticks2 = TimeUtil.NOW();
					if (ticks2 > ticks + 1000L)
					{
						this.DoLog(string.Format("DoDeadMonsterUniqueIdProc 消耗:{0}毫秒", ticks2 - ticks));
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "dynamicMonsterTimer_Tick", false, false);
			}
			long ticksB = TimeUtil.NOW();
			if (ticksB > ticksA + 1000L)
			{
				this.DoLog(string.Format("dynamicMonsterTimer_Tick 消耗:{0}毫秒", ticksB - ticksA));
			}
		}

		// Token: 0x06003C76 RID: 15478 RVA: 0x0033FF10 File Offset: 0x0033E110
		private void DoLog(string warning)
		{
			LogManager.WriteLog(LogTypes.Error, warning, null, true);
		}

		// Token: 0x06003C77 RID: 15479 RVA: 0x0033FF20 File Offset: 0x0033E120
		private void MainDispatcherWorker_DoWork(object sender, EventArgs e)
		{
			long lastTicks = TimeUtil.NOW();
			long startTicks = TimeUtil.NOW();
			long endTicks = TimeUtil.NOW();
			int maxSleepMs = 100;
			int nTimes = 0;
			for (;;)
			{
				try
				{
					startTicks = TimeUtil.NOW();
					if (startTicks - lastTicks >= 500L)
					{
						GameManager.GM_NoCheckTokenTimeRemainMS -= startTicks - lastTicks;
						lastTicks = startTicks;
						this.auxiliaryTimer_Tick(null, null);
						GlobalEventSource.getInstance().fireEvent(new GameRunningEventObject());
					}
					if (Program.NeedExitServer)
					{
						this.closingTimer_Tick(null, null);
						if (this.MustCloseNow)
						{
							break;
						}
					}
					endTicks = TimeUtil.NOW();
					int sleepMs = (int)Math.Max(5L, (long)maxSleepMs - (endTicks - startTicks));
					long endTicks2 = TimeUtil.NOW();
					long endTicks3 = DateTime.Now.Ticks / 10000L;
					Thread.Sleep(sleepMs);
					long endTicks4 = TimeUtil.NOW();
					long endTicks5 = DateTime.Now.Ticks / 10000L;
					if (endTicks4 - endTicks2 > (long)(sleepMs + 1000))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("MainDispatcherWorker_DoWork sleepMs={0} endTicks={1} dataTimeTicks={2}", sleepMs, endTicks4 - endTicks2, endTicks5 - endTicks3), null, true);
					}
					nTimes++;
					if (nTimes >= 100000)
					{
						nTimes = 0;
					}
					if (0 != Program.GetServerPIDFromFile())
					{
						Program.OnExitServer();
					}
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "MainDispatcherWorker_DoWork", false, false);
				}
			}
			SysConOut.WriteLine("主循环线程退出，回车退出系统");
			if (0 != Program.GetServerPIDFromFile())
			{
				Program.WritePIDToFile("Stop.txt");
				Program.StopThreadPoolDriverTimer();
			}
		}

		// Token: 0x06003C78 RID: 15480 RVA: 0x00340100 File Offset: 0x0033E300
		private void TwLogWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				TwLogManager.ScanLog();
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "TwLogWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C79 RID: 15481 RVA: 0x0034013C File Offset: 0x0033E33C
		private void IPStatisticsWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				GlobalEventSource.getInstance().DispatchEventAsync();
				IPStatisticsManager.getInstance().TimerProcForIP();
				IPStatisticsManager.getInstance().TimerProcForUserID();
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "IPStatisticsWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C7A RID: 15482 RVA: 0x00340194 File Offset: 0x0033E394
		private void LoadBanWorker_DoWork(object sender, EventArgs e)
		{
			long lastTicks = TimeUtil.NOW();
			long startTicks = TimeUtil.NOW();
			long endTicks = TimeUtil.NOW();
			int maxSleepMs = 20;
			for (;;)
			{
				try
				{
					startTicks = TimeUtil.NOW();
					if (startTicks - lastTicks >= 20L)
					{
						lastTicks = startTicks;
						FileBanLogic.Tick();
						BanManager.CheckBanMemory();
					}
					endTicks = TimeUtil.NOW();
					int sleepMs = (int)Math.Max(5L, (long)maxSleepMs - (endTicks - startTicks));
					Thread.Sleep(sleepMs);
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "LoadBanWorker_DoWork", false, false);
				}
			}
		}

		// Token: 0x06003C7B RID: 15483 RVA: 0x00340230 File Offset: 0x0033E430
		private void DynamicMonstersWorker_DoWork(object sender, EventArgs e)
		{
			long lastTicks = TimeUtil.NOW();
			long startTicks = TimeUtil.NOW();
			long endTicks = TimeUtil.NOW();
			int maxSleepMs = 100;
			int nTimes = 0;
			for (;;)
			{
				try
				{
					startTicks = TimeUtil.NOW();
					if (startTicks - lastTicks >= 100L)
					{
						lastTicks = startTicks;
						this.dynamicMonsterTimer_Tick(null, null);
					}
					if (Program.NeedExitServer)
					{
						if (nTimes % 2 == 0)
						{
							if (this.MustCloseNow)
							{
								break;
							}
						}
					}
					endTicks = TimeUtil.NOW();
					int sleepMs = (int)Math.Max(5L, (long)maxSleepMs - (endTicks - startTicks));
					if (sleepMs > maxSleepMs)
					{
						sleepMs = maxSleepMs;
						LogManager.WriteLog(LogTypes.Alert, string.Format("TimeMismatch#DynamicMonstersWorker_DoWork,startTicks={0},endTicks={1}", startTicks, endTicks), null, true);
					}
					GameManager.LastFlushMonsterMs = lastTicks;
					Thread.Sleep(sleepMs);
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "DynamicMonstersWorker_DoWork", false, false);
				}
			}
		}

		// Token: 0x06003C7C RID: 15484 RVA: 0x00340340 File Offset: 0x0033E540
		private void RoleStroyboardDispatcherWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			long startTicks = TimeUtil.NOW();
			long endTicks = TimeUtil.NOW();
			int maxSleepMs = 100;
			int nTimes = 0;
			for (;;)
			{
				try
				{
					startTicks = TimeUtil.NOW();
					long ticks = startTicks;
					StoryBoard4Client.runStoryBoards();
					long ticks2 = TimeUtil.NOW();
					if (ticks2 > ticks + 1000L)
					{
						this.DoLog(string.Format("StoryBoard4Client.runStoryBoards 消耗:{0}毫秒", ticks2 - ticks));
					}
					if (Program.NeedExitServer)
					{
						if (nTimes % 2 == 0)
						{
							this.closingTimer_Tick(null, null);
							if (this.MustCloseNow)
							{
								break;
							}
						}
					}
					endTicks = TimeUtil.NOW();
					int sleepMs = (int)Math.Max(5L, (long)maxSleepMs - (endTicks - startTicks));
					Thread.Sleep(sleepMs);
					nTimes++;
					if (nTimes >= 100000)
					{
						nTimes = 0;
					}
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "RoleStroyboardDispatcherWorker_DoWork", false, false);
				}
			}
			SysConOut.WriteLine("角色故事版驱动线程退出，回车退出系统");
		}

		// Token: 0x06003C7D RID: 15485 RVA: 0x00340464 File Offset: 0x0033E664
		private void eventWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				while (GameManager.SystemServerEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleTaskEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithTongQianEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithYinLiangEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithJunGongEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithYinPiaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithYuanBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleQiZhenGeBuyWithYuanBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleQiangGouBuyWithYuanBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleSaleEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleExchangeEvents1.WriteEvent())
				{
				}
				while (GameManager.SystemRoleExchangeEvents2.WriteEvent())
				{
				}
				while (GameManager.SystemRoleExchangeEvents3.WriteEvent())
				{
				}
				while (GameManager.SystemRoleGoodsEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleHorseEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBangGongEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleJingMaiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleRefreshQiZhenGeEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleWaBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleMapEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleFuBenAwardEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleWuXingAwardEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRolePaoHuanOkEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleYaBiaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleLianZhanEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleHuoDongMonsterEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleAutoSubYuanBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleAutoSubGoldEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleAutoSubEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleDigTreasureWithYaoShiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithTianDiJingYuanEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleFetchMailMoneyEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleFetchVipAwardEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithGoldEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithJingYuanZhiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithLieShaZhiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithZhuangBeiJiFenEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithJunGongZhiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithZhanHunEvents.WriteEvent())
				{
				}
				while (GameManager.SystemGlobalGameEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleGameEvents.WriteEvent())
				{
				}
				while (GameManager.SystemClientLogsEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleConsumeEvents.WriteEvent())
				{
				}
				EventLogManager.WriteAllEvents();
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "eventWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C7E RID: 15486 RVA: 0x003407B4 File Offset: 0x0033E9B4
		private void dbCommandWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				DelayForceClosingMgr.ProcessDelaySockets();
				GameManager.DBCmdMgr.ExecuteDBCmd(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "dbCommandWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C7F RID: 15487 RVA: 0x00340810 File Offset: 0x0033EA10
		private void logDBCommandWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				GameManager.logDBCmdMgr.ExecuteDBCmd(Global._TCPManager.tcpLogClientPool, Global._TCPManager.TcpOutPacketPool);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "logDBCommandWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C80 RID: 15488 RVA: 0x00340868 File Offset: 0x0033EA68
		private void clientsWorker_DoWork(object sender, EventArgs e)
		{
			DoWorkEventArgs de = e as DoWorkEventArgs;
			try
			{
				long ticksA = TimeUtil.NOW();
				GameManager.ClientMgr.DoSpriteBackgourndWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				long ticksB = TimeUtil.NOW();
				if (ticksB > ticksA + 1000L)
				{
					this.DoLog(string.Format("clientsWorker_DoWork{0} 消耗:{1}毫秒", (int)de.Argument, ticksB - ticksA));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, string.Format("clientsWorker_DoWork{0}", (int)de.Argument), false, false);
			}
		}

		// Token: 0x06003C81 RID: 15489 RVA: 0x00340924 File Offset: 0x0033EB24
		private void buffersWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long ticksA = TimeUtil.NOW();
				GameManager.ClientMgr.DoSpriteBuffersWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				long ticksB = TimeUtil.NOW();
				if (ticksB > ticksA + 1000L)
				{
					this.DoLog(string.Format("buffersWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, string.Format("buffersWorker_DoWork", new object[0]), false, false);
			}
		}

		// Token: 0x06003C82 RID: 15490 RVA: 0x003409C0 File Offset: 0x0033EBC0
		private void spriteDBWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long ticksA = TimeUtil.NOW();
				GameManager.ClientMgr.DoSpriteDBWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				long ticksB = TimeUtil.NOW();
				if (ticksB > ticksA + 1000L)
				{
					this.DoLog(string.Format("spriteDBWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, string.Format("spriteDBWorker_DoWork", new object[0]), false, false);
			}
		}

		// Token: 0x06003C83 RID: 15491 RVA: 0x00340A5C File Offset: 0x0033EC5C
		private void othersWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long ticksA = TimeUtil.NOW();
				GameManager.GridMagicHelperMgr.ExecuteAllItems();
				GameManager.BulletinMsgMgr.ProcessBulletinMsg();
				DecorationManager.ProcessAllDecos(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				GameManager.GoodsPackMgr.ProcessAllGoodsPackItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				BiaoCheManager.ProcessAllBiaoCheItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				FakeRoleManager.ProcessAllFakeRoleItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				SpecailTimeManager.ProcessDoulbeExperience();
				HuodongCachingMgr.ProcessKaiFuGiftAwardActions();
				DelayActionManager.HeartBeatDelayAction();
				GameManager.QingGongYanMgr.CheckQingGongYan(ticksA);
				HuodongCachingMgr.CheckJieRiActivityState(ticksA);
				UserReturnManager.getInstance().CheckUserReturnOpenState(ticksA);
				OlympicsManager.getInstance().CheckOlympicsOpenState(ticksA, false);
				WorldLevelManager.getInstance().ResetWorldLevel();
				MarryPartyLogic.getInstance().MarryPartyPeriodicUpdate(ticksA);
				MarryLogic.ApplyPeriodicClear(ticksA);
				GameManager.loginWaitLogic.Tick();
				TimeUtil.RecordTimeAnchor();
				JieriPlatChargeKing act = HuodongCachingMgr.GetJieriPlatChargeKingActivity();
				if (act != null)
				{
					act.Update();
				}
				JieriPlatChargeKingEveryDay actEveryDay = HuodongCachingMgr.GetJieriPCKingEveryDayActivity();
				if (actEveryDay != null)
				{
					actEveryDay.Update();
				}
				GameManager.ServerMonitor.CheckReport();
				SingletonTemplate<TradeBlackManager>.Instance().Update();
				AoYunDaTiManager.getInstance().AoyunDaTiTimer_Work();
				ZhuanPanManager.getInstance().ZhuanPanTimer_Work();
				YaoSaiMissionManager.getInstance().YaoSaiMissionTimer_Work();
				EraManager.getInstance().EraTimer_Work();
				SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != spAct)
				{
					spAct.TimerProc();
				}
				long ticksB = TimeUtil.NOW();
				if (ticksB > ticksA + 1000L)
				{
					this.DoLog(string.Format("othersWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "othersWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C84 RID: 15492 RVA: 0x00340C58 File Offset: 0x0033EE58
		private void FightingWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long ticksA = TimeUtil.NOW();
				GameManager.ArenaBattleMgr.Process();
				GameManager.BattleMgr.Process();
				GameManager.DJRoomMgr.ProcessFighting();
				JunQiManager.ProcessAllJunQiItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				JunQiManager.ProcessLingDiZhanResult();
				LuoLanChengZhanManager.getInstance().ProcessWangChengZhanResult();
				KarenBattleManager_MapWest.getInstance().TimerProc();
				KarenBattleManager_MapEast.getInstance().TimerProc();
				GameManager.AngelTempleMgr.HeartBeatAngelTempleScene();
				GameManager.BosshomeMgr.HeartBeatBossHomeScene();
				GameManager.GoldTempleMgr.HeartBeatGoldtempleScene();
				ZhuanShengShiLian.TimerProc();
				ThemeBoss.getInstance().TimerProc();
				long ticksB = TimeUtil.NOW();
				if (ticksB > ticksA + 1000L)
				{
					this.DoLog(string.Format("FightingWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "FightingWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C85 RID: 15493 RVA: 0x00340D60 File Offset: 0x0033EF60
		private void ShengXiaoGuessWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long ticksA = TimeUtil.NOW();
				GameManager.ShengXiaoGuessMgr.Process();
				long ticksB = TimeUtil.NOW();
				if (ticksB > ticksA + 1000L)
				{
					this.DoLog(string.Format("ShengXiaoGuessWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ShengXiaoGuessWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C86 RID: 15494 RVA: 0x00340DDC File Offset: 0x0033EFDC
		private void chatMsgWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long ticksA = TimeUtil.NOW();
				BanChatManager.GetBanChatDictFromDBServer();
				GameManager.ClientMgr.HandleTransferChatMsg();
				long ticksB = TimeUtil.NOW();
				if (ticksB > ticksA + 1000L)
				{
					this.DoLog(string.Format("chatMsgWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "chatMsgWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C87 RID: 15495 RVA: 0x00340E5C File Offset: 0x0033F05C
		private void fuBenWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long ticksA = TimeUtil.NOW();
				GameManager.CopyMapMgr.CheckCopyTeamDamage(ticksA, false);
				GameManager.CopyMapMgr.ProcessEndCopyMap();
				GameManager.CopyMapMgr.ProcessEndGuildCopyMapFlag();
				GameManager.CopyMapMgr.ProcessEndGuildCopyMap(ticksA);
				FreshPlayerCopySceneManager.HeartBeatFreshPlayerCopyMap();
				ExperienceCopySceneManager.HeartBeatExperienceCopyMap();
				GlodCopySceneManager.HeartBeatGlodCopyScene();
				EMoLaiXiCopySceneManager.HeartBeatEMoLaiXiCopyScene();
				GameManager.BloodCastleCopySceneMgr.HeartBeatBloodCastScene();
				HuanYingSiYuanManager.getInstance().TimerProc();
				TianTiManager.getInstance().TimerProc();
				GlobalServiceManager.TimerProc();
				KingOfBattleManager.getInstance().TimerProc();
				YongZheZhanChangManager.getInstance().TimerProc();
				KuaFuBossManager.getInstance().TimerProc();
				SingletonTemplate<MoRiJudgeManager>.Instance().TimerProc();
				BangHuiMatchManager.getInstance().TimerProc();
				KuaFuLueDuoManager.getInstance().TimerProc();
				CompManager.getInstance().TimerProc_fuBenWorker();
				CompBattleManager.getInstance().TimerProc();
				CompMineManager.getInstance().TimerProc();
				ZorkBattleManager.getInstance().TimerProc();
				ElementWarManager.getInstance().TimerProc();
				CopyWolfManager.getInstance().TimerProc();
				SingletonTemplate<CoupleArenaManager>.Instance().UpdateCopyScene();
				GameManager.DaimonSquareCopySceneMgr.HeartBeatDaimonSquareScene();
				BroadcastInfoMgr.ProcessBroadcastInfos();
				PopupWinMgr.ProcessPopupWins();
				LingDiCaiJiManager.getInstance().TimerProc();
				RebornBoss.getInstance().TimerProc_fuBenWorker();
				long ticksB = TimeUtil.NOW();
				if (ticksB > ticksA + 1000L)
				{
					this.DoLog(string.Format("fuBenWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "fuBenWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C88 RID: 15496 RVA: 0x00341004 File Offset: 0x0033F204
		private void dbWriterWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long ticks = TimeUtil.NOW();
				if (ticks - this.LastWriteDBLogTicks >= 30000L)
				{
					this.LastWriteDBLogTicks = ticks;
					Global._TCPManager.MySocketListener.ClearTimeoutSocket();
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "dbWriterWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C89 RID: 15497 RVA: 0x00341074 File Offset: 0x0033F274
		private void SocketSendCacheDataWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long ticksA = TimeUtil.NOW();
				Global._SendBufferManager.TrySendAll();
				long ticksB = TimeUtil.NOW();
				if (ticksB > ticksA + 1000L)
				{
					this.DoLog(string.Format("SocketSendCacheDataWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "SocketFlushBuffer_DoWork", false, false);
			}
		}

		// Token: 0x06003C8A RID: 15498 RVA: 0x003410F0 File Offset: 0x0033F2F0
		private void CmdPacketProcessWorker_DoWork(object sender, EventArgs e)
		{
			Queue<CmdPacket> ls = new Queue<CmdPacket>();
			while (!Program.NeedExitServer)
			{
				try
				{
					Global._TCPManager.ProcessCmdPackets(ls);
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "CmdPacketProcessWorker_DoWork", false, false);
				}
				Thread.Sleep(5);
			}
		}

		// Token: 0x06003C8B RID: 15499 RVA: 0x00341150 File Offset: 0x0033F350
		private void Gird9UpdateWorker_DoWork(object sender, EventArgs e)
		{
			DoWorkEventArgs de = e as DoWorkEventArgs;
			long startTicks = TimeUtil.NOW();
			long endTicks = TimeUtil.NOW();
			int maxSleepMs = 300;
			int nTimes = 0;
			while (!Program.NeedExitServer)
			{
				try
				{
					startTicks = TimeUtil.NOW();
					long ticks = startTicks;
					GameManager.ClientMgr.DoSpritesMapGridMove((int)de.Argument);
					long ticks2 = TimeUtil.NOW();
					if (ticks2 > ticks + 1000L)
					{
						this.DoLog(string.Format("DoSpritesMapGridMove, 序号:{0} 消耗:{1}毫秒", (int)de.Argument, ticks2 - ticks));
					}
					endTicks = TimeUtil.NOW();
					int sleepMs = (int)Math.Max(5L, (long)maxSleepMs - (endTicks - startTicks));
					if (sleepMs > maxSleepMs)
					{
						sleepMs = maxSleepMs;
						LogManager.WriteLog(LogTypes.Alert, string.Format("TimeMismatch#Gird9UpdateWorker_DoWork,startTicks={0},endTicks={1}", startTicks, endTicks), null, true);
					}
					Thread.Sleep(sleepMs);
					nTimes++;
					if (nTimes >= 100000)
					{
						nTimes = 0;
					}
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "Gird9UpdateWorker_DoWork", false, false);
				}
			}
			SysConOut.WriteLine(string.Format("9宫格更新驱动线程{0}退出...", (int)de.Argument));
		}

		// Token: 0x06003C8C RID: 15500 RVA: 0x003412BC File Offset: 0x0033F4BC
		private void RoleExtensionWorker_DoWork(object sender, EventArgs e)
		{
			long startTicks = TimeUtil.NOW();
			long endTicks = TimeUtil.NOW();
			int maxSleepMs = 100;
			int nTimes = 0;
			while (!Program.NeedExitServer)
			{
				try
				{
					startTicks = TimeUtil.NOW();
					long ticks = startTicks;
					GameManager.ClientMgr.DoSpriteExtensionWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, 0, 0);
					long ticks2 = TimeUtil.NOW();
					if (ticks2 > ticks + 1000L)
					{
						this.DoLog(string.Format("RoleExtensionWorker_DoWork, 消耗:{0}毫秒", ticks2 - ticks));
					}
					endTicks = TimeUtil.NOW();
					int sleepMs = (int)Math.Max(5L, (long)maxSleepMs - (endTicks - startTicks));
					Thread.Sleep(sleepMs);
					nTimes++;
					if (nTimes >= 100000)
					{
						nTimes = 0;
					}
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "RoleExtensionWorker_DoWork", false, false);
				}
			}
			SysConOut.WriteLine("角色拓展线程退出");
		}

		// Token: 0x06003C8D RID: 15501 RVA: 0x003413C8 File Offset: 0x0033F5C8
		private void SocketCheckWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long now = TimeUtil.NOW();
				if (now - this.LastSocketCheckTicks >= 300000L)
				{
					this.LastSocketCheckTicks = now;
					int timeCount = 900000;
					List<TMSKSocket> socketList = GameManager.OnlineUserSession.GetSocketList();
					foreach (TMSKSocket socket in socketList)
					{
						long nowSocket = TimeUtil.NOW();
						long spanSocket = nowSocket - socket.session.SocketTime[0];
						if (socket.session.SocketState < 4 && spanSocket > (long)timeCount)
						{
							GameClient otherClient = GameManager.ClientMgr.FindClient(socket);
							if (null == otherClient)
							{
								Global.ForceCloseSocket(socket, "被GM踢了, 但是这个socket上没有对应的client", true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "SocketCheckWorker_DoWork", false, false);
			}
		}

		// Token: 0x06003C8E RID: 15502 RVA: 0x003414E4 File Offset: 0x0033F6E4
		private void Window_Closing()
		{
			if (!this.MustCloseNow)
			{
				if (!this.EnterClosingMode)
				{
					this.EnterClosingMode = true;
					Global._TCPManager.MySocketListener.DontAccept = true;
					this.LastWriteDBLogTicks = 0L;
					Program.NeedExitServer = true;
				}
			}
		}

		// Token: 0x06003C8F RID: 15503 RVA: 0x00341538 File Offset: 0x0033F738
		public static string GetVersionDateTime()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			int revsion = assembly.GetName().Version.Revision;
			int build = assembly.GetName().Version.Build;
			DateTime dtbase = new DateTime(2000, 1, 1, 0, 0, 0);
			TimeSpan tsbase = new TimeSpan(dtbase.Ticks);
			TimeSpan tsv = new TimeSpan(tsbase.Days + build, 0, 0, revsion * 2);
			DateTime dtv = new DateTime(tsv.Ticks);
			string version = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute))).Version;
			return dtv.ToString("yyyy-MM-dd_HH") + string.Format("_{0}", version);
		}

		// Token: 0x06003C90 RID: 15504 RVA: 0x003415F4 File Offset: 0x0033F7F4
		public static string GetVersionStr()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			return ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute))).Version;
		}

		// Token: 0x040046DF RID: 18143
		public static FileVersionInfo AssemblyFileVersion;

		// Token: 0x040046E0 RID: 18144
		private static Program.ControlCtrlDelegate newDelegate = new Program.ControlCtrlDelegate(Program.HandlerRoutine);

		// Token: 0x040046E1 RID: 18145
		public static Program ServerConsole = new Program();

		// Token: 0x040046E2 RID: 18146
		private static Dictionary<string, Program.CmdCallback> CmdDict = new Dictionary<string, Program.CmdCallback>();

		// Token: 0x040046E3 RID: 18147
		public static bool NeedExitServer = false;

		// Token: 0x040046E4 RID: 18148
		private static string DumpBaseDir = "d:\\dumps\\";

		// Token: 0x040046E5 RID: 18149
		private static bool bDumpAndExit_ServerRunOk = false;

		// Token: 0x040046E6 RID: 18150
		public static int[] GCCollectionCounts = new int[3];

		// Token: 0x040046E7 RID: 18151
		public static int[] GCCollectionCounts1 = new int[3];

		// Token: 0x040046E8 RID: 18152
		public static int[] GCCollectionCounts5 = new int[3];

		// Token: 0x040046E9 RID: 18153
		public static int[] GCCollectionCountsNow = new int[3];

		// Token: 0x040046EA RID: 18154
		public static int[] MaxGCCollectionCounts1s = new int[3];

		// Token: 0x040046EB RID: 18155
		public static int[] MaxGCCollectionCounts5s = new int[3];

		// Token: 0x040046EC RID: 18156
		public static long[] MaxGCCollectionCounts1sTicks = new long[3];

		// Token: 0x040046ED RID: 18157
		public static long[] MaxGCCollectionCounts5sTicks = new long[3];

		// Token: 0x040046EE RID: 18158
		public Dictionary<int, string> DBServerConnectDict = new Dictionary<int, string>();

		// Token: 0x040046EF RID: 18159
		public Dictionary<int, string> LogDBServerConnectDict = new Dictionary<int, string>();

		// Token: 0x040046F0 RID: 18160
		private static string ProgramExtName = "";

		// Token: 0x040046F1 RID: 18161
		private static Timer ThreadPoolDriverTimer = null;

		// Token: 0x040046F2 RID: 18162
		private static Timer LogThreadPoolDriverTimer = null;

		// Token: 0x040046F3 RID: 18163
		private BackgroundWorker eventWorker;

		// Token: 0x040046F4 RID: 18164
		private BackgroundWorker dbCommandWorker;

		// Token: 0x040046F5 RID: 18165
		private BackgroundWorker logDBCommandWorker;

		// Token: 0x040046F6 RID: 18166
		private BackgroundWorker clientsWorker;

		// Token: 0x040046F7 RID: 18167
		private BackgroundWorker buffersWorker;

		// Token: 0x040046F8 RID: 18168
		private BackgroundWorker spriteDBWorker;

		// Token: 0x040046F9 RID: 18169
		private BackgroundWorker othersWorker;

		// Token: 0x040046FA RID: 18170
		private BackgroundWorker FightingWorker;

		// Token: 0x040046FB RID: 18171
		private BackgroundWorker chatMsgWorker;

		// Token: 0x040046FC RID: 18172
		private BackgroundWorker fuBenWorker;

		// Token: 0x040046FD RID: 18173
		private BackgroundWorker dbWriterWorker;

		// Token: 0x040046FE RID: 18174
		private BackgroundWorker SocketSendCacheDataWorker;

		// Token: 0x040046FF RID: 18175
		private BackgroundWorker ShengXiaoGuessWorker;

		// Token: 0x04004700 RID: 18176
		private BackgroundWorker MainDispatcherWorker;

		// Token: 0x04004701 RID: 18177
		private BackgroundWorker socketCheckWorker;

		// Token: 0x04004702 RID: 18178
		private BackgroundWorker dynamicMonstersWorker;

		// Token: 0x04004703 RID: 18179
		private BackgroundWorker TwLogWorker;

		// Token: 0x04004704 RID: 18180
		private BackgroundWorker BanWorker;

		// Token: 0x04004705 RID: 18181
		private BackgroundWorker IPStatisticsWorker;

		// Token: 0x04004706 RID: 18182
		private ScheduleExecutor monsterExecutor = null;

		// Token: 0x04004707 RID: 18183
		private int MaxMonsterProcessWorkersNum = 5;

		// Token: 0x04004708 RID: 18184
		private BackgroundWorker[] Gird9UpdateWorkers;

		// Token: 0x04004709 RID: 18185
		public static int MaxGird9UpdateWorkersNum = 5;

		// Token: 0x0400470A RID: 18186
		private BackgroundWorker RoleStroyboardDispatcherWorker;

		// Token: 0x0400470B RID: 18187
		private bool MustCloseNow = false;

		// Token: 0x0400470C RID: 18188
		private bool EnterClosingMode = false;

		// Token: 0x0400470D RID: 18189
		private int ClosingCounter = 6000;

		// Token: 0x0400470E RID: 18190
		private long LastWriteDBLogTicks = TimeUtil.NOW();

		// Token: 0x0400470F RID: 18191
		private long LastAuxiliaryTicks = TimeUtil.NOW();

		// Token: 0x04004710 RID: 18192
		private long LastDynamicMonsterTicks = TimeUtil.NOW();

		// Token: 0x04004711 RID: 18193
		private long LastMonsterUniqueIdProcTicks = TimeUtil.NOW();

		// Token: 0x04004712 RID: 18194
		private long LastSocketCheckTicks = TimeUtil.NOW();

		// Token: 0x02000861 RID: 2145
		// (Invoke) Token: 0x06003C99 RID: 15513
		public delegate bool ControlCtrlDelegate(int CtrlType);

		// Token: 0x02000862 RID: 2146
		// (Invoke) Token: 0x06003C9D RID: 15517
		public delegate void CmdCallback(string cmd);

		// Token: 0x02000863 RID: 2147
		// (Invoke) Token: 0x06003CA1 RID: 15521
		private delegate string PatchDelegate(string[] args);
	}
}
