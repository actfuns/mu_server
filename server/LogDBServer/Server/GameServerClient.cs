using System;
using System.Net.Sockets;
using Server.Protocol;
using Server.Tools;

namespace LogDBServer.Server
{
	// Token: 0x0200002A RID: 42
	public class GameServerClient
	{
		// Token: 0x060000E5 RID: 229 RVA: 0x000064BC File Offset: 0x000046BC
		public GameServerClient(Socket currentSocket)
		{
			this.currentSocket = currentSocket;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x000064D0 File Offset: 0x000046D0
		public Socket CurrentSocket
		{
			get
			{
				return this.currentSocket;
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000064E8 File Offset: 0x000046E8
		public void sendCmd(int cmdId, string cmdData)
		{
			TCPManager.getInstance().MySocketListener.SendData(this.currentSocket, TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId));
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000650D File Offset: 0x0000470D
		public void sendCmd<T>(int cmdId, T cmdData)
		{
			TCPManager.getInstance().MySocketListener.SendData(this.currentSocket, DataHelper.ObjectToTCPOutPacket<T>(cmdData, TCPOutPacketPool.getInstance(), cmdId));
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00006532 File Offset: 0x00004732
		public void release()
		{
			this.currentSocket = null;
		}

		// Token: 0x04000064 RID: 100
		private Socket currentSocket;
	}
}
