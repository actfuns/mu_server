using System;

namespace GameServer.Logic.YueKa
{
	// Token: 0x02000822 RID: 2082
	public enum YueKaError
	{
		// Token: 0x040044F7 RID: 17655
		YK_Success,
		// Token: 0x040044F8 RID: 17656
		YK_CannotAward_HasNotYueKa,
		// Token: 0x040044F9 RID: 17657
		YK_CannotAward_DayHasPassed,
		// Token: 0x040044FA RID: 17658
		YK_CannotAward_AlreadyAward,
		// Token: 0x040044FB RID: 17659
		YK_CannotAward_TimeNotReach,
		// Token: 0x040044FC RID: 17660
		YK_CannotAward_BagNotEnough,
		// Token: 0x040044FD RID: 17661
		YK_CannotAward_ParamInvalid,
		// Token: 0x040044FE RID: 17662
		YK_CannotAward_ConfigError,
		// Token: 0x040044FF RID: 17663
		YK_CannotAward_DBError
	}
}
