using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Maticsoft.DBUtility
{
	
	public abstract class DbHelperSQL2
	{
		
		public DbHelperSQL2()
		{
		}

		
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

		
		public static void BuildDBParameter(Database db, DbCommand dbCommand, params SqlParameter[] cmdParms)
		{
			foreach (SqlParameter sp in cmdParms)
			{
				db.AddInParameter(dbCommand, sp.ParameterName, sp.DbType, sp.Value);
			}
		}

		
		public static int ExecuteSql(string strSql)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			return db.ExecuteNonQuery(dbCommand);
		}

		
		public static int ExecuteSqlByTime(string strSql, int Times)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			dbCommand.CommandTimeout = Times;
			return db.ExecuteNonQuery(dbCommand);
		}

		
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

		
		public static int ExecuteSql(string strSql, string content)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			db.AddInParameter(dbCommand, "@content", DbType.String, content);
			return db.ExecuteNonQuery(dbCommand);
		}

		
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

		
		public static int ExecuteSqlInsertImg(string strSql, byte[] fs)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			db.AddInParameter(dbCommand, "@fs", DbType.Byte, fs);
			return db.ExecuteNonQuery(dbCommand);
		}

		
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

		
		public static SqlDataReader ExecuteReader(string strSql)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			return (SqlDataReader)db.ExecuteReader(dbCommand);
		}

		
		public static DataSet Query(string strSql)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			return db.ExecuteDataSet(dbCommand);
		}

		
		public static DataSet Query(string strSql, int Times)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			dbCommand.CommandTimeout = Times;
			return db.ExecuteDataSet(dbCommand);
		}

		
		public static int ExecuteSql(string strSql, params SqlParameter[] cmdParms)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, cmdParms);
			return db.ExecuteNonQuery(dbCommand);
		}

		
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

		
		public static SqlDataReader ExecuteReader(string strSql, params SqlParameter[] cmdParms)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, cmdParms);
			return (SqlDataReader)db.ExecuteReader(dbCommand);
		}

		
		public static DataSet Query(string strSql, params SqlParameter[] cmdParms)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, cmdParms);
			return db.ExecuteDataSet(dbCommand);
		}

		
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

		
		public static int RunProcedure(string storedProcName)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName);
			return db.ExecuteNonQuery(dbCommand);
		}

		
		public static object RunProcedure(string storedProcName, IDataParameter[] InParameters, SqlParameter OutParameter, int rowsAffected)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName);
			DbHelperSQL2.BuildDBParameter(db, dbCommand, (SqlParameter[])InParameters);
			db.AddOutParameter(dbCommand, OutParameter.ParameterName, OutParameter.DbType, OutParameter.Size);
			rowsAffected = db.ExecuteNonQuery(dbCommand);
			return db.GetParameterValue(dbCommand, "@" + OutParameter.ParameterName);
		}

		
		public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName, parameters);
			return (SqlDataReader)db.ExecuteReader(dbCommand);
		}

		
		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName, parameters);
			return db.ExecuteDataSet(dbCommand);
		}

		
		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
		{
			Database db = DatabaseFactory.CreateDatabase();
			DbCommand dbCommand = db.GetStoredProcCommand(storedProcName, parameters);
			dbCommand.CommandTimeout = Times;
			return db.ExecuteDataSet(dbCommand);
		}

		
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

		
		private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = DbHelperSQL2.BuildQueryCommand(connection, storedProcName, parameters);
			command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
			return command;
		}
	}
}
