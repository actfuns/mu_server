using System;

namespace GameServer.Logic
{
	// Token: 0x02000542 RID: 1346
	public enum QingGongYanResult
	{
		// Token: 0x0400240E RID: 9230
		Success,
		// Token: 0x0400240F RID: 9231
		CheckSuccess,
		// Token: 0x04002410 RID: 9232
		ErrorParam,
		// Token: 0x04002411 RID: 9233
		Holding,
		// Token: 0x04002412 RID: 9234
		NotKing,
		// Token: 0x04002413 RID: 9235
		OutTime,
		// Token: 0x04002414 RID: 9236
		RepeatHold,
		// Token: 0x04002415 RID: 9237
		CountNotEnough,
		// Token: 0x04002416 RID: 9238
		TotalNotEnough,
		// Token: 0x04002417 RID: 9239
		MoneyNotEnough
	}
}
