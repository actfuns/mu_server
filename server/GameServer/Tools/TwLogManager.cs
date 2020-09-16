using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.Tools;

namespace GameServer.Tools
{
	
	public class TwLogManager
	{
		
		public static void WriteLog(TwLogType logType, string logMsg)
		{
			DateTime now = TimeUtil.NowDateTime();
			lock (TwLogManager._FileLock)
			{
				LogFilePoolItem item;
				if (!TwLogManager.LogFilePoolList.TryGetValue(logType, out item))
				{
					item = new LogFilePoolItem();
					TwLogManager.LogFilePoolList.Add(logType, item);
				}
				if ((long)now.Hour != item.OpenTimeOnHours || (long)now.DayOfYear != item.OpenTimeOnDayOfYear || item._StreamWriter == null)
				{
					if (null != item._StreamWriter)
					{
						item._StreamWriter.Close();
						item._StreamWriter = null;
					}
					item._StreamWriter = File.AppendText(TwLogManager.GetLogPath() + TwLogManager.GetFileName(logType));
					item.OpenTimeOnHours = (long)now.Hour;
					item.OpenTimeOnDayOfYear = (long)now.DayOfYear;
					item._StreamWriter.AutoFlush = true;
				}
				try
				{
					TwLogManager._StreamWriter = item._StreamWriter;
					TwLogManager._StreamWriter.WriteLine(logMsg);
				}
				catch
				{
				}
			}
		}

		
		private static string GetLogPath()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (TwLogManager._PathLock)
			{
				string yearID = dateTime.ToString("yyyy");
				string monthID = dateTime.ToString("MM");
				string dayID = dateTime.ToString("dd");
				if (TwLogManager._TwLogPath == string.Empty || yearID != TwLogManager._YearID || monthID != TwLogManager._MonthID || dayID != TwLogManager._DayID)
				{
					TwLogManager._YearID = yearID;
					TwLogManager._MonthID = monthID;
					TwLogManager._DayID = dayID;
					try
					{
						TwLogManager.GetRootPath();
						TwLogManager._TwLogPath = string.Format("{0}\\{1}{2}\\", TwLogManager._RootPath, TwLogManager._YearID, TwLogManager._MonthID);
						if (!Directory.Exists(TwLogManager._TwLogPath))
						{
							Directory.CreateDirectory(TwLogManager._TwLogPath);
						}
					}
					catch (Exception)
					{
					}
				}
			}
			return TwLogManager._TwLogPath;
		}

		
		private static string GetFileName(TwLogType type)
		{
			string name = "";
			switch (type)
			{
			case TwLogType.RoleCreate:
				name = string.Format("active_{0}_{1}_{2}{3}{4}.log", new object[]
				{
					TwLogManager.GetProductID(),
					TwLogManager.GetServerID(),
					TwLogManager._YearID,
					TwLogManager._MonthID,
					TwLogManager._DayID
				});
				break;
			case TwLogType.RoleLogin:
				name = string.Format("login_{0}_{1}_{2}{3}{4}.log", new object[]
				{
					TwLogManager.GetProductID(),
					TwLogManager.GetServerID(),
					TwLogManager._YearID,
					TwLogManager._MonthID,
					TwLogManager._DayID
				});
				break;
			case TwLogType.OnlineNum:
				name = string.Format("online_{0}_{1}_{2}{3}{4}.log", new object[]
				{
					TwLogManager.GetProductID(),
					TwLogManager.GetServerID(),
					TwLogManager._YearID,
					TwLogManager._MonthID,
					TwLogManager._DayID
				});
				break;
			}
			return name;
		}

		
		public static string GetProductID()
		{
			string productID;
			if (TwLogManager._ProductID != "")
			{
				productID = TwLogManager._ProductID;
			}
			else
			{
				TwLogManager._ProductID = GameManager.PlatConfigMgr.GetGameConfigItemStr("tw_log_pid", "0");
				productID = TwLogManager._ProductID;
			}
			return productID;
		}

		
		public static string GetServerID()
		{
			string serverID;
			if (TwLogManager._ServerID != "")
			{
				serverID = TwLogManager._ServerID;
			}
			else
			{
				TwLogManager._ServerID = string.Format("{0}{1:000}", TwLogManager.GetProductID(), GameManager.ServerId);
				serverID = TwLogManager._ServerID;
			}
			return serverID;
		}

		
		public static string GetUserID(string id)
		{
			if (TwLogManager._userHeadArr == null || TwLogManager._userHeadArr.Length <= 0)
			{
				string headStr = GameManager.PlatConfigMgr.GetGameConfigItemStr("tw_log_head", "GATGoogle,GATAn,GAT,TG,YN");
				TwLogManager._userHeadArr = headStr.Split(new char[]
				{
					','
				});
			}
			foreach (string s in TwLogManager._userHeadArr)
			{
				if (id.Length > s.Length && id.Substring(0, s.Length) == s)
				{
					return id.Substring(s.Length);
				}
			}
			return id;
		}

		
		public static string GetRootPath()
		{
			string rootPath;
			if (TwLogManager._RootPath != "")
			{
				rootPath = TwLogManager._RootPath;
			}
			else
			{
				TwLogManager._RootPath = GameManager.PlatConfigMgr.GetGameConfigItemStr("tw_log_path", "d:\\data\\syslog\\platformlog");
				rootPath = TwLogManager._RootPath;
			}
			return rootPath;
		}

		
		public static void ScanLog()
		{
			string proID = TwLogManager.GetProductID();
			if (!(proID == "") && !(proID == "0"))
			{
				long nowTicks = TimeUtil.NOW();
				if (nowTicks - TwLogManager.LastScanTicks >= 600000L)
				{
					TwLogManager.LastScanTicks = nowTicks;
					string time = TimeUtil.ConvertDateTimeInt(TimeUtil.NowDateTime()).ToString();
					string num = TwLogManager.GetOnLineNum().ToString();
					string serverID = TwLogManager.GetServerID();
					string log = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", new object[]
					{
						time,
						num,
						num,
						proID,
						serverID
					});
					TwLogManager.WriteLog(TwLogType.OnlineNum, log);
				}
			}
		}

		
		private static int GetOnLineNum()
		{
			int num = 0;
			string[] dbFields = Global.ExecuteDBCmd(10063, string.Format("{0}", 0), 0);
			if (dbFields != null || dbFields.Length >= 1)
			{
				num = Global.SafeConvertToInt32(dbFields[0]);
			}
			return num;
		}

		
		private static string _YearID = string.Empty;

		
		private static string _MonthID = string.Empty;

		
		private static string _DayID = string.Empty;

		
		private static int _HourID = -1;

		
		private static string _ProductID = "";

		
		private static string _ServerID = "";

		
		private static string _RootPath = "";

		
		private static string _TwLogPath = string.Empty;

		
		private static object _PathLock = new object();

		
		private static object _FileLock = new object();

		
		private static StreamWriter _StreamWriter = null;

		
		public static Dictionary<TwLogType, LogFilePoolItem> LogFilePoolList = new Dictionary<TwLogType, LogFilePoolItem>();

		
		private static string[] _userHeadArr = null;

		
		private static long LastScanTicks = TimeUtil.NOW();
	}
}
