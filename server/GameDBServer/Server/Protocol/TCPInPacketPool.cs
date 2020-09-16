using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.Protocol
{
	
	public class TCPInPacketPool
	{
		
		internal TCPInPacketPool(int capacity)
		{
			this.pool = new Stack<TCPInPacket>(capacity);
		}

		
		
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

		
		private Stack<TCPInPacket> pool;
	}
}
