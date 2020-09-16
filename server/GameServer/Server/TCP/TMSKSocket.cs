using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using GameServer.Logic;
using GameServer.Server;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;

namespace Server.TCP
{
	
	public class TMSKSocket : IDisposable
	{
		
		
		
		public int Nid
		{
			get
			{
				return this.nNid;
			}
			set
			{
				this.nNid = value;
			}
		}

		
		
		
		public string deviceID { get; set; }

		
		public void SetAcceptIp()
		{
			if (null != this.m_Socket)
			{
				try
				{
					byte[] bytes = ((IPEndPoint)this.m_Socket.RemoteEndPoint).Address.GetAddressBytes();
					this._AcceptIpAsInt = (long)(((ulong)bytes[0] << 24) + ((ulong)bytes[1] << 16) + ((ulong)bytes[2] << 8) + (ulong)bytes[3]);
				}
				catch
				{
				}
			}
		}

		
		public void SetAcceptIp(long v, int port)
		{
			this._AcceptIpAsInt = v;
			v = (long)((ulong)IPAddress.HostToNetworkOrder((int)v));
			this._VirtualEndPoint = new IPEndPoint(v, port);
		}

		
		
		public long AcceptIpAsInt
		{
			get
			{
				return this._AcceptIpAsInt;
			}
		}

		
		~TMSKSocket()
		{
			this.MyDispose();
		}

		
		public void MyDispose()
		{
			try
			{
				if (null != this.GlobalWritePool)
				{
					lock (this.SocketAsyncEventArgsWritePool)
					{
						this.IsDisposed = true;
						while (this.SocketAsyncEventArgsWritePool.Count > 0)
						{
							this.GlobalWritePool.Push(this.SocketAsyncEventArgsWritePool.Pop());
						}
					}
				}
				if (null != this.GlobalReadPool)
				{
					lock (this.SocketAsyncEventArgsReadPool)
					{
						this.IsDisposed = true;
						while (this.SocketAsyncEventArgsReadPool.Count > 0)
						{
							this.GlobalReadPool.Push(this.SocketAsyncEventArgsReadPool.Pop());
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "~TMSKSocket()异常:");
			}
		}

		
		public void PushWriteSocketAsyncEventArgs(SocketAsyncEventArgs item)
		{
			try
			{
				lock (this.SocketAsyncEventArgsWritePool)
				{
					if (!this.IsDisposed && this.SocketAsyncEventArgsWritePool.Count <= this.maxStackSocketAsyncEventArgsWrite)
					{
						this.SocketAsyncEventArgsWritePool.Push(item);
						return;
					}
				}
				if (null == this.GlobalWritePool)
				{
					this.GlobalWritePool = Global._TCPManager.MySocketListener.writePool;
				}
				this.GlobalWritePool.Push(item);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "");
			}
		}

		
		public SocketAsyncEventArgs PopWriteSocketAsyncEventArgs()
		{
			try
			{
				lock (this.SocketAsyncEventArgsWritePool)
				{
					if (this.SocketAsyncEventArgsWritePool.Count > 0)
					{
						return this.SocketAsyncEventArgsWritePool.Pop();
					}
				}
				if (null == this.GlobalWritePool)
				{
					this.GlobalWritePool = Global._TCPManager.MySocketListener.writePool;
				}
				return this.GlobalWritePool.Pop();
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "");
			}
			return null;
		}

		
		public void PushReadSocketAsyncEventArgs(SocketAsyncEventArgs item)
		{
			try
			{
				lock (this.SocketAsyncEventArgsReadPool)
				{
					if (!this.IsDisposed && this.SocketAsyncEventArgsReadPool.Count <= this.maxStackSocketAsyncEventArgsRead)
					{
						this.SocketAsyncEventArgsReadPool.Push(item);
						return;
					}
				}
				if (null == this.GlobalReadPool)
				{
					this.GlobalReadPool = Global._TCPManager.MySocketListener.readPool;
				}
				this.GlobalReadPool.Push(item);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "");
			}
		}

		
		public SocketAsyncEventArgs PopReadSocketAsyncEventArgs()
		{
			try
			{
				lock (this.SocketAsyncEventArgsReadPool)
				{
					if (this.SocketAsyncEventArgsReadPool.Count > 0)
					{
						return this.SocketAsyncEventArgsReadPool.Pop();
					}
				}
				if (null == this.GlobalReadPool)
				{
					this.GlobalReadPool = Global._TCPManager.MySocketListener.readPool;
				}
				return this.GlobalReadPool.Pop();
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "");
			}
			return null;
		}

		
		public TMSKSocket(Socket socket)
		{
			this.m_Socket = socket;
		}

		
		public TMSKSocket(SocketInformation socketInformation)
		{
			this.m_Socket = new Socket(socketInformation);
		}

		
		public TMSKSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
		{
			this.m_Socket = new Socket(addressFamily, socketType, protocolType);
		}

		
		
		public AddressFamily AddressFamily
		{
			get
			{
				return this.m_Socket.AddressFamily;
			}
		}

		
		
		public int Available
		{
			get
			{
				return this.m_Socket.Available;
			}
		}

		
		
		
		public bool Blocking
		{
			get
			{
				return this.m_Socket.Blocking;
			}
			set
			{
				this.m_Socket.Blocking = value;
			}
		}

		
		
		public bool Connected
		{
			get
			{
				return this.m_Socket.Connected;
			}
		}

		
		
		
		public bool DontFragment
		{
			get
			{
				return this.m_Socket.DontFragment;
			}
			set
			{
				this.m_Socket.DontFragment = value;
			}
		}

		
		
		
		public bool EnableBroadcast
		{
			get
			{
				return this.m_Socket.EnableBroadcast;
			}
			set
			{
				this.m_Socket.EnableBroadcast = value;
			}
		}

		
		
		
		public bool ExclusiveAddressUse
		{
			get
			{
				return this.m_Socket.ExclusiveAddressUse;
			}
			set
			{
				this.m_Socket.ExclusiveAddressUse = value;
			}
		}

		
		
		public IntPtr Handle
		{
			get
			{
				return this.m_Socket.Handle;
			}
		}

		
		
		public bool IsBound
		{
			get
			{
				return this.m_Socket.IsBound;
			}
		}

		
		
		
		public LingerOption LingerState
		{
			get
			{
				return this.m_Socket.LingerState;
			}
			set
			{
				this.m_Socket.LingerState = value;
			}
		}

		
		
		public EndPoint LocalEndPoint
		{
			get
			{
				return this.m_Socket.LocalEndPoint;
			}
		}

		
		
		
		public bool MulticastLoopback
		{
			get
			{
				return this.m_Socket.MulticastLoopback;
			}
			set
			{
				this.m_Socket.MulticastLoopback = value;
			}
		}

		
		
		
		public bool NoDelay
		{
			get
			{
				return this.m_Socket.NoDelay;
			}
			set
			{
				this.m_Socket.NoDelay = value;
			}
		}

		
		
		public static bool OSSupportsIPv4
		{
			get
			{
				return Socket.OSSupportsIPv4;
			}
		}

		
		
		public static bool OSSupportsIPv6
		{
			get
			{
				return Socket.OSSupportsIPv6;
			}
		}

		
		
		public ProtocolType ProtocolType
		{
			get
			{
				return this.m_Socket.ProtocolType;
			}
		}

		
		
		
		public int ReceiveBufferSize
		{
			get
			{
				return this.m_Socket.ReceiveBufferSize;
			}
			set
			{
				this.m_Socket.ReceiveBufferSize = value;
			}
		}

		
		
		
		public int ReceiveTimeout
		{
			get
			{
				return this.m_Socket.ReceiveTimeout;
			}
			set
			{
				this.m_Socket.ReceiveTimeout = value;
			}
		}

		
		
		public EndPoint RemoteEndPoint
		{
			get
			{
				EndPoint result;
				if (null == this._VirtualEndPoint)
				{
					result = this.m_Socket.RemoteEndPoint;
				}
				else
				{
					result = this._VirtualEndPoint;
				}
				return result;
			}
		}

		
		
		
		public int SendBufferSize
		{
			get
			{
				return this.m_Socket.SendBufferSize;
			}
			set
			{
				this.m_Socket.SendBufferSize = value;
			}
		}

		
		
		
		public int SendTimeout
		{
			get
			{
				return this.m_Socket.SendTimeout;
			}
			set
			{
				this.m_Socket.SendTimeout = value;
			}
		}

		
		
		public SocketType SocketType
		{
			get
			{
				return this.m_Socket.SocketType;
			}
		}

		
		
		[Obsolete("SupportsIPv4 is obsoleted for this type, please use OSSupportsIPv4 instead. http://go.microsoft.com/fwlink/?linkid=14202")]
		public static bool SupportsIPv4
		{
			get
			{
				return Socket.SupportsIPv4;
			}
		}

		
		
		[Obsolete("SupportsIPv6 is obsoleted for this type, please use OSSupportsIPv6 instead. http://go.microsoft.com/fwlink/?linkid=14202")]
		public static bool SupportsIPv6
		{
			get
			{
				return Socket.SupportsIPv6;
			}
		}

		
		
		
		public short Ttl
		{
			get
			{
				return this.m_Socket.Ttl;
			}
			set
			{
				this.m_Socket.Ttl = value;
			}
		}

		
		
		
		public bool UseOnlyOverlappedIO
		{
			get
			{
				return this.m_Socket.UseOnlyOverlappedIO;
			}
			set
			{
				this.m_Socket.UseOnlyOverlappedIO = value;
			}
		}

		
		public Socket Accept()
		{
			return this.m_Socket.Accept();
		}

		
		public bool AcceptAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.AcceptAsync(e);
		}

		
		public IAsyncResult BeginAccept(AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginAccept(callback, state);
		}

		
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public IAsyncResult BeginAccept(int receiveSize, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginAccept(receiveSize, callback, state);
		}

		
		public IAsyncResult BeginAccept(Socket acceptSocket, int receiveSize, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginAccept(acceptSocket, receiveSize, callback, state);
		}

		
		public IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginConnect(remoteEP, callback, state);
		}

		
		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
		{
			return this.m_Socket.BeginConnect(address, port, requestCallback, state);
		}

		
		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state)
		{
			return this.m_Socket.BeginConnect(addresses, port, requestCallback, state);
		}

		
		public IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
		{
			return this.m_Socket.BeginConnect(host, port, requestCallback, state);
		}

		
		public IAsyncResult BeginDisconnect(bool reuseSocket, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginDisconnect(reuseSocket, callback, state);
		}

		
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceive(buffers, socketFlags, callback, state);
		}

		
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceive(buffers, socketFlags, out errorCode, callback, state);
		}

		
		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceive(buffer, offset, size, socketFlags, callback, state);
		}

		
		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceive(buffer, offset, size, socketFlags, out errorCode, callback, state);
		}

		
		public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceiveFrom(buffer, offset, size, socketFlags, ref remoteEP, callback, state);
		}

		
		public IAsyncResult BeginReceiveMessageFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceiveMessageFrom(buffer, offset, size, socketFlags, ref remoteEP, callback, state);
		}

		
		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSend(buffers, socketFlags, callback, state);
		}

		
		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSend(buffers, socketFlags, out errorCode, callback, state);
		}

		
		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSend(buffer, offset, size, socketFlags, callback, state);
		}

		
		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSend(buffer, offset, size, socketFlags, out errorCode, callback, state);
		}

		
		public IAsyncResult BeginSendFile(string fileName, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSendFile(fileName, callback, state);
		}

		
		public IAsyncResult BeginSendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSendFile(fileName, preBuffer, postBuffer, flags, callback, state);
		}

		
		public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSendTo(buffer, offset, size, socketFlags, remoteEP, callback, state);
		}

		
		public void Bind(EndPoint localEP)
		{
			this.m_Socket.Bind(localEP);
		}

		
		public static void CancelConnectAsync(SocketAsyncEventArgs e)
		{
			Socket.CancelConnectAsync(e);
		}

		
		public void Close()
		{
			this.m_Socket.Close();
		}

		
		public void Close(int timeout)
		{
			this.m_Socket.Close(timeout);
		}

		
		public void Connect(EndPoint remoteEP)
		{
			this.m_Socket.Connect(remoteEP);
		}

		
		public void Connect(IPAddress address, int port)
		{
			this.m_Socket.Connect(address, port);
		}

		
		public void Connect(IPAddress[] addresses, int port)
		{
			this.m_Socket.Connect(addresses, port);
		}

		
		public void Connect(string host, int port)
		{
			this.m_Socket.Connect(host, port);
		}

		
		public bool ConnectAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.ConnectAsync(e);
		}

		
		public static bool ConnectAsync(SocketType socketType, ProtocolType protocolType, SocketAsyncEventArgs e)
		{
			return Socket.ConnectAsync(socketType, protocolType, e);
		}

		
		public void Disconnect(bool reuseSocket)
		{
			this.m_Socket.Disconnect(reuseSocket);
		}

		
		public bool DisconnectAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.DisconnectAsync(e);
		}

		
		public void Dispose()
		{
			this.MyDispose();
			this.m_Socket.Dispose();
		}

		
		protected virtual void Dispose(bool disposing)
		{
		}

		
		public SocketInformation DuplicateAndClose(int targetProcessId)
		{
			return this.m_Socket.DuplicateAndClose(targetProcessId);
		}

		
		public Socket EndAccept(IAsyncResult asyncResult)
		{
			return this.m_Socket.EndAccept(asyncResult);
		}

		
		public Socket EndAccept(out byte[] buffer, IAsyncResult asyncResult)
		{
			return this.m_Socket.EndAccept(out buffer, asyncResult);
		}

		
		public Socket EndAccept(out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult)
		{
			return this.m_Socket.EndAccept(out buffer, out bytesTransferred, asyncResult);
		}

		
		public void EndConnect(IAsyncResult asyncResult)
		{
			this.m_Socket.EndConnect(asyncResult);
		}

		
		public void EndDisconnect(IAsyncResult asyncResult)
		{
			this.m_Socket.EndDisconnect(asyncResult);
		}

		
		public int EndReceive(IAsyncResult asyncResult)
		{
			return this.m_Socket.EndReceive(asyncResult);
		}

		
		public int EndReceive(IAsyncResult asyncResult, out SocketError errorCode)
		{
			return this.m_Socket.EndReceive(asyncResult, out errorCode);
		}

		
		public int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint endPoint)
		{
			return this.m_Socket.EndReceiveFrom(asyncResult, ref endPoint);
		}

		
		public int EndReceiveMessageFrom(IAsyncResult asyncResult, ref SocketFlags socketFlags, ref EndPoint endPoint, out IPPacketInformation ipPacketInformation)
		{
			return this.m_Socket.EndReceiveMessageFrom(asyncResult, ref socketFlags, ref endPoint, out ipPacketInformation);
		}

		
		public int EndSend(IAsyncResult asyncResult)
		{
			return this.m_Socket.EndSend(asyncResult);
		}

		
		public int EndSend(IAsyncResult asyncResult, out SocketError errorCode)
		{
			return this.m_Socket.EndSend(asyncResult, out errorCode);
		}

		
		public void EndSendFile(IAsyncResult asyncResult)
		{
			this.m_Socket.EndSendFile(asyncResult);
		}

		
		public int EndSendTo(IAsyncResult asyncResult)
		{
			return this.m_Socket.EndSendTo(asyncResult);
		}

		
		public object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName)
		{
			return this.m_Socket.GetSocketOption(optionLevel, optionName);
		}

		
		public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			this.m_Socket.GetSocketOption(optionLevel, optionName, optionValue);
		}

		
		public byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionLength)
		{
			return this.m_Socket.GetSocketOption(optionLevel, optionName, optionLength);
		}

		
		public int IOControl(int ioControlCode, byte[] optionInValue, byte[] optionOutValue)
		{
			return this.m_Socket.IOControl(ioControlCode, optionInValue, optionOutValue);
		}

		
		public int IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue)
		{
			return this.m_Socket.IOControl(ioControlCode, optionInValue, optionOutValue);
		}

		
		public void Listen(int backlog)
		{
			this.m_Socket.Listen(backlog);
		}

		
		public bool Poll(int microSeconds, SelectMode mode)
		{
			return this.m_Socket.Poll(microSeconds, mode);
		}

		
		public int Receive(byte[] buffer)
		{
			return this.m_Socket.Receive(buffer);
		}

		
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int Receive(IList<ArraySegment<byte>> buffers)
		{
			return this.m_Socket.Receive(buffers);
		}

		
		public int Receive(byte[] buffer, SocketFlags socketFlags)
		{
			return this.m_Socket.Receive(buffer, socketFlags);
		}

		
		public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			return this.m_Socket.Receive(buffers, socketFlags);
		}

		
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int Receive(byte[] buffer, int size, SocketFlags socketFlags)
		{
			return this.m_Socket.Receive(buffer, size, socketFlags);
		}

		
		public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			return this.m_Socket.Receive(buffers, socketFlags, out errorCode);
		}

		
		public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags)
		{
			return this.m_Socket.Receive(buffer, offset, size, socketFlags);
		}

		
		public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
		{
			return this.m_Socket.Receive(buffer, offset, size, socketFlags, out errorCode);
		}

		
		public bool ReceiveAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.ReceiveAsync(e);
		}

		
		public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP)
		{
			return this.m_Socket.ReceiveFrom(buffer, ref remoteEP);
		}

		
		public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			return this.m_Socket.ReceiveFrom(buffer, socketFlags, ref remoteEP);
		}

		
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int ReceiveFrom(byte[] buffer, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			return this.m_Socket.ReceiveFrom(buffer, size, socketFlags, ref remoteEP);
		}

		
		public int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			return this.m_Socket.ReceiveFrom(buffer, offset, size, socketFlags, ref remoteEP);
		}

		
		public bool ReceiveFromAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.ReceiveFromAsync(e);
		}

		
		public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation)
		{
			return this.m_Socket.ReceiveMessageFrom(buffer, offset, size, ref socketFlags, ref remoteEP, out ipPacketInformation);
		}

		
		public bool ReceiveMessageFromAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.ReceiveMessageFromAsync(e);
		}

		
		public static void Select(IList checkRead, IList checkWrite, IList checkError, int microSeconds)
		{
			Socket.Select(checkRead, checkWrite, checkError, microSeconds);
		}

		
		public int Send(byte[] buffer)
		{
			return this.m_Socket.Send(buffer);
		}

		
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int Send(IList<ArraySegment<byte>> buffers)
		{
			return this.m_Socket.Send(buffers);
		}

		
		public int Send(byte[] buffer, SocketFlags socketFlags)
		{
			return this.m_Socket.Send(buffer, socketFlags);
		}

		
		public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			return this.m_Socket.Send(buffers, socketFlags);
		}

		
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int Send(byte[] buffer, int size, SocketFlags socketFlags)
		{
			return this.m_Socket.Send(buffer, size, socketFlags);
		}

		
		public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			return this.m_Socket.Send(buffers, socketFlags, out errorCode);
		}

		
		public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags)
		{
			return this.m_Socket.Send(buffer, offset, size, socketFlags);
		}

		
		public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
		{
			return this.m_Socket.Send(buffer, offset, size, socketFlags, out errorCode);
		}

		
		public bool SendAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.SendAsync(e);
		}

		
		public void SendFile(string fileName)
		{
			this.m_Socket.SendFile(fileName);
		}

		
		public void SendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags)
		{
			this.m_Socket.SendFile(fileName, preBuffer, postBuffer, flags);
		}

		
		public bool SendPacketsAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.SendPacketsAsync(e);
		}

		
		public int SendTo(byte[] buffer, EndPoint remoteEP)
		{
			return this.m_Socket.SendTo(buffer, remoteEP);
		}

		
		public int SendTo(byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP)
		{
			return this.m_Socket.SendTo(buffer, socketFlags, remoteEP);
		}

		
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int SendTo(byte[] buffer, int size, SocketFlags socketFlags, EndPoint remoteEP)
		{
			return this.m_Socket.SendTo(buffer, size, socketFlags, remoteEP);
		}

		
		public int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
		{
			return this.m_Socket.SendTo(buffer, offset, size, socketFlags, remoteEP);
		}

		
		public bool SendToAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.SendToAsync(e);
		}

		
		public void SetIPProtectionLevel(IPProtectionLevel level)
		{
			this.m_Socket.SetIPProtectionLevel(level);
		}

		
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
		{
			this.m_Socket.SetSocketOption(optionLevel, optionName, optionValue);
		}

		
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			this.m_Socket.SetSocketOption(optionLevel, optionName, optionValue);
		}

		
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
		{
			this.m_Socket.SetSocketOption(optionLevel, optionName, optionValue);
		}

		
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
		{
			this.m_Socket.SetSocketOption(optionLevel, optionName, optionValue);
		}

		
		public void Shutdown(SocketShutdown how)
		{
			this.m_Socket.Shutdown(how);
		}

		
		public TCPInPacket _TcpInPacket;

		
		public SendBuffer _SendBuffer;

		
		public TCPSession session;

		
		public ushort magic = 0;

		
		public IntPtr Key;

		
		public ulong SortKey64 = 0UL;

		
		public bool IsKuaFuLogin = false;

		
		public int ServerId = 0;

		
		private int nNid = -1;

		
		public KuaFuServerLoginData ClientKuaFuServerLoginData = new KuaFuServerLoginData();

		
		private bool IsDisposed = false;

		
		public string CloseReason = "";

		
		public int DelayClose;

		
		public int ClientCmdSecs;

		
		public string UserID;

		
		private long _AcceptIpAsInt = 0L;

		
		private IPEndPoint _VirtualEndPoint;

		
		private Stack<SocketAsyncEventArgs> SocketAsyncEventArgsReadPool = new Stack<SocketAsyncEventArgs>();

		
		private int maxStackSocketAsyncEventArgsRead = 3;

		
		private Stack<SocketAsyncEventArgs> SocketAsyncEventArgsWritePool = new Stack<SocketAsyncEventArgs>();

		
		private int maxStackSocketAsyncEventArgsWrite = 3;

		
		public SocketAsyncEventArgsPool GlobalWritePool;

		
		public SocketAsyncEventArgsPool GlobalReadPool;

		
		public long SendCount;

		
		public Socket m_Socket = null;
	}
}
