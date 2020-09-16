using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
	
	public sealed class SocketAsyncEventArgsPool
	{
		
		internal SocketAsyncEventArgsPool(int capacity)
		{
			this.pool = new Stack<SocketAsyncEventArgs>(capacity);
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

		
		internal SocketAsyncEventArgs Pop()
		{
			SocketAsyncEventArgs result;
			lock (this.pool)
			{
				if (this.pool.Count <= 0)
				{
					result = null;
				}
				else
				{
					result = this.pool.Pop();
				}
			}
			return result;
		}

		
		internal void Push(SocketAsyncEventArgs item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到SocketAsyncEventArgsPool 的item不能是空(null)");
			}
			lock (this.pool)
			{
				if (this.pool.Count < 30000)
				{
					this.pool.Push(item);
				}
				else
				{
					item.Dispose();
				}
			}
		}

		
		private Stack<SocketAsyncEventArgs> pool;
	}
}
