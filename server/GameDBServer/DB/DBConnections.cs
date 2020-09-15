using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.DB
{
	// Token: 0x020000E3 RID: 227
	public class DBConnections
	{
		// Token: 0x060001E6 RID: 486 RVA: 0x0000A658 File Offset: 0x00008858
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

		// Token: 0x060001E7 RID: 487 RVA: 0x0000A700 File Offset: 0x00008900
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

		// Token: 0x060001E8 RID: 488 RVA: 0x0000A810 File Offset: 0x00008A10
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

		// Token: 0x060001E9 RID: 489 RVA: 0x0000A890 File Offset: 0x00008A90
		public int GetDBConnsCount()
		{
			int count;
			lock (this.DBConns)
			{
				count = this.DBConns.Count;
			}
			return count;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000A8E4 File Offset: 0x00008AE4
		private void ReleaseConn()
		{
			DBConnections.ConcurrentCount--;
			if (this.ConcurrentThreadId == Thread.CurrentThread.ManagedThreadId && DBConnections.ConcurrentCount == 0)
			{
				this.ConcurrentThreadId = 0;
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000A92C File Offset: 0x00008B2C
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

		// Token: 0x060001EC RID: 492 RVA: 0x0000AB60 File Offset: 0x00008D60
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

		// Token: 0x0400061E RID: 1566
		public static string dbNames = "";

		// Token: 0x0400061F RID: 1567
		private Semaphore SemaphoreClients = null;

		// Token: 0x04000620 RID: 1568
		private Queue<MySQLConnection> DBConns = new Queue<MySQLConnection>(100);

		// Token: 0x04000621 RID: 1569
		private string ConnectionString;

		// Token: 0x04000622 RID: 1570
		private int CurrentCount;

		// Token: 0x04000623 RID: 1571
		private int MaxCount;

		// Token: 0x04000624 RID: 1572
		[ThreadStatic]
		private static int ConcurrentCount;

		// Token: 0x04000625 RID: 1573
		private int ConcurrentThreadId;
	}
}
