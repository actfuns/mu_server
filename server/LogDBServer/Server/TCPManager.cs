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
	
	internal class TCPManager
	{
		
		private TCPManager()
		{
		}

		
		public static TCPManager getInstance()
		{
			return TCPManager.instance;
		}

		
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

		
		public GameServerClient getClient(Socket socket)
		{
			GameServerClient client = null;
			this.gameServerClients.TryGetValue(socket, out client);
			return client;
		}

		
		
		public SocketListener MySocketListener
		{
			get
			{
				return this.socketListener;
			}
		}

		
		
		
		public Program RootWindow { get; set; }

		
		
		
		public DBManager DBMgr { get; set; }

		
		public void Start(string ip, int port)
		{
			this.socketListener.Init();
			this.socketListener.Start(ip, port);
		}

		
		public void Stop()
		{
			this.socketListener.Stop();
		}

		
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

		
		private void SocketSended(object sender, SocketAsyncEventArgs e)
		{
			TCPOutPacket tcpOutPacket = (e.UserToken as AsyncUserToken).Tag as TCPOutPacket;
			this.tcpOutPacketPool.Push(tcpOutPacket);
		}

		
		private static TCPManager instance = new TCPManager();

		
		private SocketListener socketListener = null;

		
		private TCPInPacketPool tcpInPacketPool = null;

		
		private TCPOutPacketPool tcpOutPacketPool = null;

		
		private Dictionary<Socket, TCPInPacket> dictInPackets = null;

		
		private Dictionary<Socket, GameServerClient> gameServerClients = null;
	}
}
