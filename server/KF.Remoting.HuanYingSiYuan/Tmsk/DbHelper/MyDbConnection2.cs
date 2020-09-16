using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace Tmsk.DbHelper
{
	
	public class MyDbConnection2
	{
		
		public MyDbConnection2(string connStr, string pageCodeNames)
		{
			this.ConnStr = connStr;
			this.PageCodeNames = pageCodeNames;
		}

		
		public bool Open()
		{
			bool success = false;
			MySqlConnection dbConn = null;
			try
			{
				dbConn = new MySqlConnection(this.ConnStr);
				dbConn.Open();
				this.DatabaseName = dbConn.Database;
				success = true;
				this.DbConn = dbConn;
				if (!string.IsNullOrEmpty(this.PageCodeNames))
				{
					this.ExecuteNonQuery(string.Format("SET names '{0}'", this.PageCodeNames), 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			if (!success && null != dbConn)
			{
				try
				{
					dbConn.Close();
				}
				catch
				{
				}
			}
			return success;
		}

		
		public bool IsConnected()
		{
			if (null != this.DbConn)
			{
				if (this.DbConn.State != ConnectionState.Closed && ConnectionState.Closed == (this.DbConn.State & ConnectionState.Broken))
				{
					return true;
				}
			}
			return false;
		}

		
		public void Close()
		{
			if (null != this.DbConn)
			{
				try
				{
					this.DbConn.Close();
					this.DbConn.Dispose();
				}
				catch
				{
				}
			}
		}

		
		public void UseDatabase(string databaseKey, string databaseName)
		{
			if (databaseKey != this.DatabaseKey && !string.IsNullOrEmpty(databaseName))
			{
				this.DbConn.ChangeDatabase(databaseName);
				this.DatabaseKey = databaseKey;
			}
		}

		
		public int ExecuteNonQuery(string sql, int commandTimeout = 0)
		{
			int result = -1;
			try
			{
				using (MySqlCommand cmd = new MySqlCommand(sql, this.DbConn))
				{
					if (commandTimeout > 0)
					{
						cmd.CommandTimeout = commandTimeout;
					}
					result = cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return result;
		}

		
		public int ExecuteWithContent(string sql, string content)
		{
			int result = 0;
			try
			{
				using (MySqlCommand cmd = new MySqlCommand(sql, this.DbConn))
				{
					MySqlParameter myParameter = new MySqlParameter("@content", content);
					cmd.Parameters.Add(myParameter);
					result = cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return result;
		}

		
		public object GetSingle(string sql, int commandTimeout = 0, params MySqlParameter[] cmdParms)
		{
			try
			{
				using (MySqlCommand cmd = new MySqlCommand(sql, this.DbConn))
				{
					if (commandTimeout > 0)
					{
						cmd.CommandTimeout = commandTimeout;
					}
					if (cmdParms.Length > 0)
					{
						MyDbConnection2.PrepareCommand(cmd, this.DbConn, null, sql, cmdParms);
					}
					object obj = cmd.ExecuteScalar();
					if (cmdParms.Length > 0)
					{
						cmd.Parameters.Clear();
					}
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						return null;
					}
					return obj;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return null;
		}

		
		public object ExecuteSqlGet(string sql, string content)
		{
			try
			{
				using (MySqlCommand cmd = new MySqlCommand(sql, this.DbConn))
				{
					MySqlParameter myParameter = new MySqlParameter("@content", content);
					cmd.Parameters.Add(myParameter);
					object obj = cmd.ExecuteScalar();
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						return null;
					}
					return obj;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return null;
		}

		
		public MySqlDataReader ExecuteReader(string sql, params MySqlParameter[] cmdParms)
		{
			try
			{
				using (MySqlCommand cmd = new MySqlCommand(sql, this.DbConn))
				{
					if (cmdParms.Length > 0)
					{
						MyDbConnection2.PrepareCommand(cmd, this.DbConn, null, sql, cmdParms);
					}
					MySqlDataReader myReader = cmd.ExecuteReader();
					if (cmdParms.Length > 0)
					{
						cmd.Parameters.Clear();
					}
					return myReader;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return null;
		}

		
		public DataSet Query(string SQLString, int Times = 0)
		{
			DataSet ds = new DataSet();
			MySqlDataAdapter mySqlDataAdapter;
			MySqlDataAdapter command = mySqlDataAdapter = new MySqlDataAdapter(SQLString, this.DbConn);
			try
			{
				if (Times > 0)
				{
					command.SelectCommand.CommandTimeout = Times;
				}
				command.Fill(ds, "ds");
			}
			catch (MySqlException ex)
			{
				throw new Exception(SQLString + "\r\n" + ex.Message);
			}
			finally
			{
				if (mySqlDataAdapter != null)
				{
					((IDisposable)mySqlDataAdapter).Dispose();
				}
			}
			return ds;
		}

		
		public DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
		{
			DataSet ds = new DataSet();
			MySqlCommand cmd = new MySqlCommand();
			MyDbConnection2.PrepareCommand(cmd, this.DbConn, null, SQLString, cmdParms);
			DataSet result;
			using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
			{
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
				}
				result = ds;
			}
			return result;
		}

		
		public int ExecuteSqlTran(List<string> SQLStringList)
		{
			MySqlConnection connection = this.DbConn;
			int result;
			using (MySqlCommand cmd = new MySqlCommand())
			{
				cmd.Connection = connection;
				MySqlTransaction tx = connection.BeginTransaction();
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

		
		public int ExecuteSqlInsertImg(string strSQL, List<Tuple<string, byte[]>> imgList)
		{
			int result = 0;
			try
			{
				using (MySqlCommand cmd = new MySqlCommand(strSQL, this.DbConn))
				{
					foreach (Tuple<string, byte[]> t in imgList)
					{
						string imgTag = t.Item1;
						byte[] imgData = t.Item2;
						MySqlParameter myParameter = new MySqlParameter(imgTag, imgData);
						cmd.Parameters.Add(myParameter);
					}
					result = cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(strSQL + "\r\n" + ex.ToString());
			}
			return result;
		}

		
		public int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			MySqlConnection connection = this.DbConn;
			int result;
			using (MySqlCommand cmd = new MySqlCommand())
			{
				cmd.Connection = connection;
				MySqlTransaction tx = connection.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					foreach (CommandInfo myDE in list)
					{
						string cmdText = myDE.CommandText;
						MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
						MyDbConnection2.PrepareCommand(cmd, connection, tx, cmdText, cmdParms);
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
			}
			return result;
		}

		
		public int ExecuteSql(string sql, params MySqlParameter[] cmdParms)
		{
			int result = 0;
			try
			{
				using (MySqlCommand cmd = new MySqlCommand(sql, this.DbConn))
				{
					MyDbConnection2.PrepareCommand(cmd, this.DbConn, null, sql, cmdParms);
					result = cmd.ExecuteNonQuery();
					cmd.Parameters.Clear();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return result;
		}

		
		public void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (MySqlTransaction trans = this.DbConn.BeginTransaction())
			{
				using (MySqlCommand cmd = new MySqlCommand())
				{
					try
					{
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry myDE = (DictionaryEntry)obj;
							string cmdText = myDE.Key.ToString();
							MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
							MyDbConnection2.PrepareCommand(cmd, this.DbConn, trans, cmdText, cmdParms);
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

		
		public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (MySqlTransaction trans = this.DbConn.BeginTransaction())
			{
				using (MySqlCommand cmd = new MySqlCommand())
				{
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
							MyDbConnection2.PrepareCommand(cmd, this.DbConn, trans, cmdText, cmdParms);
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
				}
			}
		}

		
		public int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			int result;
			using (MySqlTransaction trans = this.DbConn.BeginTransaction())
			{
				using (MySqlCommand cmd = new MySqlCommand())
				{
					try
					{
						int count = 0;
						foreach (CommandInfo myDE in cmdList)
						{
							string cmdText = myDE.CommandText;
							MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
							MyDbConnection2.PrepareCommand(cmd, this.DbConn, trans, cmdText, cmdParms);
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
			using (MySqlTransaction trans = this.DbConn.BeginTransaction())
			{
				using (MySqlCommand cmd = new MySqlCommand())
				{
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
							MyDbConnection2.PrepareCommand(cmd, this.DbConn, trans, cmdText, cmdParms);
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
				}
			}
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

		
		public MySqlConnection DbConn = null;

		
		public string DatabaseKey = null;

		
		private string DatabaseName = null;

		
		private string ConnStr;

		
		private string PageCodeNames;
	}
}
