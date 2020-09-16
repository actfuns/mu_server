using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Server
{
	
	public class GameDbClientPool : IConnectInfoContainer
	{
		
		public GameDbClientPool()
		{
			this.RootWindow = this;
			this.pool = new Queue<TCPClient>();
		}

		
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

		
		public void ChangeIpPort(string ip, int port)
		{
			this.RemoteIP = ip;
			this.RemotePort = port;
		}

		
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

		
		public int GetPoolCount()
		{
			int count;
			lock (this.pool)
			{
				count = this.pool.Count;
			}
			return count;
		}

		
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

		
		public Dictionary<int, string> DBServerConnectDict = new Dictionary<int, string>();

		
		private int _InitCount = 0;

		
		public int ErrCount = 0;

		
		private int ItemCount = 0;

		
		public string RemoteIP = "";

		
		public int RemotePort = 0;

		
		private Queue<TCPClient> pool;

		
		private Semaphore semaphoreClients;

		
		public IConnectInfoContainer RootWindow;

		
		private string ServerName = "";

		
		public DateTime LastConnectErrorTime;

		
		private Stack<TCPClient> ErrorClientStack = new Stack<TCPClient>();
	}
}
