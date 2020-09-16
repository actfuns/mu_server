using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using LogDBServer.DB;
using LogDBServer.Logic;
using LogDBServer.Server;
using MySQLDriverCS;
using Server.Tools;

namespace LogDBServer
{
    
    public class Program
    {
        
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
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

        
        [DllImport("user32.dll")]
        private static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        
        private static void HideCloseBtn()
        {
            Console.Title = "游戏数据库服务器";
            IntPtr windowHandle = Program.FindWindow(null, Console.Title);
            IntPtr closeMenu = Program.GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 61536U;
            Program.RemoveMenu(closeMenu, SC_CLOSE, 0U);
        }

        
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception exception = e.ExceptionObject as Exception;
                DataHelper.WriteFormatExceptionLog(exception, "CurrentDomain_UnhandledException", UnhandedException.ShowErrMsgBox, true);
            }
            catch
            {
            }
        }

        
        private static void ExceptionHook()
        {
            AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
        }

        
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

        
        public static void WritePIDToFile(string strFile)
        {
            string strFileName = Directory.GetCurrentDirectory() + "\\" + strFile;
            Process processes = Process.GetCurrentProcess();
            int nPID = processes.Id;
            File.WriteAllText(strFileName, string.Concat(nPID));
        }

        
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

        
        private static void Main(string[] args)
        {
            Program.DeleteFile("Start.txt");
            Program.DeleteFile("Stop.txt");
            Program.DeleteFile("GameServerStop.txt");
            Program.HideCloseBtn();
            Program.SetConsoleCtrlHandler(Program.newDelegate, true);
            Program.ExceptionHook();
            Program.InitCommonCmd();
            Program.OnStartServer();
            Program.ShowCmdHelpInfo(null);
            Program.WritePIDToFile("Start.txt");
            Thread thread = new Thread(new ParameterizedThreadStart(Program.ConsoleInputThread));
            thread.IsBackground = true;
            thread.Start();
            while (!Program.NeedExitServer || !Program.ServerConsole.MustCloseNow || Program.ServerConsole.MainDispatcherWorker.IsBusy)
            {
                Thread.Sleep(1000);
            }
            thread.Abort();
        }

        
        public static void ConsoleInputThread(object obj)
        {
            string cmd = Console.ReadLine();
            while (!Program.NeedExitServer)
            {
                if (cmd == null || 0 == cmd.CompareTo("exit"))
                {
                    if (Program.ServerConsole.CanExit())
                    {
                        Console.WriteLine("确认退出吗(输入 y 将立即退出)？");
                        cmd = Console.ReadLine();
                        if (0 == cmd.CompareTo("y"))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Program.ParseInputCmd(cmd);
                }
                cmd = Console.ReadLine();
            }
            Program.OnExitServer();
        }

        
        private static void ParseInputCmd(string cmd)
        {
            Program.CmdCallback cb = null;
            if (Program.CmdDict.TryGetValue(cmd, out cb) && null != cb)
            {
                cb(cmd);
            }
            else
            {
                Console.WriteLine("未知命令,输入 help 查看具体命令信息");
            }
        }

        
        private static void OnStartServer()
        {
            Program.ServerConsole.InitServer();
            Console.Title = string.Format("日志数据库服务器{0}区@{1}@{2}", GameDBManager.ZoneID, Program.GetVersionDateTime(), Program.ProgramExtName);
        }

        
        private static void OnExitServer()
        {
            Program.ServerConsole.ExitServer();
        }

        
        public static void Exit()
        {
            Program.NeedExitServer = true;
        }

        
        private static void InitCommonCmd()
        {
            Program.CmdDict.Add("help", new Program.CmdCallback(Program.ShowCmdHelpInfo));
            Program.CmdDict.Add("gc", new Program.CmdCallback(Program.GarbageCollect));
            Program.CmdDict.Add("show baseinfo", new Program.CmdCallback(Program.ShowServerBaseInfo));
        }

        
        private static void ShowCmdHelpInfo(string cmd = null)
        {
            Console.WriteLine(string.Format("日志数据库服务器", new object[0]));
            Console.WriteLine("输入 help， 显示帮助信息");
            Console.WriteLine("输入 exit， 然后输入y退出？");
            Console.WriteLine("输入 gc， 执行垃圾回收");
            Console.WriteLine("输入 show baseinfo， 查看基础运行信息");
        }

        
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

        
        private static void ShowServerBaseInfo(string cmd = null)
        {
            string info = string.Format("已经连接的登录服务器连接数:{0}", Program.ServerConsole.TotalConnections);
            Console.WriteLine(info);
            info = string.Format("数据库连接池个数: {0}", Program.ServerConsole._DBManger.GetMaxConnsCount());
            Console.WriteLine(info);
        }

        
        
        
        public int TotalConnections
        {
            get
            {
                return this._TotalConnections;
            }
            set
            {
                this._TotalConnections = value;
            }
        }

        
        private void closingTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string title = "";
                this.ClosingCounter -= 200;
                if (this.ClosingCounter <= 0)
                {
                    this.MustCloseNow = true;
                }
                else
                {
                    int counter = this.ClosingCounter / 200;
                    title = string.Format("日志数据库服务器{0} 关闭中, 倒计时:{1}", GameDBManager.ZoneID, counter);
                }
                Console.Title = title;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "closingTimer_Tick", false, false);
            }
        }

        
        private void ByDayCreateLogTableWorker_DoWork(object sender, EventArgs e)
        {
            int sleepMs = 3600000;
            for (; ; )
            {
                try
                {
                    DBItemLogWriter.getInstance().AddItemLogTable(DBManager.getInstance());
                    if (Program.NeedExitServer)
                    {
                        sleepMs = 200;
                        this.closingTimer_Tick(null, null);
                        if (this.MustCloseNow)
                        {
                            break;
                        }
                    }
                    Thread.Sleep(sleepMs);
                }
                catch (Exception ex)
                {
                    DataHelper.WriteFormatExceptionLog(ex, "ByDayCreateLogTableWorker_DoWork", false, false);
                }
            }
            Console.WriteLine("日志表处理线程退出，回车退出系统");
        }

        
        private void MainDispatcherWorker_DoWork(object sender, EventArgs e)
        {
            long startTicks = DateTime.Now.Ticks / 10000L;
            long endTicks = DateTime.Now.Ticks / 10000L;
            int maxSleepMs = 1000;
            for (; ; )
            {
                try
                {
                    startTicks = DateTime.Now.Ticks / 10000L;
                    this.ExecuteBackgroundWorkers(null, EventArgs.Empty);
                    if (Program.NeedExitServer)
                    {
                        maxSleepMs = 200;
                        this.closingTimer_Tick(null, null);
                        if (this.MustCloseNow)
                        {
                            break;
                        }
                    }
                    endTicks = DateTime.Now.Ticks / 10000L;
                    int sleepMs = (int)Math.Max(1L, (long)maxSleepMs - (endTicks - startTicks));
                    Thread.Sleep(sleepMs);
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
            Console.WriteLine("主循环线程退出，回车退出系统");
            if (0 != Program.GetServerPIDFromFile())
            {
                Program.WritePIDToFile("Stop.txt");
            }
        }

        
        private void ShowInfoTicks(object sender, EventArgs e)
        {
            try
            {
                this.ExecuteBackgroundWorkers(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "ShowInfoTicks", false, false);
            }
        }

        
        private static void InitProgramExtName()
        {
            Program.ProgramExtName = AppDomain.CurrentDomain.BaseDirectory;
        }

        
        public void InitServer()
        {
            Program.InitProgramExtName();
            Console.WriteLine("正在初始化语言文件");
            Global.LoadLangDict();
            XElement xml = null;
            Console.WriteLine("正在初始化系统配置文件");
            try
            {
                xml = XElement.Load("AppConfig.xml");
            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", "AppConfig.xml"));
            }
            LogManager.LogTypeToWrite = (LogTypes)Global.GetSafeAttributeLong(xml, "Server", "LogType");
            GameDBManager.SystemServerSQLEvents.EventLevel = (EventLevels)Global.GetSafeAttributeLong(xml, "Server", "EventLevel");
            int dbLog = Math.Max(0, (int)Global.GetSafeAttributeLong(xml, "DBLog", "DBLogEnable"));
            GameDBManager.ZoneID = (int)Global.GetSafeAttributeLong(xml, "Zone", "ID");
            string uname = StringEncrypt.Decrypt(Global.GetSafeAttributeStr(xml, "Database", "uname"), "eabcix675u49,/", "3&3i4x4^+-0");
            string upasswd = StringEncrypt.Decrypt(Global.GetSafeAttributeStr(xml, "Database", "upasswd"), "eabcix675u49,/", "3&3i4x4^+-0");
            Console.WriteLine("服务器正在建立数据链接池个数: {0}", (int)Global.GetSafeAttributeLong(xml, "Database", "maxConns"));
            Console.WriteLine("数据库地址: {0}", Global.GetSafeAttributeStr(xml, "Database", "ip"));
            Console.WriteLine("数据库名称: {0}", Global.GetSafeAttributeStr(xml, "Database", "dname"));
            Console.WriteLine("数据库字符集: {0}", Global.GetSafeAttributeStr(xml, "Database", "names"));
            DBConnections.dbNames = Global.GetSafeAttributeStr(xml, "Database", "names");
            Console.WriteLine("正在初始化数据库链接");
            this._DBManger.LoadDatabase(new MySQLConnectionString(Global.GetSafeAttributeStr(xml, "Database", "ip"), Global.GetSafeAttributeStr(xml, "Database", "dname"), uname, upasswd), (int)Global.GetSafeAttributeLong(xml, "Database", "maxConns"), (int)Global.GetSafeAttributeLong(xml, "Database", "codePage"));
            string strTableName = "t_log_" + DateTime.Now.ToString("yyyyMMdd");
            MySQLConnection conn = this._DBManger.DBConns.PopDBConnection();
            DBItemLogWriter.getInstance().ConformTableColumns(conn, strTableName);
            this._DBManger.DBConns.PushDBConnection(conn);
            Console.WriteLine("正在初始化网络");
            this._TCPManager = TCPManager.getInstance();
            this._TCPManager.initialize((int)Global.GetSafeAttributeLong(xml, "Socket", "capacity"));
            this._TCPManager.DBMgr = this._DBManger;
            this._TCPManager.RootWindow = this;
            this._TCPManager.Start(Global.GetSafeAttributeStr(xml, "Socket", "ip"), (int)Global.GetSafeAttributeLong(xml, "Socket", "port"));
            Console.WriteLine("正在配置后台线程");
            this.eventWorker = new BackgroundWorker();
            this.eventWorker.DoWork += this.eventWorker_DoWork;
            this.ByDayCreateLogTableWorker = new BackgroundWorker();
            this.ByDayCreateLogTableWorker.DoWork += new DoWorkEventHandler(this.ByDayCreateLogTableWorker_DoWork);
            this.MainDispatcherWorker = new BackgroundWorker();
            this.MainDispatcherWorker.DoWork += new DoWorkEventHandler(this.MainDispatcherWorker_DoWork);
            UnhandedException.ShowErrMsgBox = false;
            GlobalServiceManager.initialize();
            GlobalServiceManager.startup();
            if (!this.MainDispatcherWorker.IsBusy)
            {
                this.MainDispatcherWorker.RunWorkerAsync();
            }
            if (!this.ByDayCreateLogTableWorker.IsBusy)
            {
                this.ByDayCreateLogTableWorker.RunWorkerAsync();
            }
            Console.WriteLine("系统启动完毕");
        }

        
        public static void ValidateZoneID()
        {
            Console.WriteLine("即将配置数据库表自增长值,请输入区号进行验证:");
            string readLine = Console.ReadLine();
            for (; ; )
            {
                try
                {
                    int inputZone = int.Parse(readLine);
                    if (inputZone == GameDBManager.ZoneID)
                    {
                        Console.WriteLine("区号验证成功!!");
                        break;
                    }
                    Console.WriteLine("输入区号非法!!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("请重新输入区号进行验证:");
                readLine = Console.ReadLine();
            }
        }

        
        public void ExitServer()
        {
            if (!Program.NeedExitServer)
            {
                this._TCPManager.Stop();
                GlobalServiceManager.showdown();
                GlobalServiceManager.destroy();
                this.Window_Closing();
                Console.WriteLine("正在尝试关闭服务器,看到服务器关闭完毕提示后回车退出系统");
                if (0 == Program.GetServerPIDFromFile())
                {
                    string cmd = Console.ReadLine();
                    while (this.MainDispatcherWorker.IsBusy)
                    {
                        Console.WriteLine("正在尝试关闭服务器");
                        cmd = Console.ReadLine();
                    }
                }
            }
        }

        
        private void ExecuteBackgroundWorkers(object sender, EventArgs e)
        {
            if (!this.eventWorker.IsBusy)
            {
                this.eventWorker.RunWorkerAsync();
            }
        }

        
        private void eventWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (GameDBManager.SystemServerSQLEvents.WriteEvent())
                {
                }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "eventWorker_DoWork", false, false);
            }
        }

        
        public bool CanExit()
        {
            bool result;
            if (this._TCPManager.MySocketListener.ConnectedSocketsCount > 0)
            {
                Console.WriteLine(string.Format("有游戏服务器或者分线服务器连接到{0}，无法断开!", Console.Title));
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }

        
        private void Window_Closing()
        {
            if (!this.MustCloseNow)
            {
                if (!this.EnterClosingMode)
                {
                    this.EnterClosingMode = true;
                    this.LastWriteDBLogTicks = 0L;
                    Program.NeedExitServer = true;
                }
            }
        }

        
        public static string GetVersionDateTime()
        {
            int revsion = Assembly.GetExecutingAssembly().GetName().Version.Revision;
            int build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            DateTime dtbase = new DateTime(2000, 1, 1, 0, 0, 0);
            TimeSpan tsbase = new TimeSpan(dtbase.Ticks);
            TimeSpan tsv = new TimeSpan(tsbase.Days + build, 0, 0, revsion * 2);
            DateTime dtv = new DateTime(tsv.Ticks);
            return dtv.ToString("yyyy-MM-dd HH");
        }

        
        private static Program.ControlCtrlDelegate newDelegate = new Program.ControlCtrlDelegate(Program.HandlerRoutine);

        
        public static Program ServerConsole = new Program();

        
        private static Dictionary<string, Program.CmdCallback> CmdDict = new Dictionary<string, Program.CmdCallback>();

        
        private static bool NeedExitServer = false;

        
        private int _TotalConnections = 0;

        
        private DBManager _DBManger = DBManager.getInstance();

        
        private TCPManager _TCPManager = null;

        
        private bool MustCloseNow = false;

        
        private bool EnterClosingMode = false;

        
        private int ClosingCounter = 6000;

        
        private long LastWriteDBLogTicks = DateTime.Now.Ticks / 10000L;

        
        private static string ProgramExtName = "";

        
        private BackgroundWorker eventWorker;

        
        private BackgroundWorker MainDispatcherWorker;

        
        private BackgroundWorker ByDayCreateLogTableWorker;

        
        
        public delegate bool ControlCtrlDelegate(int CtrlType);

        
        
        public delegate void CmdCallback(string cmd);
    }
}
