using System;
using System.Collections.Generic;
using System.Net.Sockets;
using LogDBServer.DB;
using LogDBServer.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace LogDBServer.Server
{
	// Token: 0x0200002F RID: 47
	internal class TCPManager
	{
		// Token: 0x060000F3 RID: 243 RVA: 0x000067D4 File Offset: 0x000049D4
		private TCPManager()
		{
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00006804 File Offset: 0x00004A04
		public static TCPManager getInstance()
		{
			return TCPManager.instance;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000681C File Offset: 0x00004A1C
		public void initialize(int capacity)
		{
			this.socketListener = new SocketListener(capacity, 32768);
			this.socketListener.SocketClosed += this.SocketClosed;
			this.socketListener.SocketConnected += this.SocketConnected;
			this.socketListener.SocketReceived += this.SocketReceived;
			this.socketListener.SocketSended += this.SocketSended;
			this.tcpInPacketPool = new TCPInPacketPool(capacity);
			this.tcpOutPacketPool = TCPOutPacketPool.getInstance();
			this.tcpOutPacketPool.initialize(capacity * 5);
			TCPCmdDispatcher.getInstance().initialize();
			this.dictInPackets = new Dictionary<Socket, TCPInPacket>(capacity);
			this.gameServerClients = new Dictionary<Socket, GameServerClient>();
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000068E4 File Offset: 0x00004AE4
		public GameServerClient getClient(Socket socket)
		{
			GameServerClient client = null;
			this.gameServerClients.TryGetValue(socket, out client);
			return client;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00006908 File Offset: 0x00004B08
		public SocketListener MySocketListener
		{
			get
			{
				return this.socketListener;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00006920 File Offset: 0x00004B20
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00006937 File Offset: 0x00004B37
		public Program RootWindow { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00006940 File Offset: 0x00004B40
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00006957 File Offset: 0x00004B57
		public DBManager DBMgr { get; set; }

		// Token: 0x060000FC RID: 252 RVA: 0x00006960 File Offset: 0x00004B60
		public void Start(string ip, int port)
		{
			this.socketListener.Init();
			this.socketListener.Start(ip, port);
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000697D File Offset: 0x00004B7D
		public void Stop()
		{
			this.socketListener.Stop();
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000698C File Offset: 0x00004B8C
		private bool TCPCmdPacketEvent(object sender)
		{
			TCPInPacket tcpInPacket = sender as TCPInPacket;
			TCPOutPacket tcpOutPacket = null;
			GameServerClient client = null;
			bool result2;
			if (!this.gameServerClients.TryGetValue(tcpInPacket.CurrentSocket, out client))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("未建立会话或会话已关闭: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket)));
				result2 = false;
			}
			else
			{
				TCPProcessCmdResults result = TCPCmdHandler.ProcessCmd(client, this.DBMgr, this.tcpOutPacketPool, (int)tcpInPacket.PacketCmdID, tcpInPacket.GetPacketBytes(), tcpInPacket.PacketDataSize, out tcpOutPacket);
				if (result == TCPProcessCmdResults.RESULT_DATA && null != tcpOutPacket)
				{
					this.socketListener.SendData(tcpInPacket.CurrentSocket, tcpOutPacket);
				}
				else if (result == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析并执行命令失败: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket)));
					return false;
				}
				result2 = true;
			}
			return result2;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006A84 File Offset: 0x00004C84
		private void SocketConnected(object sender, SocketAsyncEventArgs e)
		{
			SocketListener sl = sender as SocketListener;
			this.RootWindow.TotalConnections = sl.ConnectedSocketsCount;
			lock (this.gameServerClients)
			{
				GameServerClient client = null;
				Socket s = (e.UserToken as AsyncUserToken).CurrentSocket;
				if (!this.gameServerClients.TryGetValue(s, out client))
				{
					client = new GameServerClient(s);
					this.gameServerClients.Add(s, client);
				}
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00006B24 File Offset: 0x00004D24
		private void SocketClosed(object sender, SocketAsyncEventArgs e)
		{
			SocketListener sl = sender as SocketListener;
			Socket s = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.dictInPackets)
			{
				if (this.dictInPackets.ContainsKey(s))
				{
					TCPInPacket tcpInPacket = this.dictInPackets[s];
					this.tcpInPacketPool.Push(tcpInPacket);
					this.dictInPackets.Remove(s);
				}
			}
			lock (this.gameServerClients)
			{
				GameServerClient client = null;
				if (this.gameServerClients.TryGetValue(s, out client))
				{
					client.release();
					this.gameServerClients.Remove(s);
				}
			}
			this.RootWindow.TotalConnections = sl.ConnectedSocketsCount;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00006C40 File Offset: 0x00004E40
		private bool SocketReceived(object sender, SocketAsyncEventArgs e)
		{
			SocketListener sl = sender as SocketListener;
			TCPInPacket tcpInPacket = null;
			Socket s = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.dictInPackets)
			{
				if (!this.dictInPackets.TryGetValue(s, out tcpInPacket))
				{
					tcpInPacket = this.tcpInPacketPool.Pop(s, new TCPCmdPacketEventHandler(this.TCPCmdPacketEvent));
					this.dictInPackets[s] = tcpInPacket;
				}
			}
			return tcpInPacket.WriteData(e.Buffer, e.Offset, e.BytesTransferred);
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00006D0C File Offset: 0x00004F0C
		private void SocketSended(object sender, SocketAsyncEventArgs e)
		{
			TCPOutPacket tcpOutPacket = (e.UserToken as AsyncUserToken).Tag as TCPOutPacket;
			this.tcpOutPacketPool.Push(tcpOutPacket);
		}

		// Token: 0x04000408 RID: 1032
		private static TCPManager instance = new TCPManager();

		// Token: 0x04000409 RID: 1033
		private SocketListener socketListener = null;

		// Token: 0x0400040A RID: 1034
		private TCPInPacketPool tcpInPacketPool = null;

		// Token: 0x0400040B RID: 1035
		private TCPOutPacketPool tcpOutPacketPool = null;

		// Token: 0x0400040C RID: 1036
		private Dictionary<Socket, TCPInPacket> dictInPackets = null;

		// Token: 0x0400040D RID: 1037
		private Dictionary<Socket, GameServerClient> gameServerClients = null;
	}
}
