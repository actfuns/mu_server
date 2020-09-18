using System;
using System.Collections.Generic;

namespace GameServer.Logic.Algorithm
{
	
	public class UsedStateList<T>
	{
		
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

		
		private LinkedList<T> usedLinkedList = new LinkedList<T>();

		
		private LinkedList<T> unusedLinkedList = new LinkedList<T>();
	}
}
