using System;
using System.Collections.Generic;

namespace Server.Protocol
{
	
	public class TCPOutPacketPool
	{
		
		private TCPOutPacketPool()
		{
		}

		
		public static TCPOutPacketPool getInstance()
		{
			return TCPOutPacketPool.instance;
		}

		
		public void initialize(int capacity)
		{
			this.pool = new Stack<TCPOutPacket>(capacity);
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

		
		private Stack<TCPOutPacket> pool;

		
		private static TCPOutPacketPool instance = new TCPOutPacketPool();
	}
}
