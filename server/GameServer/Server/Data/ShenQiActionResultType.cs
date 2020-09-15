using System;

namespace Server.Data
{
	// Token: 0x02000405 RID: 1029
	public enum ShenQiActionResultType
	{
		// Token: 0x04001B5F RID: 7007
		End = 3,
		// Token: 0x04001B60 RID: 7008
		Next = 2,
		// Token: 0x04001B61 RID: 7009
		Success = 1,
		// Token: 0x04001B62 RID: 7010
		Efail = 0,
		// Token: 0x04001B63 RID: 7011
		EnoOpen = -1,
		// Token: 0x04001B64 RID: 7012
		EnoShenLiJingHua = -2,
		// Token: 0x04001B65 RID: 7013
		EnoDiamond = -3,
		// Token: 0x04001B66 RID: 7014
		EnoJinBi = -4,
		// Token: 0x04001B67 RID: 7015
		None = -100
	}
}
