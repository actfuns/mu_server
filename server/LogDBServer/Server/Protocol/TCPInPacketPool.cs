using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.Protocol
{
	// Token: 0x02000022 RID: 34
	public class TCPInPacketPool
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x00005B4C File Offset: 0x00003D4C
		internal TCPInPacketPool(int capacity)
		{
			this.pool = new Stack<TCPInPacket>(capacity);
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00005B64 File Offset: 0x00003D64
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

		// Token: 0x060000BA RID: 186 RVA: 0x00005BC0 File Offset: 0x00003DC0
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

		// Token: 0x060000BB RID: 187 RVA: 0x00005C58 File Offset: 0x00003E58
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

		// Token: 0x04000058 RID: 88
		private Stack<TCPInPacket> pool;
	}
}
