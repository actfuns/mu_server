using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Maticsoft.DBUtility
{
	
	public class DbHelperSQLP
	{
		
		public DbHelperSQLP()
		{
		}

		
		public DbHelperSQLP(string ConnectionString)
		{
			this.connectionString = ConnectionString;
		}

		
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

		
		private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = DbHelperSQLP.BuildQueryCommand(connection, storedProcName, parameters);
			command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
			return command;
		}

		
		public string connectionString = PubConstant.ConnectionString;
	}
}
