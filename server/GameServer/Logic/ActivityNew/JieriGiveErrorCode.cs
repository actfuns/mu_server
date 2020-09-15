using System;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001B4 RID: 436
	public enum JieriGiveErrorCode
	{
		// Token: 0x040009BD RID: 2493
		Success,
		// Token: 0x040009BE RID: 2494
		ActivityNotOpen,
		// Token: 0x040009BF RID: 2495
		NotAwardTime,
		// Token: 0x040009C0 RID: 2496
		GoodsIDError,
		// Token: 0x040009C1 RID: 2497
		GoodsNotEnough,
		// Token: 0x040009C2 RID: 2498
		ReceiverNotExist,
		// Token: 0x040009C3 RID: 2499
		ReceiverCannotSelf,
		// Token: 0x040009C4 RID: 2500
		DBFailed,
		// Token: 0x040009C5 RID: 2501
		ConfigError,
		// Token: 0x040009C6 RID: 2502
		NoBagSpace,
		// Token: 0x040009C7 RID: 2503
		NotMeetAwardCond
	}
}
