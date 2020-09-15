using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameDBServer.DB;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001D1 RID: 465
	public class LiPinMaManager
	{
		// Token: 0x060009C5 RID: 2501 RVA: 0x0005DDA3 File Offset: 0x0005BFA3
		public static void LoadLiPinMaDB(DBManager dbMgr)
		{
			LiPinMaManager._LiPinMaDict = DBQuery.QueryLiPinMaDict(dbMgr);
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x0005DDB4 File Offset: 0x0005BFB4
		public static void LoadLiPinMaFromFile(DBManager dbMgr, bool toAppend = false)
		{
			try
			{
				if (File.Exists("./礼品码_导入文件.txt"))
				{
					StreamReader sr = new StreamReader("./礼品码_导入文件.txt", Encoding.GetEncoding("gb2312"));
					if (null != sr)
					{
						if (!toAppend)
						{
							LiPinMaManager._LiPinMaDict = null;
							DBWriter.ClearAllLiPinMa(dbMgr);
						}
						Dictionary<string, LiPinMaItem> liPinMaDict = new Dictionary<string, LiPinMaItem>();
						string line;
						while ((line = sr.ReadLine()) != null)
						{
							if (!string.IsNullOrEmpty(line))
							{
								string[] sa = line.Split(new char[]
								{
									' '
								});
								if (sa.Length == 5)
								{
									DBWriter.InsertNewLiPinMa(dbMgr, sa[0], sa[1], sa[2], sa[3], sa[4], "0");
									LiPinMaItem liPinMaItem = new LiPinMaItem
									{
										LiPinMa = sa[0],
										HuodongID = Convert.ToInt32(sa[1]),
										MaxNum = Convert.ToInt32(sa[2]),
										UsedNum = 0,
										PingTaiID = Convert.ToInt32(sa[3]),
										PingTaiRepeat = Convert.ToInt32(sa[4])
									};
									liPinMaDict[liPinMaItem.LiPinMa] = liPinMaItem;
								}
							}
						}
						sr.Close();
						if (!toAppend || null == LiPinMaManager._LiPinMaDict)
						{
							LiPinMaManager._LiPinMaDict = liPinMaDict;
						}
						else
						{
							Dictionary<string, LiPinMaItem> oldLiPinMaDict = LiPinMaManager._LiPinMaDict;
							foreach (string key in liPinMaDict.Keys)
							{
								LiPinMaItem liPinMaItem = liPinMaDict[key];
								lock (LiPinMaManager.Mutex)
								{
									oldLiPinMaDict[key] = liPinMaItem;
								}
							}
						}
						File.Delete("./礼品码_导入文件.txt");
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x0005E024 File Offset: 0x0005C224
		public static int GetLiPinMaPingTaiID(DBManager dbMgr, int songLiID, string liPinMa)
		{
			int result;
			if (null == LiPinMaManager._LiPinMaDict)
			{
				result = -1010;
			}
			else
			{
				Dictionary<string, LiPinMaItem> liPinMaDict = LiPinMaManager._LiPinMaDict;
				liPinMa = liPinMa.ToUpper();
				lock (LiPinMaManager.Mutex)
				{
					LiPinMaItem liPinMaItem = null;
					if (!liPinMaDict.TryGetValue(liPinMa, out liPinMaItem))
					{
						result = -1020;
					}
					else
					{
						result = liPinMaItem.PingTaiID;
					}
				}
			}
			return result;
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x0005E0B4 File Offset: 0x0005C2B4
		public static int UseLiPinMa(DBManager dbMgr, int roleID, int songLiID, string liPinMa, bool insertLiPinMa = false)
		{
			int result;
			if (null == LiPinMaManager._LiPinMaDict)
			{
				result = -1010;
			}
			else
			{
				Dictionary<string, LiPinMaItem> liPinMaDict = LiPinMaManager._LiPinMaDict;
				int usedNum = 0;
				liPinMa = liPinMa.ToUpper();
				lock (LiPinMaManager.Mutex)
				{
					LiPinMaItem liPinMaItem = null;
					if (!liPinMaDict.TryGetValue(liPinMa, out liPinMaItem))
					{
						return -1020;
					}
					if (liPinMaItem.HuodongID != songLiID)
					{
						return -1030;
					}
					if (liPinMaItem.MaxNum > 0 && liPinMaItem.UsedNum >= liPinMaItem.MaxNum)
					{
						return -1040;
					}
					if (liPinMaItem.PingTaiRepeat <= 0)
					{
						int pingTaiID = DBQuery.QueryPingTaiIDByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
						if (pingTaiID == liPinMaItem.PingTaiID)
						{
							return -10000;
						}
					}
					DBWriter.AddUsedLiPinMa(dbMgr, liPinMaItem.HuodongID, liPinMaItem.LiPinMa, liPinMaItem.PingTaiID, roleID);
					liPinMaItem.UsedNum++;
					usedNum = liPinMaItem.UsedNum;
				}
				DBWriter.UpdateLiPinMaUsedNum(dbMgr, liPinMa, usedNum);
				result = 0;
			}
			return result;
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x0005E214 File Offset: 0x0005C414
		public static int GetLiPinMaPingTaiID2(DBManager dbMgr, int songLiID, string liPinMa, int roleZoneID)
		{
			liPinMa = liPinMa.ToUpper();
			int ptid = -1;
			int ptrepeat = 0;
			int zoneID = 0;
			int maxUseNum = 0;
			int result;
			if (!LiPinMaParse.ParseLiPinMa2(liPinMa, out ptid, out ptrepeat, out zoneID, out maxUseNum))
			{
				result = -1020;
			}
			else if (zoneID > 0 && roleZoneID != zoneID)
			{
				result = -1021;
			}
			else
			{
				result = ptid;
			}
			return result;
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x0005E274 File Offset: 0x0005C474
		public static int GetLiPinMaPingTaiIDNX(DBManager dbMgr, int songLiID, string liPinMa, int roleZoneID)
		{
			liPinMa = liPinMa.ToUpper();
			int ptid = -1;
			int ptrepeat = 0;
			int zoneID = 0;
			int maxUseNum = 0;
			int result;
			if (!LiPinMaParse.ParseLiPinMaNX2(liPinMa, out ptid, out ptrepeat, out zoneID, out maxUseNum))
			{
				result = -1020;
			}
			else if (zoneID > 0 && roleZoneID != zoneID)
			{
				result = -1021;
			}
			else
			{
				result = ptid;
			}
			return result;
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x0005E2D4 File Offset: 0x0005C4D4
		public static int UseLiPinMa2(DBManager dbMgr, int roleID, int songLiID, string liPinMa, int roleZoneID)
		{
			int result;
			if (null == LiPinMaManager._LiPinMaDict)
			{
				result = -1010;
			}
			else
			{
				Dictionary<string, LiPinMaItem> liPinMaDict = LiPinMaManager._LiPinMaDict;
				liPinMa = liPinMa.ToUpper();
				int ptid = -1;
				int ptrepeat = 0;
				int zoneID = 0;
				int maxUseNum = 0;
				if (!LiPinMaParse.ParseLiPinMa2(liPinMa, out ptid, out ptrepeat, out zoneID, out maxUseNum))
				{
					result = -1020;
				}
				else if (zoneID > 0 && roleZoneID != zoneID)
				{
					result = -1021;
				}
				else
				{
					lock (LiPinMaManager.Mutex)
					{
						LiPinMaItem liPinMaItem = null;
						bool bIsNew = false;
						if (!liPinMaDict.TryGetValue(liPinMa, out liPinMaItem))
						{
							liPinMaItem = new LiPinMaItem
							{
								LiPinMa = liPinMa,
								HuodongID = 1,
								MaxNum = maxUseNum,
								UsedNum = 0,
								PingTaiID = ptid,
								PingTaiRepeat = ptrepeat
							};
							liPinMaDict[liPinMa] = liPinMaItem;
							bIsNew = true;
						}
						if (liPinMaItem.HuodongID != songLiID)
						{
							return -1030;
						}
						if (liPinMaItem.MaxNum > 0 && liPinMaItem.UsedNum >= liPinMaItem.MaxNum)
						{
							return -1040;
						}
						if (liPinMaItem.PingTaiRepeat <= 0)
						{
							int pingTaiID = DBQuery.QueryPingTaiIDByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
							if (pingTaiID == liPinMaItem.PingTaiID)
							{
								if (liPinMaItem.MaxNum <= 1 || !bIsNew)
								{
									return -10000;
								}
								int nUseNum = DBQuery.QueryUseNumByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
								if (nUseNum >= liPinMaItem.MaxNum)
								{
									return -1040;
								}
							}
						}
						DBWriter.AddUsedLiPinMa(dbMgr, liPinMaItem.HuodongID, liPinMaItem.LiPinMa, liPinMaItem.PingTaiID, roleID);
						liPinMaItem.UsedNum++;
						int usedNum = liPinMaItem.UsedNum;
					}
					DBWriter.InsertNewLiPinMa(dbMgr, liPinMa, songLiID.ToString(), "1", ptid.ToString(), ptrepeat.ToString(), "1");
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x0005E554 File Offset: 0x0005C754
		public static int UseLiPinMaNX(DBManager dbMgr, int roleID, int songLiID, string liPinMa, int roleZoneID)
		{
			int result;
			if (null == LiPinMaManager._LiPinMaDict)
			{
				result = -1010;
			}
			else
			{
				Dictionary<string, LiPinMaItem> liPinMaDict = LiPinMaManager._LiPinMaDict;
				liPinMa = liPinMa.ToUpper();
				int ptid = -1;
				int ptrepeat = 0;
				int zoneID = 0;
				int maxUseNum = 0;
				if (!LiPinMaParse.ParseLiPinMaNX2(liPinMa, out ptid, out ptrepeat, out zoneID, out maxUseNum))
				{
					result = -1020;
				}
				else if (zoneID > 0 && roleZoneID != zoneID)
				{
					result = -1021;
				}
				else
				{
					lock (LiPinMaManager.Mutex)
					{
						LiPinMaItem liPinMaItem = null;
						bool bIsNew = false;
						if (!liPinMaDict.TryGetValue(liPinMa, out liPinMaItem))
						{
							liPinMaItem = new LiPinMaItem
							{
								LiPinMa = liPinMa,
								HuodongID = 1,
								MaxNum = maxUseNum,
								UsedNum = 0,
								PingTaiID = ptid,
								PingTaiRepeat = ptrepeat
							};
							liPinMaDict[liPinMa] = liPinMaItem;
							bIsNew = true;
						}
						if (liPinMaItem.HuodongID != songLiID)
						{
							return -1030;
						}
						if (liPinMaItem.MaxNum > 0 && liPinMaItem.UsedNum >= liPinMaItem.MaxNum)
						{
							return -1040;
						}
						if (liPinMaItem.PingTaiRepeat <= 0)
						{
							int pingTaiID = DBQuery.QueryPingTaiIDByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
							if (pingTaiID == liPinMaItem.PingTaiID)
							{
								if (liPinMaItem.MaxNum <= 1 || !bIsNew)
								{
									return -10000;
								}
								int nUseNum = DBQuery.QueryUseNumByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
								if (nUseNum >= liPinMaItem.MaxNum)
								{
									return -1040;
								}
							}
						}
						DBWriter.AddUsedLiPinMa(dbMgr, liPinMaItem.HuodongID, liPinMaItem.LiPinMa, liPinMaItem.PingTaiID, roleID);
						liPinMaItem.UsedNum++;
						int usedNum = liPinMaItem.UsedNum;
					}
					DBWriter.InsertNewLiPinMa(dbMgr, liPinMa, songLiID.ToString(), "1", ptid.ToString(), ptrepeat.ToString(), "1");
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x04000BF5 RID: 3061
		private static object Mutex = new object();

		// Token: 0x04000BF6 RID: 3062
		private static Dictionary<string, LiPinMaItem> _LiPinMaDict = null;
	}
}
