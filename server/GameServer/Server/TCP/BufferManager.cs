using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
	// Token: 0x020008C5 RID: 2245
	internal sealed class BufferManager
	{
		// Token: 0x06004002 RID: 16386 RVA: 0x003BA37C File Offset: 0x003B857C
		internal BufferManager(int totalBytes, int bufferSize)
		{
			this.numBytes = totalBytes;
			this.currentIndex = 0;
			this.bufferSize = bufferSize;
			this.freeIndexPool = new Stack<int>();
		}

		// Token: 0x06004003 RID: 16387 RVA: 0x003BA3A7 File Offset: 0x003B85A7
		internal void FreeBuffer(SocketAsyncEventArgs args)
		{
			this.freeIndexPool.Push(args.Offset);
			args.SetBuffer(null, 0, 0);
		}

		// Token: 0x06004004 RID: 16388 RVA: 0x003BA3C6 File Offset: 0x003B85C6
		internal void InitBuffer()
		{
			this.buffer = new byte[this.numBytes];
		}

		// Token: 0x06004005 RID: 16389 RVA: 0x003BA3DC File Offset: 0x003B85DC
		internal bool SetBuffer(SocketAsyncEventArgs args)
		{
			if (this.freeIndexPool.Count > 0)
			{
				args.SetBuffer(this.buffer, this.freeIndexPool.Pop(), this.bufferSize);
			}
			else
			{
				if (this.numBytes - this.bufferSize < this.currentIndex)
				{
					return false;
				}
				args.SetBuffer(this.buffer, this.currentIndex, this.bufferSize);
				this.currentIndex += this.bufferSize;
			}
			return true;
		}

		// Token: 0x04004F12 RID: 20242
		private byte[] buffer;

		// Token: 0x04004F13 RID: 20243
		private int bufferSize;

		// Token: 0x04004F14 RID: 20244
		private int currentIndex;

		// Token: 0x04004F15 RID: 20245
		private Stack<int> freeIndexPool;

		// Token: 0x04004F16 RID: 20246
		private int numBytes;
	}
}
