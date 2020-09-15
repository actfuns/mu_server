using System;

namespace GameServer.Logic
{
	// Token: 0x02000667 RID: 1639
	public enum VipPriorityTypes
	{
		// Token: 0x0400319C RID: 12700
		None,
		// Token: 0x0400319D RID: 12701
		GetDailyYuanBao,
		// Token: 0x0400319E RID: 12702
		GetDailyLingLi = 5,
		// Token: 0x0400319F RID: 12703
		GetDailyYinLiang,
		// Token: 0x040031A0 RID: 12704
		GetDailyAttackFuZhou,
		// Token: 0x040031A1 RID: 12705
		GetDailyDefenseFuZhou,
		// Token: 0x040031A2 RID: 12706
		GetDailyLifeFuZhou,
		// Token: 0x040031A3 RID: 12707
		GetDailyTongQian,
		// Token: 0x040031A4 RID: 12708
		GetDailyZhenQi = 21,
		// Token: 0x040031A5 RID: 12709
		MaxVal = 100
	}
}
