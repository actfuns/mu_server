using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Server
{
	// Token: 0x020008BB RID: 2235
	public class GameDbClientPool : IConnectInfoContainer
	{
		// Token: 0x06003DF6 RID: 15862 RVA: 0x0034D464 File Offset: 0x0034B664
		public GameDbClientPool()
		{
			this.RootWindow = this;
			this.pool = new Queue<TCPClient>();
		}

		// Token: 0x06003DF7 RID: 15863 RVA: 0x0034D4D4 File Offset: 0x0034B6D4
		public void AddDBConnectInfo(int index, string info)
		{
			lock (this.DBServerConnectDict)
			{
				if (this.DBServerConnectDict.ContainsKey(index))
				{
					this.DBServerConnectDict[index] = info;
				}
				else
				{
					this.DBServerConnectDict.Add(index, info);
				}
			}
		}

		// Token: 0x06003DF8 RID: 15864 RVA: 0x0034D550 File Offset: 0x0034B750
		public void initialize(int capacity)
		{
			this.pool = new Queue<TCPClient>(capacity);
		}

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x06003DF9 RID: 15865 RVA: 0x0034D560 File Offset: 0x0034B760
		public int InitCount
		{
			get
			{
				return this._InitCount;
			}
		}

		// Token: 0x06003DFA RID: 15866 RVA: 0x0034D578 File Offset: 0x0034B778
		public void ChangeIpPort(string ip, int port)
		{
			this.RemoteIP = ip;
			this.RemotePort = port;
		}

		// Token: 0x06003DFB RID: 15867 RVA: 0x0034D58C File Offset: 0x0034B78C
		public bool Init(int count, string ip, int port, string serverName)
		{
			bool result;
			if (null != this.semaphoreClients)
			{
				LogManager.WriteLog(LogTypes.Error, "不正确的重复调用函数GameDbClientPool.Init(int count, string ip, int port, string serverName)", null, true);
				result = false;
			}
			else
			{
				this.ServerName = serverName;
				this._InitCount = count;
				this.ItemCount = count;
				this.RemoteIP = ip;
				this.RemotePort = port;
				this.semaphoreClients = new Semaphore(0, count);
				for (int i = 0; i < count; i++)
				{
					TCPClient tcpClient = new TCPClient
					{
						RootWindow = this.RootWindow,
						ListIndex = this.ItemCount,
						NoDelay = false
					};
					this.ErrorClientStack.Push(tcpClient);
					this.ErrCount++;
					try
					{
						this.RootWindow.AddDBConnectInfo(this.ItemCount, string.Format("{0}, 准备连接到{1}: {2}{3}", new object[]
						{
							this.ItemCount,
							this.ServerName,
							this.RemoteIP,
							this.RemotePort
						}));
					}
					catch (Exception ex)
					{
					}
				}
				result = this.Supply();
			}
			return result;
		}

		// Token: 0x06003DFC RID: 15868 RVA: 0x0034D6D0 File Offset: 0x0034B8D0
		public void Clear()
		{
			try
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
			catch
			{
			}
		}

		// Token: 0x06003DFD RID: 15869 RVA: 0x0034D76C File Offset: 0x0034B96C
		public int GetPoolCount()
		{
			int count;
			lock (this.pool)
			{
				count = this.pool.Count;
			}
			return count;
		}

		// Token: 0x06003DFE RID: 15870 RVA: 0x0034D7C0 File Offset: 0x0034B9C0
		public bool Supply()
		{
			lock (this.pool)
			{
				if (this.ErrCount <= 0)
				{
					return true;
				}
				DateTime now = TimeUtil.NowDateTime();
				if ((now - this.LastConnectErrorTime).TotalSeconds < 10.0)
				{
					return false;
				}
				if (this.ErrCount > 0)
				{
					while (this.ErrorClientStack.Count > 0)
					{
						TCPClient tcpClient = this.ErrorClientStack.Pop();
						try
						{
							tcpClient.Connect(this.RemoteIP, this.RemotePort, this.ServerName);
							this.pool.Enqueue(tcpClient);
							this.ErrCount--;
							this.semaphoreClients.Release();
						}
						catch (Exception)
						{
							this.LastConnectErrorTime = now;
							this.ErrorClientStack.Push(tcpClient);
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06003DFF RID: 15871 RVA: 0x0034D920 File Offset: 0x0034BB20
		public TCPClient Pop()
		{
			TCPClient tcpClient = null;
			lock (this.pool)
			{
				if (this.ErrCount >= this._InitCount)
				{
					if (!this.Supply())
					{
						return null;
					}
				}
			}
			if (this.semaphoreClients.WaitOne(20000))
			{
				lock (this.pool)
				{
					tcpClient = this.pool.Dequeue();
					if (!tcpClient.ValidateIpPort(this.RemoteIP, this.RemotePort))
					{
						try
						{
							tcpClient.Disconnect();
							tcpClient.Connect(this.RemoteIP, this.RemotePort, tcpClient.ServerName);
						}
						catch (Exception ex)
						{
							this.ErrCount++;
							this.ErrorClientStack.Push(tcpClient);
							this.LastConnectErrorTime = TimeUtil.NowDateTime();
							LogManager.WriteExceptionUseCache(ex.ToString());
						}
					}
				}
			}
			return tcpClient;
		}

		// Token: 0x06003E00 RID: 15872 RVA: 0x0034DA88 File Offset: 0x0034BC88
		public void Push(TCPClient tcpClient)
		{
			if (!tcpClient.IsConnected())
			{
				lock (this.pool)
				{
					this.ErrCount++;
					this.ErrorClientStack.Push(tcpClient);
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

		// Token: 0x040047FE RID: 18430
		public Dictionary<int, string> DBServerConnectDict = new Dictionary<int, string>();

		// Token: 0x040047FF RID: 18431
		private int _InitCount = 0;

		// Token: 0x04004800 RID: 18432
		public int ErrCount = 0;

		// Token: 0x04004801 RID: 18433
		private int ItemCount = 0;

		// Token: 0x04004802 RID: 18434
		public string RemoteIP = "";

		// Token: 0x04004803 RID: 18435
		public int RemotePort = 0;

		// Token: 0x04004804 RID: 18436
		private Queue<TCPClient> pool;

		// Token: 0x04004805 RID: 18437
		private Semaphore semaphoreClients;

		// Token: 0x04004806 RID: 18438
		public IConnectInfoContainer RootWindow;

		// Token: 0x04004807 RID: 18439
		private string ServerName = "";

		// Token: 0x04004808 RID: 18440
		public DateTime LastConnectErrorTime;

		// Token: 0x04004809 RID: 18441
		private Stack<TCPClient> ErrorClientStack = new Stack<TCPClient>();
	}
}
