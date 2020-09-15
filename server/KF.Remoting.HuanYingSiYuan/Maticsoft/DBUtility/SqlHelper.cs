using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Maticsoft.DBUtility
{
	// Token: 0x02000052 RID: 82
	public abstract class SqlHelper
	{
		// Token: 0x060003C1 RID: 961 RVA: 0x00031AA4 File Offset: 0x0002FCA4
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

		// Token: 0x060003C2 RID: 962 RVA: 0x00031B08 File Offset: 0x0002FD08
		public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00031B40 File Offset: 0x0002FD40
		public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00031B80 File Offset: 0x0002FD80
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

		// Token: 0x060003C5 RID: 965 RVA: 0x00031BE0 File Offset: 0x0002FDE0
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

		// Token: 0x060003C6 RID: 966 RVA: 0x00031C44 File Offset: 0x0002FE44
		public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
			object val = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			return val;
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x00031C7C File Offset: 0x0002FE7C
		public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
		{
			SqlHelper.parmCache[cacheKey] = commandParameters;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x00031C8C File Offset: 0x0002FE8C
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

		// Token: 0x060003C9 RID: 969 RVA: 0x00031CF4 File Offset: 0x0002FEF4
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

		// Token: 0x040001F8 RID: 504
		public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.AppSettings["SQLConnString1"];

		// Token: 0x040001F9 RID: 505
		public static readonly string ConnectionStringInventoryDistributedTransaction = ConfigurationManager.AppSettings["SQLConnString2"];

		// Token: 0x040001FA RID: 506
		public static readonly string ConnectionStringOrderDistributedTransaction = ConfigurationManager.AppSettings["SQLConnString3"];

		// Token: 0x040001FB RID: 507
		public static readonly string ConnectionStringProfile = ConfigurationManager.AppSettings["SQLProfileConnString"];

		// Token: 0x040001FC RID: 508
		private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
	}
}
