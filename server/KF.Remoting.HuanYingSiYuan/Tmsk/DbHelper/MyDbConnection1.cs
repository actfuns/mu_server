using System;
using System.Collections.Generic;
using System.Data;
using MySQLDriverCS;
using Server.Tools;

namespace Tmsk.DbHelper
{
	// Token: 0x0200001B RID: 27
	public class MyDbConnection1
	{
		// Token: 0x060000CC RID: 204 RVA: 0x0000AB68 File Offset: 0x00008D68
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

		// Token: 0x060000CD RID: 205 RVA: 0x0000AD38 File Offset: 0x00008F38
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

		// Token: 0x060000CE RID: 206 RVA: 0x0000AD88 File Offset: 0x00008F88
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

		// Token: 0x060000CF RID: 207 RVA: 0x0000ADCC File Offset: 0x00008FCC
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

		// Token: 0x040000A6 RID: 166
		private MySQLConnection DbConn = null;

		// Token: 0x040000A7 RID: 167
		private MySQLConnectionString ConnStr = null;

		// Token: 0x040000A8 RID: 168
		public string DatabaseKey = null;

		// Token: 0x040000A9 RID: 169
		private string DatabaseName = null;
	}
}
