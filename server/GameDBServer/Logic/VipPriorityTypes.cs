using System;

namespace GameDBServer.Logic
{
	// Token: 0x020001B9 RID: 441
	public enum VipPriorityTypes
	{
		// Token: 0x04000A87 RID: 2695
		None,
		// Token: 0x04000A88 RID: 2696
		GetDailyYuanBao,
		// Token: 0x04000A89 RID: 2697
		GetDailyLingLi = 5,
		// Token: 0x04000A8A RID: 2698
		GetDailyYinLiang,
		// Token: 0x04000A8B RID: 2699
		GetDailyAttackFuZhou,
		// Token: 0x04000A8C RID: 2700
		GetDailyDefenseFuZhou,
		// Token: 0x04000A8D RID: 2701
		GetDailyLifeFuZhou,
		// Token: 0x04000A8E RID: 2702
		GetDailyTongQian,
		// Token: 0x04000A8F RID: 2703
		GetDailyZhenQi = 21,
		// Token: 0x04000A90 RID: 2704
		MaxVal = 100
	}
}
