using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using Tmsk.DbHelper;

namespace Maticsoft.DBUtility
{
	// Token: 0x0200004B RID: 75
	public abstract class DbHelperMySQL3
	{
		// Token: 0x06000330 RID: 816 RVA: 0x0002D060 File Offset: 0x0002B260
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

		// Token: 0x06000331 RID: 817 RVA: 0x0002D198 File Offset: 0x0002B398
		public static void PushDBConnection(MyDbConnection2 conn)
		{
			if (conn != null)
			{
				DbHelperMySQL3.SemaphoreClientsNoPool.Release();
				conn.Close();
			}
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0002D1CC File Offset: 0x0002B3CC
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

		// Token: 0x06000333 RID: 819 RVA: 0x0002D214 File Offset: 0x0002B414
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

		// Token: 0x06000334 RID: 820 RVA: 0x0002D274 File Offset: 0x0002B474
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

		// Token: 0x06000335 RID: 821 RVA: 0x0002D2D4 File Offset: 0x0002B4D4
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

		// Token: 0x06000336 RID: 822 RVA: 0x0002D324 File Offset: 0x0002B524
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

		// Token: 0x06000337 RID: 823 RVA: 0x0002D374 File Offset: 0x0002B574
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

		// Token: 0x06000338 RID: 824 RVA: 0x0002D3C4 File Offset: 0x0002B5C4
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

		// Token: 0x06000339 RID: 825 RVA: 0x0002D414 File Offset: 0x0002B614
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

		// Token: 0x0600033A RID: 826 RVA: 0x0002D464 File Offset: 0x0002B664
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

		// Token: 0x0600033B RID: 827 RVA: 0x0002D4B8 File Offset: 0x0002B6B8
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

		// Token: 0x0600033C RID: 828 RVA: 0x0002D508 File Offset: 0x0002B708
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

		// Token: 0x0600033D RID: 829 RVA: 0x0002D560 File Offset: 0x0002B760
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

		// Token: 0x0600033E RID: 830 RVA: 0x0002D5B8 File Offset: 0x0002B7B8
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

		// Token: 0x0600033F RID: 831 RVA: 0x0002D5FC File Offset: 0x0002B7FC
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

		// Token: 0x06000340 RID: 832 RVA: 0x0002D64C File Offset: 0x0002B84C
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

		// Token: 0x06000341 RID: 833 RVA: 0x0002D69C File Offset: 0x0002B89C
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

		// Token: 0x06000342 RID: 834 RVA: 0x0002D6EC File Offset: 0x0002B8EC
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

		// Token: 0x06000343 RID: 835 RVA: 0x0002D73C File Offset: 0x0002B93C
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

		// Token: 0x06000344 RID: 836 RVA: 0x0002D78C File Offset: 0x0002B98C
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

		// Token: 0x06000345 RID: 837 RVA: 0x0002D7DC File Offset: 0x0002B9DC
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

		// Token: 0x06000346 RID: 838 RVA: 0x0002D82C File Offset: 0x0002BA2C
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

		// Token: 0x06000347 RID: 839 RVA: 0x0002D884 File Offset: 0x0002BA84
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

		// Token: 0x06000348 RID: 840 RVA: 0x0002D8D4 File Offset: 0x0002BAD4
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

		// Token: 0x06000349 RID: 841 RVA: 0x0002D924 File Offset: 0x0002BB24
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

		// Token: 0x040001D4 RID: 468
		public const int LimitCount = 100;

		// Token: 0x040001D5 RID: 469
		public const int InitCount = 1;

		// Token: 0x040001D6 RID: 470
		public const bool UsePool = false;

		// Token: 0x040001D7 RID: 471
		public static object Mutex = new object();

		// Token: 0x040001D8 RID: 472
		public static string connectionString = PubConstant.ConnectionString;

		// Token: 0x040001D9 RID: 473
		public static int MaxCount = 5;

		// Token: 0x040001DA RID: 474
		public static int ConnCount = 0;

		// Token: 0x040001DB RID: 475
		public static string CodePageNames = "utf8";

		// Token: 0x040001DC RID: 476
		public static int CodePage = 65001;

		// Token: 0x040001DD RID: 477
		private static Dictionary<string, MyDbConnectionPool> DBConnsDict = new Dictionary<string, MyDbConnectionPool>();

		// Token: 0x040001DE RID: 478
		public static Dictionary<string, string> ConnectionStringDict = new Dictionary<string, string>();

		// Token: 0x040001DF RID: 479
		public static Semaphore SemaphoreClientsNoPool = new Semaphore(50, 50);
	}
}
