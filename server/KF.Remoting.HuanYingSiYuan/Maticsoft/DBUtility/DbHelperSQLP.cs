using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Maticsoft.DBUtility
{
	// Token: 0x0200004D RID: 77
	public class DbHelperSQLP
	{
		// Token: 0x06000368 RID: 872 RVA: 0x0002E3B7 File Offset: 0x0002C5B7
		public DbHelperSQLP()
		{
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0002E3CD File Offset: 0x0002C5CD
		public DbHelperSQLP(string ConnectionString)
		{
			this.connectionString = ConnectionString;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0002E3EC File Offset: 0x0002C5EC
		public bool ColumnExists(string tableName, string columnName)
		{
			string sql = string.Concat(new string[]
			{
				"select count(1) from syscolumns where [id]=object_id('",
				tableName,
				"') and [name]='",
				columnName,
				"'"
			});
			object res = this.GetSingle(sql);
			return res != null && Convert.ToInt32(res) > 0;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0002E450 File Offset: 0x0002C650
		public int GetMaxID(string FieldName, string TableName)
		{
			string strsql = "select max(" + FieldName + ")+1 from " + TableName;
			object obj = this.GetSingle(strsql);
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

		// Token: 0x0600036C RID: 876 RVA: 0x0002E498 File Offset: 0x0002C698
		public bool Exists(string strSql)
		{
			object obj = this.GetSingle(strSql);
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

		// Token: 0x0600036D RID: 877 RVA: 0x0002E4F8 File Offset: 0x0002C6F8
		public bool TabExists(string TableName)
		{
			string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
			object obj = this.GetSingle(strsql);
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

		// Token: 0x0600036E RID: 878 RVA: 0x0002E56C File Offset: 0x0002C76C
		public bool Exists(string strSql, params SqlParameter[] cmdParms)
		{
			object obj = this.GetSingle(strSql, cmdParms);
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

		// Token: 0x0600036F RID: 879 RVA: 0x0002E5CC File Offset: 0x0002C7CC
		public int ExecuteSql(string SQLString)
		{
			int result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						int rows = cmd.ExecuteNonQuery();
						result = rows;
					}
					catch (SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
			return result;
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0002E660 File Offset: 0x0002C860
		public int ExecuteSqlByTime(string SQLString, int Times)
		{
			int result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						cmd.CommandTimeout = Times;
						int rows = cmd.ExecuteNonQuery();
						result = rows;
					}
					catch (SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
			return result;
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0002E6FC File Offset: 0x0002C8FC
		public int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			int result;
			using (SqlConnection conn = new SqlConnection(this.connectionString))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				SqlTransaction tx = conn.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					foreach (CommandInfo myDE in list)
					{
						string cmdText = myDE.CommandText;
						SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
						DbHelperSQLP.PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
						if (myDE.EffentNextType == EffentNextType.SolicitationEvent)
						{
							if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
							{
								tx.Rollback();
								throw new Exception("违背要求" + myDE.CommandText + "必须符合select count(..的格式");
							}
							object obj = cmd.ExecuteScalar();
							if (obj == null && obj == DBNull.Value)
							{
							}
							bool isHave = Convert.ToInt32(obj) > 0;
							if (isHave)
							{
								myDE.OnSolicitationEvent();
							}
						}
						if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
						{
							if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
							{
								tx.Rollback();
								throw new Exception("SQL:违背要求" + myDE.CommandText + "必须符合select count(..的格式");
							}
							object obj = cmd.ExecuteScalar();
							if (obj == null && obj == DBNull.Value)
							{
							}
							bool isHave = Convert.ToInt32(obj) > 0;
							if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
							{
								tx.Rollback();
								throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须大于0");
							}
							if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
							{
								tx.Rollback();
								throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须等于0");
							}
						}
						else
						{
							int val = cmd.ExecuteNonQuery();
							if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
							{
								tx.Rollback();
								throw new Exception("SQL:违背要求" + myDE.CommandText + "必须有影响行");
							}
							cmd.Parameters.Clear();
						}
					}
					string oraConnectionString = PubConstant.GetConnectionString("ConnectionStringPPC");
					if (!OracleHelper.ExecuteSqlTran(oraConnectionString, oracleCmdSqlList))
					{
						tx.Rollback();
						throw new Exception("Oracle执行失败");
					}
					tx.Commit();
					result = 1;
				}
				catch (SqlException e)
				{
					tx.Rollback();
					throw e;
				}
				catch (Exception e2)
				{
					tx.Rollback();
					throw e2;
				}
			}
			return result;
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0002EA74 File Offset: 0x0002CC74
		public int ExecuteSqlTran(List<string> SQLStringList)
		{
			int result;
			using (SqlConnection conn = new SqlConnection(this.connectionString))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				SqlTransaction tx = conn.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					int count = 0;
					for (int i = 0; i < SQLStringList.Count; i++)
					{
						string strsql = SQLStringList[i];
						if (strsql.Trim().Length > 1)
						{
							cmd.CommandText = strsql;
							count += cmd.ExecuteNonQuery();
						}
					}
					tx.Commit();
					result = count;
				}
				catch
				{
					tx.Rollback();
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0002EB54 File Offset: 0x0002CD54
		public int ExecuteSql(string SQLString, string content)
		{
			int result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				SqlCommand cmd = new SqlCommand(SQLString, connection);
				SqlParameter myParameter = new SqlParameter("@content", SqlDbType.NText);
				myParameter.Value = content;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					int rows = cmd.ExecuteNonQuery();
					result = rows;
				}
				catch (SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
			return result;
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0002EC04 File Offset: 0x0002CE04
		public object ExecuteSqlGet(string SQLString, string content)
		{
			object result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				SqlCommand cmd = new SqlCommand(SQLString, connection);
				SqlParameter myParameter = new SqlParameter("@content", SqlDbType.NText);
				myParameter.Value = content;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					object obj = cmd.ExecuteScalar();
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						result = null;
					}
					else
					{
						result = obj;
					}
				}
				catch (SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
			return result;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0002ECDC File Offset: 0x0002CEDC
		public int ExecuteSqlInsertImg(string strSQL, byte[] fs)
		{
			int result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				SqlCommand cmd = new SqlCommand(strSQL, connection);
				SqlParameter myParameter = new SqlParameter("@fs", SqlDbType.Image);
				myParameter.Value = fs;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					int rows = cmd.ExecuteNonQuery();
					result = rows;
				}
				catch (SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
			return result;
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0002ED88 File Offset: 0x0002CF88
		public object GetSingle(string SQLString)
		{
			object result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						object obj = cmd.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = null;
						}
						else
						{
							result = obj;
						}
					}
					catch (SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
			return result;
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0002EE44 File Offset: 0x0002D044
		public object GetSingle(string SQLString, int Times)
		{
			object result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						cmd.CommandTimeout = Times;
						object obj = cmd.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = null;
						}
						else
						{
							result = obj;
						}
					}
					catch (SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
			return result;
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0002EF08 File Offset: 0x0002D108
		public SqlDataReader ExecuteReader(string strSQL)
		{
			SqlConnection connection = new SqlConnection(this.connectionString);
			SqlCommand cmd = new SqlCommand(strSQL, connection);
			SqlDataReader result;
			try
			{
				connection.Open();
				SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				result = myReader;
			}
			catch (SqlException e)
			{
				throw e;
			}
			return result;
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0002EF58 File Offset: 0x0002D158
		public DataSet Query(string SQLString)
		{
			DataSet result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				DataSet ds = new DataSet();
				try
				{
					connection.Open();
					SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
					command.Fill(ds, "ds");
				}
				catch (SqlException ex)
				{
					throw new Exception(ex.Message);
				}
				result = ds;
			}
			return result;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0002EFE0 File Offset: 0x0002D1E0
		public DataSet Query(string SQLString, int Times)
		{
			DataSet result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				DataSet ds = new DataSet();
				try
				{
					connection.Open();
					new SqlDataAdapter(SQLString, connection)
					{
						SelectCommand = 
						{
							CommandTimeout = Times
						}
					}.Fill(ds, "ds");
				}
				catch (SqlException ex)
				{
					throw new Exception(ex.Message);
				}
				result = ds;
			}
			return result;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x0002F074 File Offset: 0x0002D274
		public int ExecuteSql(string SQLString, params SqlParameter[] cmdParms)
		{
			int result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						DbHelperSQLP.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
						int rows = cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
						result = rows;
					}
					catch (SqlException e)
					{
						throw e;
					}
				}
			}
			return result;
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0002F110 File Offset: 0x0002D310
		public void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (SqlConnection conn = new SqlConnection(this.connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry myDE = (DictionaryEntry)obj;
							string cmdText = myDE.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
							DbHelperSQLP.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0002F23C File Offset: 0x0002D43C
		public int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			int result;
			using (SqlConnection conn = new SqlConnection(this.connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int count = 0;
						foreach (CommandInfo myDE in cmdList)
						{
							string cmdText = myDE.CommandText;
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
							DbHelperSQLP.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
							{
								if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
								{
									trans.Rollback();
									return 0;
								}
								object obj = cmd.ExecuteScalar();
								if (obj == null && obj == DBNull.Value)
								{
								}
								bool isHave = Convert.ToInt32(obj) > 0;
								if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
								{
									trans.Rollback();
									return 0;
								}
								if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
								{
									trans.Rollback();
									return 0;
								}
							}
							else
							{
								int val = cmd.ExecuteNonQuery();
								count += val;
								if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
								{
									trans.Rollback();
									return 0;
								}
								cmd.Parameters.Clear();
							}
						}
						trans.Commit();
						result = count;
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
			return result;
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0002F490 File Offset: 0x0002D690
		public void ExecuteSqlTranWithIndentity(List<CommandInfo> SQLStringList)
		{
			using (SqlConnection conn = new SqlConnection(this.connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int indentity = 0;
						foreach (CommandInfo myDE in SQLStringList)
						{
							string cmdText = myDE.CommandText;
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							DbHelperSQLP.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.Output)
								{
									indentity = Convert.ToInt32(q.Value);
								}
							}
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0002F670 File Offset: 0x0002D870
		public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (SqlConnection conn = new SqlConnection(this.connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int indentity = 0;
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry myDE = (DictionaryEntry)obj;
							string cmdText = myDE.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							DbHelperSQLP.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.Output)
								{
									indentity = Convert.ToInt32(q.Value);
								}
							}
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0002F868 File Offset: 0x0002DA68
		public object GetSingle(string SQLString, params SqlParameter[] cmdParms)
		{
			object result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						DbHelperSQLP.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
						object obj = cmd.ExecuteScalar();
						cmd.Parameters.Clear();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = null;
						}
						else
						{
							result = obj;
						}
					}
					catch (SqlException e)
					{
						throw e;
					}
				}
			}
			return result;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0002F92C File Offset: 0x0002DB2C
		public SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
		{
			SqlConnection connection = new SqlConnection(this.connectionString);
			SqlCommand cmd = new SqlCommand();
			SqlDataReader result;
			try
			{
				DbHelperSQLP.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				cmd.Parameters.Clear();
				result = myReader;
			}
			catch (SqlException e)
			{
				throw e;
			}
			return result;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0002F98C File Offset: 0x0002DB8C
		public DataSet Query(string SQLString, params SqlParameter[] cmdParms)
		{
			DataSet result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				SqlCommand cmd = new SqlCommand();
				DbHelperSQLP.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				using (SqlDataAdapter da = new SqlDataAdapter(cmd))
				{
					DataSet ds = new DataSet();
					try
					{
						da.Fill(ds, "ds");
						cmd.Parameters.Clear();
					}
					catch (SqlException ex)
					{
						throw new Exception(ex.Message);
					}
					result = ds;
				}
			}
			return result;
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0002FA48 File Offset: 0x0002DC48
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

		// Token: 0x06000384 RID: 900 RVA: 0x0002FAFC File Offset: 0x0002DCFC
		public SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
		{
			SqlDataReader result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				connection.Open();
				SqlCommand command = DbHelperSQLP.BuildQueryCommand(connection, storedProcName, parameters);
				command.CommandType = CommandType.StoredProcedure;
				SqlDataReader returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
				result = returnReader;
			}
			return result;
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0002FB60 File Offset: 0x0002DD60
		public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
		{
			DataSet result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				DataSet dataSet = new DataSet();
				connection.Open();
				new SqlDataAdapter
				{
					SelectCommand = DbHelperSQLP.BuildQueryCommand(connection, storedProcName, parameters)
				}.Fill(dataSet, tableName);
				connection.Close();
				result = dataSet;
			}
			return result;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0002FBD8 File Offset: 0x0002DDD8
		public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
		{
			DataSet result;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				DataSet dataSet = new DataSet();
				connection.Open();
                new SqlDataAdapter { SelectCommand = BuildQueryCommand(connection, storedProcName, parameters) }.Fill(dataSet, tableName);
				connection.Close();
				result = dataSet;
			}
			return result;
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0002FC5C File Offset: 0x0002DE5C
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

		// Token: 0x06000388 RID: 904 RVA: 0x0002FCF8 File Offset: 0x0002DEF8
		public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
		{
			int result2;
			using (SqlConnection connection = new SqlConnection(this.connectionString))
			{
				connection.Open();
				SqlCommand command = DbHelperSQLP.BuildIntCommand(connection, storedProcName, parameters);
				rowsAffected = command.ExecuteNonQuery();
				int result = (int)command.Parameters["ReturnValue"].Value;
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0002FD70 File Offset: 0x0002DF70
		private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = DbHelperSQLP.BuildQueryCommand(connection, storedProcName, parameters);
			command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
			return command;
		}

		// Token: 0x040001E0 RID: 480
		public string connectionString = PubConstant.ConnectionString;
	}
}
