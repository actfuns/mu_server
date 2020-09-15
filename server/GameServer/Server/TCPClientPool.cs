using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GameServer.Server
{
	// Token: 0x020008BA RID: 2234
	public class TCPClientPool
	{
		// Token: 0x06003DE8 RID: 15848 RVA: 0x0034CF3C File Offset: 0x0034B13C
		private TCPClientPool()
		{
		}

		// Token: 0x06003DE9 RID: 15849 RVA: 0x0034CF7C File Offset: 0x0034B17C
		public static TCPClientPool getInstance()
		{
			return TCPClientPool.instance;
		}

		// Token: 0x06003DEA RID: 15850 RVA: 0x0034CF94 File Offset: 0x0034B194
		public static TCPClientPool getLogInstance()
		{
			return TCPClientPool.logInstance;
		}

		// Token: 0x06003DEB RID: 15851 RVA: 0x0034CFAB File Offset: 0x0034B1AB
		public void initialize(int capacity)
		{
			this.pool = new Queue<TCPClient>(capacity);
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06003DEC RID: 15852 RVA: 0x0034CFBC File Offset: 0x0034B1BC
		public int InitCount
		{
			get
			{
				return this._InitCount;
			}
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x06003DED RID: 15853 RVA: 0x0034CFD4 File Offset: 0x0034B1D4
		// (set) Token: 0x06003DEE RID: 15854 RVA: 0x0034CFEB File Offset: 0x0034B1EB
		public Program RootWindow { get; set; }

		// Token: 0x06003DEF RID: 15855 RVA: 0x0034CFF4 File Offset: 0x0034B1F4
		public void Init(int count, string ip, int port, string serverName)
		{
			this.ServerName = serverName;
			this._InitCount = count;
			this.ItemCount = 0;
			this.RemoteIP = ip;
			this.RemotePort = port;
			this.semaphoreClients = new Semaphore(count, count);
			for (int i = 0; i < count; i++)
			{
				TCPClient tcpClient = new TCPClient
				{
					RootWindow = this.RootWindow,
					ListIndex = this.ItemCount
				};
				this.RootWindow.AddDBConnectInfo(this.ItemCount, string.Format("{0}, 准备连接到{1}: {2}{3}", new object[]
				{
					this.ItemCount,
					this.ServerName,
					this.RemoteIP,
					this.RemotePort
				}));
				tcpClient.Connect(this.RemoteIP, this.RemotePort, this.ServerName);
				this.pool.Enqueue(tcpClient);
				this.ItemCount++;
			}
		}

		// Token: 0x06003DF0 RID: 15856 RVA: 0x0034D0F8 File Offset: 0x0034B2F8
		public void Clear()
		{
			lock (this.pool)
			{
				for (int i = 0; i < this.pool.Count; i++)
				{
					TCPClient tcpClient = this.pool.ElementAt(i);
					tcpClient.Disconnect();
				}
				this.pool.Clear();
			}
		}

		// Token: 0x06003DF1 RID: 15857 RVA: 0x0034D180 File Offset: 0x0034B380
		public int GetPoolCount()
		{
			int count;
			lock (this.pool)
			{
				count = this.pool.Count;
			}
			return count;
		}

		// Token: 0x06003DF2 RID: 15858 RVA: 0x0034D1D4 File Offset: 0x0034B3D4
		public void Supply()
		{
			lock (this.pool)
			{
				if (this.ErrCount > 0)
				{
					if (this.ErrCount > 0)
					{
						try
						{
							TCPClient tcpClient = new TCPClient
							{
								RootWindow = this.RootWindow,
								ListIndex = this.ItemCount
							};
							this.RootWindow.AddDBConnectInfo(this.ItemCount, string.Format("{0}, 准备连接到{1}: {2}{3}", new object[]
							{
								this.ItemCount,
								this.ServerName,
								this.RemoteIP,
								this.RemotePort
							}));
							this.ItemCount++;
							tcpClient.Connect(this.RemoteIP, this.RemotePort, this.ServerName);
							this.pool.Enqueue(tcpClient);
							this.ErrCount--;
							this.semaphoreClients.Release();
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}

		// Token: 0x06003DF3 RID: 15859 RVA: 0x0034D340 File Offset: 0x0034B540
		public TCPClient Pop()
		{
			this.semaphoreClients.WaitOne();
			TCPClient result;
			lock (this.pool)
			{
				result = this.pool.Dequeue();
			}
			return result;
		}

		// Token: 0x06003DF4 RID: 15860 RVA: 0x0034D3A0 File Offset: 0x0034B5A0
		public void Push(TCPClient tcpClient)
		{
			if (!tcpClient.IsConnected())
			{
				lock (this.pool)
				{
					this.ErrCount++;
				}
			}
			else
			{
				lock (this.pool)
				{
					this.pool.Enqueue(tcpClient);
				}
				this.semaphoreClients.Release();
			}
		}

		// Token: 0x040047F3 RID: 18419
		private static TCPClientPool instance = new TCPClientPool();

		// Token: 0x040047F4 RID: 18420
		private static TCPClientPool logInstance = new TCPClientPool();

		// Token: 0x040047F5 RID: 18421
		private int _InitCount = 0;

		// Token: 0x040047F6 RID: 18422
		private int ErrCount = 0;

		// Token: 0x040047F7 RID: 18423
		private int ItemCount = 0;

		// Token: 0x040047F8 RID: 18424
		private string RemoteIP = "";

		// Token: 0x040047F9 RID: 18425
		private int RemotePort = 0;

		// Token: 0x040047FA RID: 18426
		private Queue<TCPClient> pool;

		// Token: 0x040047FB RID: 18427
		private Semaphore semaphoreClients;

		// Token: 0x040047FC RID: 18428
		private string ServerName = "";
	}
}
