using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
	
	internal sealed class BufferManager
	{
		
		internal BufferManager(int totalBytes, int bufferSize)
		{
			this.numBytes = totalBytes;
			this.currentIndex = 0;
			this.bufferSize = bufferSize;
			this.freeIndexPool = new Stack<int>();
		}

		
		internal void FreeBuffer(SocketAsyncEventArgs args)
		{
			this.freeIndexPool.Push(args.Offset);
			args.SetBuffer(null, 0, 0);
		}

		
		internal void InitBuffer()
		{
			this.buffer = new byte[this.numBytes];
		}

		
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

		
		private byte[] buffer;

		
		private int bufferSize;

		
		private int currentIndex;

		
		private Stack<int> freeIndexPool;

		
		private int numBytes;
	}
}
