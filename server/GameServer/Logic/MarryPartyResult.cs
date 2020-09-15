using System;

namespace GameServer.Logic
{
	// Token: 0x02000530 RID: 1328
	public enum MarryPartyResult
	{
		// Token: 0x04002359 RID: 9049
		Success,
		// Token: 0x0400235A RID: 9050
		PartyNotFound,
		// Token: 0x0400235B RID: 9051
		InvalidParam,
		// Token: 0x0400235C RID: 9052
		NotEnoughMoney,
		// Token: 0x0400235D RID: 9053
		NotMarry,
		// Token: 0x0400235E RID: 9054
		AlreadyRequest,
		// Token: 0x0400235F RID: 9055
		AlreadyStart,
		// Token: 0x04002360 RID: 9056
		NotOriginator,
		// Token: 0x04002361 RID: 9057
		NotStart,
		// Token: 0x04002362 RID: 9058
		ZeroPartyJoinCount,
		// Token: 0x04002363 RID: 9059
		ZeroPlayerJoinCount,
		// Token: 0x04002364 RID: 9060
		NotOpen
	}
}
