using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	
	public class TimeOutEventQueue<T>
	{
		
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

		
		private object Mutex = new object();

		
		private LinkedList<TimeOutEventBlock<T>> ShengBeiBufferTimeListQueue = new LinkedList<TimeOutEventBlock<T>>();
	}
}
