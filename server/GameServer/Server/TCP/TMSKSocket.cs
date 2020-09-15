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
	// Token: 0x020008D2 RID: 2258
	public class TMSKSocket : IDisposable
	{
		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x0600407E RID: 16510 RVA: 0x003BD6B0 File Offset: 0x003BB8B0
		// (set) Token: 0x0600407F RID: 16511 RVA: 0x003BD6C8 File Offset: 0x003BB8C8
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

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x06004080 RID: 16512 RVA: 0x003BD6D4 File Offset: 0x003BB8D4
		// (set) Token: 0x06004081 RID: 16513 RVA: 0x003BD6EB File Offset: 0x003BB8EB
		public string deviceID { get; set; }

		// Token: 0x06004082 RID: 16514 RVA: 0x003BD6F4 File Offset: 0x003BB8F4
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

		// Token: 0x06004083 RID: 16515 RVA: 0x003BD768 File Offset: 0x003BB968
		public void SetAcceptIp(long v, int port)
		{
			this._AcceptIpAsInt = v;
			v = (long)((ulong)IPAddress.HostToNetworkOrder((int)v));
			this._VirtualEndPoint = new IPEndPoint(v, port);
		}

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x06004084 RID: 16516 RVA: 0x003BD78C File Offset: 0x003BB98C
		public long AcceptIpAsInt
		{
			get
			{
				return this._AcceptIpAsInt;
			}
		}

		// Token: 0x06004085 RID: 16517 RVA: 0x003BD7A4 File Offset: 0x003BB9A4
		~TMSKSocket()
		{
			this.MyDispose();
		}

		// Token: 0x06004086 RID: 16518 RVA: 0x003BD7D8 File Offset: 0x003BB9D8
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

		// Token: 0x06004087 RID: 16519 RVA: 0x003BD8FC File Offset: 0x003BBAFC
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

		// Token: 0x06004088 RID: 16520 RVA: 0x003BD9CC File Offset: 0x003BBBCC
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

		// Token: 0x06004089 RID: 16521 RVA: 0x003BDA98 File Offset: 0x003BBC98
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

		// Token: 0x0600408A RID: 16522 RVA: 0x003BDB68 File Offset: 0x003BBD68
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

		// Token: 0x0600408B RID: 16523 RVA: 0x003BDC34 File Offset: 0x003BBE34
		public TMSKSocket(Socket socket)
		{
			this.m_Socket = socket;
		}

		// Token: 0x0600408C RID: 16524 RVA: 0x003BDCC8 File Offset: 0x003BBEC8
		public TMSKSocket(SocketInformation socketInformation)
		{
			this.m_Socket = new Socket(socketInformation);
		}

		// Token: 0x0600408D RID: 16525 RVA: 0x003BDD60 File Offset: 0x003BBF60
		public TMSKSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
		{
			this.m_Socket = new Socket(addressFamily, socketType, protocolType);
		}

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x0600408E RID: 16526 RVA: 0x003BDDF8 File Offset: 0x003BBFF8
		public AddressFamily AddressFamily
		{
			get
			{
				return this.m_Socket.AddressFamily;
			}
		}

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x0600408F RID: 16527 RVA: 0x003BDE18 File Offset: 0x003BC018
		public int Available
		{
			get
			{
				return this.m_Socket.Available;
			}
		}

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x06004090 RID: 16528 RVA: 0x003BDE38 File Offset: 0x003BC038
		// (set) Token: 0x06004091 RID: 16529 RVA: 0x003BDE55 File Offset: 0x003BC055
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

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x06004092 RID: 16530 RVA: 0x003BDE68 File Offset: 0x003BC068
		public bool Connected
		{
			get
			{
				return this.m_Socket.Connected;
			}
		}

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x06004093 RID: 16531 RVA: 0x003BDE88 File Offset: 0x003BC088
		// (set) Token: 0x06004094 RID: 16532 RVA: 0x003BDEA5 File Offset: 0x003BC0A5
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

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x06004095 RID: 16533 RVA: 0x003BDEB8 File Offset: 0x003BC0B8
		// (set) Token: 0x06004096 RID: 16534 RVA: 0x003BDED5 File Offset: 0x003BC0D5
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

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06004097 RID: 16535 RVA: 0x003BDEE8 File Offset: 0x003BC0E8
		// (set) Token: 0x06004098 RID: 16536 RVA: 0x003BDF05 File Offset: 0x003BC105
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

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06004099 RID: 16537 RVA: 0x003BDF18 File Offset: 0x003BC118
		public IntPtr Handle
		{
			get
			{
				return this.m_Socket.Handle;
			}
		}

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x0600409A RID: 16538 RVA: 0x003BDF38 File Offset: 0x003BC138
		public bool IsBound
		{
			get
			{
				return this.m_Socket.IsBound;
			}
		}

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x0600409B RID: 16539 RVA: 0x003BDF58 File Offset: 0x003BC158
		// (set) Token: 0x0600409C RID: 16540 RVA: 0x003BDF75 File Offset: 0x003BC175
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

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x0600409D RID: 16541 RVA: 0x003BDF88 File Offset: 0x003BC188
		public EndPoint LocalEndPoint
		{
			get
			{
				return this.m_Socket.LocalEndPoint;
			}
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x0600409E RID: 16542 RVA: 0x003BDFA8 File Offset: 0x003BC1A8
		// (set) Token: 0x0600409F RID: 16543 RVA: 0x003BDFC5 File Offset: 0x003BC1C5
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

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x060040A0 RID: 16544 RVA: 0x003BDFD8 File Offset: 0x003BC1D8
		// (set) Token: 0x060040A1 RID: 16545 RVA: 0x003BDFF5 File Offset: 0x003BC1F5
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

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x060040A2 RID: 16546 RVA: 0x003BE008 File Offset: 0x003BC208
		public static bool OSSupportsIPv4
		{
			get
			{
				return Socket.OSSupportsIPv4;
			}
		}

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x060040A3 RID: 16547 RVA: 0x003BE020 File Offset: 0x003BC220
		public static bool OSSupportsIPv6
		{
			get
			{
				return Socket.OSSupportsIPv6;
			}
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x060040A4 RID: 16548 RVA: 0x003BE038 File Offset: 0x003BC238
		public ProtocolType ProtocolType
		{
			get
			{
				return this.m_Socket.ProtocolType;
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x060040A5 RID: 16549 RVA: 0x003BE058 File Offset: 0x003BC258
		// (set) Token: 0x060040A6 RID: 16550 RVA: 0x003BE075 File Offset: 0x003BC275
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

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x060040A7 RID: 16551 RVA: 0x003BE088 File Offset: 0x003BC288
		// (set) Token: 0x060040A8 RID: 16552 RVA: 0x003BE0A5 File Offset: 0x003BC2A5
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

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x060040A9 RID: 16553 RVA: 0x003BE0B8 File Offset: 0x003BC2B8
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

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x060040AA RID: 16554 RVA: 0x003BE0F0 File Offset: 0x003BC2F0
		// (set) Token: 0x060040AB RID: 16555 RVA: 0x003BE10D File Offset: 0x003BC30D
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

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x060040AC RID: 16556 RVA: 0x003BE120 File Offset: 0x003BC320
		// (set) Token: 0x060040AD RID: 16557 RVA: 0x003BE13D File Offset: 0x003BC33D
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

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x060040AE RID: 16558 RVA: 0x003BE150 File Offset: 0x003BC350
		public SocketType SocketType
		{
			get
			{
				return this.m_Socket.SocketType;
			}
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x060040AF RID: 16559 RVA: 0x003BE170 File Offset: 0x003BC370
		[Obsolete("SupportsIPv4 is obsoleted for this type, please use OSSupportsIPv4 instead. http://go.microsoft.com/fwlink/?linkid=14202")]
		public static bool SupportsIPv4
		{
			get
			{
				return Socket.SupportsIPv4;
			}
		}

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x060040B0 RID: 16560 RVA: 0x003BE188 File Offset: 0x003BC388
		[Obsolete("SupportsIPv6 is obsoleted for this type, please use OSSupportsIPv6 instead. http://go.microsoft.com/fwlink/?linkid=14202")]
		public static bool SupportsIPv6
		{
			get
			{
				return Socket.SupportsIPv6;
			}
		}

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x060040B1 RID: 16561 RVA: 0x003BE1A0 File Offset: 0x003BC3A0
		// (set) Token: 0x060040B2 RID: 16562 RVA: 0x003BE1BD File Offset: 0x003BC3BD
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

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x060040B3 RID: 16563 RVA: 0x003BE1D0 File Offset: 0x003BC3D0
		// (set) Token: 0x060040B4 RID: 16564 RVA: 0x003BE1ED File Offset: 0x003BC3ED
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

		// Token: 0x060040B5 RID: 16565 RVA: 0x003BE200 File Offset: 0x003BC400
		public Socket Accept()
		{
			return this.m_Socket.Accept();
		}

		// Token: 0x060040B6 RID: 16566 RVA: 0x003BE220 File Offset: 0x003BC420
		public bool AcceptAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.AcceptAsync(e);
		}

		// Token: 0x060040B7 RID: 16567 RVA: 0x003BE240 File Offset: 0x003BC440
		public IAsyncResult BeginAccept(AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginAccept(callback, state);
		}

		// Token: 0x060040B8 RID: 16568 RVA: 0x003BE260 File Offset: 0x003BC460
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public IAsyncResult BeginAccept(int receiveSize, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginAccept(receiveSize, callback, state);
		}

		// Token: 0x060040B9 RID: 16569 RVA: 0x003BE280 File Offset: 0x003BC480
		public IAsyncResult BeginAccept(Socket acceptSocket, int receiveSize, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginAccept(acceptSocket, receiveSize, callback, state);
		}

		// Token: 0x060040BA RID: 16570 RVA: 0x003BE2A4 File Offset: 0x003BC4A4
		public IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginConnect(remoteEP, callback, state);
		}

		// Token: 0x060040BB RID: 16571 RVA: 0x003BE2C4 File Offset: 0x003BC4C4
		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
		{
			return this.m_Socket.BeginConnect(address, port, requestCallback, state);
		}

		// Token: 0x060040BC RID: 16572 RVA: 0x003BE2E8 File Offset: 0x003BC4E8
		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state)
		{
			return this.m_Socket.BeginConnect(addresses, port, requestCallback, state);
		}

		// Token: 0x060040BD RID: 16573 RVA: 0x003BE30C File Offset: 0x003BC50C
		public IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
		{
			return this.m_Socket.BeginConnect(host, port, requestCallback, state);
		}

		// Token: 0x060040BE RID: 16574 RVA: 0x003BE330 File Offset: 0x003BC530
		public IAsyncResult BeginDisconnect(bool reuseSocket, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginDisconnect(reuseSocket, callback, state);
		}

		// Token: 0x060040BF RID: 16575 RVA: 0x003BE350 File Offset: 0x003BC550
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceive(buffers, socketFlags, callback, state);
		}

		// Token: 0x060040C0 RID: 16576 RVA: 0x003BE374 File Offset: 0x003BC574
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceive(buffers, socketFlags, out errorCode, callback, state);
		}

		// Token: 0x060040C1 RID: 16577 RVA: 0x003BE398 File Offset: 0x003BC598
		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceive(buffer, offset, size, socketFlags, callback, state);
		}

		// Token: 0x060040C2 RID: 16578 RVA: 0x003BE3C0 File Offset: 0x003BC5C0
		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceive(buffer, offset, size, socketFlags, out errorCode, callback, state);
		}

		// Token: 0x060040C3 RID: 16579 RVA: 0x003BE3E8 File Offset: 0x003BC5E8
		public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceiveFrom(buffer, offset, size, socketFlags, ref remoteEP, callback, state);
		}

		// Token: 0x060040C4 RID: 16580 RVA: 0x003BE410 File Offset: 0x003BC610
		public IAsyncResult BeginReceiveMessageFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginReceiveMessageFrom(buffer, offset, size, socketFlags, ref remoteEP, callback, state);
		}

		// Token: 0x060040C5 RID: 16581 RVA: 0x003BE438 File Offset: 0x003BC638
		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSend(buffers, socketFlags, callback, state);
		}

		// Token: 0x060040C6 RID: 16582 RVA: 0x003BE45C File Offset: 0x003BC65C
		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSend(buffers, socketFlags, out errorCode, callback, state);
		}

		// Token: 0x060040C7 RID: 16583 RVA: 0x003BE480 File Offset: 0x003BC680
		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSend(buffer, offset, size, socketFlags, callback, state);
		}

		// Token: 0x060040C8 RID: 16584 RVA: 0x003BE4A8 File Offset: 0x003BC6A8
		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSend(buffer, offset, size, socketFlags, out errorCode, callback, state);
		}

		// Token: 0x060040C9 RID: 16585 RVA: 0x003BE4D0 File Offset: 0x003BC6D0
		public IAsyncResult BeginSendFile(string fileName, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSendFile(fileName, callback, state);
		}

		// Token: 0x060040CA RID: 16586 RVA: 0x003BE4F0 File Offset: 0x003BC6F0
		public IAsyncResult BeginSendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSendFile(fileName, preBuffer, postBuffer, flags, callback, state);
		}

		// Token: 0x060040CB RID: 16587 RVA: 0x003BE518 File Offset: 0x003BC718
		public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return this.m_Socket.BeginSendTo(buffer, offset, size, socketFlags, remoteEP, callback, state);
		}

		// Token: 0x060040CC RID: 16588 RVA: 0x003BE540 File Offset: 0x003BC740
		public void Bind(EndPoint localEP)
		{
			this.m_Socket.Bind(localEP);
		}

		// Token: 0x060040CD RID: 16589 RVA: 0x003BE550 File Offset: 0x003BC750
		public static void CancelConnectAsync(SocketAsyncEventArgs e)
		{
			Socket.CancelConnectAsync(e);
		}

		// Token: 0x060040CE RID: 16590 RVA: 0x003BE55A File Offset: 0x003BC75A
		public void Close()
		{
			this.m_Socket.Close();
		}

		// Token: 0x060040CF RID: 16591 RVA: 0x003BE569 File Offset: 0x003BC769
		public void Close(int timeout)
		{
			this.m_Socket.Close(timeout);
		}

		// Token: 0x060040D0 RID: 16592 RVA: 0x003BE579 File Offset: 0x003BC779
		public void Connect(EndPoint remoteEP)
		{
			this.m_Socket.Connect(remoteEP);
		}

		// Token: 0x060040D1 RID: 16593 RVA: 0x003BE589 File Offset: 0x003BC789
		public void Connect(IPAddress address, int port)
		{
			this.m_Socket.Connect(address, port);
		}

		// Token: 0x060040D2 RID: 16594 RVA: 0x003BE59A File Offset: 0x003BC79A
		public void Connect(IPAddress[] addresses, int port)
		{
			this.m_Socket.Connect(addresses, port);
		}

		// Token: 0x060040D3 RID: 16595 RVA: 0x003BE5AB File Offset: 0x003BC7AB
		public void Connect(string host, int port)
		{
			this.m_Socket.Connect(host, port);
		}

		// Token: 0x060040D4 RID: 16596 RVA: 0x003BE5BC File Offset: 0x003BC7BC
		public bool ConnectAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.ConnectAsync(e);
		}

		// Token: 0x060040D5 RID: 16597 RVA: 0x003BE5DC File Offset: 0x003BC7DC
		public static bool ConnectAsync(SocketType socketType, ProtocolType protocolType, SocketAsyncEventArgs e)
		{
			return Socket.ConnectAsync(socketType, protocolType, e);
		}

		// Token: 0x060040D6 RID: 16598 RVA: 0x003BE5F6 File Offset: 0x003BC7F6
		public void Disconnect(bool reuseSocket)
		{
			this.m_Socket.Disconnect(reuseSocket);
		}

		// Token: 0x060040D7 RID: 16599 RVA: 0x003BE608 File Offset: 0x003BC808
		public bool DisconnectAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.DisconnectAsync(e);
		}

		// Token: 0x060040D8 RID: 16600 RVA: 0x003BE626 File Offset: 0x003BC826
		public void Dispose()
		{
			this.MyDispose();
			this.m_Socket.Dispose();
		}

		// Token: 0x060040D9 RID: 16601 RVA: 0x003BE63C File Offset: 0x003BC83C
		protected virtual void Dispose(bool disposing)
		{
		}

		// Token: 0x060040DA RID: 16602 RVA: 0x003BE640 File Offset: 0x003BC840
		public SocketInformation DuplicateAndClose(int targetProcessId)
		{
			return this.m_Socket.DuplicateAndClose(targetProcessId);
		}

		// Token: 0x060040DB RID: 16603 RVA: 0x003BE660 File Offset: 0x003BC860
		public Socket EndAccept(IAsyncResult asyncResult)
		{
			return this.m_Socket.EndAccept(asyncResult);
		}

		// Token: 0x060040DC RID: 16604 RVA: 0x003BE680 File Offset: 0x003BC880
		public Socket EndAccept(out byte[] buffer, IAsyncResult asyncResult)
		{
			return this.m_Socket.EndAccept(out buffer, asyncResult);
		}

		// Token: 0x060040DD RID: 16605 RVA: 0x003BE6A0 File Offset: 0x003BC8A0
		public Socket EndAccept(out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult)
		{
			return this.m_Socket.EndAccept(out buffer, out bytesTransferred, asyncResult);
		}

		// Token: 0x060040DE RID: 16606 RVA: 0x003BE6C0 File Offset: 0x003BC8C0
		public void EndConnect(IAsyncResult asyncResult)
		{
			this.m_Socket.EndConnect(asyncResult);
		}

		// Token: 0x060040DF RID: 16607 RVA: 0x003BE6D0 File Offset: 0x003BC8D0
		public void EndDisconnect(IAsyncResult asyncResult)
		{
			this.m_Socket.EndDisconnect(asyncResult);
		}

		// Token: 0x060040E0 RID: 16608 RVA: 0x003BE6E0 File Offset: 0x003BC8E0
		public int EndReceive(IAsyncResult asyncResult)
		{
			return this.m_Socket.EndReceive(asyncResult);
		}

		// Token: 0x060040E1 RID: 16609 RVA: 0x003BE700 File Offset: 0x003BC900
		public int EndReceive(IAsyncResult asyncResult, out SocketError errorCode)
		{
			return this.m_Socket.EndReceive(asyncResult, out errorCode);
		}

		// Token: 0x060040E2 RID: 16610 RVA: 0x003BE720 File Offset: 0x003BC920
		public int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint endPoint)
		{
			return this.m_Socket.EndReceiveFrom(asyncResult, ref endPoint);
		}

		// Token: 0x060040E3 RID: 16611 RVA: 0x003BE740 File Offset: 0x003BC940
		public int EndReceiveMessageFrom(IAsyncResult asyncResult, ref SocketFlags socketFlags, ref EndPoint endPoint, out IPPacketInformation ipPacketInformation)
		{
			return this.m_Socket.EndReceiveMessageFrom(asyncResult, ref socketFlags, ref endPoint, out ipPacketInformation);
		}

		// Token: 0x060040E4 RID: 16612 RVA: 0x003BE764 File Offset: 0x003BC964
		public int EndSend(IAsyncResult asyncResult)
		{
			return this.m_Socket.EndSend(asyncResult);
		}

		// Token: 0x060040E5 RID: 16613 RVA: 0x003BE784 File Offset: 0x003BC984
		public int EndSend(IAsyncResult asyncResult, out SocketError errorCode)
		{
			return this.m_Socket.EndSend(asyncResult, out errorCode);
		}

		// Token: 0x060040E6 RID: 16614 RVA: 0x003BE7A3 File Offset: 0x003BC9A3
		public void EndSendFile(IAsyncResult asyncResult)
		{
			this.m_Socket.EndSendFile(asyncResult);
		}

		// Token: 0x060040E7 RID: 16615 RVA: 0x003BE7B4 File Offset: 0x003BC9B4
		public int EndSendTo(IAsyncResult asyncResult)
		{
			return this.m_Socket.EndSendTo(asyncResult);
		}

		// Token: 0x060040E8 RID: 16616 RVA: 0x003BE7D4 File Offset: 0x003BC9D4
		public object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName)
		{
			return this.m_Socket.GetSocketOption(optionLevel, optionName);
		}

		// Token: 0x060040E9 RID: 16617 RVA: 0x003BE7F3 File Offset: 0x003BC9F3
		public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			this.m_Socket.GetSocketOption(optionLevel, optionName, optionValue);
		}

		// Token: 0x060040EA RID: 16618 RVA: 0x003BE808 File Offset: 0x003BCA08
		public byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionLength)
		{
			return this.m_Socket.GetSocketOption(optionLevel, optionName, optionLength);
		}

		// Token: 0x060040EB RID: 16619 RVA: 0x003BE828 File Offset: 0x003BCA28
		public int IOControl(int ioControlCode, byte[] optionInValue, byte[] optionOutValue)
		{
			return this.m_Socket.IOControl(ioControlCode, optionInValue, optionOutValue);
		}

		// Token: 0x060040EC RID: 16620 RVA: 0x003BE848 File Offset: 0x003BCA48
		public int IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue)
		{
			return this.m_Socket.IOControl(ioControlCode, optionInValue, optionOutValue);
		}

		// Token: 0x060040ED RID: 16621 RVA: 0x003BE868 File Offset: 0x003BCA68
		public void Listen(int backlog)
		{
			this.m_Socket.Listen(backlog);
		}

		// Token: 0x060040EE RID: 16622 RVA: 0x003BE878 File Offset: 0x003BCA78
		public bool Poll(int microSeconds, SelectMode mode)
		{
			return this.m_Socket.Poll(microSeconds, mode);
		}

		// Token: 0x060040EF RID: 16623 RVA: 0x003BE898 File Offset: 0x003BCA98
		public int Receive(byte[] buffer)
		{
			return this.m_Socket.Receive(buffer);
		}

		// Token: 0x060040F0 RID: 16624 RVA: 0x003BE8B8 File Offset: 0x003BCAB8
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int Receive(IList<ArraySegment<byte>> buffers)
		{
			return this.m_Socket.Receive(buffers);
		}

		// Token: 0x060040F1 RID: 16625 RVA: 0x003BE8D8 File Offset: 0x003BCAD8
		public int Receive(byte[] buffer, SocketFlags socketFlags)
		{
			return this.m_Socket.Receive(buffer, socketFlags);
		}

		// Token: 0x060040F2 RID: 16626 RVA: 0x003BE8F8 File Offset: 0x003BCAF8
		public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			return this.m_Socket.Receive(buffers, socketFlags);
		}

		// Token: 0x060040F3 RID: 16627 RVA: 0x003BE918 File Offset: 0x003BCB18
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int Receive(byte[] buffer, int size, SocketFlags socketFlags)
		{
			return this.m_Socket.Receive(buffer, size, socketFlags);
		}

		// Token: 0x060040F4 RID: 16628 RVA: 0x003BE938 File Offset: 0x003BCB38
		public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			return this.m_Socket.Receive(buffers, socketFlags, out errorCode);
		}

		// Token: 0x060040F5 RID: 16629 RVA: 0x003BE958 File Offset: 0x003BCB58
		public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags)
		{
			return this.m_Socket.Receive(buffer, offset, size, socketFlags);
		}

		// Token: 0x060040F6 RID: 16630 RVA: 0x003BE97C File Offset: 0x003BCB7C
		public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
		{
			return this.m_Socket.Receive(buffer, offset, size, socketFlags, out errorCode);
		}

		// Token: 0x060040F7 RID: 16631 RVA: 0x003BE9A0 File Offset: 0x003BCBA0
		public bool ReceiveAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.ReceiveAsync(e);
		}

		// Token: 0x060040F8 RID: 16632 RVA: 0x003BE9C0 File Offset: 0x003BCBC0
		public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP)
		{
			return this.m_Socket.ReceiveFrom(buffer, ref remoteEP);
		}

		// Token: 0x060040F9 RID: 16633 RVA: 0x003BE9E0 File Offset: 0x003BCBE0
		public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			return this.m_Socket.ReceiveFrom(buffer, socketFlags, ref remoteEP);
		}

		// Token: 0x060040FA RID: 16634 RVA: 0x003BEA00 File Offset: 0x003BCC00
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int ReceiveFrom(byte[] buffer, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			return this.m_Socket.ReceiveFrom(buffer, size, socketFlags, ref remoteEP);
		}

		// Token: 0x060040FB RID: 16635 RVA: 0x003BEA24 File Offset: 0x003BCC24
		public int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			return this.m_Socket.ReceiveFrom(buffer, offset, size, socketFlags, ref remoteEP);
		}

		// Token: 0x060040FC RID: 16636 RVA: 0x003BEA48 File Offset: 0x003BCC48
		public bool ReceiveFromAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.ReceiveFromAsync(e);
		}

		// Token: 0x060040FD RID: 16637 RVA: 0x003BEA68 File Offset: 0x003BCC68
		public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation)
		{
			return this.m_Socket.ReceiveMessageFrom(buffer, offset, size, ref socketFlags, ref remoteEP, out ipPacketInformation);
		}

		// Token: 0x060040FE RID: 16638 RVA: 0x003BEA90 File Offset: 0x003BCC90
		public bool ReceiveMessageFromAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.ReceiveMessageFromAsync(e);
		}

		// Token: 0x060040FF RID: 16639 RVA: 0x003BEAAE File Offset: 0x003BCCAE
		public static void Select(IList checkRead, IList checkWrite, IList checkError, int microSeconds)
		{
			Socket.Select(checkRead, checkWrite, checkError, microSeconds);
		}

		// Token: 0x06004100 RID: 16640 RVA: 0x003BEABC File Offset: 0x003BCCBC
		public int Send(byte[] buffer)
		{
			return this.m_Socket.Send(buffer);
		}

		// Token: 0x06004101 RID: 16641 RVA: 0x003BEADC File Offset: 0x003BCCDC
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int Send(IList<ArraySegment<byte>> buffers)
		{
			return this.m_Socket.Send(buffers);
		}

		// Token: 0x06004102 RID: 16642 RVA: 0x003BEAFC File Offset: 0x003BCCFC
		public int Send(byte[] buffer, SocketFlags socketFlags)
		{
			return this.m_Socket.Send(buffer, socketFlags);
		}

		// Token: 0x06004103 RID: 16643 RVA: 0x003BEB1C File Offset: 0x003BCD1C
		public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			return this.m_Socket.Send(buffers, socketFlags);
		}

		// Token: 0x06004104 RID: 16644 RVA: 0x003BEB3C File Offset: 0x003BCD3C
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int Send(byte[] buffer, int size, SocketFlags socketFlags)
		{
			return this.m_Socket.Send(buffer, size, socketFlags);
		}

		// Token: 0x06004105 RID: 16645 RVA: 0x003BEB5C File Offset: 0x003BCD5C
		public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			return this.m_Socket.Send(buffers, socketFlags, out errorCode);
		}

		// Token: 0x06004106 RID: 16646 RVA: 0x003BEB7C File Offset: 0x003BCD7C
		public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags)
		{
			return this.m_Socket.Send(buffer, offset, size, socketFlags);
		}

		// Token: 0x06004107 RID: 16647 RVA: 0x003BEBA0 File Offset: 0x003BCDA0
		public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
		{
			return this.m_Socket.Send(buffer, offset, size, socketFlags, out errorCode);
		}

		// Token: 0x06004108 RID: 16648 RVA: 0x003BEBC4 File Offset: 0x003BCDC4
		public bool SendAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.SendAsync(e);
		}

		// Token: 0x06004109 RID: 16649 RVA: 0x003BEBE2 File Offset: 0x003BCDE2
		public void SendFile(string fileName)
		{
			this.m_Socket.SendFile(fileName);
		}

		// Token: 0x0600410A RID: 16650 RVA: 0x003BEBF2 File Offset: 0x003BCDF2
		public void SendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags)
		{
			this.m_Socket.SendFile(fileName, preBuffer, postBuffer, flags);
		}

		// Token: 0x0600410B RID: 16651 RVA: 0x003BEC08 File Offset: 0x003BCE08
		public bool SendPacketsAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.SendPacketsAsync(e);
		}

		// Token: 0x0600410C RID: 16652 RVA: 0x003BEC28 File Offset: 0x003BCE28
		public int SendTo(byte[] buffer, EndPoint remoteEP)
		{
			return this.m_Socket.SendTo(buffer, remoteEP);
		}

		// Token: 0x0600410D RID: 16653 RVA: 0x003BEC48 File Offset: 0x003BCE48
		public int SendTo(byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP)
		{
			return this.m_Socket.SendTo(buffer, socketFlags, remoteEP);
		}

		// Token: 0x0600410E RID: 16654 RVA: 0x003BEC68 File Offset: 0x003BCE68
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public int SendTo(byte[] buffer, int size, SocketFlags socketFlags, EndPoint remoteEP)
		{
			return this.m_Socket.SendTo(buffer, size, socketFlags, remoteEP);
		}

		// Token: 0x0600410F RID: 16655 RVA: 0x003BEC8C File Offset: 0x003BCE8C
		public int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
		{
			return this.m_Socket.SendTo(buffer, offset, size, socketFlags, remoteEP);
		}

		// Token: 0x06004110 RID: 16656 RVA: 0x003BECB0 File Offset: 0x003BCEB0
		public bool SendToAsync(SocketAsyncEventArgs e)
		{
			return this.m_Socket.SendToAsync(e);
		}

		// Token: 0x06004111 RID: 16657 RVA: 0x003BECCE File Offset: 0x003BCECE
		public void SetIPProtectionLevel(IPProtectionLevel level)
		{
			this.m_Socket.SetIPProtectionLevel(level);
		}

		// Token: 0x06004112 RID: 16658 RVA: 0x003BECDE File Offset: 0x003BCEDE
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
		{
			this.m_Socket.SetSocketOption(optionLevel, optionName, optionValue);
		}

		// Token: 0x06004113 RID: 16659 RVA: 0x003BECF0 File Offset: 0x003BCEF0
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			this.m_Socket.SetSocketOption(optionLevel, optionName, optionValue);
		}

		// Token: 0x06004114 RID: 16660 RVA: 0x003BED02 File Offset: 0x003BCF02
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
		{
			this.m_Socket.SetSocketOption(optionLevel, optionName, optionValue);
		}

		// Token: 0x06004115 RID: 16661 RVA: 0x003BED14 File Offset: 0x003BCF14
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
		{
			this.m_Socket.SetSocketOption(optionLevel, optionName, optionValue);
		}

		// Token: 0x06004116 RID: 16662 RVA: 0x003BED26 File Offset: 0x003BCF26
		public void Shutdown(SocketShutdown how)
		{
			this.m_Socket.Shutdown(how);
		}

		// Token: 0x04004F55 RID: 20309
		public TCPInPacket _TcpInPacket;

		// Token: 0x04004F56 RID: 20310
		public SendBuffer _SendBuffer;

		// Token: 0x04004F57 RID: 20311
		public TCPSession session;

		// Token: 0x04004F58 RID: 20312
		public ushort magic = 0;

		// Token: 0x04004F59 RID: 20313
		public IntPtr Key;

		// Token: 0x04004F5A RID: 20314
		public ulong SortKey64 = 0UL;

		// Token: 0x04004F5B RID: 20315
		public bool IsKuaFuLogin = false;

		// Token: 0x04004F5C RID: 20316
		public int ServerId = 0;

		// Token: 0x04004F5D RID: 20317
		private int nNid = -1;

		// Token: 0x04004F5E RID: 20318
		public KuaFuServerLoginData ClientKuaFuServerLoginData = new KuaFuServerLoginData();

		// Token: 0x04004F5F RID: 20319
		private bool IsDisposed = false;

		// Token: 0x04004F60 RID: 20320
		public string CloseReason = "";

		// Token: 0x04004F61 RID: 20321
		public int DelayClose;

		// Token: 0x04004F62 RID: 20322
		public int ClientCmdSecs;

		// Token: 0x04004F63 RID: 20323
		public string UserID;

		// Token: 0x04004F64 RID: 20324
		private long _AcceptIpAsInt = 0L;

		// Token: 0x04004F65 RID: 20325
		private IPEndPoint _VirtualEndPoint;

		// Token: 0x04004F66 RID: 20326
		private Stack<SocketAsyncEventArgs> SocketAsyncEventArgsReadPool = new Stack<SocketAsyncEventArgs>();

		// Token: 0x04004F67 RID: 20327
		private int maxStackSocketAsyncEventArgsRead = 3;

		// Token: 0x04004F68 RID: 20328
		private Stack<SocketAsyncEventArgs> SocketAsyncEventArgsWritePool = new Stack<SocketAsyncEventArgs>();

		// Token: 0x04004F69 RID: 20329
		private int maxStackSocketAsyncEventArgsWrite = 3;

		// Token: 0x04004F6A RID: 20330
		public SocketAsyncEventArgsPool GlobalWritePool;

		// Token: 0x04004F6B RID: 20331
		public SocketAsyncEventArgsPool GlobalReadPool;

		// Token: 0x04004F6C RID: 20332
		public long SendCount;

		// Token: 0x04004F6D RID: 20333
		public Socket m_Socket = null;
	}
}
