using System;
using System.Collections.Generic;
using System.Threading;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200074C RID: 1868
	public class TimedActionManager
	{
		// Token: 0x06002EFE RID: 12030 RVA: 0x002A1018 File Offset: 0x0029F218
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

		// Token: 0x06002EFF RID: 12031 RVA: 0x002A1168 File Offset: 0x0029F368
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

		// Token: 0x06002F00 RID: 12032 RVA: 0x002A1200 File Offset: 0x0029F400
		private void Reset(long nowTicks)
		{
			this.StartTicks = nowTicks;
			this.EndTicks = this.StartTicks + 20000L;
			this.CurrentIndex = 0;
			this.ExecTicks = this.StartTicks + 100L;
		}

		// Token: 0x06002F01 RID: 12033 RVA: 0x002A1234 File Offset: 0x0029F434
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

		// Token: 0x06002F02 RID: 12034 RVA: 0x002A139C File Offset: 0x0029F59C
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

		// Token: 0x06002F03 RID: 12035 RVA: 0x002A1448 File Offset: 0x0029F648
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

		// Token: 0x06002F04 RID: 12036 RVA: 0x002A154C File Offset: 0x0029F74C
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

		// Token: 0x04003C98 RID: 15512
		private const long PeriodTicks = 100L;

		// Token: 0x04003C99 RID: 15513
		private const long MaxIndex = 200L;

		// Token: 0x04003C9A RID: 15514
		private const long MaxTicks = 20000L;

		// Token: 0x04003C9B RID: 15515
		private object Mutex = new object();

		// Token: 0x04003C9C RID: 15516
		private long UniqueId;

		// Token: 0x04003C9D RID: 15517
		private Queue<TimedActionItem> TimedActionQueue = new Queue<TimedActionItem>();

		// Token: 0x04003C9E RID: 15518
		public TimedActionList[] TimedActionArray = new TimedActionList[200L];

		// Token: 0x04003C9F RID: 15519
		public TimedActionList PreList = new TimedActionList();

		// Token: 0x04003CA0 RID: 15520
		public TimedActionList PostList = new TimedActionList();

		// Token: 0x04003CA1 RID: 15521
		public int CurrentIndex;

		// Token: 0x04003CA2 RID: 15522
		public long ExecTicks;

		// Token: 0x04003CA3 RID: 15523
		public long StartTicks;

		// Token: 0x04003CA4 RID: 15524
		public long EndTicks;

		// Token: 0x04003CA5 RID: 15525
		public bool Initialized;
	}
}
