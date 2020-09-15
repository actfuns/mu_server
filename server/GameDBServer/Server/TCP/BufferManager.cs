using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
	// Token: 0x0200020C RID: 524
	internal sealed class BufferManager
	{
		// Token: 0x06000C30 RID: 3120 RVA: 0x0009EE69 File Offset: 0x0009D069
		internal BufferManager(int totalBytes, int bufferSize)
		{
			this.numBytes = totalBytes;
			this.currentIndex = 0;
			this.bufferSize = bufferSize;
			this.freeIndexPool = new Stack<int>();
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x0009EE94 File Offset: 0x0009D094
		internal void FreeBuffer(SocketAsyncEventArgs args)
		{
			this.freeIndexPool.Push(args.Offset);
			args.SetBuffer(null, 0, 0);
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x0009EEB3 File Offset: 0x0009D0B3
		internal void InitBuffer()
		{
			this.buffer = new byte[this.numBytes];
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x0009EEC8 File Offset: 0x0009D0C8
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

		// Token: 0x04001210 RID: 4624
		private byte[] buffer;

		// Token: 0x04001211 RID: 4625
		private int bufferSize;

		// Token: 0x04001212 RID: 4626
		private int currentIndex;

		// Token: 0x04001213 RID: 4627
		private Stack<int> freeIndexPool;

		// Token: 0x04001214 RID: 4628
		private int numBytes;
	}
}
