using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Maticsoft.DBUtility
{
	
	public abstract class SqlHelper
	{
		
		public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			int result;
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlHelper.PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
				int val = cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
				result = val;
			}
			return result;
		}

		
		public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		
		public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		
		public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlConnection conn = new SqlConnection(connectionString);
			SqlDataReader result;
			try
			{
				SqlHelper.PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
				SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				cmd.Parameters.Clear();
				result = rdr;
			}
			catch
			{
				conn.Close();
				throw;
			}
			return result;
		}

		
		public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			object result;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlHelper.PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
				object val = cmd.ExecuteScalar();
				cmd.Parameters.Clear();
				result = val;
			}
			return result;
		}

		
		public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
			object val = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			return val;
		}

		
		public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
		{
			SqlHelper.parmCache[cacheKey] = commandParameters;
		}

		
		public static SqlParameter[] GetCachedParameters(string cacheKey)
		{
			SqlParameter[] cachedParms = (SqlParameter[])SqlHelper.parmCache[cacheKey];
			SqlParameter[] result;
			if (cachedParms == null)
			{
				result = null;
			}
			else
			{
				SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];
				int i = 0;
				int j = cachedParms.Length;
				while (i < j)
				{
					clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();
					i++;
				}
				result = clonedParms;
			}
			return result;
		}

		
		private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
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
			cmd.CommandType = cmdType;
			if (cmdParms != null)
			{
				foreach (SqlParameter parm in cmdParms)
				{
					cmd.Parameters.Add(parm);
				}
			}
		}

		
		public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.AppSettings["SQLConnString1"];

		
		public static readonly string ConnectionStringInventoryDistributedTransaction = ConfigurationManager.AppSettings["SQLConnString2"];

		
		public static readonly string ConnectionStringOrderDistributedTransaction = ConfigurationManager.AppSettings["SQLConnString3"];

		
		public static readonly string ConnectionStringProfile = ConfigurationManager.AppSettings["SQLProfileConnString"];

		
		private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
	}
}
