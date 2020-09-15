using System;
using GameDBServer.DB;
using MySQLDriverCS;

namespace GameDBServer.Logic
{
	// Token: 0x02000178 RID: 376
	public class SpreadManager
	{
		// Token: 0x0600069F RID: 1695 RVA: 0x0003C62C File Offset: 0x0003A82C
		public static string GetAward(DBManager dbMgr, int zoneID, int roleID)
		{
			string result = "";
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("SELECT type,state FROM t_spread_award WHERE zoneID = '{0}' AND roleID = '{1}'", zoneID, roleID);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				while (reader.Read())
				{
					if (result != "")
					{
						result += "$";
					}
					result = result + reader["type"].ToString() + "#";
					result += reader["state"].ToString();
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				cmd.Dispose();
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return result;
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x0003C734 File Offset: 0x0003A934
		public static string UpdateAward(DBManager dbMgr, int zoneID, int roleID, int awardType, string award)
		{
			string result = "";
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_spread_award(zoneID,roleID,type,state) VALUES({0}, {1}, {2}, '{3}');", new object[]
				{
					zoneID,
					roleID,
					awardType,
					award
				});
				if (conn.ExecuteNonQuery(cmdText, 0) >= 0)
				{
					result = "1";
				}
			}
			return result;
		}
	}
}
