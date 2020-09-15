using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.Protocol
{
	// Token: 0x020001E4 RID: 484
	public class TCPInPacketPool
	{
		// Token: 0x06000A18 RID: 2584 RVA: 0x000610A0 File Offset: 0x0005F2A0
		internal TCPInPacketPool(int capacity)
		{
			this.pool = new Stack<TCPInPacket>(capacity);
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000A19 RID: 2585 RVA: 0x000610B8 File Offset: 0x0005F2B8
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

		// Token: 0x06000A1A RID: 2586 RVA: 0x00061114 File Offset: 0x0005F314
		internal TCPInPacket Pop(Socket s, TCPCmdPacketEventHandler TCPCmdPacketEvent)
		{
			TCPInPacket result;
			lock (this.pool)
			{
				if (this.pool.Count <= 0)
				{
					TCPInPacket tcpInPacket = new TCPInPacket(131072)
					{
						CurrentSocket = s
					};
					tcpInPacket.TCPCmdPacketEvent += TCPCmdPacketEvent;
					result = tcpInPacket;
				}
				else
				{
					TCPInPacket tcpInPacket = this.pool.Pop();
					tcpInPacket.CurrentSocket = s;
					result = tcpInPacket;
				}
			}
			return result;
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x000611AC File Offset: 0x0005F3AC
		internal void Push(TCPInPacket item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到TCPInPacketPool 的item不能是空(null)");
			}
			lock (this.pool)
			{
				item.Reset();
				this.pool.Push(item);
			}
		}

		// Token: 0x04000C3F RID: 3135
		private Stack<TCPInPacket> pool;
	}
}
