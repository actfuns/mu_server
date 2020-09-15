using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server.Protocol;
using Server.Tools;

namespace Server.TCP
{
	// Token: 0x02000213 RID: 531
	public sealed class SocketListener
	{
		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000C4E RID: 3150 RVA: 0x0009F104 File Offset: 0x0009D304
		public int ConnectedSocketsCount
		{
			get
			{
				int i = 0;
				Interlocked.Exchange(ref i, this.numConnectedSockets);
				return i;
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000C4F RID: 3151 RVA: 0x0009F128 File Offset: 0x0009D328
		public int TotalBytesReadSize
		{
			get
			{
				int i = 0;
				Interlocked.Exchange(ref i, this.totalBytesRead);
				return i;
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000C50 RID: 3152 RVA: 0x0009F14C File Offset: 0x0009D34C
		public int TotalBytesWriteSize
		{
			get
			{
				int i = 0;
				Interlocked.Exchange(ref i, this.totalBytesWrite);
				return i;
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000C51 RID: 3153 RVA: 0x0009F170 File Offset: 0x0009D370
		// (remove) Token: 0x06000C52 RID: 3154 RVA: 0x0009F1AC File Offset: 0x0009D3AC
		public event SocketConnectedEventHandler SocketConnected = null;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000C53 RID: 3155 RVA: 0x0009F1E8 File Offset: 0x0009D3E8
		// (remove) Token: 0x06000C54 RID: 3156 RVA: 0x0009F224 File Offset: 0x0009D424
		public event SocketClosedEventHandler SocketClosed = null;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000C55 RID: 3157 RVA: 0x0009F260 File Offset: 0x0009D460
		// (remove) Token: 0x06000C56 RID: 3158 RVA: 0x0009F29C File Offset: 0x0009D49C
		public event SocketReceivedEventHandler SocketReceived = null;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000C57 RID: 3159 RVA: 0x0009F2D8 File Offset: 0x0009D4D8
		// (remove) Token: 0x06000C58 RID: 3160 RVA: 0x0009F314 File Offset: 0x0009D514
		public event SocketSendedEventHandler SocketSended = null;

		// Token: 0x06000C59 RID: 3161 RVA: 0x0009F350 File Offset: 0x0009D550
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

		// Token: 0x06000C5A RID: 3162 RVA: 0x0009F3E8 File Offset: 0x0009D5E8
		private void AddSocket(Socket socket)
		{
			lock (this.ConnectedSocketsDict)
			{
				this.ConnectedSocketsDict.Add(socket, true);
			}
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x0009F43C File Offset: 0x0009D63C
		private void RemoveSocket(Socket socket)
		{
			lock (this.ConnectedSocketsDict)
			{
				this.ConnectedSocketsDict.Remove(socket);
			}
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x0009F490 File Offset: 0x0009D690
		private bool FindSocket(Socket socket)
		{
			bool ret = false;
			lock (this.ConnectedSocketsDict)
			{
				ret = this.ConnectedSocketsDict.ContainsKey(socket);
			}
			return ret;
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x0009F4EC File Offset: 0x0009D6EC
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

		// Token: 0x06000C5E RID: 3166 RVA: 0x0009F650 File Offset: 0x0009D850
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

		// Token: 0x06000C5F RID: 3167 RVA: 0x0009F738 File Offset: 0x0009D938
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

		// Token: 0x06000C60 RID: 3168 RVA: 0x0009F7A4 File Offset: 0x0009D9A4
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

		// Token: 0x06000C61 RID: 3169 RVA: 0x0009F824 File Offset: 0x0009DA24
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

		// Token: 0x06000C62 RID: 3170 RVA: 0x0009F888 File Offset: 0x0009DA88
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

		// Token: 0x06000C63 RID: 3171 RVA: 0x0009F8EC File Offset: 0x0009DAEC
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

		// Token: 0x06000C64 RID: 3172 RVA: 0x0009F9AC File Offset: 0x0009DBAC
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

		// Token: 0x06000C65 RID: 3173 RVA: 0x0009FB60 File Offset: 0x0009DD60
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

		// Token: 0x06000C66 RID: 3174 RVA: 0x0009FC20 File Offset: 0x0009DE20
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

		// Token: 0x06000C67 RID: 3175 RVA: 0x0009FCB0 File Offset: 0x0009DEB0
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

		// Token: 0x06000C68 RID: 3176 RVA: 0x0009FD18 File Offset: 0x0009DF18
		public void Stop()
		{
			Socket s = this.listenSocket;
			this.listenSocket = null;
			s.Close();
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x0009FD3C File Offset: 0x0009DF3C
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

		// Token: 0x06000C6A RID: 3178 RVA: 0x0009FD6C File Offset: 0x0009DF6C
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

		// Token: 0x04001218 RID: 4632
		private const int opsToPreAlloc = 1;

		// Token: 0x04001219 RID: 4633
		private int ReceiveBufferSize;

		// Token: 0x0400121A RID: 4634
		private BufferManager bufferManager;

		// Token: 0x0400121B RID: 4635
		private Socket listenSocket;

		// Token: 0x0400121C RID: 4636
		private int numConnectedSockets;

		// Token: 0x0400121D RID: 4637
		private Dictionary<Socket, bool> ConnectedSocketsDict;

		// Token: 0x0400121E RID: 4638
		private int numConnections;

		// Token: 0x0400121F RID: 4639
		private SocketAsyncEventArgsPool readPool;

		// Token: 0x04001220 RID: 4640
		private SocketAsyncEventArgsPool writePool;

		// Token: 0x04001221 RID: 4641
		private Semaphore semaphoreAcceptedClients;

		// Token: 0x04001222 RID: 4642
		private int totalBytesRead;

		// Token: 0x04001223 RID: 4643
		private int totalBytesWrite;
	}
}
