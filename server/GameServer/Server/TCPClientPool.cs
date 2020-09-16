using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GameServer.Server
{
	
	public class TCPClientPool
	{
		
		private TCPClientPool()
		{
		}

		
		public static TCPClientPool getInstance()
		{
			return TCPClientPool.instance;
		}

		
		public static TCPClientPool getLogInstance()
		{
			return TCPClientPool.logInstance;
		}

		
		public void initialize(int capacity)
		{
			this.pool = new Queue<TCPClient>(capacity);
		}

		
		
		public int InitCount
		{
			get
			{
				return this._InitCount;
			}
		}

		
		
		
		public Program RootWindow { get; set; }

		
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

		
		public int GetPoolCount()
		{
			int count;
			lock (this.pool)
			{
				count = this.pool.Count;
			}
			return count;
		}

		
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

		
		private static TCPClientPool instance = new TCPClientPool();

		
		private static TCPClientPool logInstance = new TCPClientPool();

		
		private int _InitCount = 0;

		
		private int ErrCount = 0;

		
		private int ItemCount = 0;

		
		private string RemoteIP = "";

		
		private int RemotePort = 0;

		
		private Queue<TCPClient> pool;

		
		private Semaphore semaphoreClients;

		
		private string ServerName = "";
	}
}
