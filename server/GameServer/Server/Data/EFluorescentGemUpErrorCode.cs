using System;

namespace Server.Data
{
	// Token: 0x0200013C RID: 316
	public enum EFluorescentGemUpErrorCode
	{
		// Token: 0x040006FA RID: 1786
		NotOpen = -2,
		// Token: 0x040006FB RID: 1787
		Error,
		// Token: 0x040006FC RID: 1788
		Success,
		// Token: 0x040006FD RID: 1789
		GoodsNotExist,
		// Token: 0x040006FE RID: 1790
		UpDataError,
		// Token: 0x040006FF RID: 1791
		MaxLevel,
		// Token: 0x04000700 RID: 1792
		NextLevelDataError,
		// Token: 0x04000701 RID: 1793
		GoldNotEnough,
		// Token: 0x04000702 RID: 1794
		PositionIndexError,
		// Token: 0x04000703 RID: 1795
		GemTypeError,
		// Token: 0x04000704 RID: 1796
		GemNotEnough,
		// Token: 0x04000705 RID: 1797
		AddGoodsError,
		// Token: 0x04000706 RID: 1798
		DecGoodsError,
		// Token: 0x04000707 RID: 1799
		DecGoodsNotExist,
		// Token: 0x04000708 RID: 1800
		DecGoodsNotEnough,
		// Token: 0x04000709 RID: 1801
		EquipError,
		// Token: 0x0400070A RID: 1802
		NotGem,
		// Token: 0x0400070B RID: 1803
		BagNotEnoughOne
	}
}
