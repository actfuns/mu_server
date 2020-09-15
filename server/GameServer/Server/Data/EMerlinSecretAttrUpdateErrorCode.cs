using System;

namespace Server.Data
{
	// Token: 0x02000385 RID: 901
	public enum EMerlinSecretAttrUpdateErrorCode
	{
		// Token: 0x040017CF RID: 6095
		Error = -1,
		// Token: 0x040017D0 RID: 6096
		Success,
		// Token: 0x040017D1 RID: 6097
		LevelError,
		// Token: 0x040017D2 RID: 6098
		SecretDataError,
		// Token: 0x040017D3 RID: 6099
		NeedGoodsIDError,
		// Token: 0x040017D4 RID: 6100
		NeedGoodsCountError,
		// Token: 0x040017D5 RID: 6101
		GoodsNotEnough
	}
}
