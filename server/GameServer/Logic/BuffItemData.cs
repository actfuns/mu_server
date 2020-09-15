using System;

namespace GameServer.Logic
{
	// Token: 0x02000063 RID: 99
	public class BuffItemData
	{
		// Token: 0x04000234 RID: 564
		public int buffId;

		// Token: 0x04000235 RID: 565
		public int buffSecs;

		// Token: 0x04000236 RID: 566
		public long startTicks;

		// Token: 0x04000237 RID: 567
		public long buffVal;

		// Token: 0x04000238 RID: 568
		public long endTicks;

		// Token: 0x04000239 RID: 569
		public bool enabled;

		// Token: 0x0400023A RID: 570
		public bool clientEnabledState;

		// Token: 0x0400023B RID: 571
		public bool enabledByTime;

		// Token: 0x0400023C RID: 572
		public bool enabledByMap;

		// Token: 0x0400023D RID: 573
		public bool isUpdateByTime;

		// Token: 0x0400023E RID: 574
		public bool isUpdateByVip;

		// Token: 0x0400023F RID: 575
		public bool isUpdateByMapCode;

		// Token: 0x04000240 RID: 576
		public int flags;

		// Token: 0x04000241 RID: 577
		public double buffValEx;
	}
}
