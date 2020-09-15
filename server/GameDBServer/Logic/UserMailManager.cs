using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	// Token: 0x020001DA RID: 474
	public class UserMailManager
	{
		// Token: 0x060009F1 RID: 2545 RVA: 0x0005F5CC File Offset: 0x0005D7CC
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

		// Token: 0x060009F2 RID: 2546 RVA: 0x0005F798 File Offset: 0x0005D998
		public static void ClearOverdueMails(DBManager dbMgr)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			if (nowTicks - UserMailManager.LastClearMailTicks >= 123428447L)
			{
				UserMailManager.LastClearMailTicks = nowTicks;
				DBWriter.ClearOverdueMails(dbMgr, DateTime.Now.AddDays(-15.0));
			}
		}

		// Token: 0x04000C17 RID: 3095
		private const int OverdueDays = 15;

		// Token: 0x04000C18 RID: 3096
		private const long ClearOverdueMailInterval = 123428447L;

		// Token: 0x04000C19 RID: 3097
		private static long LastScanMailTicks = DateTime.Now.Ticks / 10000L;

		// Token: 0x04000C1A RID: 3098
		private static long LastClearMailTicks = DateTime.Now.Ticks / 10000L;
	}
}
