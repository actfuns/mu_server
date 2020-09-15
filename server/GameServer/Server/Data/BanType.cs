using System;

namespace Server.Data
{
	// Token: 0x0200023B RID: 571
	public enum BanType
	{
		// Token: 0x04000D79 RID: 3449
		Old,
		// Token: 0x04000D7A RID: 3450
		BanLog,
		// Token: 0x04000D7B RID: 3451
		BanKick,
		// Token: 0x04000D7C RID: 3452
		BanClose,
		// Token: 0x04000D7D RID: 3453
		BanKickBreak,
		// Token: 0x04000D7E RID: 3454
		BanCloseBreak,
		// Token: 0x04000D7F RID: 3455
		BanRate,
		// Token: 0x04000D80 RID: 3456
		BanRateNum,
		// Token: 0x04000D81 RID: 3457
		BanLogBig = 10,
		// Token: 0x04000D82 RID: 3458
		BanLogNormal,
		// Token: 0x04000D83 RID: 3459
		BanLogInvalid,
		// Token: 0x04000D84 RID: 3460
		BanLogAppVM = 20,
		// Token: 0x04000D85 RID: 3461
		BanKickAppVM,
		// Token: 0x04000D86 RID: 3462
		BanCloseAppVM,
		// Token: 0x04000D87 RID: 3463
		BanRateAppVM,
		// Token: 0x04000D88 RID: 3464
		BanLogAppPlatformCount = 30,
		// Token: 0x04000D89 RID: 3465
		BanKickAppPlatformCount,
		// Token: 0x04000D8A RID: 3466
		BanCloseAppPlatformCount,
		// Token: 0x04000D8B RID: 3467
		BanRateAppPlatformCount,
		// Token: 0x04000D8C RID: 3468
		BanLogDeviceNull = 40,
		// Token: 0x04000D8D RID: 3469
		BanKickDeviceNull,
		// Token: 0x04000D8E RID: 3470
		BanCloseDeviceNull,
		// Token: 0x04000D8F RID: 3471
		BanRateDeviceNull,
		// Token: 0x04000D90 RID: 3472
		BanLogSpecialApp = 50,
		// Token: 0x04000D91 RID: 3473
		BanKickSpecialApp,
		// Token: 0x04000D92 RID: 3474
		BanCloseSpecialApp,
		// Token: 0x04000D93 RID: 3475
		BanRateSpecialApp,
		// Token: 0x04000D94 RID: 3476
		BanLogAppCount = 60,
		// Token: 0x04000D95 RID: 3477
		BanKickAppCount,
		// Token: 0x04000D96 RID: 3478
		BanCloseAppCount,
		// Token: 0x04000D97 RID: 3479
		BanRateAppCount,
		// Token: 0x04000D98 RID: 3480
		BanLogMulti = 70,
		// Token: 0x04000D99 RID: 3481
		BanKickMulti,
		// Token: 0x04000D9A RID: 3482
		BanCloseMulti,
		// Token: 0x04000D9B RID: 3483
		BanRateMulti,
		// Token: 0x04000D9C RID: 3484
		BanLogTimeOut = 80,
		// Token: 0x04000D9D RID: 3485
		BanKickTimeOut,
		// Token: 0x04000D9E RID: 3486
		BanCloseTimeOut,
		// Token: 0x04000D9F RID: 3487
		BanRateTimeOut,
		// Token: 0x04000DA0 RID: 3488
		BanLogDecrypt = 90,
		// Token: 0x04000DA1 RID: 3489
		BanKickDecrypt,
		// Token: 0x04000DA2 RID: 3490
		BanCloseDecrypt,
		// Token: 0x04000DA3 RID: 3491
		BanRateDecrypt,
		// Token: 0x04000DA4 RID: 3492
		VmLog = 110,
		// Token: 0x04000DA5 RID: 3493
		VmKick,
		// Token: 0x04000DA6 RID: 3494
		VmClose,
		// Token: 0x04000DA7 RID: 3495
		VmRate,
		// Token: 0x04000DA8 RID: 3496
		VmLogSign = 120,
		// Token: 0x04000DA9 RID: 3497
		VmKickSign,
		// Token: 0x04000DAA RID: 3498
		VmCloseSign,
		// Token: 0x04000DAB RID: 3499
		VmRateSign,
		// Token: 0x04000DAC RID: 3500
		Max = 10000
	}
}
