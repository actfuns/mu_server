using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.DB
{
	// Token: 0x020000F2 RID: 242
	public class MyDbConnection3 : IDisposable
	{
		// Token: 0x06000427 RID: 1063 RVA: 0x000201D8 File Offset: 0x0001E3D8
		public MyDbConnection3(bool logSelectSqlText = false)
		{
			this._MySQLDataReader = null;
			this.m_logSelectSqlText = logSelectSqlText;
			this.DbConn = DBManager.getInstance().DBConns.PopDBConnection();
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00020226 File Offset: 0x0001E426
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x00020238 File Offset: 0x0001E438
		protected virtual void Dispose(bool isDisposing)
		{
			if (!this.m_disposed)
			{
				if (isDisposing)
				{
					if (this._MySQLDataReader != null && !this._MySQLDataReader.IsClosed)
					{
						this._MySQLDataReader.Close();
						this._MySQLDataReader = null;
					}
					DBManager.getInstance().DBConns.PushDBConnection(this.DbConn);
				}
				this.m_disposed = true;
			}
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x000202A9 File Offset: 0x0001E4A9
		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x000202B4 File Offset: 0x0001E4B4
		private void LogSql(string sqlText)
		{
			if (MyDbConnection3.LogSQLString)
			{
				if (!sqlText.StartsWith("select", StringComparison.OrdinalIgnoreCase) || this.m_logSelectSqlText)
				{
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sqlText), EventLevels.Important);
				}
			}
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00020308 File Offset: 0x0001E508
		public bool ExecuteNonQueryBool(string sql, int commandTimeout = 0)
		{
			return this.ExecuteNonQuery(sql, commandTimeout) >= 0;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00020328 File Offset: 0x0001E528
		public int ExecuteNonQuery(string sql, int commandTimeout = 0)
		{
			int result = -1;
			try
			{
				using (MySQLCommand cmd = new MySQLCommand(sql, this.DbConn))
				{
					if (commandTimeout > 0)
					{
						cmd.CommandTimeout = commandTimeout;
					}
					result = cmd.ExecuteNonQuery();
					this.LogSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				result = -1;
			}
			return result;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x000203E0 File Offset: 0x0001E5E0
		public int ExecuteWithContent(string sql, string content)
		{
			int result = 0;
			try
			{
				using (MySQLCommand cmd = new MySQLCommand(sql, this.DbConn))
				{
					MySQLParameter myParameter = new MySQLParameter("@content", content);
					cmd.Parameters.Add(myParameter);
					result = cmd.ExecuteNonQuery();
					this.LogSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				result = -1;
			}
			return result;
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x0002049C File Offset: 0x0001E69C
		public int GetSingleInt(string sql, int commandTimeout = 0, params MySQLParameter[] cmdParms)
		{
			object obj = this.GetSingle(sql, commandTimeout, cmdParms);
			int result;
			if (obj == null)
			{
				result = 0;
			}
			else
			{
				result = Convert.ToInt32(obj.ToString());
			}
			return result;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x000204D4 File Offset: 0x0001E6D4
		public long GetSingleLong(string sql, int commandTimeout = 0, params MySQLParameter[] cmdParms)
		{
			object obj = this.GetSingle(sql, commandTimeout, cmdParms);
			long result;
			if (obj == null)
			{
				result = 0L;
			}
			else
			{
				result = Convert.ToInt64(obj.ToString());
			}
			return result;
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x0002050C File Offset: 0x0001E70C
		public object GetSingle(string sql, int commandTimeout = 0, params MySQLParameter[] cmdParms)
		{
			try
			{
				using (MySQLCommand cmd = new MySQLCommand(sql, this.DbConn))
				{
					if (commandTimeout > 0)
					{
						cmd.CommandTimeout = commandTimeout;
					}
					if (cmdParms.Length > 0)
					{
						MyDbConnection3.PrepareCommand(cmd, this.DbConn, null, sql, cmdParms);
					}
					object obj = cmd.ExecuteScalar();
					if (cmdParms.Length > 0)
					{
						cmd.Parameters.Clear();
					}
					this.LogSql(sql);
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						return null;
					}
					return obj;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
			}
			return null;
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00020620 File Offset: 0x0001E820
		public object ExecuteSqlGet(string sql, string content)
		{
			try
			{
				using (MySQLCommand cmd = new MySQLCommand(sql, this.DbConn))
				{
					MySQLParameter myParameter = new MySQLParameter("@content", content);
					cmd.Parameters.Add(myParameter);
					object obj = cmd.ExecuteScalar();
					this.LogSql(sql);
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						return null;
					}
					return obj;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
			}
			return null;
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x000206FC File Offset: 0x0001E8FC
		public MySQLDataReader ExecuteReader(string sql, params MySQLParameter[] cmdParms)
		{
			try
			{
				if (this._MySQLDataReader != null && !this._MySQLDataReader.IsClosed)
				{
					this._MySQLDataReader.Close();
					this._MySQLDataReader = null;
				}
				using (MySQLCommand cmd = new MySQLCommand(sql, this.DbConn))
				{
					if (cmdParms.Length > 0)
					{
						MyDbConnection3.PrepareCommand(cmd, this.DbConn, null, sql, cmdParms);
					}
					MySQLDataReader myReader = cmd.ExecuteReaderEx();
					if (cmdParms.Length > 0)
					{
						cmd.Parameters.Clear();
					}
					this._MySQLDataReader = myReader;
					this.LogSql(sql);
					return myReader;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
			}
			return null;
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x0002080C File Offset: 0x0001EA0C
		public DataSet Query(string sql, int Times = 0)
		{
			DataSet ds = new DataSet();
			MySQLDataAdapter mySQLDataAdapter;
			MySQLDataAdapter command = mySQLDataAdapter = new MySQLDataAdapter(sql, this.DbConn);
			try
			{
				if (Times > 0)
				{
					command.SelectCommand.CommandTimeout = Times;
				}
				command.Fill(ds, "ds");
				this.LogSql(sql);
			}
			catch (MySQLException ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
			}
			finally
			{
				if (mySQLDataAdapter != null)
				{
					((IDisposable)mySQLDataAdapter).Dispose();
				}
			}
			return ds;
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x000208D8 File Offset: 0x0001EAD8
		public DataSet Query(string sql, params MySQLParameter[] cmdParms)
		{
			DataSet ds = new DataSet();
			MySQLCommand cmd = new MySQLCommand();
			MyDbConnection3.PrepareCommand(cmd, this.DbConn, null, sql, cmdParms);
			DataSet result;
			using (MySQLDataAdapter da = new MySQLDataAdapter(cmd))
			{
				try
				{
					da.Fill(ds, "ds");
					cmd.Parameters.Clear();
					this.LogSql(sql);
				}
				catch (MySQLException ex)
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

		// Token: 0x06000436 RID: 1078 RVA: 0x000209A0 File Offset: 0x0001EBA0
		public int ExecuteSqlTran(List<string> SQLStringList)
		{
			MySQLConnection connection = this.DbConn;
			int result;
			using (MySQLCommand cmd = new MySQLCommand())
			{
				cmd.Connection = connection;
				DbTransaction tx = connection.BeginTransaction();
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
							this.LogSql(strsql);
						}
					}
					tx.Commit();
					result = count;
				}
				catch
				{
					tx.Rollback();
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x00020A80 File Offset: 0x0001EC80
		public int ExecuteSqlInsertImg(string sql, byte[] content)
		{
			int result = 0;
			try
			{
				using (MySQLCommand cmd = new MySQLCommand(sql, this.DbConn))
				{
					MySQLParameter myParameter = new MySQLParameter("@content", content);
					cmd.Parameters.Add(myParameter);
					result = cmd.ExecuteNonQuery();
					this.LogSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				result = -1;
			}
			return result;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00020B3C File Offset: 0x0001ED3C
		public int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			MySQLConnection connection = this.DbConn;
			int result;
			using (MySQLCommand cmd = new MySQLCommand())
			{
				cmd.Connection = connection;
				DbTransaction tx = connection.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					foreach (CommandInfo myDE in list)
					{
						string cmdText = myDE.CommandText;
						MySQLParameter[] cmdParms = myDE.Parameters;
						MyDbConnection3.PrepareCommand(cmd, connection, tx, cmdText, cmdParms);
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
					tx.Commit();
					result = 1;
				}
				catch (MySQLException e)
				{
					tx.Rollback();
					throw e;
				}
			}
			return result;
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x00020E4C File Offset: 0x0001F04C
		public int ExecuteSql(string sql, params MySQLParameter[] cmdParms)
		{
			int result = 0;
			try
			{
				using (MySQLCommand cmd = new MySQLCommand(sql, this.DbConn))
				{
					MyDbConnection3.PrepareCommand(cmd, this.DbConn, null, sql, cmdParms);
					result = cmd.ExecuteNonQuery();
					cmd.Parameters.Clear();
					this.LogSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				result = -1;
			}
			return result;
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x00020F08 File Offset: 0x0001F108
		public void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (DbTransaction trans = this.DbConn.BeginTransaction())
			{
				using (MySQLCommand cmd = new MySQLCommand())
				{
					try
					{
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry myDE = (DictionaryEntry)obj;
							string cmdText = myDE.Key.ToString();
							MySQLParameter[] cmdParms = (MySQLParameter[])myDE.Value;
							MyDbConnection3.PrepareCommand(cmd, this.DbConn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							cmd.Parameters.Clear();
							this.LogSql(cmdText);
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

		// Token: 0x0600043B RID: 1083 RVA: 0x00021030 File Offset: 0x0001F230
		public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (DbTransaction trans = this.DbConn.BeginTransaction())
			{
				using (MySQLCommand cmd = new MySQLCommand())
				{
					try
					{
						int indentity = 0;
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry myDE = (DictionaryEntry)obj;
							string cmdText = myDE.Key.ToString();
							MySQLParameter[] cmdParms = (MySQLParameter[])myDE.Value;
							foreach (MySQLParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							MyDbConnection3.PrepareCommand(cmd, this.DbConn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (MySQLParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.Output)
								{
									indentity = Convert.ToInt32(q.Value);
								}
							}
							cmd.Parameters.Clear();
							this.LogSql(cmdText);
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

		// Token: 0x0600043C RID: 1084 RVA: 0x00021228 File Offset: 0x0001F428
		public int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			int result;
			using (DbTransaction trans = this.DbConn.BeginTransaction())
			{
				using (MySQLCommand cmd = new MySQLCommand())
				{
					try
					{
						int count = 0;
						foreach (CommandInfo myDE in cmdList)
						{
							string cmdText = myDE.CommandText;
							MySQLParameter[] cmdParms = myDE.Parameters;
							MyDbConnection3.PrepareCommand(cmd, this.DbConn, trans, cmdText, cmdParms);
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
								this.LogSql(cmdText);
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

		// Token: 0x0600043D RID: 1085 RVA: 0x00021474 File Offset: 0x0001F674
		public void ExecuteSqlTranWithIndentity(List<CommandInfo> SQLStringList)
		{
			using (DbTransaction trans = this.DbConn.BeginTransaction())
			{
				using (MySQLCommand cmd = new MySQLCommand())
				{
					try
					{
						int indentity = 0;
						foreach (CommandInfo myDE in SQLStringList)
						{
							string cmdText = myDE.CommandText;
							MySQLParameter[] cmdParms = myDE.Parameters;
							foreach (MySQLParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							MyDbConnection3.PrepareCommand(cmd, this.DbConn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (MySQLParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.Output)
								{
									indentity = Convert.ToInt32(q.Value);
								}
							}
							cmd.Parameters.Clear();
							this.LogSql(cmdText);
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

		// Token: 0x0600043E RID: 1086 RVA: 0x00021650 File Offset: 0x0001F850
		private static void PrepareCommand(MySQLCommand cmd, MySQLConnection conn, DbTransaction trans, string cmdText, MySQLParameter[] cmdParms)
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
				foreach (MySQLParameter parameter in cmdParms)
				{
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
					{
						parameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(parameter);
				}
			}
		}

		// Token: 0x040006DE RID: 1758
		public MySQLConnection DbConn = null;

		// Token: 0x040006DF RID: 1759
		private MySQLDataReader _MySQLDataReader;

		// Token: 0x040006E0 RID: 1760
		public static bool LogSQLString = true;

		// Token: 0x040006E1 RID: 1761
		private bool m_disposed = false;

		// Token: 0x040006E2 RID: 1762
		private bool m_logSelectSqlText = false;
	}
}
