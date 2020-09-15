using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
	// Token: 0x02000031 RID: 49
	internal sealed class SocketAsyncEventArgsPool
	{
		// Token: 0x06000108 RID: 264 RVA: 0x00006E41 File Offset: 0x00005041
		internal SocketAsyncEventArgsPool(int capacity)
		{
			this.pool = new Stack<SocketAsyncEventArgs>(capacity);
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000109 RID: 265 RVA: 0x00006E58 File Offset: 0x00005058
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

		// Token: 0x0600010A RID: 266 RVA: 0x00006EB4 File Offset: 0x000050B4
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

		// Token: 0x0600010B RID: 267 RVA: 0x00006F1C File Offset: 0x0000511C
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

		// Token: 0x04000415 RID: 1045
		private Stack<SocketAsyncEventArgs> pool;
	}
}
