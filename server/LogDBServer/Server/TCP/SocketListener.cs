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

		
		// (add) Token: 0x06000125 RID: 293 RVA: 0x00007050 File Offset: 0x00005250
		// (remove) Token: 0x06000126 RID: 294 RVA: 0x0000708C File Offset: 0x0000528C
		public event SocketConnectedEventHandler SocketConnected = null;

		
		// (add) Token: 0x06000127 RID: 295 RVA: 0x000070C8 File Offset: 0x000052C8
		// (remove) Token: 0x06000128 RID: 296 RVA: 0x00007104 File Offset: 0x00005304
		public event SocketClosedEventHandler SocketClosed = null;

		
		// (add) Token: 0x06000129 RID: 297 RVA: 0x00007140 File Offset: 0x00005340
		// (remove) Token: 0x0600012A RID: 298 RVA: 0x0000717C File Offset: 0x0000537C
		public event SocketReceivedEventHandler SocketReceived = null;

		
		// (add) Token: 0x0600012B RID: 299 RVA: 0x000071B8 File Offset: 0x000053B8
		// (remove) Token: 0x0600012C RID: 300 RVA: 0x000071F4 File Offset: 0x000053F4
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
						LogManager.WriteLog(LogTypes.Info, string.Format("关闭客户端连接: {0}, 操作: {1}, 原因: {2}", s.RemoteEndPoint, e.LastOperation, e.SocketError));
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
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::OnAcceptCompleted 中发生了异常错误", new object[0]));
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
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::OnIOCompleted 中发生了异常错误", new object[0]));
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
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::_ReceiveAsync 中发生了异常错误", new object[0]));
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
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::_SendAsync 中发生了异常错误", new object[0]));
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
					LogManager.WriteLog(LogTypes.Error, string.Format("新远程连接: {0}, 但是readPool内的缓存不足，直接关闭连接:{1}", s.RemoteEndPoint, this.ConnectedSocketsCount));
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
					LogManager.WriteLog(LogTypes.Info, string.Format("新远程连接: {0}, 当前总共: {1}", s.RemoteEndPoint, this.numConnectedSockets));
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
					recvReturn = this.SocketReceived(this, e);
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
