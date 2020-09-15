using System;

namespace GameServer.Logic
{
	// Token: 0x0200051E RID: 1310
	public class ManyTimeDmageQueueItem
	{
		// Token: 0x040022D1 RID: 8913
		public long ToExecTicks = 0L;

		// Token: 0x040022D2 RID: 8914
		public int enemy = -1;

		// Token: 0x040022D3 RID: 8915
		public int enemyX = 0;

		// Token: 0x040022D4 RID: 8916
		public int enemyY = 0;

		// Token: 0x040022D5 RID: 8917
		public int realEnemyX = 0;

		// Token: 0x040022D6 RID: 8918
		public int realEnemyY = 0;

		// Token: 0x040022D7 RID: 8919
		public int magicCode = 0;

		// Token: 0x040022D8 RID: 8920
		public int manyRangeIndex = 0;

		// Token: 0x040022D9 RID: 8921
		public double manyRangeInjuredPercent = 1.0;
	}
}
