using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020001DC RID: 476
	public class DeControlRuntimeData
	{
		// Token: 0x04000A7E RID: 2686
		public object Mutex = new object();

		// Token: 0x04000A7F RID: 2687
		public bool IsGongNengOpend;

		// Token: 0x04000A80 RID: 2688
		public List<DeControlItem>[] DeControlItemListArray = new List<DeControlItem>[177];
	}
}
