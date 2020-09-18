using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameDBServer.DB;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class LiPinMaManager
	{
		
		public static void LoadLiPinMaDB(DBManager dbMgr)
		{
			LiPinMaManager._LiPinMaDict = DBQuery.QueryLiPinMaDict(dbMgr);
		}

		
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

		
		private static object Mutex = new object();

		
		private static Dictionary<string, LiPinMaItem> _LiPinMaDict = null;
	}
}
