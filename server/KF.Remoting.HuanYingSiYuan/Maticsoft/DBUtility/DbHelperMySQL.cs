using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace Maticsoft.DBUtility
{
	
	public abstract class DbHelperMySQL
	{
		
		public DbHelperMySQL()
		{
		}

		
		public static int GetMaxID(string FieldName, string TableName)
		{
			string strsql = "select max(" + FieldName + ")+1 from " + TableName;
			object obj = DbHelperMySQL.GetSingle(strsql);
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

		
		public static bool Exists(string strSql)
		{
			object obj = DbHelperMySQL.GetSingle(strSql);
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

		
		public static bool Exists(string strSql, params MySqlParameter[] cmdParms)
		{
			object obj = DbHelperMySQL.GetSingle(strSql, cmdParms);
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

		
		public static int ExecuteSql(string SQLString)
		{
			int result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						int rows = cmd.ExecuteNonQuery();
						result = rows;
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
						if (connection != null)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static int ExecuteSqlByTime(string SQLString, int Times)
		{
			int result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						cmd.CommandTimeout = Times;
						int rows = cmd.ExecuteNonQuery();
						result = rows;
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
						if (connection != null)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			int result;
			using (MySqlConnection conn = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = conn;
				MySqlTransaction tx = conn.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					foreach (CommandInfo myDE in list)
					{
						string cmdText = myDE.CommandText;
						MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
						DbHelperMySQL.PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
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
						throw new Exception("执行失败");
					}
					tx.Commit();
					result = 1;
				}
				catch (MySqlException e)
				{
					tx.Rollback();
					throw e;
				}
				finally
				{
					if (cmd != null)
					{
						cmd.Dispose();
					}
					if (conn != null)
					{
						conn.Close();
					}
				}
			}
			return result;
		}

		
		public static int ExecuteSqlTran(List<string> SQLStringList)
		{
			int result;
			using (MySqlConnection conn = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = conn;
				MySqlTransaction tx = conn.BeginTransaction();
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
				finally
				{
					if (cmd != null)
					{
						cmd.Dispose();
					}
					if (conn != null)
					{
						conn.Close();
					}
				}
			}
			return result;
		}

		
		public static int ExecuteSql(string SQLString, string content)
		{
			int result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				MySqlCommand cmd = new MySqlCommand(SQLString, connection);
				MySqlParameter myParameter = new MySqlParameter("@content", content);
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					int rows = cmd.ExecuteNonQuery();
					result = rows;
				}
				catch (MySqlException e)
				{
					LogManager.WriteException(e.ToString());
					throw;
				}
				finally
				{
					if (cmd != null)
					{
						cmd.Dispose();
					}
					if (connection != null)
					{
						connection.Close();
					}
				}
			}
			return result;
		}

		
		public static object ExecuteSqlGet(string SQLString, string content)
		{
			object result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				MySqlCommand cmd = new MySqlCommand(SQLString, connection);
				MySqlParameter myParameter = new MySqlParameter("@content", SqlDbType.NText);
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
					if (connection != null)
					{
						connection.Close();
					}
				}
			}
			return result;
		}

		
		public static int ExecuteSqlInsertImg(string strSQL, List<Tuple<string, byte[]>> imgList)
		{
			int result = 0;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand cmd = new MySqlCommand(strSQL, connection))
				{
					try
					{
						connection.Open();
						foreach (Tuple<string, byte[]> t in imgList)
						{
							string imgTag = t.Item1;
							byte[] imgData = t.Item2;
							MySqlParameter myParameter = new MySqlParameter(imgTag, imgData);
							cmd.Parameters.Add(myParameter);
						}
						result = cmd.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						throw ex;
					}
					finally
					{
						if (cmd != null)
						{
							cmd.Dispose();
						}
						if (connection != null)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static long ExecuteSqlGetIncrement(string SQLString, List<Tuple<string, byte[]>> imgList = null)
		{
			long result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						if (null != imgList)
						{
							foreach (Tuple<string, byte[]> t in imgList)
							{
								string imgTag = t.Item1;
								byte[] imgData = t.Item2;
								MySqlParameter myParameter = new MySqlParameter(imgTag, imgData);
								cmd.Parameters.Add(myParameter);
							}
						}
						cmd.ExecuteNonQuery();
						cmd.CommandText = "SELECT LAST_INSERT_ID();";
						object obj = cmd.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = 0L;
						}
						else
						{
							long increment;
							long.TryParse(obj.ToString(), out increment);
							result = increment;
						}
					}
					catch (MySqlException e)
					{
						result = 0L;
					}
					finally
					{
						if (cmd != null)
						{
							cmd.Dispose();
						}
						if (connection != null)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static object GetSingle(string SQLString)
		{
			object result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
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
						if (connection != null)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static long GetSingleLong(string sql)
		{
			long longValue = 0L;
			object obj = DbHelperMySQL.GetSingle(sql);
			long result;
			if (obj != null && long.TryParse(obj.ToString(), out longValue))
			{
				result = longValue;
			}
			else
			{
				result = -1L;
			}
			return result;
		}

		
		public static int GetSingleValues(string SQLString, out object[] values)
		{
			int result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
				{
					try
					{
						int ret = 0;
						connection.Open();
						MySqlDataReader myReader = cmd.ExecuteReader();
						values = new object[myReader.FieldCount];
						if (myReader.Read())
						{
							ret = myReader.GetValues(values);
						}
						myReader.Close();
						result = ret;
					}
					catch (MySqlException e)
					{
						throw;
					}
					finally
					{
						if (cmd != null)
						{
							cmd.Dispose();
						}
						if (connection != null)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static object GetSingle(string SQLString, int Times)
		{
			object result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
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
						if (connection != null)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static MySqlDataReader ExecuteReader(string strSQL, bool islog = false)
		{
			MySqlConnection connection;
			if (islog)
			{
				connection = new MySqlConnection(DbHelperMySQL.connectionLogString);
			}
			else
			{
				connection = new MySqlConnection(DbHelperMySQL.connectionString);
			}
			MySqlCommand cmd = new MySqlCommand(strSQL, connection);
			MySqlDataReader result;
			try
			{
				connection.Open();
				MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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
					cmd.Clone();
				}
			}
			return result;
		}

		
		public static DataSet Query(string SQLString)
		{
			DataSet result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				DataSet ds = new DataSet();
				MySqlDataAdapter command = null;
				try
				{
					connection.Open();
					command = new MySqlDataAdapter(SQLString, connection);
					command.Fill(ds, "ds");
				}
				catch (MySqlException ex)
				{
					throw new Exception(ex.Message);
				}
				finally
				{
					if (command != null)
					{
						command.Dispose();
					}
					if (connection != null)
					{
						connection.Close();
					}
				}
				result = ds;
			}
			return result;
		}

		
		public static DataSet Query(string SQLString, int Times)
		{
			DataSet result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				DataSet ds = new DataSet();
				MySqlDataAdapter command = null;
				try
				{
					connection.Open();
					command = new MySqlDataAdapter(SQLString, connection);
					command.SelectCommand.CommandTimeout = Times;
					command.Fill(ds, "ds");
				}
				catch (MySqlException ex)
				{
					throw new Exception(ex.Message);
				}
				finally
				{
					if (command != null)
					{
						command.Dispose();
					}
					if (connection != null)
					{
						connection.Close();
					}
				}
				result = ds;
			}
			return result;
		}

		
		public static int ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
		{
			int result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand cmd = new MySqlCommand())
				{
					try
					{
						DbHelperMySQL.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
						int rows = cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
						result = rows;
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
						if (connection != null)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (MySqlConnection conn = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				conn.Open();
				using (MySqlTransaction trans = conn.BeginTransaction())
				{
					MySqlCommand cmd = new MySqlCommand();
					try
					{
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry myDE = (DictionaryEntry)obj;
							string cmdText = myDE.Key.ToString();
							MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
							DbHelperMySQL.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
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
					finally
					{
						if (cmd != null)
						{
							cmd.Dispose();
						}
						if (conn != null)
						{
							conn.Close();
						}
					}
				}
			}
		}

		
		public static int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			int result;
			using (MySqlConnection conn = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				conn.Open();
				using (MySqlTransaction trans = conn.BeginTransaction())
				{
					MySqlCommand cmd = new MySqlCommand();
					try
					{
						int count = 0;
						foreach (CommandInfo myDE in cmdList)
						{
							string cmdText = myDE.CommandText;
							MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
							DbHelperMySQL.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
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
					finally
					{
						if (cmd != null)
						{
							cmd.Dispose();
						}
						if (conn != null)
						{
							conn.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static void ExecuteSqlTranWithIndentity(List<CommandInfo> SQLStringList)
		{
			using (MySqlConnection conn = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				conn.Open();
				using (MySqlTransaction trans = conn.BeginTransaction())
				{
					MySqlCommand cmd = new MySqlCommand();
					try
					{
						int indentity = 0;
						foreach (CommandInfo myDE in SQLStringList)
						{
							string cmdText = myDE.CommandText;
							MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
							foreach (MySqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							DbHelperMySQL.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (MySqlParameter q in cmdParms)
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
					finally
					{
						if (cmd != null)
						{
							cmd.Dispose();
						}
						if (conn != null)
						{
							conn.Close();
						}
					}
				}
			}
		}

		
		public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (MySqlConnection conn = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				conn.Open();
				using (MySqlTransaction trans = conn.BeginTransaction())
				{
					MySqlCommand cmd = new MySqlCommand();
					try
					{
						int indentity = 0;
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry myDE = (DictionaryEntry)obj;
							string cmdText = myDE.Key.ToString();
							MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
							foreach (MySqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							DbHelperMySQL.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (MySqlParameter q in cmdParms)
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
					finally
					{
						if (cmd != null)
						{
							cmd.Dispose();
						}
						if (conn != null)
						{
							conn.Close();
						}
					}
				}
			}
		}

		
		public static object GetSingle(string SQLString, params MySqlParameter[] cmdParms)
		{
			object result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand cmd = new MySqlCommand())
				{
					try
					{
						DbHelperMySQL.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
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
						if (connection != null)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		
		public static MySqlDataReader ExecuteReader(string SQLString, params MySqlParameter[] cmdParms)
		{
			MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString);
			MySqlCommand cmd = new MySqlCommand();
			MySqlDataReader result;
			try
			{
				DbHelperMySQL.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				cmd.Parameters.Clear();
				result = myReader;
			}
			catch (MySqlException e)
			{
				throw e;
			}
			return result;
		}

		
		public static DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
		{
			DataSet result;
			using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				MySqlCommand cmd = new MySqlCommand();
				DbHelperMySQL.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
				{
					DataSet ds = new DataSet();
					try
					{
						da.Fill(ds, "ds");
						cmd.Parameters.Clear();
					}
					catch (MySqlException ex)
					{
						throw new Exception(ex.Message);
					}
					finally
					{
						if (cmd != null)
						{
							cmd.Dispose();
						}
						if (connection != null)
						{
							connection.Close();
						}
					}
					result = ds;
				}
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

		
		public static string connectionString = PubConstant.ConnectionString;

		
		public static string connectionLogString = PubConstant.ConnectionLogString;
	}
}
