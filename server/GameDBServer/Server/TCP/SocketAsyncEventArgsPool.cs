using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
	// Token: 0x0200020D RID: 525
	internal sealed class SocketAsyncEventArgsPool
	{
		// Token: 0x06000C34 RID: 3124 RVA: 0x0009EF61 File Offset: 0x0009D161
		internal SocketAsyncEventArgsPool(int capacity)
		{
			this.pool = new Stack<SocketAsyncEventArgs>(capacity);
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000C35 RID: 3125 RVA: 0x0009EF78 File Offset: 0x0009D178
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

		// Token: 0x06000C36 RID: 3126 RVA: 0x0009EFD4 File Offset: 0x0009D1D4
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

		// Token: 0x06000C37 RID: 3127 RVA: 0x0009F03C File Offset: 0x0009D23C
		internal void Push(SocketAsyncEventArgs item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到SocketAsyncEventArgsPool 的item不能是空(null)");
			}
			lock (this.pool)
			{
				this.pool.Push(item);
			}
		}

		// Token: 0x04001215 RID: 4629
		private Stack<SocketAsyncEventArgs> pool;
	}
}
