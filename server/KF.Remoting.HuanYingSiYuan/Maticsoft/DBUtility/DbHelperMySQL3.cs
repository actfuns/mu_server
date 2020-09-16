using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using Tmsk.DbHelper;

namespace Maticsoft.DBUtility
{
	
	public abstract class DbHelperMySQL3
	{
		
		public static MyDbConnection2 PopDBConnection(string dbKey)
		{
			MyDbConnection2 conn = null;
			DbHelperMySQL3.SemaphoreClientsNoPool.WaitOne();
			try
			{
				string connectionString;
				lock (DbHelperMySQL3.Mutex)
				{
					if (!DbHelperMySQL3.ConnectionStringDict.TryGetValue(dbKey, out connectionString))
					{
						connectionString = PubConstant.ConnectionString;
						string dbName = PubConstant.GetDatabaseName(dbKey);
						int idx0 = connectionString.IndexOf("database=") + "database=".Length;
						int idx = connectionString.IndexOf(';', idx0);
						string datebaseName = connectionString.Substring(idx0, idx - idx0);
						connectionString = connectionString.Replace(datebaseName, dbName);
						DbHelperMySQL3.ConnectionStringDict[dbKey] = connectionString;
					}
				}
				conn = new MyDbConnection2(connectionString, DbHelperMySQL3.CodePageNames);
				if (!conn.Open())
				{
					conn = null;
				}
			}
			catch (Exception ex)
			{
				conn = null;
			}
			finally
			{
				if (null == conn)
				{
					DbHelperMySQL3.SemaphoreClientsNoPool.Release();
				}
			}
			return conn;
		}

		
		public static void PushDBConnection(MyDbConnection2 conn)
		{
			if (conn != null)
			{
				DbHelperMySQL3.SemaphoreClientsNoPool.Release();
				conn.Close();
			}
		}

		
		public static int GetMaxID(string dbKey, string FieldName, string TableName)
		{
			string strsql = "select max(" + FieldName + ")+1 from " + TableName;
			object obj = DbHelperMySQL3.GetSingle(dbKey, strsql);
			int result;
			if (obj == null)
			{
				result = 1;
			}
			else
			{
				result = int.Parse(obj.ToString());
			}
			return result;
		}

		
		public static bool Exists(string dbKey, string strSql)
		{
			object obj = DbHelperMySQL3.GetSingle(dbKey, strSql);
			int cmdresult;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				cmdresult = 0;
			}
			else
			{
				cmdresult = int.Parse(obj.ToString());
			}
			return cmdresult != 0;
		}

		
		public static bool Exists(string dbKey, string strSql, params MySqlParameter[] cmdParms)
		{
			object obj = DbHelperMySQL3.GetSingle(dbKey, strSql, cmdParms);
			int cmdresult;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				cmdresult = 0;
			}
			else
			{
				cmdresult = int.Parse(obj.ToString());
			}
			return cmdresult != 0;
		}

		
		public static int ExecuteSql(string dbKey, string SQLString)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					return conn.ExecuteNonQuery(SQLString, 0);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return -1;
		}

		
		public static int ExecuteSqlByTime(string dbKey, string SQLString, int Times)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					return conn.ExecuteNonQuery(SQLString, Times);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return -1;
		}

		
		public static int ExecuteSqlTran(string dbKey, List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					return conn.ExecuteSqlTran(list, oracleCmdSqlList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return 0;
		}

		
		public static int ExecuteSqlTran(string dbKey, List<string> SQLStringList)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					return conn.ExecuteSqlTran(SQLStringList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return 0;
		}

		
		public static int ExecuteSql(string dbKey, string SQLString, string content)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					return conn.ExecuteWithContent(SQLString, content);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return 0;
		}

		
		public static object ExecuteSqlGet(string dbKey, string SQLString, string content)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == conn)
				{
					return conn.ExecuteSqlGet(SQLString, content);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return null;
		}

		
		public static int ExecuteSqlInsertImg(string dbKey, string strSQL, List<Tuple<string, byte[]>> imgList)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					return conn.ExecuteSqlInsertImg(strSQL, imgList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return -1;
		}

		
		public static object GetSingle(string dbKey, string SQLString)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					return conn.GetSingle(SQLString, 0, new MySqlParameter[0]);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return null;
		}

		
		public static object GetSingle(string dbKey, string SQLString, int Times)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					return conn.GetSingle(SQLString, Times, new MySqlParameter[0]);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return null;
		}

		
		public static MyDataReader ExecuteReader(string dbKey, string strSQL)
		{
			MyDbConnection2 conn = DbHelperMySQL3.PopDBConnection(dbKey);
			MyDataReader result;
			if (null != conn)
			{
				MySqlDataReader mySqlDataReader = conn.ExecuteReader(strSQL, new MySqlParameter[0]);
				MyDataReader myDataReader = new MyDataReader(conn, mySqlDataReader);
				result = myDataReader;
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public static DataSet Query(string dbKey, string SQLString)
		{
			MyDbConnection2 conn = null;
			DataSet result;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == conn)
				{
					result = null;
				}
				else
				{
					result = conn.Query(SQLString, 0);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return result;
		}

		
		public static DataSet Query(string dbKey, string SQLString, int Times)
		{
			MyDbConnection2 conn = null;
			DataSet result;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == conn)
				{
					result = null;
				}
				else
				{
					result = conn.Query(SQLString, Times);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return result;
		}

		
		public static int ExecuteSql(string dbKey, string SQLString, params MySqlParameter[] cmdParms)
		{
			MyDbConnection2 conn = null;
			int result;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == conn)
				{
					result = -1;
				}
				else
				{
					result = conn.ExecuteSql(SQLString, cmdParms);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return result;
		}

		
		public static void ExecuteSqlTran(string dbKey, Hashtable SQLStringList)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					conn.ExecuteSqlTran(SQLStringList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
		}

		
		public static int ExecuteSqlTran(string dbKey, List<CommandInfo> cmdList)
		{
			MyDbConnection2 conn = null;
			int result;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == conn)
				{
					result = -1;
				}
				else
				{
					result = conn.ExecuteSqlTran(cmdList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return result;
		}

		
		public static void ExecuteSqlTranWithIndentity(string dbKey, List<CommandInfo> SQLStringList)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					conn.ExecuteSqlTranWithIndentity(SQLStringList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
		}

		
		public static void ExecuteSqlTranWithIndentity(string dbKey, Hashtable SQLStringList)
		{
			MyDbConnection2 conn = null;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != conn)
				{
					conn.ExecuteSqlTranWithIndentity(SQLStringList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
		}

		
		public static object GetSingle(string dbKey, string SQLString, params MySqlParameter[] cmdParms)
		{
			MyDbConnection2 conn = null;
			object result;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == conn)
				{
					result = -1;
				}
				else
				{
					result = conn.GetSingle(SQLString, 0, cmdParms);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return result;
		}

		
		public static MySqlDataReader ExecuteReader(string dbKey, string SQLString, params MySqlParameter[] cmdParms)
		{
			MyDbConnection2 conn = null;
			MySqlDataReader result;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == conn)
				{
					result = null;
				}
				else
				{
					result = conn.ExecuteReader(SQLString, cmdParms);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return result;
		}

		
		public static DataSet Query(string dbKey, string SQLString, params MySqlParameter[] cmdParms)
		{
			MyDbConnection2 conn = null;
			DataSet result;
			try
			{
				conn = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == conn)
				{
					result = null;
				}
				else
				{
					result = conn.Query(SQLString, cmdParms);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(conn);
			}
			return result;
		}

		
		private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
		{
			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			if (trans != null)
			{
				cmd.Transaction = trans;
			}
			cmd.CommandType = CommandType.Text;
			if (cmdParms != null)
			{
				foreach (MySqlParameter parameter in cmdParms)
				{
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
					{
						parameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(parameter);
				}
			}
		}

		
		public const int LimitCount = 100;

		
		public const int InitCount = 1;

		
		public const bool UsePool = false;

		
		public static object Mutex = new object();

		
		public static string connectionString = PubConstant.ConnectionString;

		
		public static int MaxCount = 5;

		
		public static int ConnCount = 0;

		
		public static string CodePageNames = "utf8";

		
		public static int CodePage = 65001;

		
		private static Dictionary<string, MyDbConnectionPool> DBConnsDict = new Dictionary<string, MyDbConnectionPool>();

		
		public static Dictionary<string, string> ConnectionStringDict = new Dictionary<string, string>();

		
		public static Semaphore SemaphoreClientsNoPool = new Semaphore(50, 50);
	}
}
