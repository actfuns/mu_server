using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GameDBServer.Core;

namespace Server.Tools
{
	// Token: 0x0200021A RID: 538
	public class LogManager
	{
		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000CA9 RID: 3241 RVA: 0x000A1A9C File Offset: 0x0009FC9C
		// (set) Token: 0x06000CAA RID: 3242 RVA: 0x000A1AB2 File Offset: 0x0009FCB2
		public static LogTypes LogTypeToWrite { get; set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000CAB RID: 3243 RVA: 0x000A1ABC File Offset: 0x0009FCBC
		// (set) Token: 0x06000CAC RID: 3244 RVA: 0x000A1B58 File Offset: 0x0009FD58
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

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000CAD RID: 3245 RVA: 0x000A1BC0 File Offset: 0x0009FDC0
		// (set) Token: 0x06000CAE RID: 3246 RVA: 0x000A1C5C File Offset: 0x0009FE5C
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

		// Token: 0x06000CAF RID: 3247 RVA: 0x000A1CC4 File Offset: 0x0009FEC4
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
				string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\t") + logMsg;
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

		// Token: 0x06000CB0 RID: 3248 RVA: 0x000A1D74 File Offset: 0x0009FF74
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

		// Token: 0x06000CB1 RID: 3249 RVA: 0x000A1DDC File Offset: 0x0009FFDC
		public static void WriteLog(LogTypes logType, string logMsg, Exception ex = null, bool bConsole = true)
		{
			if (logType >= LogManager.LogTypeToWrite)
			{
				lock (LogManager.mutex)
				{
					LogManager.WriteLog(logType.ToString(), logMsg);
				}
				if (logType >= LogTypes.Fatal && bConsole && LogManager.LogTypeToWrite <= LogTypes.Warning)
				{
					ConsoleColor color = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Red;
					SysConOut.WriteLine(logMsg);
					Console.ForegroundColor = color;
				}
				if (null != ex)
				{
					LogManager.WriteException(logMsg + ex.ToString());
				}
			}
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x000A1E9C File Offset: 0x000A009C
		public static void WriteException(string exceptionMsg)
		{
			LogManager.WriteLog(LogTypes.Exception, "##exception##\r\n" + exceptionMsg, null, true);
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x000A1EB4 File Offset: 0x000A00B4
		public static void WriteExceptionUseCache(string exStr)
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				lock (LogManager.ExceptionCacheDict)
				{
					if (now.Hour != LogManager.LastClearCacheTime.Hour || LogManager.ExceptionCacheDict.Count > 10000)
					{
						LogManager.ExceptionCacheDict.Clear();
						LogManager.LastClearCacheTime = now;
					}
					int count = 0;
					if (!LogManager.ExceptionCacheDict.TryGetValue(exStr, out count))
					{
						StackTrace stackTrace = new StackTrace(2, true);
						LogManager.WriteException(exStr + stackTrace.ToString());
					}
					count = (LogManager.ExceptionCacheDict[exStr] = count + 1);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x04001239 RID: 4665
		private static Dictionary<string, int> ExceptionCacheDict = new Dictionary<string, int>();

		// Token: 0x0400123A RID: 4666
		private static DateTime LastClearCacheTime;

		// Token: 0x0400123B RID: 4667
		public static bool EnableDbgView = false;

		// Token: 0x0400123C RID: 4668
		private static string _LogPath = string.Empty;

		// Token: 0x0400123D RID: 4669
		private static string _ExceptionPath = string.Empty;

		// Token: 0x0400123E RID: 4670
		private static object mutex = new object();
	}
}
