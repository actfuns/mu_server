using System;
using System.Net.Sockets;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Server
{
	// Token: 0x02000201 RID: 513
	public class GameServerClient
	{
		// Token: 0x06000A97 RID: 2711 RVA: 0x00065548 File Offset: 0x00063748
		public GameServerClient(Socket currentSocket)
		{
			this.currentSocket = currentSocket;
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000A98 RID: 2712 RVA: 0x0006555C File Offset: 0x0006375C
		public Socket CurrentSocket
		{
			get
			{
				return this.currentSocket;
			}
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x00065574 File Offset: 0x00063774
		public void sendCmd(int cmdId, string cmdData)
		{
			TCPManager.getInstance().MySocketListener.SendData(this.currentSocket, TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId));
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x00065599 File Offset: 0x00063799
		public void sendCmd<T>(int cmdId, T cmdData)
		{
			TCPManager.getInstance().MySocketListener.SendData(this.currentSocket, DataHelper.ObjectToTCPOutPacket<T>(cmdData, TCPOutPacketPool.getInstance(), cmdId));
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x000655BE File Offset: 0x000637BE
		public void release()
		{
			this.currentSocket = null;
		}

		// Token: 0x04000C70 RID: 3184
		private Socket currentSocket;

		// Token: 0x04000C71 RID: 3185
		public int LineId;
	}
}
