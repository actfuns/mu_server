using System;
using System.Collections.Generic;

namespace Server.Protocol
{
	// Token: 0x0200086B RID: 2155
	public class TCPOutPacketPool
	{
		// Token: 0x06003CDD RID: 15581 RVA: 0x00342611 File Offset: 0x00340811
		private TCPOutPacketPool()
		{
		}

		// Token: 0x06003CDE RID: 15582 RVA: 0x0034261C File Offset: 0x0034081C
		public static TCPOutPacketPool getInstance()
		{
			return TCPOutPacketPool.instance;
		}

		// Token: 0x06003CDF RID: 15583 RVA: 0x00342633 File Offset: 0x00340833
		public void initialize(int capacity)
		{
			this.pool = new Stack<TCPOutPacket>(capacity);
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x06003CE0 RID: 15584 RVA: 0x00342644 File Offset: 0x00340844
		internal int Count
		{
			get
			{
				int count = 0;
				lock (this.pool)
				{
					count = this.pool.Count;
				}
				return count;
			}
		}

		// Token: 0x06003CE1 RID: 15585 RVA: 0x003426A0 File Offset: 0x003408A0
		internal TCPOutPacket Pop()
		{
			return new TCPOutPacket();
		}

		// Token: 0x06003CE2 RID: 15586 RVA: 0x003426B8 File Offset: 0x003408B8
		internal void Push(TCPOutPacket item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到TCPOutPacketPool 的item不能是空(null)");
			}
			item.Reset();
			item.Dispose();
		}

		// Token: 0x04004739 RID: 18233
		private Stack<TCPOutPacket> pool;

		// Token: 0x0400473A RID: 18234
		private static TCPOutPacketPool instance = new TCPOutPacketPool();
	}
}
