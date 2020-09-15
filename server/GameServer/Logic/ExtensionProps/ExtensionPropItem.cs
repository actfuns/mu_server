using System;
using System.Collections.Generic;

namespace GameServer.Logic.ExtensionProps
{
	// Token: 0x020006BC RID: 1724
	public class ExtensionPropItem
	{
		// Token: 0x0400366D RID: 13933
		public int ID = 0;

		// Token: 0x0400366E RID: 13934
		public Dictionary<int, byte> PrevTuoZhanShuXing = null;

		// Token: 0x0400366F RID: 13935
		public int TargetType = 0;

		// Token: 0x04003670 RID: 13936
		public int ActionType = 0;

		// Token: 0x04003671 RID: 13937
		public int Probability = 0;

		// Token: 0x04003672 RID: 13938
		public Dictionary<int, byte> NeedSkill = null;

		// Token: 0x04003673 RID: 13939
		public int Icon = 0;

		// Token: 0x04003674 RID: 13940
		public int TargetDecoration = 0;

		// Token: 0x04003675 RID: 13941
		public int DelayDecoration = 0;
	}
}
