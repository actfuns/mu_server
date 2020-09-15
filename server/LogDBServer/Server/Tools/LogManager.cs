using System;
using System.Diagnostics;
using System.IO;

namespace Server.Tools
{
	// Token: 0x0200003C RID: 60
	public class LogManager
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00008AA8 File Offset: 0x00006CA8
		// (set) Token: 0x06000166 RID: 358 RVA: 0x00008ABE File Offset: 0x00006CBE
		public static LogTypes LogTypeToWrite { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000167 RID: 359 RVA: 0x00008AC8 File Offset: 0x00006CC8
		// (set) Token: 0x06000168 RID: 360 RVA: 0x00008B64 File Offset: 0x00006D64
		public static string LogPath
		{
			get
			{
				lock (LogManager.mutex)
				{
					if (LogManager._LogPath == string.Empty)
					{
						LogManager._LogPath = AppDomain.CurrentDomain.BaseDirectory + "log/";
						if (!Directory.Exists(LogManager._LogPath))
						{
							Directory.CreateDirectory(LogManager._LogPath);
						}
					}
				}
				return LogManager._LogPath;
			}
			set
			{
				lock (LogManager.mutex)
				{
					LogManager._LogPath = value;
				}
				if (!Directory.Exists(LogManager._LogPath))
				{
					Directory.CreateDirectory(LogManager._LogPath);
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00008BCC File Offset: 0x00006DCC
		// (set) Token: 0x0600016A RID: 362 RVA: 0x00008C68 File Offset: 0x00006E68
		public static string ExceptionPath
		{
			get
			{
				lock (LogManager.mutex)
				{
					if (LogManager._ExceptionPath == string.Empty)
					{
						LogManager._ExceptionPath = AppDomain.CurrentDomain.BaseDirectory + "Exception/";
						if (!Directory.Exists(LogManager._ExceptionPath))
						{
							Directory.CreateDirectory(LogManager._ExceptionPath);
						}
					}
				}
				return LogManager._ExceptionPath;
			}
			set
			{
				lock (LogManager.mutex)
				{
					LogManager._ExceptionPath = value;
				}
				if (!Directory.Exists(LogManager._ExceptionPath))
				{
					Directory.CreateDirectory(LogManager._ExceptionPath);
				}
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00008CD0 File Offset: 0x00006ED0
		private static void WriteLog(string logFile, string logMsg)
		{
			try
			{
				StreamWriter sw = File.AppendText(string.Concat(new string[]
				{
					LogManager.LogPath,
					logFile,
					"_",
					DateTime.Now.ToString("yyyyMMdd"),
					".log"
				}));
				string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + logMsg;
				if (LogManager.EnableDbgView)
				{
					Debug.WriteLine(text);
				}
				sw.WriteLine(text);
				sw.Close();
			}
			catch
			{
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00008D80 File Offset: 0x00006F80
		private static void _WriteException(string exceptionMsg)
		{
			try
			{
				StreamWriter sw = File.CreateText(LogManager.ExceptionPath + "Exception_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
				sw.WriteLine(exceptionMsg);
				sw.Close();
			}
			catch
			{
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00008DE8 File Offset: 0x00006FE8
		public static void WriteLog(LogTypes logType, string logMsg)
		{
			if (logType >= LogManager.LogTypeToWrite)
			{
				lock (LogManager.mutex)
				{
					LogManager.WriteLog(logType.ToString(), logMsg);
				}
			}
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00008E50 File Offset: 0x00007050
		public static void WriteException(string exceptionMsg)
		{
			lock (LogManager.mutex)
			{
				LogManager._WriteException(exceptionMsg);
			}
		}

		// Token: 0x04000435 RID: 1077
		public static bool EnableDbgView = false;

		// Token: 0x04000436 RID: 1078
		private static string _LogPath = string.Empty;

		// Token: 0x04000437 RID: 1079
		private static string _ExceptionPath = string.Empty;

		// Token: 0x04000438 RID: 1080
		private static object mutex = new object();
	}
}
