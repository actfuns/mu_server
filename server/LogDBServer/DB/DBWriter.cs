using System;
using LogDBServer.Logic;
using MySQLDriverCS;
using Server.Tools;

namespace LogDBServer.DB
{
	// Token: 0x02000018 RID: 24
	public class DBWriter
	{
		// Token: 0x0600007E RID: 126 RVA: 0x000047B0 File Offset: 0x000029B0
		public static bool RemoveRole(DBManager dbMgr, int roleID)
		{
			bool ret = false;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("UPDATE t_roles SET isdel=1, deltime='{0}' WHERE rid={1}", today, roleID);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
				}
				cmd.Dispose();
				cmd = null;
				ret = true;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return ret;
		}
	}
}
