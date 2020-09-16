using System;
using System.Net.Sockets;
using Server.Protocol;
using Server.Tools;

namespace LogDBServer.Server
{
	
	public class GameServerClient
	{
		
		public GameServerClient(Socket currentSocket)
		{
			this.currentSocket = currentSocket;
		}

		
		
		public Socket CurrentSocket
		{
			get
			{
				return this.currentSocket;
			}
		}

		
		public void sendCmd(int cmdId, string cmdData)
		{
			TCPManager.getInstance().MySocketListener.SendData(this.currentSocket, TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId));
		}

		
		public void sendCmd<T>(int cmdId, T cmdData)
		{
			TCPManager.getInstance().MySocketListener.SendData(this.currentSocket, DataHelper.ObjectToTCPOutPacket<T>(cmdData, TCPOutPacketPool.getInstance(), cmdId));
		}

		
		public void release()
		{
			this.currentSocket = null;
		}

		
		private Socket currentSocket;
	}
}
