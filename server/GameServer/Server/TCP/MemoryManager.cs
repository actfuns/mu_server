using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Server.TCP
{
	// Token: 0x020008D1 RID: 2257
	public class MemoryManager
	{
		// Token: 0x06004075 RID: 16501 RVA: 0x003BCFD1 File Offset: 0x003BB1D1
		public void AddBatchBlock(int blockNum, int blockSize)
		{
			this.AddBatchBlock2(blockNum, blockSize);
		}

		// Token: 0x06004076 RID: 16502 RVA: 0x003BCFE0 File Offset: 0x003BB1E0
		public void AddBatchBlock2(int blockNum, int blockSize)
		{
			blockNum /= 8;
			lock (this.MemoryDict)
			{
				if (this.MemoryDict.ContainsKey(blockSize))
				{
					this.MemoryDict.Remove(blockSize);
				}
				MemoryStackArray stackArray = new MemoryStackArray();
				for (int idx = 0; idx < 16; idx++)
				{
					Stack<MemoryBlock> blockList = new Stack<MemoryBlock>();
					for (int i = 0; i < blockNum; i++)
					{
						MemoryBlock mb = new MemoryBlock(blockSize, true);
						blockList.Push(mb);
						this.BlockDict[mb] = 1;
					}
					stackArray.StackList[idx] = blockList;
				}
				this.MemoryDict.Add(blockSize, stackArray);
			}
			lock (this.BlockSizeList)
			{
				this.BlockSizeList.Add(blockSize);
				this.BlockSizeList.Sort();
			}
		}

		// Token: 0x06004077 RID: 16503 RVA: 0x003BD114 File Offset: 0x003BB314
		public void Push(MemoryBlock item)
		{
			if (null == item)
			{
				throw new ArgumentNullException("添加到MemoryManager 的item不能是空(null)");
			}
			MemoryStackArray stackArray;
			if (!item.isManaged)
			{
				Interlocked.Add(ref MemoryManager.TotalNewAllocMemorySize, (long)(-(long)item.BlockSize));
			}
			else if (this.MemoryDict.TryGetValue(item.BlockSize, out stackArray))
			{
				int index = Interlocked.Increment(ref stackArray.PushIndex) & 15;
				Stack<MemoryBlock> blockList = stackArray.StackList[index];
				lock (blockList)
				{
					byte state = 0;
					this.BlockDict.TryGetValue(item, out state);
					if (state <= 0)
					{
						blockList.Push(item);
						this.BlockDict[item] = 1;
					}
				}
			}
		}

		// Token: 0x06004078 RID: 16504 RVA: 0x003BD210 File Offset: 0x003BB410
		public MemoryBlock Pop(int needSize)
		{
			MemoryStackArray stackArray;
			if (this.MemoryDict.TryGetValue(this.GetIndex(needSize), out stackArray))
			{
				int index = Interlocked.Increment(ref stackArray.PopIndex) & 15;
				Stack<MemoryBlock> blockList = stackArray.StackList[index];
				lock (blockList)
				{
					if (blockList.Count > 0)
					{
						MemoryBlock item = blockList.Pop();
						this.BlockDict[item] = 0;
						return item;
					}
				}
			}
			Interlocked.Add(ref MemoryManager.TotalNewAllocMemorySize, (long)needSize);
			return new MemoryBlock(needSize, false);
		}

		// Token: 0x06004079 RID: 16505 RVA: 0x003BD2E0 File Offset: 0x003BB4E0
		private int GetIndex(int needSize)
		{
			int destSize = -1;
			foreach (int nSizeKey in this.BlockSizeList)
			{
				if (needSize <= nSizeKey)
				{
					destSize = nSizeKey;
					break;
				}
			}
			return destSize;
		}

		// Token: 0x0600407A RID: 16506 RVA: 0x003BD374 File Offset: 0x003BB574
		public string GetCacheInfoStr()
		{
			StringBuilder bufferTxt = new StringBuilder();
			Dictionary<int, List<int>> memoryInfoDict = new Dictionary<int, List<int>>();
			foreach (KeyValuePair<int, MemoryStackArray> item in this.MemoryDict)
			{
				int blockSize = item.Key;
				List<int> blockNumList = null;
				foreach (Stack<MemoryBlock> sk in item.Value.StackList)
				{
					lock (sk)
					{
						if (memoryInfoDict.TryGetValue(blockSize, out blockNumList))
						{
							blockNumList.Add(sk.Count);
						}
						else
						{
							memoryInfoDict[blockSize] = new List<int>
							{
								sk.Count
							};
						}
					}
				}
			}
			foreach (KeyValuePair<int, List<int>> item2 in memoryInfoDict)
			{
				int totalCount = 0;
				string countListStr = "";
				item2.Value.ForEach(delegate(int x)
				{
					totalCount += x;
					countListStr = x.ToString();
				});
				bufferTxt.AppendFormat(string.Format("大小 {0} bytes 缓存中数量 {1} [{2}]\r\n", item2.Key, totalCount, countListStr), new object[0]);
			}
			bufferTxt.AppendFormat("非缓存分配，正在使用的内存: {0}", MemoryManager.GetNewAllocMemorySize());
			return bufferTxt.ToString();
		}

		// Token: 0x0600407B RID: 16507 RVA: 0x003BD56C File Offset: 0x003BB76C
		public string GetUsedMemoryAllocStackTrace()
		{
			StringBuilder sb = new StringBuilder();
			lock (this.MemoryBlockStackTraceDict)
			{
				foreach (KeyValuePair<MemoryBlock, StackTrace> kv in this.MemoryBlockStackTraceDict)
				{
					if (kv.Value != null)
					{
						sb.AppendFormat("BlockSize:{0},StackTrace:{1}\r\n", kv.Key.BlockSize, kv.Value.ToString());
					}
				}
			}
			return sb.ToString();
		}

		// Token: 0x0600407C RID: 16508 RVA: 0x003BD648 File Offset: 0x003BB848
		public static long GetNewAllocMemorySize()
		{
			long memorySize = 0L;
			lock (MemoryManager.MemoryLock)
			{
				memorySize = MemoryManager.TotalNewAllocMemorySize;
			}
			return memorySize;
		}

		// Token: 0x04004F4D RID: 20301
		public const int ConstSplitePoolNum = 16;

		// Token: 0x04004F4E RID: 20302
		public const int ConstSplitePoolMask = 15;

		// Token: 0x04004F4F RID: 20303
		private static object MemoryLock = new object();

		// Token: 0x04004F50 RID: 20304
		private static long TotalNewAllocMemorySize = 0L;

		// Token: 0x04004F51 RID: 20305
		private Dictionary<int, MemoryStackArray> MemoryDict = new Dictionary<int, MemoryStackArray>();

		// Token: 0x04004F52 RID: 20306
		private Dictionary<MemoryBlock, StackTrace> MemoryBlockStackTraceDict = new Dictionary<MemoryBlock, StackTrace>();

		// Token: 0x04004F53 RID: 20307
		private List<int> BlockSizeList = new List<int>();

		// Token: 0x04004F54 RID: 20308
		private Dictionary<MemoryBlock, byte> BlockDict = new Dictionary<MemoryBlock, byte>();
	}
}
