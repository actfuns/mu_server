using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace Maticsoft.DBUtility
{
	// Token: 0x0200004A RID: 74
	public abstract class DbHelperMySQL
	{
		// Token: 0x06000313 RID: 787 RVA: 0x0002B302 File Offset: 0x00029502
		public DbHelperMySQL()
		{
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0002B310 File Offset: 0x00029510
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

		// Token: 0x06000315 RID: 789 RVA: 0x0002B358 File Offset: 0x00029558
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

		// Token: 0x06000316 RID: 790 RVA: 0x0002B3B8 File Offset: 0x000295B8
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

		// Token: 0x06000317 RID: 791 RVA: 0x0002B418 File Offset: 0x00029618
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

		// Token: 0x06000318 RID: 792 RVA: 0x0002B4D8 File Offset: 0x000296D8
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

		// Token: 0x06000319 RID: 793 RVA: 0x0002B5A0 File Offset: 0x000297A0
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

		// Token: 0x0600031A RID: 794 RVA: 0x0002B930 File Offset: 0x00029B30
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

		// Token: 0x0600031B RID: 795 RVA: 0x0002BA44 File Offset: 0x00029C44
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

		// Token: 0x0600031C RID: 796 RVA: 0x0002BB0C File Offset: 0x00029D0C
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

		// Token: 0x0600031D RID: 797 RVA: 0x0002BC00 File Offset: 0x00029E00
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

		// Token: 0x0600031E RID: 798 RVA: 0x0002BD3C File Offset: 0x00029F3C
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

		// Token: 0x0600031F RID: 799 RVA: 0x0002BF00 File Offset: 0x0002A100
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

		// Token: 0x06000320 RID: 800 RVA: 0x0002BFE8 File Offset: 0x0002A1E8
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

		// Token: 0x06000321 RID: 801 RVA: 0x0002C028 File Offset: 0x0002A228
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

		// Token: 0x06000322 RID: 802 RVA: 0x0002C118 File Offset: 0x0002A318
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

		// Token: 0x06000323 RID: 803 RVA: 0x0002C208 File Offset: 0x0002A408
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

		// Token: 0x06000324 RID: 804 RVA: 0x0002C298 File Offset: 0x0002A498
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

		// Token: 0x06000325 RID: 805 RVA: 0x0002C358 File Offset: 0x0002A558
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

		// Token: 0x06000326 RID: 806 RVA: 0x0002C424 File Offset: 0x0002A624
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

		// Token: 0x06000327 RID: 807 RVA: 0x0002C4F4 File Offset: 0x0002A6F4
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

		// Token: 0x06000328 RID: 808 RVA: 0x0002C658 File Offset: 0x0002A858
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

		// Token: 0x06000329 RID: 809 RVA: 0x0002C8F0 File Offset: 0x0002AAF0
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

		// Token: 0x0600032A RID: 810 RVA: 0x0002CB14 File Offset: 0x0002AD14
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

		// Token: 0x0600032B RID: 811 RVA: 0x0002CD50 File Offset: 0x0002AF50
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

		// Token: 0x0600032C RID: 812 RVA: 0x0002CE48 File Offset: 0x0002B048
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

		// Token: 0x0600032D RID: 813 RVA: 0x0002CEA4 File Offset: 0x0002B0A4
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

		// Token: 0x0600032E RID: 814 RVA: 0x0002CF98 File Offset: 0x0002B198
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

		// Token: 0x040001D2 RID: 466
		public static string connectionString = PubConstant.ConnectionString;

		// Token: 0x040001D3 RID: 467
		public static string connectionLogString = PubConstant.ConnectionLogString;
	}
}
