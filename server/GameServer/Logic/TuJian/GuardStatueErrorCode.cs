using System;

namespace GameServer.Logic.TuJian
{
	// Token: 0x02000456 RID: 1110
	public enum GuardStatueErrorCode
	{
		// Token: 0x04001DFB RID: 7675
		Success,
		// Token: 0x04001DFC RID: 7676
		NotOpen,
		// Token: 0x04001DFD RID: 7677
		ContainNotActiveTuJian,
		// Token: 0x04001DFE RID: 7678
		MoreThanTodayCanRecover,
		// Token: 0x04001DFF RID: 7679
		GuardPointNotEnough,
		// Token: 0x04001E00 RID: 7680
		MaterialNotEnough,
		// Token: 0x04001E01 RID: 7681
		NeedSuitUp,
		// Token: 0x04001E02 RID: 7682
		NeedLevelUp,
		// Token: 0x04001E03 RID: 7683
		SuitIsFull,
		// Token: 0x04001E04 RID: 7684
		LevelIsFull,
		// Token: 0x04001E05 RID: 7685
		ConfigError,
		// Token: 0x04001E06 RID: 7686
		DBFailed
	}
}
