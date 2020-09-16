using System;
using LogDBServer.Logic;
using MySQLDriverCS;
using Server.Tools;

namespace LogDBServer.DB
{
	
	public class DBWriter
	{
		
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
