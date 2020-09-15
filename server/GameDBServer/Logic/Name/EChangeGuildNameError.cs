using System;

namespace GameDBServer.Logic.Name
{
	// Token: 0x0200014D RID: 333
	public enum EChangeGuildNameError
	{
		// Token: 0x04000839 RID: 2105
		Success,
		// Token: 0x0400083A RID: 2106
		InvalidName,
		// Token: 0x0400083B RID: 2107
		DBFailed,
		// Token: 0x0400083C RID: 2108
		NameAlreadyUsed,
		// Token: 0x0400083D RID: 2109
		OperatorDenied,
		// Token: 0x0400083E RID: 2110
		LengthError
	}
}
