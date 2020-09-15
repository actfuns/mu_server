using System;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	// Token: 0x020001AA RID: 426
	public enum ESevenDayActErrorCode
	{
		// Token: 0x0400096E RID: 2414
		Success,
		// Token: 0x0400096F RID: 2415
		NotInActivityTime,
		// Token: 0x04000970 RID: 2416
		ServerConfigError,
		// Token: 0x04000971 RID: 2417
		NoBagSpace,
		// Token: 0x04000972 RID: 2418
		DBFailed,
		// Token: 0x04000973 RID: 2419
		NotReachCondition,
		// Token: 0x04000974 RID: 2420
		VisitParamsWrong,
		// Token: 0x04000975 RID: 2421
		ZuanShiNotEnough,
		// Token: 0x04000976 RID: 2422
		NoEnoughGoodsCanBuy
	}
}
