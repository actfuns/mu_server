using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Server.TCP
{
	
	public class MemoryManager
	{
		
		public void AddBatchBlock(int blockNum, int blockSize)
		{
			this.AddBatchBlock2(blockNum, blockSize);
		}

		
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

		
		public static long GetNewAllocMemorySize()
		{
			long memorySize = 0L;
			lock (MemoryManager.MemoryLock)
			{
				memorySize = MemoryManager.TotalNewAllocMemorySize;
			}
			return memorySize;
		}

		
		public const int ConstSplitePoolNum = 16;

		
		public const int ConstSplitePoolMask = 15;

		
		private static object MemoryLock = new object();

		
		private static long TotalNewAllocMemorySize = 0L;

		
		private Dictionary<int, MemoryStackArray> MemoryDict = new Dictionary<int, MemoryStackArray>();

		
		private Dictionary<MemoryBlock, StackTrace> MemoryBlockStackTraceDict = new Dictionary<MemoryBlock, StackTrace>();

		
		private List<int> BlockSizeList = new List<int>();

		
		private Dictionary<MemoryBlock, byte> BlockDict = new Dictionary<MemoryBlock, byte>();
	}
}
