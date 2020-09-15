using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.Tools;

namespace GameServer.Tools
{
	// Token: 0x020008FE RID: 2302
	public class TwLogManager
	{
		// Token: 0x0600429C RID: 17052 RVA: 0x003CA1D8 File Offset: 0x003C83D8
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

		// Token: 0x0600429D RID: 17053 RVA: 0x003CA31C File Offset: 0x003C851C
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

		// Token: 0x0600429E RID: 17054 RVA: 0x003CA44C File Offset: 0x003C864C
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

		// Token: 0x0600429F RID: 17055 RVA: 0x003CA538 File Offset: 0x003C8738
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

		// Token: 0x060042A0 RID: 17056 RVA: 0x003CA588 File Offset: 0x003C8788
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

		// Token: 0x060042A1 RID: 17057 RVA: 0x003CA5DC File Offset: 0x003C87DC
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

		// Token: 0x060042A2 RID: 17058 RVA: 0x003CA6A8 File Offset: 0x003C88A8
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

		// Token: 0x060042A3 RID: 17059 RVA: 0x003CA6F8 File Offset: 0x003C88F8
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

		// Token: 0x060042A4 RID: 17060 RVA: 0x003CA7C0 File Offset: 0x003C89C0
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

		// Token: 0x04005047 RID: 20551
		private static string _YearID = string.Empty;

		// Token: 0x04005048 RID: 20552
		private static string _MonthID = string.Empty;

		// Token: 0x04005049 RID: 20553
		private static string _DayID = string.Empty;

		// Token: 0x0400504A RID: 20554
		private static int _HourID = -1;

		// Token: 0x0400504B RID: 20555
		private static string _ProductID = "";

		// Token: 0x0400504C RID: 20556
		private static string _ServerID = "";

		// Token: 0x0400504D RID: 20557
		private static string _RootPath = "";

		// Token: 0x0400504E RID: 20558
		private static string _TwLogPath = string.Empty;

		// Token: 0x0400504F RID: 20559
		private static object _PathLock = new object();

		// Token: 0x04005050 RID: 20560
		private static object _FileLock = new object();

		// Token: 0x04005051 RID: 20561
		private static StreamWriter _StreamWriter = null;

		// Token: 0x04005052 RID: 20562
		public static Dictionary<TwLogType, LogFilePoolItem> LogFilePoolList = new Dictionary<TwLogType, LogFilePoolItem>();

		// Token: 0x04005053 RID: 20563
		private static string[] _userHeadArr = null;

		// Token: 0x04005054 RID: 20564
		private static long LastScanTicks = TimeUtil.NOW();
	}
}
