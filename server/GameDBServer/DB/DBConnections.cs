using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.DB
{
	
	public class DBConnections
	{
		
		public void BuidConnections(MySQLConnectionString connStr, int maxCount)
		{
			lock (this.DBConns)
			{
				this.ConnectionString = connStr.AsString;
				this.MaxCount = maxCount;
				this.SemaphoreClients = new Semaphore(0, maxCount);
				for (int i = 0; i < maxCount; i++)
				{
					MySQLConnection dbConn = this.CreateAConnection();
					if (null == dbConn)
					{
						throw new Exception(string.Format("连接MYSQL时失败", new object[0]));
					}
				}
			}
		}

		
		private MySQLConnection CreateAConnection()
		{
			try
			{
				MySQLConnection dbConn = null;
				dbConn = new MySQLConnection(this.ConnectionString);
				dbConn.Open();
				if (!string.IsNullOrEmpty(DBConnections.dbNames))
				{
					using (MySQLCommand cmd = new MySQLCommand(string.Format("SET names '{0}'", DBConnections.dbNames), dbConn))
					{
						cmd.ExecuteNonQuery();
					}
				}
				lock (this.DBConns)
				{
					this.DBConns.Enqueue(dbConn);
					this.CurrentCount++;
					this.SemaphoreClients.Release();
				}
				return dbConn;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("创建数据库连接异常: \r\n{0}", ex.ToString()), null, true);
			}
			return null;
		}

		
		public bool SupplyConnections()
		{
			bool result = false;
			lock (this.DBConns)
			{
				if (this.CurrentCount < this.MaxCount)
				{
					MySQLConnection dbConn = this.CreateAConnection();
					if (null == dbConn)
					{
					}
				}
			}
			return result;
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

		
		private void ReleaseConn()
		{
			DBConnections.ConcurrentCount--;
			if (this.ConcurrentThreadId == Thread.CurrentThread.ManagedThreadId && DBConnections.ConcurrentCount == 0)
			{
				this.ConcurrentThreadId = 0;
			}
		}

		
		public MySQLConnection PopDBConnection()
		{
			MySQLConnection conn = null;
			bool lost = false;
			for (;;)
			{
				string cmdText = "select 1";
				lost = true;
				this.SemaphoreClients.WaitOne();
				lock (this.DBConns)
				{
					if (this.DBConns.Count < 5)
					{
						int threadId = Thread.CurrentThread.ManagedThreadId;
						if (this.ConcurrentThreadId > 0 && this.ConcurrentThreadId != threadId)
						{
							this.SemaphoreClients.Release();
							Thread.Sleep(80);
							goto IL_1C6;
						}
						if (this.ConcurrentThreadId == 0)
						{
							this.ConcurrentThreadId = threadId;
						}
					}
					DBConnections.ConcurrentCount++;
					if (DBConnections.ConcurrentCount >= 4)
					{
						LogManager.WriteLog(LogTypes.Fatal, "同时使用数据库连接数过多,可能导致资源耗尽死锁,应当优化:" + new StackTrace(1, true).ToString(), null, true);
					}
					conn = this.DBConns.Dequeue();
				}
				goto IL_106;
				IL_1C6:
				if (!lost)
				{
					break;
				}
				continue;
				IL_106:
				try
				{
					using (MySQLCommand cmd = new MySQLCommand(cmdText, conn))
					{
						try
						{
							cmd.ExecuteNonQuery();
							lost = false;
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(LogTypes.Exception, string.Format("检测数据库连接有效性异常: {0}\r\n{1}", cmdText, ex.ToString()), null, true);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				finally
				{
					if (lost)
					{
						lock (this.DBConns)
						{
							this.ReleaseConn();
							this.CurrentCount--;
						}
					}
				}
				goto IL_1C6;
			}
			return conn;
		}

		
		public void PushDBConnection(MySQLConnection conn)
		{
			if (null != conn)
			{
				lock (this.DBConns)
				{
					this.ReleaseConn();
					this.DBConns.Enqueue(conn);
				}
				this.SemaphoreClients.Release();
			}
		}

		
		public static string dbNames = "";

		
		private Semaphore SemaphoreClients = null;

		
		private Queue<MySQLConnection> DBConns = new Queue<MySQLConnection>(100);

		
		private string ConnectionString;

		
		private int CurrentCount;

		
		private int MaxCount;

		
		[ThreadStatic]
		private static int ConcurrentCount;

		
		private int ConcurrentThreadId;
	}
}
