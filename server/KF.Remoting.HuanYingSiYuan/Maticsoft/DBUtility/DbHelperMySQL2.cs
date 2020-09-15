using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MySQLDriverCS;

namespace Maticsoft.DBUtility
{
	// Token: 0x0200001F RID: 31
	public abstract class DbHelperMySQL2
	{
		// Token: 0x060000EE RID: 238 RVA: 0x0000C3CE File Offset: 0x0000A5CE
		public DbHelperMySQL2()
		{
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000C3DC File Offset: 0x0000A5DC
		public static void LoadConnectStr()
		{
			Dictionary<string, string> mapParams = new Dictionary<string, string>();
			string[] strParams = DbHelperMySQL2.connectionLogString.Split(new char[]
			{
				';'
			});
			foreach (string param in strParams)
			{
				string[] map = param.Split(new char[]
				{
					'='
				});
				if (map.Length == 2)
				{
					map[0] = map[0].Trim();
					map[1] = map[1].Trim();
					mapParams[map[0]] = map[1];
				}
			}
			MySQLConnectionString connStrLog = new MySQLConnectionString(mapParams["host"], mapParams["database"], mapParams["user id"], mapParams["password"]);
			mapParams.Clear();
			strParams = DbHelperMySQL2.connectionString.Split(new char[]
			{
				';'
			});
			foreach (string param in strParams)
			{
				string[] map = param.Split(new char[]
				{
					'='
				});
				if (map.Length == 2)
				{
					map[0] = map[0].Trim();
					map[1] = map[1].Trim();
					mapParams[map[0]] = map[1];
				}
			}
			MySQLConnectionString connStrGame = new MySQLConnectionString(mapParams["host"], mapParams["database"], mapParams["user id"], mapParams["password"]);
			DbHelperMySQL2.realConnStrGame = connStrGame.AsString;
			DbHelperMySQL2.realConnStrLog = connStrLog.AsString;
			DbHelperMySQL2.loadConnectStr = true;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000C58C File Offset: 0x0000A78C
		public static MySQLDataReader ExecuteReader(string strSQL, bool islog = false)
		{
			if (!DbHelperMySQL2.loadConnectStr)
			{
				DbHelperMySQL2.LoadConnectStr();
			}
			MySQLConnection connection = null;
			if (islog)
			{
				connection = new MySQLConnection(DbHelperMySQL2.realConnStrLog);
			}
			else
			{
				connection = new MySQLConnection(DbHelperMySQL2.realConnStrGame);
			}
			MySQLCommand cmd = new MySQLCommand(strSQL, connection);
			MySQLCommand cmdname = new MySQLCommand("SET NAMES 'latin1'", connection);
			MySQLDataReader result;
			try
			{
				connection.Open();
				int res = cmdname.ExecuteNonQuery();
				MySQLDataReader myReader = cmd.ExecuteReaderEx();
				result = myReader;
			}
			catch (MySqlException e)
			{
				throw e;
			}
			finally
			{
				if (cmd != null)
				{
					cmd.Dispose();
				}
				if (cmdname != null)
				{
					cmdname.Dispose();
				}
				if (connection != null)
				{
					connection.Dispose();
					connection.Close();
				}
			}
			return result;
		}

		// Token: 0x040000B6 RID: 182
		public static string connectionString = PubConstant.ConnectionString;

		// Token: 0x040000B7 RID: 183
		public static string connectionLogString = PubConstant.ConnectionLogString;

		// Token: 0x040000B8 RID: 184
		public static bool loadConnectStr = false;

		// Token: 0x040000B9 RID: 185
		public static string realConnStrGame = "";

		// Token: 0x040000BA RID: 186
		public static string realConnStrLog = "";
	}
}
