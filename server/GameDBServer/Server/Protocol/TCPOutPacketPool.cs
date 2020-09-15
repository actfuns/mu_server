using System;
using System.Collections.Generic;

namespace Server.Protocol
{
	// Token: 0x020001E6 RID: 486
	public class TCPOutPacketPool
	{
		// Token: 0x06000A29 RID: 2601 RVA: 0x0006147E File Offset: 0x0005F67E
		private TCPOutPacketPool()
		{
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x0006148C File Offset: 0x0005F68C
		public static TCPOutPacketPool getInstance()
		{
			return TCPOutPacketPool.instance;
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x000614A3 File Offset: 0x0005F6A3
		public void initialize(int capacity)
		{
			this.pool = new Stack<TCPOutPacket>(capacity);
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000A2C RID: 2604 RVA: 0x000614B4 File Offset: 0x0005F6B4
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

		// Token: 0x06000A2D RID: 2605 RVA: 0x00061510 File Offset: 0x0005F710
		internal TCPOutPacket Pop()
		{
			TCPOutPacket result;
			lock (this.pool)
			{
				if (this.pool.Count <= 0)
				{
					result = new TCPOutPacket();
				}
				else
				{
					result = this.pool.Pop();
				}
			}
			return result;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x0006157C File Offset: 0x0005F77C
		internal void Push(TCPOutPacket item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到TCPOutPacketPool 的item不能是空(null)");
			}
			lock (this.pool)
			{
				item.Reset();
				this.pool.Push(item);
			}
		}

		// Token: 0x04000C44 RID: 3140
		private Stack<TCPOutPacket> pool;

		// Token: 0x04000C45 RID: 3141
		private static TCPOutPacketPool instance = new TCPOutPacketPool();
	}
}
