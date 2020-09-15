using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameDBServer.Server
{
	// Token: 0x0200020B RID: 523
	public class TCPManager
	{
		// Token: 0x06000C1F RID: 3103 RVA: 0x0009E830 File Offset: 0x0009CA30
		private TCPManager()
		{
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x0009E860 File Offset: 0x0009CA60
		public static TCPManager getInstance()
		{
			return TCPManager.instance;
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x0009E878 File Offset: 0x0009CA78
		public void initialize(int capacity)
		{
			capacity = Math.Max(capacity, 250);
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

		// Token: 0x06000C22 RID: 3106 RVA: 0x0009E94C File Offset: 0x0009CB4C
		public GameServerClient getClient(Socket socket)
		{
			GameServerClient client = null;
			this.gameServerClients.TryGetValue(socket, out client);
			return client;
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000C23 RID: 3107 RVA: 0x0009E970 File Offset: 0x0009CB70
		public SocketListener MySocketListener
		{
			get
			{
				return this.socketListener;
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000C24 RID: 3108 RVA: 0x0009E988 File Offset: 0x0009CB88
		// (set) Token: 0x06000C25 RID: 3109 RVA: 0x0009E99F File Offset: 0x0009CB9F
		public Program RootWindow { get; set; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000C26 RID: 3110 RVA: 0x0009E9A8 File Offset: 0x0009CBA8
		// (set) Token: 0x06000C27 RID: 3111 RVA: 0x0009E9BF File Offset: 0x0009CBBF
		public DBManager DBMgr { get; set; }

		// Token: 0x06000C28 RID: 3112 RVA: 0x0009E9C8 File Offset: 0x0009CBC8
		public void Start(string ip, int port)
		{
			this.socketListener.Init();
			this.socketListener.Start(ip, port);
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x0009E9E5 File Offset: 0x0009CBE5
		public void Stop()
		{
			this.socketListener.Stop();
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x0009E9F4 File Offset: 0x0009CBF4
		private bool TCPCmdPacketEvent(object sender)
		{
			TCPInPacket tcpInPacket = sender as TCPInPacket;
			TCPOutPacket tcpOutPacket = null;
			GameServerClient client = null;
			bool result2;
			if (!this.gameServerClients.TryGetValue(tcpInPacket.CurrentSocket, out client))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("未建立会话或会话已关闭: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket)), null, true);
				result2 = false;
			}
			else
			{
				TCPManager.CurrentClient = client;
				long processBeginTime = TimeUtil.NowEx();
				TCPProcessCmdResults result = TCPCmdHandler.ProcessCmd(client, this.DBMgr, this.tcpOutPacketPool, (int)tcpInPacket.PacketCmdID, tcpInPacket.GetPacketBytes(), tcpInPacket.PacketDataSize, out tcpOutPacket);
				long processTime = TimeUtil.NowEx() - processBeginTime;
				if (result == TCPProcessCmdResults.RESULT_DATA && null != tcpOutPacket)
				{
					this.socketListener.SendData(tcpInPacket.CurrentSocket, tcpOutPacket);
				}
				else if (result == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析并执行命令失败: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket)), null, true);
					return false;
				}
				lock (TCPManager.cmdMoniter)
				{
					int cmdID = (int)tcpInPacket.PacketCmdID;
					PorcessCmdMoniter moniter = null;
					if (!TCPManager.cmdMoniter.TryGetValue(cmdID, out moniter))
					{
						moniter = new PorcessCmdMoniter(cmdID, processTime);
						TCPManager.cmdMoniter.Add(cmdID, moniter);
					}
					moniter.onProcessNoWait(processTime);
				}
				TCPManager.CurrentClient = null;
				result2 = true;
			}
			return result2;
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x0009EB8C File Offset: 0x0009CD8C
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

		// Token: 0x06000C2C RID: 3116 RVA: 0x0009EC2C File Offset: 0x0009CE2C
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

		// Token: 0x06000C2D RID: 3117 RVA: 0x0009ED48 File Offset: 0x0009CF48
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

		// Token: 0x06000C2E RID: 3118 RVA: 0x0009EE14 File Offset: 0x0009D014
		private void SocketSended(object sender, SocketAsyncEventArgs e)
		{
			TCPOutPacket tcpOutPacket = (e.UserToken as AsyncUserToken).Tag as TCPOutPacket;
			this.tcpOutPacketPool.Push(tcpOutPacket);
		}

		// Token: 0x04001204 RID: 4612
		private static TCPManager instance = new TCPManager();

		// Token: 0x04001205 RID: 4613
		public static long processCmdNum = 0L;

		// Token: 0x04001206 RID: 4614
		public static long processTotalTime = 0L;

		// Token: 0x04001207 RID: 4615
		public static Dictionary<int, PorcessCmdMoniter> cmdMoniter = new Dictionary<int, PorcessCmdMoniter>();

		// Token: 0x04001208 RID: 4616
		private SocketListener socketListener = null;

		// Token: 0x04001209 RID: 4617
		private TCPInPacketPool tcpInPacketPool = null;

		// Token: 0x0400120A RID: 4618
		private TCPOutPacketPool tcpOutPacketPool = null;

		// Token: 0x0400120B RID: 4619
		[ThreadStatic]
		public static GameServerClient CurrentClient;

		// Token: 0x0400120C RID: 4620
		private Dictionary<Socket, TCPInPacket> dictInPackets = null;

		// Token: 0x0400120D RID: 4621
		private Dictionary<Socket, GameServerClient> gameServerClients = null;
	}
}
