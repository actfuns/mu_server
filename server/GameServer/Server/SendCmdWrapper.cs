using System;
using Server.Protocol;
using Server.TCP;

namespace GameServer.Server
{
	// Token: 0x020008B7 RID: 2231
	public class SendCmdWrapper
	{
		// Token: 0x06003DCB RID: 15819 RVA: 0x0034C3DB File Offset: 0x0034A5DB
		public SendCmdWrapper()
		{
			SendCmdWrapper.IncInstanceCount();
		}

		// Token: 0x06003DCC RID: 15820 RVA: 0x0034C3FA File Offset: 0x0034A5FA
		public void Release()
		{
			SendCmdWrapper.DecInstanceCount();
		}

		// Token: 0x06003DCD RID: 15821 RVA: 0x0034C404 File Offset: 0x0034A604
		public static void IncInstanceCount()
		{
			lock (SendCmdWrapper.CountLock)
			{
				SendCmdWrapper.TotalInstanceCount++;
			}
		}

		// Token: 0x06003DCE RID: 15822 RVA: 0x0034C454 File Offset: 0x0034A654
		public static void DecInstanceCount()
		{
			lock (SendCmdWrapper.CountLock)
			{
				SendCmdWrapper.TotalInstanceCount--;
			}
		}

		// Token: 0x06003DCF RID: 15823 RVA: 0x0034C4A4 File Offset: 0x0034A6A4
		public static int GetInstanceCount()
		{
			int count = 0;
			lock (SendCmdWrapper.CountLock)
			{
				count = SendCmdWrapper.TotalInstanceCount;
			}
			return count;
		}

		// Token: 0x040047DA RID: 18394
		public TMSKSocket socket = null;

		// Token: 0x040047DB RID: 18395
		public TCPOutPacket tcpOutPacket = null;

		// Token: 0x040047DC RID: 18396
		private static object CountLock = new object();

		// Token: 0x040047DD RID: 18397
		private static int TotalInstanceCount = 0;
	}
}
