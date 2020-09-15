using System;
using System.Collections.Generic;

namespace GameServer.Logic.Algorithm
{
	// Token: 0x020001EC RID: 492
	public class UsedStateList<T>
	{
		// Token: 0x06000637 RID: 1591 RVA: 0x00057548 File Offset: 0x00055748
		public bool SetNodeState(LinkedListNode<T> item, bool state)
		{
			LinkedList<T> list = item.List;
			if (list == this.usedLinkedList)
			{
				if (state)
				{
					return true;
				}
				this.usedLinkedList.Remove(item);
			}
			else if (list == this.unusedLinkedList)
			{
				if (!state)
				{
					return true;
				}
				this.unusedLinkedList.Remove(item);
			}
			if (state)
			{
				this.usedLinkedList.AddLast(item);
			}
			else
			{
				this.unusedLinkedList.AddLast(item);
			}
			return true;
		}

		// Token: 0x04000AD6 RID: 2774
		private LinkedList<T> usedLinkedList = new LinkedList<T>();

		// Token: 0x04000AD7 RID: 2775
		private LinkedList<T> unusedLinkedList = new LinkedList<T>();
	}
}
