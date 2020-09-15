using System;
using System.Collections.Generic;
using Server.TCP;

namespace Server.Protocol
{
	// Token: 0x02000869 RID: 2153
	public class TCPInPacketPool
	{
		// Token: 0x06003CC4 RID: 15556 RVA: 0x003420D6 File Offset: 0x003402D6
		internal TCPInPacketPool(int capacity)
		{
			this.pool = new Queue<TCPInPacket>(capacity);
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x06003CC5 RID: 15557 RVA: 0x003420F0 File Offset: 0x003402F0
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

		// Token: 0x06003CC6 RID: 15558 RVA: 0x0034214C File Offset: 0x0034034C
		internal TCPInPacket Pop(TMSKSocket s, TCPCmdPacketEventHandler TCPCmdPacketEvent)
		{
			TCPInPacket result;
			lock (this.pool)
			{
				if (this.pool.Count <= 0)
				{
					TCPInPacket tcpInPacket = new TCPInPacket(6144)
					{
						CurrentSocket = s
					};
					tcpInPacket.TCPCmdPacketEvent += TCPCmdPacketEvent;
					result = tcpInPacket;
				}
				else
				{
					TCPInPacket tcpInPacket = this.pool.Dequeue();
					tcpInPacket.CurrentSocket = s;
					result = tcpInPacket;
				}
			}
			return result;
		}

		// Token: 0x06003CC7 RID: 15559 RVA: 0x003421E4 File Offset: 0x003403E4
		internal void Push(TCPInPacket item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到TCPInPacketPool 的item不能是空(null)");
			}
			lock (this.pool)
			{
				item.Reset();
				this.pool.Enqueue(item);
			}
		}

		// Token: 0x04004731 RID: 18225
		private Queue<TCPInPacket> pool;
	}
}
