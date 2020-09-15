using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
	// Token: 0x02000030 RID: 48
	internal sealed class BufferManager
	{
		// Token: 0x06000104 RID: 260 RVA: 0x00006D49 File Offset: 0x00004F49
		internal BufferManager(int totalBytes, int bufferSize)
		{
			this.numBytes = totalBytes;
			this.currentIndex = 0;
			this.bufferSize = bufferSize;
			this.freeIndexPool = new Stack<int>();
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00006D74 File Offset: 0x00004F74
		internal void FreeBuffer(SocketAsyncEventArgs args)
		{
			this.freeIndexPool.Push(args.Offset);
			args.SetBuffer(null, 0, 0);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006D93 File Offset: 0x00004F93
		internal void InitBuffer()
		{
			this.buffer = new byte[this.numBytes];
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00006DA8 File Offset: 0x00004FA8
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

		// Token: 0x04000410 RID: 1040
		private byte[] buffer;

		// Token: 0x04000411 RID: 1041
		private int bufferSize;

		// Token: 0x04000412 RID: 1042
		private int currentIndex;

		// Token: 0x04000413 RID: 1043
		private Stack<int> freeIndexPool;

		// Token: 0x04000414 RID: 1044
		private int numBytes;
	}
}
