using System;
using System.Collections.Generic;
using System.Threading;
using MySQLDriverCS;
using Server.Tools;

namespace LogDBServer.DB
{
	
	public class DBConnections
	{
		
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

		
		public int GetDBConnsCount()
		{
			int count;
			lock (this.DBConns)
			{
				count = this.DBConns.Count;
			}
			return count;
		}

		
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

		
		public void PushDBConnection(MySQLConnection conn)
		{
			lock (this.DBConns)
			{
				this.DBConns.Enqueue(conn);
			}
			this.SemaphoreClients.Release();
		}

		
		public static string dbNames = "";

		
		private Semaphore SemaphoreClients = null;

		
		private Queue<MySQLConnection> DBConns = new Queue<MySQLConnection>(100);
	}
}
