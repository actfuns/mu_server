using System;
using System.Collections.Generic;

namespace Server.Protocol
{
	// Token: 0x02000024 RID: 36
	public class TCPOutPacketPool
	{
		// Token: 0x060000C9 RID: 201 RVA: 0x00005EE6 File Offset: 0x000040E6
		private TCPOutPacketPool()
		{
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00005EF4 File Offset: 0x000040F4
		public static TCPOutPacketPool getInstance()
		{
			return TCPOutPacketPool.instance;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00005F0B File Offset: 0x0000410B
		public void initialize(int capacity)
		{
			this.pool = new Stack<TCPOutPacket>(capacity);
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00005F1C File Offset: 0x0000411C
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

		// Token: 0x060000CD RID: 205 RVA: 0x00005F78 File Offset: 0x00004178
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

		// Token: 0x060000CE RID: 206 RVA: 0x00005FE4 File Offset: 0x000041E4
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

		// Token: 0x0400005D RID: 93
		private Stack<TCPOutPacket> pool;

		// Token: 0x0400005E RID: 94
		private static TCPOutPacketPool instance = new TCPOutPacketPool();
	}
}
