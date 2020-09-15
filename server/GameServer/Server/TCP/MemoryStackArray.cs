using System;
using System.Collections.Generic;

namespace Server.TCP
{
	// Token: 0x020008D0 RID: 2256
	public class MemoryStackArray
	{
		// Token: 0x04004F4A RID: 20298
		public Stack<MemoryBlock>[] StackList = new Stack<MemoryBlock>[16];

		// Token: 0x04004F4B RID: 20299
		public int PushIndex;

		// Token: 0x04004F4C RID: 20300
		public int PopIndex;
	}
}
