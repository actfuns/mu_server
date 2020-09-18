using System;
using GameDBServer.DB;
using MySQLDriverCS;

namespace GameDBServer.Logic
{
	
	public class SpreadManager
	{
		
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
