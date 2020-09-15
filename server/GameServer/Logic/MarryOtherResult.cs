using System;

namespace GameServer.Logic
{
	// Token: 0x0200052A RID: 1322
	public enum MarryOtherResult
	{
		// Token: 0x0400232D RID: 9005
		Error = -1,
		// Token: 0x0400232E RID: 9006
		Success,
		// Token: 0x0400232F RID: 9007
		NotMarriaged,
		// Token: 0x04002330 RID: 9008
		NotFindItem,
		// Token: 0x04002331 RID: 9009
		ItemNotRose,
		// Token: 0x04002332 RID: 9010
		NeedGam,
		// Token: 0x04002333 RID: 9011
		NeedRose,
		// Token: 0x04002334 RID: 9012
		MessageLimit,
		// Token: 0x04002335 RID: 9013
		NotRing,
		// Token: 0x04002336 RID: 9014
		CirEffect,
		// Token: 0x04002337 RID: 9015
		NotNexRise,
		// Token: 0x04002338 RID: 9016
		MaxLimit,
		// Token: 0x04002339 RID: 9017
		NotOpen
	}
}
