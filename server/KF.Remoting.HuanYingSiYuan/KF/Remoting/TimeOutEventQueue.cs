using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	// Token: 0x02000080 RID: 128
	public class TimeOutEventQueue<T>
	{
		// Token: 0x0600062E RID: 1582 RVA: 0x00055180 File Offset: 0x00053380
		public void EnqueueTimeOutEventItem(T item, DateTime endTime)
		{
			lock (this.Mutex)
			{
				TimeOutEventBlock<T> timeOutEventBlock = this.ShengBeiBufferTimeListQueue.First.Value;
				if (endTime.Ticks - timeOutEventBlock.EndTime.Ticks >= 10000000L)
				{
					timeOutEventBlock = new TimeOutEventBlock<T>();
					timeOutEventBlock.EndTime = endTime;
					this.ShengBeiBufferTimeListQueue.AddFirst(timeOutEventBlock);
				}
				timeOutEventBlock.ChildList.Add(item);
			}
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x00055220 File Offset: 0x00053420
		public bool DequeueTimeOutEventItem(List<T> outputList, DateTime now)
		{
			bool result = false;
			lock (this.Mutex)
			{
				for (LinkedListNode<TimeOutEventBlock<T>> node = this.ShengBeiBufferTimeListQueue.Last; node != null; node = node.Previous)
				{
					if (node.Value.EndTime > now)
					{
						break;
					}
					if (!result)
					{
						result = true;
					}
					outputList.AddRange(node.Value.ChildList);
					this.ShengBeiBufferTimeListQueue.RemoveLast();
				}
			}
			return result;
		}

		// Token: 0x04000376 RID: 886
		private object Mutex = new object();

		// Token: 0x04000377 RID: 887
		private LinkedList<TimeOutEventBlock<T>> ShengBeiBufferTimeListQueue = new LinkedList<TimeOutEventBlock<T>>();
	}
}
