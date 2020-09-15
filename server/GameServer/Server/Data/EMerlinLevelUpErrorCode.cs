using System;

namespace Server.Data
{
	// Token: 0x02000384 RID: 900
	public enum EMerlinLevelUpErrorCode
	{
		// Token: 0x040017C3 RID: 6083
		Error = -1,
		// Token: 0x040017C4 RID: 6084
		Success,
		// Token: 0x040017C5 RID: 6085
		LevelError,
		// Token: 0x040017C6 RID: 6086
		MaxLevelNum,
		// Token: 0x040017C7 RID: 6087
		NotMaxStarNum,
		// Token: 0x040017C8 RID: 6088
		LevelDataError,
		// Token: 0x040017C9 RID: 6089
		NeedGoodsIDError,
		// Token: 0x040017CA RID: 6090
		NeedGoodsCountError,
		// Token: 0x040017CB RID: 6091
		GoodsNotEnough,
		// Token: 0x040017CC RID: 6092
		DiamondNotEnough,
		// Token: 0x040017CD RID: 6093
		Fail
	}
}
