using System;

namespace GameServer.Logic.Name
{
	// Token: 0x0200037F RID: 895
	public enum EChangeGuildNameError
	{
		// Token: 0x040017A3 RID: 6051
		Success,
		// Token: 0x040017A4 RID: 6052
		InvalidName,
		// Token: 0x040017A5 RID: 6053
		DBFailed,
		// Token: 0x040017A6 RID: 6054
		NameAlreadyUsed,
		// Token: 0x040017A7 RID: 6055
		OperatorDenied,
		// Token: 0x040017A8 RID: 6056
		LengthError
	}
}
