using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;

namespace Maticsoft.DBUtility
{
	// Token: 0x02000050 RID: 80
	public abstract class OracleHelper
	{
		// Token: 0x060003A6 RID: 934 RVA: 0x00030DD8 File Offset: 0x0002EFD8
		public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
		{
			OracleCommand cmd = new OracleCommand();
			int result;
			using (OracleConnection connection = new OracleConnection(connectionString))
			{
				OracleHelper.PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
				int val = cmd.ExecuteNonQuery();
				connection.Close();
				cmd.Parameters.Clear();
				result = val;
			}
			return result;
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00030E44 File Offset: 0x0002F044
		public static DataSet Query(string connectionString, string SQLString)
		{
			DataSet result;
			using (OracleConnection connection = new OracleConnection(connectionString))
			{
				DataSet ds = new DataSet();
				try
				{
					connection.Open();
					OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
					command.Fill(ds, "ds");
				}
				catch (OracleException ex)
				{
					throw new Exception(ex.Message);
				}
				finally
				{
					if (connection.State != ConnectionState.Closed)
					{
						connection.Close();
					}
				}
				result = ds;
			}
			return result;
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00030EF0 File Offset: 0x0002F0F0
		public static DataSet Query(string connectionString, string SQLString, params OracleParameter[] cmdParms)
		{
			DataSet result;
			using (OracleConnection connection = new OracleConnection(connectionString))
			{
				OracleCommand cmd = new OracleCommand();
				OracleHelper.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				using (OracleDataAdapter da = new OracleDataAdapter(cmd))
				{
					DataSet ds = new DataSet();
					try
					{
						da.Fill(ds, "ds");
						cmd.Parameters.Clear();
					}
					catch (OracleException ex)
					{
						throw new Exception(ex.Message);
					}
					finally
					{
						if (connection.State != ConnectionState.Closed)
						{
							connection.Close();
						}
					}
					result = ds;
				}
			}
			return result;
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00030FD4 File Offset: 0x0002F1D4
		private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
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
				foreach (OracleParameter parameter in cmdParms)
				{
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
					{
						parameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(parameter);
				}
			}
		}

		// Token: 0x060003AA RID: 938 RVA: 0x00031088 File Offset: 0x0002F288
		public static object GetSingle(string connectionString, string SQLString)
		{
			object result;
			using (OracleConnection connection = new OracleConnection(connectionString))
			{
				using (OracleCommand cmd = new OracleCommand(SQLString, connection))
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
					catch (OracleException ex)
					{
						throw new Exception(ex.Message);
					}
					finally
					{
						if (connection.State != ConnectionState.Closed)
						{
							connection.Close();
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x00031168 File Offset: 0x0002F368
		public static bool Exists(string connectionString, string strOracle)
		{
			object obj = OracleHelper.GetSingle(connectionString, strOracle);
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

		// Token: 0x060003AC RID: 940 RVA: 0x000311C8 File Offset: 0x0002F3C8
		public static int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
		{
			OracleCommand cmd = new OracleCommand();
			OracleHelper.PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00031208 File Offset: 0x0002F408
		public static int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
		{
			OracleCommand cmd = new OracleCommand();
			OracleHelper.PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00031240 File Offset: 0x0002F440
		public static int ExecuteNonQuery(string connectionString, string cmdText)
		{
			OracleCommand cmd = new OracleCommand();
			OracleConnection connection = new OracleConnection(connectionString);
			OracleHelper.PrepareCommand(cmd, connection, null, CommandType.Text, cmdText, null);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00031280 File Offset: 0x0002F480
		public static OracleDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
		{
			OracleCommand cmd = new OracleCommand();
			OracleConnection conn = new OracleConnection(connectionString);
			OracleDataReader result;
			try
			{
				OracleHelper.PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
				OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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

		// Token: 0x060003B0 RID: 944 RVA: 0x000312E0 File Offset: 0x0002F4E0
		public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
		{
			OracleCommand cmd = new OracleCommand();
			object result;
			using (OracleConnection conn = new OracleConnection(connectionString))
			{
				OracleHelper.PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
				object val = cmd.ExecuteScalar();
				cmd.Parameters.Clear();
				result = val;
			}
			return result;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00031344 File Offset: 0x0002F544
		public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked\tor commited, please\tprovide\tan open\ttransaction.", "transaction");
			}
			OracleCommand cmd = new OracleCommand();
			OracleHelper.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
			object retval = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			return retval;
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x000313C0 File Offset: 0x0002F5C0
		public static object ExecuteScalar(OracleConnection connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
		{
			OracleCommand cmd = new OracleCommand();
			OracleHelper.PrepareCommand(cmd, connectionString, null, cmdType, cmdText, commandParameters);
			object val = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			return val;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x000313F8 File Offset: 0x0002F5F8
		public static void CacheParameters(string cacheKey, params OracleParameter[] commandParameters)
		{
			OracleHelper.parmCache[cacheKey] = commandParameters;
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x00031408 File Offset: 0x0002F608
		public static OracleParameter[] GetCachedParameters(string cacheKey)
		{
			OracleParameter[] cachedParms = (OracleParameter[])OracleHelper.parmCache[cacheKey];
			OracleParameter[] result;
			if (cachedParms == null)
			{
				result = null;
			}
			else
			{
				OracleParameter[] clonedParms = new OracleParameter[cachedParms.Length];
				int i = 0;
				int j = cachedParms.Length;
				while (i < j)
				{
					clonedParms[i] = (OracleParameter)((ICloneable)cachedParms[i]).Clone();
					i++;
				}
				result = clonedParms;
			}
			return result;
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00031470 File Offset: 0x0002F670
		private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] commandParameters)
		{
			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			cmd.CommandType = cmdType;
			if (trans != null)
			{
				cmd.Transaction = trans;
			}
			if (commandParameters != null)
			{
				foreach (OracleParameter parm in commandParameters)
				{
					cmd.Parameters.Add(parm);
				}
			}
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x000314F0 File Offset: 0x0002F6F0
		public static string OraBit(bool value)
		{
			string result;
			if (value)
			{
				result = "Y";
			}
			else
			{
				result = "N";
			}
			return result;
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x00031518 File Offset: 0x0002F718
		public static bool OraBool(string value)
		{
			return value.Equals("Y");
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x00031544 File Offset: 0x0002F744
		public static bool ExecuteSqlTran(string conStr, List<CommandInfo> cmdList)
		{
			bool result;
			using (OracleConnection conn = new OracleConnection(conStr))
			{
				conn.Open();
				OracleCommand cmd = new OracleCommand();
				cmd.Connection = conn;
				OracleTransaction tx = conn.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					foreach (CommandInfo c in cmdList)
					{
						if (!string.IsNullOrEmpty(c.CommandText))
						{
							OracleHelper.PrepareCommand(cmd, conn, tx, CommandType.Text, c.CommandText, (OracleParameter[])c.Parameters);
							if (c.EffentNextType == EffentNextType.WhenHaveContine || c.EffentNextType == EffentNextType.WhenNoHaveContine)
							{
								if (c.CommandText.ToLower().IndexOf("count(") == -1)
								{
									tx.Rollback();
									throw new Exception("Oracle:违背要求" + c.CommandText + "必须符合select count(..的格式");
								}
								object obj = cmd.ExecuteScalar();
								if (obj == null && obj == DBNull.Value)
								{
								}
								bool isHave = Convert.ToInt32(obj) > 0;
								if (c.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
								{
									tx.Rollback();
									throw new Exception("Oracle:违背要求" + c.CommandText + "返回值必须大于0");
								}
								if (c.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
								{
									tx.Rollback();
									throw new Exception("Oracle:违背要求" + c.CommandText + "返回值必须等于0");
								}
							}
							else
							{
								int res = cmd.ExecuteNonQuery();
								if (c.EffentNextType == EffentNextType.ExcuteEffectRows && res == 0)
								{
									tx.Rollback();
									throw new Exception("Oracle:违背要求" + c.CommandText + "必须有影像行");
								}
							}
						}
					}
					tx.Commit();
					result = true;
				}
				catch (OracleException E)
				{
					tx.Rollback();
					throw E;
				}
				finally
				{
					if (conn.State != ConnectionState.Closed)
					{
						conn.Close();
					}
				}
			}
			return result;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x000317F0 File Offset: 0x0002F9F0
		public static void ExecuteSqlTran(string conStr, List<string> SQLStringList)
		{
			using (OracleConnection conn = new OracleConnection(conStr))
			{
				conn.Open();
				OracleCommand cmd = new OracleCommand();
				cmd.Connection = conn;
				OracleTransaction tx = conn.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					foreach (string sql in SQLStringList)
					{
						if (!string.IsNullOrEmpty(sql))
						{
							cmd.CommandText = sql;
							cmd.ExecuteNonQuery();
						}
					}
					tx.Commit();
				}
				catch (OracleException E)
				{
					tx.Rollback();
					throw new Exception(E.Message);
				}
				finally
				{
					if (conn.State != ConnectionState.Closed)
					{
						conn.Close();
					}
				}
			}
		}

		// Token: 0x040001F0 RID: 496
		public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.AppSettings["OraConnString1"];

		// Token: 0x040001F1 RID: 497
		public static readonly string ConnectionStringInventoryDistributedTransaction = ConfigurationManager.AppSettings["OraConnString2"];

		// Token: 0x040001F2 RID: 498
		public static readonly string ConnectionStringOrderDistributedTransaction = ConfigurationManager.AppSettings["OraConnString3"];

		// Token: 0x040001F3 RID: 499
		public static readonly string ConnectionStringProfile = ConfigurationManager.AppSettings["OraProfileConnString"];

		// Token: 0x040001F4 RID: 500
		public static readonly string ConnectionStringMembership = ConfigurationManager.AppSettings["OraMembershipConnString"];

		// Token: 0x040001F5 RID: 501
		private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
	}
}
