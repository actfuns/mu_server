using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Maticsoft.DBUtility
{
	// Token: 0x0200004C RID: 76
	public abstract class DbHelperSQL2
	{
		// Token: 0x0600034C RID: 844 RVA: 0x0002DA43 File Offset: 0x0002BC43
		public DbHelperSQL2()
		{
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0002DA50 File Offset: 0x0002BC50
		public static int GetMaxID(string FieldName, string TableName)
		{
			string strSql = "select max(" + FieldName + ")+1 from " + TableName;
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			object obj = db.ExecuteScalar(dbCommand);
			int result;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				result = 1;
			}
			else
			{
				result = int.Parse(obj.ToString());
			}
			return result;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0002DAC0 File Offset: 0x0002BCC0
		public static bool Exists(string strSql)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			object obj = db.ExecuteScalar(dbCommand);
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

		// Token: 0x0600034F RID: 847 RVA: 0x0002DB34 File Offset: 0x0002BD34
		public static bool Exists(string strSql, params SqlParameter[] cmdParms)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, cmdParms);
			object obj = db.ExecuteScalar(dbCommand);
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

		// Token: 0x06000350 RID: 848 RVA: 0x0002DBB4 File Offset: 0x0002BDB4
		public static void BuildDBParameter(Database db, DbCommand dbCommand, params SqlParameter[] cmdParms)
		{
			foreach (SqlParameter sp in cmdParms)
			{
				db.AddInParameter(dbCommand, sp.ParameterName, sp.DbType, sp.Value);
			}
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0002DBF8 File Offset: 0x0002BDF8
		public static int ExecuteSql(string strSql)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			return db.ExecuteNonQuery(dbCommand);
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0002DC20 File Offset: 0x0002BE20
		public static int ExecuteSqlByTime(string strSql, int Times)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			dbCommand.CommandTimeout = Times;
			return db.ExecuteNonQuery(dbCommand);
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0002DC50 File Offset: 0x0002BE50
		public static void ExecuteSqlTran(ArrayList SQLStringList)
		{
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection dbconn = db.CreateConnection())
			{
				dbconn.Open();
				DbTransaction dbtran = dbconn.BeginTransaction();
				try
				{
					for (int i = 0; i < SQLStringList.Count; i++)
					{
						string strsql = SQLStringList[i].ToString();
						if (strsql.Trim().Length > 1)
						{
							DbCommand dbCommand = db.GetSqlStringCommand(strsql);
							db.ExecuteNonQuery(dbCommand);
						}
					}
					dbtran.Commit();
				}
				catch
				{
					dbtran.Rollback();
				}
				finally
				{
					dbconn.Close();
				}
			}
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0002DD34 File Offset: 0x0002BF34
		public static int ExecuteSql(string strSql, string content)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			db.AddInParameter(dbCommand, "@content", DbType.String, content);
			return db.ExecuteNonQuery(dbCommand);
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0002DD6C File Offset: 0x0002BF6C
		public static object ExecuteSqlGet(string strSql, string content)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			db.AddInParameter(dbCommand, "@content", DbType.String, content);
			object obj = db.ExecuteNonQuery(dbCommand);
			object result;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				result = null;
			}
			else
			{
				result = obj;
			}
			return result;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0002DDD4 File Offset: 0x0002BFD4
		public static int ExecuteSqlInsertImg(string strSql, byte[] fs)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			db.AddInParameter(dbCommand, "@fs", DbType.Byte, fs);
			return db.ExecuteNonQuery(dbCommand);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0002DE0C File Offset: 0x0002C00C
		public static object GetSingle(string strSql)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			object obj = db.ExecuteScalar(dbCommand);
			object result;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				result = null;
			}
			else
			{
				result = obj;
			}
			return result;
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0002DE5C File Offset: 0x0002C05C
		public static SqlDataReader ExecuteReader(string strSql)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			return (SqlDataReader)db.ExecuteReader(dbCommand);
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0002DE8C File Offset: 0x0002C08C
		public static DataSet Query(string strSql)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			return db.ExecuteDataSet(dbCommand);
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0002DEB4 File Offset: 0x0002C0B4
		public static DataSet Query(string strSql, int Times)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			dbCommand.CommandTimeout = Times;
			return db.ExecuteDataSet(dbCommand);
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0002DEE4 File Offset: 0x0002C0E4
		public static int ExecuteSql(string strSql, params SqlParameter[] cmdParms)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, cmdParms);
			return db.ExecuteNonQuery(dbCommand);
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0002DF14 File Offset: 0x0002C114
		public static void ExecuteSqlTran(Hashtable SQLStringList)
		{
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection dbconn = db.CreateConnection())
			{
				dbconn.Open();
				DbTransaction dbtran = dbconn.BeginTransaction();
				try
				{
					foreach (object obj in SQLStringList)
					{
						DictionaryEntry myDE = (DictionaryEntry)obj;
						string strsql = myDE.Key.ToString();
						SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
						if (strsql.Trim().Length > 1)
						{
							DbCommand dbCommand = db.GetSqlStringCommand(strsql);
							DbHelperSQL2.BuildDBParameter(db, dbCommand, cmdParms);
							db.ExecuteNonQuery(dbCommand);
						}
					}
					dbtran.Commit();
				}
				catch
				{
					dbtran.Rollback();
				}
				finally
				{
					dbconn.Close();
				}
			}
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0002E048 File Offset: 0x0002C248
		public static object GetSingle(string strSql, params SqlParameter[] cmdParms)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, cmdParms);
			object obj = db.ExecuteScalar(dbCommand);
			object result;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				result = null;
			}
			else
			{
				result = obj;
			}
			return result;
		}

		// Token: 0x0600035E RID: 862 RVA: 0x0002E0A4 File Offset: 0x0002C2A4
		public static SqlDataReader ExecuteReader(string strSql, params SqlParameter[] cmdParms)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, cmdParms);
			return (SqlDataReader)db.ExecuteReader(dbCommand);
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0002E0DC File Offset: 0x0002C2DC
		public static DataSet Query(string strSql, params SqlParameter[] cmdParms)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, cmdParms);
			return db.ExecuteDataSet(dbCommand);
		}

		// Token: 0x06000360 RID: 864 RVA: 0x0002E10C File Offset: 0x0002C30C
		private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
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
				foreach (SqlParameter parameter in cmdParms)
				{
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
					{
						parameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(parameter);
				}
			}
		}

		// Token: 0x06000361 RID: 865 RVA: 0x0002E1C0 File Offset: 0x0002C3C0
		public static int RunProcedure(string storedProcName)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName);
			return db.ExecuteNonQuery(dbCommand);
		}

		// Token: 0x06000362 RID: 866 RVA: 0x0002E1E8 File Offset: 0x0002C3E8
		public static object RunProcedure(string storedProcName, IDataParameter[] InParameters, SqlParameter OutParameter, int rowsAffected)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, (SqlParameter[])InParameters);
			db.AddOutParameter(dbCommand, OutParameter.ParameterName, OutParameter.DbType, OutParameter.Size);
			rowsAffected = db.ExecuteNonQuery(dbCommand);
			return db.GetParameterValue(dbCommand, "@" + OutParameter.ParameterName);
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0002E250 File Offset: 0x0002C450
		public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName, parameters);
			return (SqlDataReader)db.ExecuteReader(dbCommand);
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0002E280 File Offset: 0x0002C480
		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName, parameters);
			return db.ExecuteDataSet(dbCommand);
		}

		// Token: 0x06000365 RID: 869 RVA: 0x0002E2A8 File Offset: 0x0002C4A8
		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName, parameters);
			dbCommand.CommandTimeout = Times;
			return db.ExecuteDataSet(dbCommand);
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0002E2D8 File Offset: 0x0002C4D8
		private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = new SqlCommand(storedProcName, connection);
			command.CommandType = CommandType.StoredProcedure;
			foreach (SqlParameter parameter in parameters)
			{
				if (parameter != null)
				{
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
					{
						parameter.Value = DBNull.Value;
					}
					command.Parameters.Add(parameter);
				}
			}
			return command;
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0002E374 File Offset: 0x0002C574
		private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = DbHelperSQL2.BuildQueryCommand(connection, storedProcName, parameters);
			command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
			return command;
		}
	}
}
