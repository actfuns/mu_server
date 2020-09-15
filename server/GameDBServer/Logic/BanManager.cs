using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	// Token: 0x020001A7 RID: 423
	public class BanManager
	{
		// Token: 0x060008F7 RID: 2295 RVA: 0x00053AD8 File Offset: 0x00051CD8
		public static void BanRoleName(string roleName, int state)
		{
			lock (BanManager._RoleNameDict)
			{
				BanManager._RoleNameDict[roleName] = state;
			}
			lock (BanManager._RoleNameTicksDict)
			{
				if (state > 0)
				{
					BanManager._RoleNameTicksDict[roleName] = DateTime.Now.Ticks / 10000L;
				}
				else
				{
					BanManager._RoleNameTicksDict[roleName] = 0L;
				}
			}
		}

		// Token: 0x060008F8 RID: 2296 RVA: 0x00053BA0 File Offset: 0x00051DA0
		public static int IsBanRoleName(string roleName)
		{
			int state = 0;
			lock (BanManager._RoleNameDict)
			{
				if (!BanManager._RoleNameDict.TryGetValue(roleName, out state))
				{
					state = 0;
				}
			}
			if (state > 0)
			{
				lock (BanManager._RoleNameTicksDict)
				{
					long oldTicks = 0L;
					if (!BanManager._RoleNameTicksDict.TryGetValue(roleName, out oldTicks))
					{
						state = 0;
					}
					else
					{
						long nowTicks = DateTime.Now.Ticks / 10000L;
						if (nowTicks - oldTicks >= (long)(state * 60 * 1000))
						{
							state = 0;
						}
					}
				}
			}
			return state;
		}

		// Token: 0x060008F9 RID: 2297 RVA: 0x00053CA0 File Offset: 0x00051EA0
		public static int GmBanCheckAdd(DBManager dbMgr, int roleID, string banIDs)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string strTableName = "t_ban_check";
				string cmdText = string.Format("INSERT INTO {0} (roleID, banIDs, logTime) VALUES({1}, '{2}', now())", strTableName, roleID, banIDs);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x00053D08 File Offset: 0x00051F08
		public static int GmBanCheckClear(DBManager dbMgr)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string strTableName = "t_ban_check";
				string cmdText = string.Format("DELETE FROM {0}", strTableName);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x00053D68 File Offset: 0x00051F68
		public static int GmBanLogAdd(DBManager dbMgr, int zoneID, string userID, int roleID, int banType, string banID, int banCount, string deviceID)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string strTableName = "t_ban_log";
				string cmdText = string.Format("INSERT INTO {0} (zoneID, userID, roleID, banType, banID, banCount, logTime, deviceID) VALUES({1}, '{2}', {3}, {4}, '{5}',{6}, now(), '{7}')", new object[]
				{
					strTableName,
					zoneID,
					userID,
					roleID,
					banType,
					banID,
					banCount,
					deviceID
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x040009AA RID: 2474
		private static Dictionary<string, int> _RoleNameDict = new Dictionary<string, int>();

		// Token: 0x040009AB RID: 2475
		private static Dictionary<string, long> _RoleNameTicksDict = new Dictionary<string, long>();
	}
}
