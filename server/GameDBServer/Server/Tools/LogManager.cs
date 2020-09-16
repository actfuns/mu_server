using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GameDBServer.Core;

namespace Server.Tools
{
	
	public class LogManager
	{
		
		
		
		public static LogTypes LogTypeToWrite { get; set; }

		
		
		
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

		
		public static void WriteException(string exceptionMsg)
		{
			LogManager.WriteLog(LogTypes.Exception, "##exception##\r\n" + exceptionMsg, null, true);
		}

		
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

		
		private static Dictionary<string, int> ExceptionCacheDict = new Dictionary<string, int>();

		
		private static DateTime LastClearCacheTime;

		
		public static bool EnableDbgView = false;

		
		private static string _LogPath = string.Empty;

		
		private static string _ExceptionPath = string.Empty;

		
		private static object mutex = new object();
	}
}
