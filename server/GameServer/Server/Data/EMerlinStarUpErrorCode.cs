using System;

namespace Server.Data
{
	// Token: 0x02000383 RID: 899
	public enum EMerlinStarUpErrorCode
	{
		// Token: 0x040017B8 RID: 6072
		Error = -1,
		// Token: 0x040017B9 RID: 6073
		Success,
		// Token: 0x040017BA RID: 6074
		MaxStarNum,
		// Token: 0x040017BB RID: 6075
		StarDataError,
		// Token: 0x040017BC RID: 6076
		NeedGoodsIDError,
		// Token: 0x040017BD RID: 6077
		NeedGoodsCountError,
		// Token: 0x040017BE RID: 6078
		GoodsNotEnough,
		// Token: 0x040017BF RID: 6079
		DiamondNotEnough,
		// Token: 0x040017C0 RID: 6080
		LevelError,
		// Token: 0x040017C1 RID: 6081
		StarError
	}
}
