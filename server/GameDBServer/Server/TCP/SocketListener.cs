using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server.Protocol;
using Server.Tools;

namespace Server.TCP
{
	
	public sealed class SocketListener
	{
		
		
		public int ConnectedSocketsCount
		{
			get
			{
				int i = 0;
				Interlocked.Exchange(ref i, this.numConnectedSockets);
				return i;
			}
		}

		
		
		public int TotalBytesReadSize
		{
			get
			{
				int i = 0;
				Interlocked.Exchange(ref i, this.totalBytesRead);
				return i;
			}
		}

		
		
		public int TotalBytesWriteSize
		{
			get
			{
				int i = 0;
				Interlocked.Exchange(ref i, this.totalBytesWrite);
				return i;
			}
		}

		
		// (add) Token: 0x06000C51 RID: 3153 RVA: 0x0009F170 File Offset: 0x0009D370
		// (remove) Token: 0x06000C52 RID: 3154 RVA: 0x0009F1AC File Offset: 0x0009D3AC
		public event SocketConnectedEventHandler SocketConnected = null;

		
		// (add) Token: 0x06000C53 RID: 3155 RVA: 0x0009F1E8 File Offset: 0x0009D3E8
		// (remove) Token: 0x06000C54 RID: 3156 RVA: 0x0009F224 File Offset: 0x0009D424
		public event SocketClosedEventHandler SocketClosed = null;

		
		// (add) Token: 0x06000C55 RID: 3157 RVA: 0x0009F260 File Offset: 0x0009D460
		// (remove) Token: 0x06000C56 RID: 3158 RVA: 0x0009F29C File Offset: 0x0009D49C
		public event SocketReceivedEventHandler SocketReceived = null;

		
		// (add) Token: 0x06000C57 RID: 3159 RVA: 0x0009F2D8 File Offset: 0x0009D4D8
		// (remove) Token: 0x06000C58 RID: 3160 RVA: 0x0009F314 File Offset: 0x0009D514
		public event SocketSendedEventHandler SocketSended = null;

		
		internal SocketListener(int numConnections, int receiveBufferSize)
		{
			this.totalBytesRead = 0;
			this.totalBytesWrite = 0;
			this.numConnectedSockets = 0;
			this.numConnections = numConnections;
			this.ReceiveBufferSize = receiveBufferSize;
			this.bufferManager = new BufferManager(receiveBufferSize * numConnections, receiveBufferSize);
			this.ConnectedSocketsDict = new Dictionary<Socket, bool>(numConnections);
			this.readPool = new SocketAsyncEventArgsPool(numConnections);
			this.writePool = new SocketAsyncEventArgsPool(numConnections * 5);
			this.semaphoreAcceptedClients = new Semaphore(numConnections, numConnections);
		}

		
		private void AddSocket(Socket socket)
		{
			lock (this.ConnectedSocketsDict)
			{
				this.ConnectedSocketsDict.Add(socket, true);
			}
		}

		
		private void RemoveSocket(Socket socket)
		{
			lock (this.ConnectedSocketsDict)
			{
				this.ConnectedSocketsDict.Remove(socket);
			}
		}

		
		private bool FindSocket(Socket socket)
		{
			bool ret = false;
			lock (this.ConnectedSocketsDict)
			{
				ret = this.ConnectedSocketsDict.ContainsKey(socket);
			}
			return ret;
		}

		
		private void CloseClientSocket(SocketAsyncEventArgs e)
		{
			AsyncUserToken aut = e.UserToken as AsyncUserToken;
			try
			{
				Socket s = aut.CurrentSocket;
				if (this.FindSocket(s))
				{
					this.RemoveSocket(s);
					try
					{
						LogManager.WriteLog(LogTypes.Info, string.Format("关闭客户端连接: {0}, 操作: {1}, 原因: {2}", s.RemoteEndPoint, e.LastOperation, e.SocketError), null, true);
					}
					catch (Exception)
					{
					}
					this.semaphoreAcceptedClients.Release();
					Interlocked.Decrement(ref this.numConnectedSockets);
					if (null != this.SocketClosed)
					{
						this.SocketClosed(this, e);
					}
					try
					{
						s.Shutdown(SocketShutdown.Both);
					}
					catch (Exception)
					{
					}
					try
					{
						s.Close();
					}
					catch (Exception)
					{
					}
				}
			}
			finally
			{
				aut.CurrentSocket = null;
				aut.Tag = null;
				if (e.LastOperation == SocketAsyncOperation.Send)
				{
					e.SetBuffer(null, 0, 0);
					this.writePool.Push(e);
				}
				else if (e.LastOperation == SocketAsyncOperation.Receive)
				{
					this.readPool.Push(e);
				}
			}
		}

		
		internal void Init()
		{
			this.bufferManager.InitBuffer();
			for (int i = 0; i < this.numConnections; i++)
			{
				SocketAsyncEventArgs readWriteEventArg = new SocketAsyncEventArgs();
				readWriteEventArg.Completed += this.OnIOCompleted;
				readWriteEventArg.UserToken = new AsyncUserToken
				{
					CurrentSocket = null,
					Tag = null
				};
				this.bufferManager.SetBuffer(readWriteEventArg);
				this.readPool.Push(readWriteEventArg);
				for (int j = 0; j < 5; j++)
				{
					readWriteEventArg = new SocketAsyncEventArgs();
					readWriteEventArg.Completed += this.OnIOCompleted;
					readWriteEventArg.UserToken = new AsyncUserToken
					{
						CurrentSocket = null,
						Tag = null
					};
					this.writePool.Push(readWriteEventArg);
				}
			}
		}

		
		private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				if (null != this.listenSocket)
				{
					this.ProcessAccept(e);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::OnAcceptCompleted 中发生了异常错误", new object[0]), null, true);
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				SocketAsyncOperation lastOperation = e.LastOperation;
				if (lastOperation != SocketAsyncOperation.Receive)
				{
					if (lastOperation != SocketAsyncOperation.Send)
					{
						throw new ArgumentException("The last operation completed on the socket was not a receive or send");
					}
					this.ProcessSend(e);
				}
				else
				{
					this.ProcessReceive(e);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::OnIOCompleted 中发生了异常错误", new object[0]), null, true);
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		private bool _ReceiveAsync(SocketAsyncEventArgs readEventArgs)
		{
			bool result;
			try
			{
				Socket s = (readEventArgs.UserToken as AsyncUserToken).CurrentSocket;
				result = s.ReceiveAsync(readEventArgs);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::_ReceiveAsync 中发生了异常错误", new object[0]), null, true);
				this.CloseClientSocket(readEventArgs);
				result = true;
			}
			return result;
		}

		
		private bool _SendAsync(SocketAsyncEventArgs writeEventArgs, out bool exception)
		{
			exception = false;
			bool result;
			try
			{
				Socket s = (writeEventArgs.UserToken as AsyncUserToken).CurrentSocket;
				result = s.SendAsync(writeEventArgs);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::_SendAsync 中发生了异常错误", new object[0]), null, true);
				exception = true;
				result = true;
			}
			return result;
		}

		
		internal bool SendData(Socket s, TCPOutPacket tcpOutPacket)
		{
			SocketAsyncEventArgs writeEventArgs = this.writePool.Pop();
			if (null == writeEventArgs)
			{
				writeEventArgs = new SocketAsyncEventArgs();
				writeEventArgs.Completed += this.OnIOCompleted;
				writeEventArgs.UserToken = new AsyncUserToken
				{
					CurrentSocket = null,
					Tag = null
				};
			}
			writeEventArgs.SetBuffer(tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize);
			(writeEventArgs.UserToken as AsyncUserToken).CurrentSocket = s;
			(writeEventArgs.UserToken as AsyncUserToken).Tag = tcpOutPacket;
			bool exception = false;
			if (!this._SendAsync(writeEventArgs, out exception))
			{
				this.ProcessSend(writeEventArgs);
			}
			return !exception;
		}

		
		private void ProcessAccept(SocketAsyncEventArgs e)
		{
			SocketAsyncEventArgs readEventArgs = null;
			Interlocked.Increment(ref this.numConnectedSockets);
			Socket s = e.AcceptSocket;
			readEventArgs = this.readPool.Pop();
			if (null == readEventArgs)
			{
				try
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("新远程连接: {0}, 但是readPool内的缓存不足，直接关闭连接:{1}", s.RemoteEndPoint, this.ConnectedSocketsCount), null, true);
				}
				catch (Exception)
				{
				}
				try
				{
					s.Shutdown(SocketShutdown.Both);
				}
				catch (Exception)
				{
				}
				try
				{
					s.Close();
				}
				catch (Exception)
				{
				}
				Interlocked.Decrement(ref this.numConnectedSockets);
				this.StartAccept(e);
			}
			else
			{
				(readEventArgs.UserToken as AsyncUserToken).CurrentSocket = e.AcceptSocket;
				byte[] inOptionValues = new byte[12];
				BitConverter.GetBytes(1U).CopyTo(inOptionValues, 0);
				BitConverter.GetBytes(120000U).CopyTo(inOptionValues, 4);
				BitConverter.GetBytes(5000U).CopyTo(inOptionValues, 8);
				s.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
				this.AddSocket(s);
				try
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("新远程连接: {0}, 当前总共: {1}", s.RemoteEndPoint, this.numConnectedSockets), null, true);
				}
				catch (Exception)
				{
				}
				if (null != this.SocketConnected)
				{
					this.SocketConnected(this, readEventArgs);
				}
				if (!this._ReceiveAsync(readEventArgs))
				{
					this.ProcessReceive(readEventArgs);
				}
				this.StartAccept(e);
			}
		}

		
		private void ProcessReceive(SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				Interlocked.Add(ref this.totalBytesRead, e.BytesTransferred);
				bool recvReturn = true;
				if (null != this.SocketReceived)
				{
					try
					{
						recvReturn = this.SocketReceived(this, e);
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
						recvReturn = false;
					}
				}
				if (recvReturn)
				{
					if (!this._ReceiveAsync(e))
					{
						this.ProcessReceive(e);
					}
				}
				else
				{
					this.CloseClientSocket(e);
				}
			}
			else
			{
				this.CloseClientSocket(e);
			}
		}

		
		private void ProcessSend(SocketAsyncEventArgs e)
		{
			if (null != this.SocketSended)
			{
				this.SocketSended(this, e);
			}
			if (e.SocketError == SocketError.Success)
			{
				Interlocked.Add(ref this.totalBytesWrite, e.BytesTransferred);
			}
			e.SetBuffer(null, 0, 0);
			(e.UserToken as AsyncUserToken).CurrentSocket = null;
			(e.UserToken as AsyncUserToken).Tag = null;
			this.writePool.Push(e);
		}

		
		internal void Start(string ip, int port)
		{
			if ("" == ip)
			{
				ip = "0.0.0.0";
			}
			IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
			this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.listenSocket.Bind(localEndPoint);
			this.listenSocket.Listen(100);
			this.StartAccept(null);
		}

		
		public void Stop()
		{
			Socket s = this.listenSocket;
			this.listenSocket = null;
			s.Close();
		}

		
		private void CloseSocket(Socket s)
		{
			try
			{
				s.Shutdown(SocketShutdown.Both);
			}
			catch (Exception)
			{
			}
		}

		
		private void StartAccept(SocketAsyncEventArgs acceptEventArg)
		{
			if (acceptEventArg == null)
			{
				acceptEventArg = new SocketAsyncEventArgs();
				acceptEventArg.Completed += this.OnAcceptCompleted;
			}
			else
			{
				acceptEventArg.AcceptSocket = null;
			}
			this.semaphoreAcceptedClients.WaitOne();
			if (!this.listenSocket.AcceptAsync(acceptEventArg))
			{
				this.ProcessAccept(acceptEventArg);
			}
		}

		
		private const int opsToPreAlloc = 1;

		
		private int ReceiveBufferSize;

		
		private BufferManager bufferManager;

		
		private Socket listenSocket;

		
		private int numConnectedSockets;

		
		private Dictionary<Socket, bool> ConnectedSocketsDict;

		
		private int numConnections;

		
		private SocketAsyncEventArgsPool readPool;

		
		private SocketAsyncEventArgsPool writePool;

		
		private Semaphore semaphoreAcceptedClients;

		
		private int totalBytesRead;

		
		private int totalBytesWrite;
	}
}
