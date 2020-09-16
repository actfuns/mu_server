using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MySQLDriverCS;

namespace Maticsoft.DBUtility
{
	
	public abstract class DbHelperMySQL2
	{
		
		public DbHelperMySQL2()
		{
		}

		
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

		
		public static string connectionString = PubConstant.ConnectionString;

		
		public static string connectionLogString = PubConstant.ConnectionLogString;

		
		public static bool loadConnectStr = false;

		
		public static string realConnStrGame = "";

		
		public static string realConnStrLog = "";
	}
}
