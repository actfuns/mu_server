using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
	// Token: 0x020008C9 RID: 2249
	public sealed class SocketAsyncEventArgsPool
	{
		// Token: 0x06004033 RID: 16435 RVA: 0x003BB5F2 File Offset: 0x003B97F2
		internal SocketAsyncEventArgsPool(int capacity)
		{
			this.pool = new Stack<SocketAsyncEventArgs>(capacity);
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06004034 RID: 16436 RVA: 0x003BB60C File Offset: 0x003B980C
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

		// Token: 0x06004035 RID: 16437 RVA: 0x003BB668 File Offset: 0x003B9868
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

		// Token: 0x06004036 RID: 16438 RVA: 0x003BB6D0 File Offset: 0x003B98D0
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

		// Token: 0x04004F34 RID: 20276
		private Stack<SocketAsyncEventArgs> pool;
	}
}
