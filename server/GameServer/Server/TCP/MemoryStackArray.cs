using System;
using System.Collections.Generic;

namespace Server.TCP
{
	
	public class MemoryStackArray
	{
		
		public Stack<MemoryBlock>[] StackList = new Stack<MemoryBlock>[16];

		
		public int PushIndex;

		
		public int PopIndex;
	}
}
