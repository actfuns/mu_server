using System;
using System.Collections.Generic;
using System.Threading;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class TimedActionManager
	{
		
		public long AddItem(long startTicks, long period, int execCount, int type, Action<long, object> action, object context)
		{
			long result;
			if (execCount > 1000 || execCount < 1)
			{
				LogManager.WriteLog(LogTypes.Error, "非法参数#AddItem,execCount", null, true);
				result = -1L;
			}
			else
			{
				TimedActionItem item;
				lock (this.Mutex)
				{
					if (!this.Initialized)
					{
						this.Initialized = true;
						for (int i = 0; i < this.TimedActionArray.Length; i++)
						{
							this.TimedActionArray[i] = new TimedActionList();
						}
					}
					if (this.TimedActionQueue.Count > 0)
					{
						item = this.TimedActionQueue.Dequeue();
					}
					else
					{
						item = new TimedActionItem();
					}
					item.Id = Interlocked.Increment(ref this.UniqueId);
					item.StartTicks = startTicks;
					item.ExecCount = execCount;
					item.EndTicks = startTicks + (long)execCount * period;
					item.PeriodTicks = period;
					item.NextTicks = startTicks;
					item.ActionProc = action;
					item.ContextObject = context;
					item.Type = type;
					this.PreList.AddLast(item);
				}
				result = item.Id;
			}
			return result;
		}

		
		public void RemoveItem(int type)
		{
			lock (this.Mutex)
			{
				if (this.Initialized)
				{
					int i = 0;
					while ((long)i < 200L)
					{
						this.RemoveType(this.TimedActionArray[i], type);
						i++;
					}
					this.RemoveType(this.PreList, type);
					this.RemoveType(this.PostList, type);
				}
			}
		}

		
		private void Reset(long nowTicks)
		{
			this.StartTicks = nowTicks;
			this.EndTicks = this.StartTicks + 20000L;
			this.CurrentIndex = 0;
			this.ExecTicks = this.StartTicks + 100L;
		}

		
		public void Run(long nowTicks)
		{
			lock (this.Mutex)
			{
				if (this.Initialized)
				{
					if (this.ExecTicks < nowTicks)
					{
						if (this.CurrentIndex == 0)
						{
							if (nowTicks - this.ExecTicks > 5000L)
							{
								this.Reset(nowTicks);
								this.Run(nowTicks);
							}
						}
						while (this.ExecTicks < nowTicks && (long)this.CurrentIndex < 200L)
						{
							this.Traverse(this.TimedActionArray[this.CurrentIndex]);
							this.ExecTicks += 100L;
							this.CurrentIndex++;
						}
						this.Traverse(this.PreList);
						if ((long)this.CurrentIndex >= 200L)
						{
							this.Reset(this.ExecTicks);
							this.Traverse(this.PostList);
						}
					}
					else if (this.ExecTicks - nowTicks > 5000L)
					{
						this.ExecTicks = 0L;
					}
				}
			}
		}

		
		private bool ExecItem(LinkedListNode<TimedActionItem> node)
		{
			while (node.Value.ExecCount > 0)
			{
				if (node.Value.NextTicks >= this.ExecTicks)
				{
					break;
				}
				node.Value.ExecCount--;
				long execTicks = node.Value.NextTicks;
				node.Value.NextTicks += node.Value.PeriodTicks;
				node.Value.Exec(execTicks);
			}
			return node.Value.ExecCount > 0;
		}

		
		private void Traverse(LinkedList<TimedActionItem> list)
		{
			if (list.Count > 0)
			{
				LinkedListNode<TimedActionItem> node = list.First;
				LinkedListNode<TimedActionItem> last = list.Last;
				while (node != null)
				{
					LinkedListNode<TimedActionItem> next = node.Next;
					if (node.List == list)
					{
						list.Remove(node);
						if (this.ExecItem(node))
						{
							if (node.Value.NextTicks >= this.EndTicks)
							{
								this.PostList.AddLast(node);
							}
							else if (node.Value.NextTicks >= this.ExecTicks)
							{
								long subTicks = node.Value.NextTicks - this.StartTicks;
								long index = subTicks / 100L;
								this.TimedActionArray[(int)(checked((IntPtr)index))].AddLast(node);
							}
						}
					}
					if (last == node)
					{
						break;
					}
					node = next;
				}
			}
		}

		
		private void RemoveType(LinkedList<TimedActionItem> list, int type)
		{
			if (list.Count > 0)
			{
				LinkedListNode<TimedActionItem> node = list.First;
				LinkedListNode<TimedActionItem> last = list.Last;
				while (node != null)
				{
					LinkedListNode<TimedActionItem> next = node.Next;
					if (node.Value.Type == type)
					{
						list.Remove(node);
					}
					node = next;
				}
			}
		}

		
		private const long PeriodTicks = 100L;

		
		private const long MaxIndex = 200L;

		
		private const long MaxTicks = 20000L;

		
		private object Mutex = new object();

		
		private long UniqueId;

		
		private Queue<TimedActionItem> TimedActionQueue = new Queue<TimedActionItem>();

		
		public TimedActionList[] TimedActionArray = new TimedActionList[200L];

		
		public TimedActionList PreList = new TimedActionList();

		
		public TimedActionList PostList = new TimedActionList();

		
		public int CurrentIndex;

		
		public long ExecTicks;

		
		public long StartTicks;

		
		public long EndTicks;

		
		public bool Initialized;
	}
}
