using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;

namespace Maticsoft.DBUtility
{
	
	public abstract class OracleHelper
	{
		
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

		
		public static int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
		{
			OracleCommand cmd = new OracleCommand();
			OracleHelper.PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		
		public static int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
		{
			OracleCommand cmd = new OracleCommand();
			OracleHelper.PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		
		public static int ExecuteNonQuery(string connectionString, string cmdText)
		{
			OracleCommand cmd = new OracleCommand();
			OracleConnection connection = new OracleConnection(connectionString);
			OracleHelper.PrepareCommand(cmd, connection, null, CommandType.Text, cmdText, null);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;
		}

		
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

		
		public static object ExecuteScalar(OracleConnection connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
		{
			OracleCommand cmd = new OracleCommand();
			OracleHelper.PrepareCommand(cmd, connectionString, null, cmdType, cmdText, commandParameters);
			object val = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			return val;
		}

		
		public static void CacheParameters(string cacheKey, params OracleParameter[] commandParameters)
		{
			OracleHelper.parmCache[cacheKey] = commandParameters;
		}

		
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

		
		public static bool OraBool(string value)
		{
			return value.Equals("Y");
		}

		
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

		
		public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.AppSettings["OraConnString1"];

		
		public static readonly string ConnectionStringInventoryDistributedTransaction = ConfigurationManager.AppSettings["OraConnString2"];

		
		public static readonly string ConnectionStringOrderDistributedTransaction = ConfigurationManager.AppSettings["OraConnString3"];

		
		public static readonly string ConnectionStringProfile = ConfigurationManager.AppSettings["OraProfileConnString"];

		
		public static readonly string ConnectionStringMembership = ConfigurationManager.AppSettings["OraMembershipConnString"];

		
		private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
	}
}
