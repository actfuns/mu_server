using System;
using System.Collections.Generic;
using System.Threading;
using MySQLDriverCS;
using Server.Tools;

namespace LogDBServer.DB
{
	// Token: 0x0200000E RID: 14
	public class DBConnections
	{
		// Token: 0x0600002E RID: 46 RVA: 0x00002A48 File Offset: 0x00000C48
		public void BuidConnections(MySQLConnectionString connStr, int maxCount)
		{
			MySQLConnection dbConn = null;
			for (int i = 0; i < maxCount; i++)
			{
				dbConn = new MySQLConnection(connStr.AsString);
				try
				{
					dbConn.Open();
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "BuidConnections", false, false);
					throw ex;
				}
				if (!string.IsNullOrEmpty(DBConnections.dbNames))
				{
					MySQLCommand cmd = new MySQLCommand(string.Format("SET names '{0}'", DBConnections.dbNames), dbConn);
					cmd.ExecuteNonQuery();
				}
				this.DBConns.Enqueue(dbConn);
			}
			this.SemaphoreClients = new Semaphore(maxCount, maxCount);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002AF4 File Offset: 0x00000CF4
		public int GetDBConnsCount()
		{
			int count;
			lock (this.DBConns)
			{
				count = this.DBConns.Count;
			}
			return count;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002B48 File Offset: 0x00000D48
		public MySQLConnection PopDBConnection()
		{
			this.SemaphoreClients.WaitOne();
			MySQLConnection result;
			lock (this.DBConns)
			{
				result = this.DBConns.Dequeue();
			}
			return result;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002BA8 File Offset: 0x00000DA8
		public void PushDBConnection(MySQLConnection conn)
		{
			lock (this.DBConns)
			{
				this.DBConns.Enqueue(conn);
			}
			this.SemaphoreClients.Release();
		}

		// Token: 0x0400001A RID: 26
		public static string dbNames = "";

		// Token: 0x0400001B RID: 27
		private Semaphore SemaphoreClients = null;

		// Token: 0x0400001C RID: 28
		private Queue<MySQLConnection> DBConns = new Queue<MySQLConnection>(100);
	}
}
