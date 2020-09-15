using System;

namespace GameServer.Logic.Today
{
	// Token: 0x0200044B RID: 1099
	public enum ETodayState
	{
		// Token: 0x04001DA2 RID: 7586
		NotOpen = -11,
		// Token: 0x04001DA3 RID: 7587
		TaoCancel = -8,
		// Token: 0x04001DA4 RID: 7588
		EFubenConfig,
		// Token: 0x04001DA5 RID: 7589
		IsFuben,
		// Token: 0x04001DA6 RID: 7590
		IsAllAward,
		// Token: 0x04001DA7 RID: 7591
		IsAward,
		// Token: 0x04001DA8 RID: 7592
		NoType,
		// Token: 0x04001DA9 RID: 7593
		NoBag,
		// Token: 0x04001DAA RID: 7594
		Fail,
		// Token: 0x04001DAB RID: 7595
		Default,
		// Token: 0x04001DAC RID: 7596
		Success
	}
}
