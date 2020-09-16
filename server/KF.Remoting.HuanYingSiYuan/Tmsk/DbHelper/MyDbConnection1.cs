using System;
using System.Collections.Generic;
using System.Data;
using MySQLDriverCS;
using Server.Tools;

namespace Tmsk.DbHelper
{
	
	public class MyDbConnection1
	{
		
		public MyDbConnection1(string connStr, string dbNames)
		{
			bool success = false;
			MySQLConnection dbConn = null;
			try
			{
				Dictionary<string, string> mapParams = new Dictionary<string, string>();
				string[] strParams = connStr.Split(new char[]
				{
					';'
				});
				foreach (string param in strParams)
				{
					string[] map = param.Split(new char[]
					{
						'='
					});
					if (map.Length == 2)
					{
						map[0] = map[0].Trim();
						map[1] = map[1].Trim();
						mapParams[map[0]] = map[1];
					}
				}
				this.ConnStr = new MySQLConnectionString(mapParams["host"], mapParams["database"], mapParams["user id"], mapParams["password"]);
				dbConn = new MySQLConnection(this.ConnStr.AsString);
				dbConn.Open();
				if (!string.IsNullOrEmpty(dbNames))
				{
					MySQLCommand cmd = new MySQLCommand(string.Format("SET names '{0}'", dbNames), dbConn);
					cmd.ExecuteNonQuery();
				}
				this.DatabaseName = this.DbConn.Database;
				success = true;
				this.DbConn = dbConn;
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
				}
				catch
				{
				}
			}
		}

		
		public void UseDatabase(string databaseKey, string databaseName)
		{
			if (databaseKey != this.DatabaseKey)
			{
				try
				{
					MySQLCommand cmd = new MySQLCommand(string.Format("use '{0}'", databaseName), this.DbConn);
					cmd.ExecuteNonQuery();
					cmd.Dispose();
					this.DatabaseKey = databaseKey;
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		
		private MySQLConnection DbConn = null;

		
		private MySQLConnectionString ConnStr = null;

		
		public string DatabaseKey = null;

		
		private string DatabaseName = null;
	}
}
