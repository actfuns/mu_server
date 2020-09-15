using System;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001BC RID: 444
	public enum JieriLianXuChargeErrorCode
	{
		// Token: 0x040009E3 RID: 2531
		Success,
		// Token: 0x040009E4 RID: 2532
		InvalidParam,
		// Token: 0x040009E5 RID: 2533
		ActivityNotOpen,
		// Token: 0x040009E6 RID: 2534
		NotAwardTime,
		// Token: 0x040009E7 RID: 2535
		DBFailed,
		// Token: 0x040009E8 RID: 2536
		ConfigError,
		// Token: 0x040009E9 RID: 2537
		NoBagSpace,
		// Token: 0x040009EA RID: 2538
		NotMeetAwardCond
	}
}
