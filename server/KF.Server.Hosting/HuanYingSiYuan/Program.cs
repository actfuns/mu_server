using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.Threading;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpStaticServer;
using AutoCSer.SubBuffer;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting;
using KF.Remoting.Data;
using KF.Remoting.HuanYingSiYuan.TcpStaticServer;
using KF.Remoting.IPStatistics;
using KF.Remoting.KFBoCai;
using KF.TcpCall;
using Remoting;
using Server.Tools;

namespace KF.Hosting.HuanYingSiYuan
{
	
	internal class Program
	{
		
		[DllImport("User32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		
		[DllImport("user32.dll")]
		private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		
		[DllImport("User32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

		
		[DllImport("User32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int type);

		
		public static void SetWindowMin()
		{
			Console.Title = "KF.Server.Hosting";
			IntPtr ParenthWnd = new IntPtr(0);
			IntPtr et = new IntPtr(0);
			ParenthWnd = Program.FindWindow(null, "KF.Server.Hosting");
			Program.ShowWindow(ParenthWnd, 2);
		}

		
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleCtrlHandler(Program.ControlCtrlDelegate HandlerRoutine, bool Add);

		
		public static bool HandlerRoutine(int CtrlType)
		{
			switch (CtrlType)
			{
			}
			return true;
		}

		
		[DllImport("user32.dll")]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

		
		[DllImport("user32.dll")]
		private static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

		
		private static void HideCloseBtn()
		{
			IntPtr windowHandle = Program.FindWindow(null, Console.Title);
			IntPtr closeMenu = Program.GetSystemMenu(windowHandle, IntPtr.Zero);
			uint SC_CLOSE = 61536U;
			Program.RemoveMenu(closeMenu, SC_CLOSE, 0U);
		}

		
		public static string GetVersionDateTime()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(KuaFuServerManager));
			int revsion = assembly.GetName().Version.Revision;
			int build = assembly.GetName().Version.Build;
			DateTime dtbase = new DateTime(2000, 1, 1, 0, 0, 0);
			TimeSpan tsbase = new TimeSpan(dtbase.Ticks);
			TimeSpan tsv = new TimeSpan(tsbase.Days + build, 0, 0, revsion * 2);
			DateTime dtv = new DateTime(tsv.Ticks);
			string version = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute))).Version;
			return dtv.ToString("yyyy-MM-dd_HH") + string.Format("_{0}", version);
		}

		
		private static void Main(string[] args)
		{
			try
			{
				string exeFile = Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName;
				string version = Program.GetVersionDateTime();
				Console.Title = string.Format("跨服中心服务器@{0}@{1}", version, exeFile);
				File.WriteAllText("Pid.txt", Process.GetCurrentProcess().Id.ToString());
				FileStream fs = File.Open("Pid.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "本程序已经启动了一个进程,按任意键退出!", ex, true);
				return;
			}
			if (args.Contains("-gmsettime"))
			{
				KuaFuServerManager.EnableGMSetAllServerTime = true;
			}
			if (args.Contains("-testmode"))
			{
				Global.TestMode = true;
				Consts.TestMode = true;
			}
			if (args.Contains("-waitdebug"))
			{
				Console.WriteLine("等待调试器连接,按任意键继续!");
				do
				{
					Thread.Sleep(100);
				}
				while (!Console.KeyAvailable);
				Console.ReadKey();
			}
			Program.HideCloseBtn();
			Program.SetConsoleCtrlHandler(Program.newDelegate, true);
			if (Console.WindowWidth < 88)
			{
				Console.BufferWidth = 88;
				Console.WindowWidth = 88;
			}
			Console.WriteLine("跨服中心服务器启动!");
			LogManager.WriteLog(LogTypes.Info, "跨服中心服务器启动!", null, true);
			if (!KuaFuServerManager.CheckConfig())
			{
				Console.WriteLine("服务器无法启动!");
				Thread.Sleep(300000);
			}
			else
			{
				RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
				KuaFuServerManager.StartServerConfigThread();
				if (!KuaFuServerManager.LoadConfig())
				{
					Console.WriteLine("服务器无法启动!");
					Thread.Sleep(300000);
				}
				else
				{
					KuaFuServerManager.InitServer();
					Program.StartServices();
					Program.InitCmdDict();
					KuaFuServerManager.OnStartServer();
					new Thread(new ParameterizedThreadStart(Program.ConsoleInputThread))
					{
						IsBackground = true
					}.Start();
					while (!KuaFuServerManager.WaitStop(0))
					{
						Thread.Sleep(1000);
					}
				}
			}
		}

		
		public static void ConsoleInputThread(object obj)
		{
			for (;;)
			{
				try
				{
					Program.ShowCmdHelp();
					string cmd = Console.ReadLine();
					if (null != cmd)
					{
						Program.CmdDict.ExcuteCmd(cmd);
					}
					if (Program.NeedExitServer)
					{
						KuaFuServerManager.OnStopServer();
						break;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
		}

		
		private static void InitCmdDict()
		{
			Program.CmdDict.AddCmdHandler("exit", new ParameterizedThreadStart(Program.ExitCmdHandler));
			Program.CmdDict.AddCmdHandler("reload", new ParameterizedThreadStart(Program.ReloadCmdHandler));
			Program.CmdDict.AddCmdHandler("clear", new ParameterizedThreadStart(Program.ClearCmdHandler));
			Program.CmdDict.AddCmdHandler("load", new ParameterizedThreadStart(Program.ReloadCmdHandler));
			Program.CmdDict.AddCmdHandler("tcpinfo", new ParameterizedThreadStart(CmdMonitor.ShowServerTCPInfo));
			Program.CmdDict.AddCmdHandler("opt", delegate(object x)
			{
				Program.OptCmdProc(x);
			});
			Program.CmdDict.AddCmdHelp("组队天梯,排行刷新", new string[]
			{
				"tianti5v5",
				"paihang"
			});
			Program.CmdDict.AddCmdHelp("组队天梯,排行刷新,重建月度排行", new string[]
			{
				"tianti5v5",
				"paihang",
				"month"
			});
			Program.CmdDict.AddCmdHandler("tianti5v5", delegate(object x)
			{
				string[] cmdFields = x as string[];
				if (cmdFields.Length >= 2 && cmdFields[1] == "paihang")
				{
					TianTi5v5Service.UpdateZhanDuiRankData(TimeUtil.NowDateTime(), cmdFields.Length >= 3 && cmdFields[2] == "month");
					Console.Write("组队天梯,排行刷新");
				}
				else
				{
					Console.Write("组队天梯,未知命令!");
				}
			});
			Program.CmdDict.AddCmdHelp("组队天梯,排行重建,会先重建组队竞技月排行", new string[]
			{
				"zhanduizhengba",
				"load"
			});
			Program.CmdDict.AddCmdHelp("战队争霸,排行刷新", new string[]
			{
				"zhanduizhengba",
				"reload"
			});
			Program.CmdDict.AddCmdHandler("zhanduizhengba", delegate(object x)
			{
				string[] cmdFields = x as string[];
				if (cmdFields.Length >= 2 && cmdFields[1] == "load")
				{
					TianTi5v5Service.PaiHangCopy(TimeUtil.NowDateTime());
					ZhanDuiZhengBa_K.LoadSyncData(TimeUtil.NowDateTime(), false);
					Console.Write("战队争霸,排行重建");
				}
				else if (cmdFields.Length >= 2 && cmdFields[1] == "reload")
				{
					ZhanDuiZhengBa_K.LoadSyncData(TimeUtil.NowDateTime(), cmdFields[1] == "reload");
					Console.Write("战队争霸,排行刷新");
				}
				else
				{
					Console.Write("战队争霸,未知命令!");
				}
			});
			Program.CmdDict.AddCmdHandler("serverlist", delegate(object x)
			{
				KuaFuServerManager.UpdateServerListAge();
			});
			Program.CmdDict.AddCmdHandler("flushcity", delegate(object x)
			{
				YongZheZhanChangPersistence.Instance.LangHunLingYuBroadcastServerIdHashSet.Clear();
			});
			Program.CmdDict.AddCmdHandler("testmode", delegate(object x)
			{
				Global.TestMode = !Global.TestMode;
				if (Global.TestMode)
				{
					Consts.TianTiRoleCountTotal = 1;
				}
				else
				{
					Consts.TianTiRoleCountTotal = 2;
				}
				Console.WriteLine("测试模式状态:{0}", Global.TestMode);
			});
			Program.CmdDict.AddCmdHandler("-maxteamcopy", delegate(object x)
			{
				try
				{
					string[] cmdFields = x as string[];
					int copyId = Convert.ToInt32(cmdFields[1]);
					int num = Convert.ToInt32(cmdFields[2]);
					ConstData.MaxCopyTeamMemberNumDict[copyId] = num;
					string msg = string.Format("设置组队副本{0}人数上线为{1}", copyId, num);
					Console.WriteLine(msg);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			});
			Program.CmdDict.AddCmdHandler("-help", delegate(object x)
			{
				try
				{
					string[] args = x as string[];
					if (args != null && args.Length > 0)
					{
						args = args.Skip(1).ToArray<string>();
					}
					Program.CmdDict.ShowHelp(args);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			});
		}

		
		private static void OptCmdProc(object obj)
		{
			string[] args = obj as string[];
			if (args != null && args.Length >= 2)
			{
				if (args[1] == "0")
				{
					KuaFuServerManager.OptimizationServerList = !KuaFuServerManager.OptimizationServerList;
				}
				else if (args[1] == "1" && args.Length == 3)
				{
					int count;
					if (int.TryParse(args[2], out count))
					{
						KuaFuServerManager.MaxGetAsyncItemDataCount = count;
					}
				}
			}
		}

		
		private static void ShowCmdHelp()
		{
			Console.WriteLine("\n命令列表:");
			Console.WriteLine("exit : 退出");
			Console.WriteLine("load paihang {mongth} : 天梯排行加载相关指令");
			Console.WriteLine("tcpinfo : 显示指令处理统计信息");
			Console.WriteLine("serverlist : 刷新全服服务器列表");
			Console.WriteLine("flushcity : 广播当前城池信息");
			Console.WriteLine("testmode : 测试模式");
			Console.WriteLine("clear : 清空控制台输出");
		}

		
		public static void ExitCmdHandler(object obj)
		{
			Console.WriteLine("确定要退出?请输入'y'");
			if (Console.ReadLine() == "y")
			{
				Program.NeedExitServer = true;
				Console.WriteLine("退出程序!");
			}
		}

		
		public static void ReloadCmdHandler(object obj)
		{
			try
			{
				string[] args = obj as string[];
				if (args.Length == 1 && args[0] == "reload")
				{
					KuaFuServerManager.LoadConfig();
					IPStatisticsPersistence.Instance.LoadConfig();
					KFBoCaiConfigManager.LoadConfig(true);
				}
				else
				{
					TianTiService.Instance.ExecCommand(args);
				}
			}
			catch
			{
			}
			Console.WriteLine("重新加载配置成功!");
		}

		
		public static void ReloadPaiHangCmdHandler(object obj)
		{
			try
			{
				TianTiService.Instance.ExecCommand(new string[]
				{
					"reload",
					"paihang"
				});
			}
			catch
			{
			}
			Console.WriteLine("重新加载配置成功!");
		}

		
		public static void ClearCmdHandler(object obj)
		{
			Console.Clear();
		}

		
		public static void StartServices()
		{
			AsyncDataItem.InitKnownTypes();
			ConfigData.InitConfig();
			RemotingConfiguration.Configure(Process.GetCurrentProcess().MainModule.FileName + ".config", false);
			AutoCSer.Net.TcpInternalServer.ServerAttribute attribute = AutoCSer.Net.TcpStaticServer.ServerAttribute.GetConfig("K", typeof(ZhanDuiZhengBa_K), true);
			attribute.IsAutoServer = true;
			attribute.Port = ConfigData.ServicePort;
			attribute.Host = ConfigData.ServiceHost;
			attribute.SendBufferSize = Size.Kilobyte64;
			attribute.ReceiveBufferSize = Size.Kilobyte64;
			attribute.ServerSendBufferMaxSize = 33554432;
			attribute.ClientSendBufferMaxSize = 4194304;
			Program.AutoCserServices.Add(new KfCall(attribute, null, null, null));
		}

		
		private static Program.ControlCtrlDelegate newDelegate = new Program.ControlCtrlDelegate(Program.HandlerRoutine);

		
		private static bool NeedExitServer = false;

		
		private static bool TestMode = false;

		
		private static CmdHandlerDict CmdDict = new CmdHandlerDict();

		
		private static List<AutoCSer.Net.TcpInternalServer.Server> AutoCserServices = new List<AutoCSer.Net.TcpInternalServer.Server>();

		
		private static List<ServiceHost> hostList = new List<ServiceHost>();

		
		
		public delegate bool ControlCtrlDelegate(int CtrlType);
	}
}
