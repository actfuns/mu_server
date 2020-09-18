using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	
	public class UserMailManager
	{
		
		public static void ScanLastMails(DBManager dbMgr)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			if (nowTicks - UserMailManager.LastScanMailTicks >= 30000L)
			{
				UserMailManager.LastScanMailTicks = nowTicks;
				Dictionary<int, int> lastMailDict = DBQuery.ScanLastMailIDListFromTable(dbMgr);
				if (lastMailDict != null && lastMailDict.Count > 0)
				{
					string gmCmd = "";
					string mailIDsToDel = "";
					foreach (KeyValuePair<int, int> item in lastMailDict)
					{
						int roleID = item.Key;
						DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
						if (null != dbRoleInfo)
						{
							if (gmCmd.Length > 0)
							{
								gmCmd += "_";
							}
							dbRoleInfo.LastMailID = item.Value;
							gmCmd += string.Format("{0}|{1}", dbRoleInfo.RoleID, item.Value);
						}
						else
						{
							DBWriter.UpdateRoleLastMail(dbMgr, item.Key, item.Value);
						}
						if (mailIDsToDel.Length > 0)
						{
							mailIDsToDel += ",";
						}
						mailIDsToDel += item.Value;
					}
					if (gmCmd.Length > 0)
					{
						string gmCmdData = string.Format("-notifymail {0}", gmCmd);
						ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
					}
					if (mailIDsToDel.Length >= 0)
					{
						DBWriter.DeleteLastScanMailIDs(dbMgr, lastMailDict);
					}
				}
			}
		}

		
		public static void ClearOverdueMails(DBManager dbMgr)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			if (nowTicks - UserMailManager.LastClearMailTicks >= 123428447L)
			{
				UserMailManager.LastClearMailTicks = nowTicks;
				DBWriter.ClearOverdueMails(dbMgr, DateTime.Now.AddDays(-15.0));
			}
		}

		
		private const int OverdueDays = 15;

		
		private const long ClearOverdueMailInterval = 123428447L;

		
		private static long LastScanMailTicks = DateTime.Now.Ticks / 10000L;

		
		private static long LastClearMailTicks = DateTime.Now.Ticks / 10000L;
	}
}
