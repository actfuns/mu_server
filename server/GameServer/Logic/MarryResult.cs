using System;

namespace GameServer.Logic
{
	// Token: 0x02000524 RID: 1316
	public enum MarryResult
	{
		// Token: 0x04002303 RID: 8963
		Success,
		// Token: 0x04002304 RID: 8964
		SelfMarried,
		// Token: 0x04002305 RID: 8965
		SelfLevelNotEnough,
		// Token: 0x04002306 RID: 8966
		SelfBusy,
		// Token: 0x04002307 RID: 8967
		TargetMarried,
		// Token: 0x04002308 RID: 8968
		TargetBusy,
		// Token: 0x04002309 RID: 8969
		TargetLevelNotEnough,
		// Token: 0x0400230A RID: 8970
		TargetOffline,
		// Token: 0x0400230B RID: 8971
		InvalidSex,
		// Token: 0x0400230C RID: 8972
		ApplyCD,
		// Token: 0x0400230D RID: 8973
		ApplyTimeout,
		// Token: 0x0400230E RID: 8974
		MoneyNotEnough,
		// Token: 0x0400230F RID: 8975
		NotMarried,
		// Token: 0x04002310 RID: 8976
		AutoReject,
		// Token: 0x04002311 RID: 8977
		NotOpen,
		// Token: 0x04002312 RID: 8978
		TargetNotOpen,
		// Token: 0x04002313 RID: 8979
		DeniedByCoupleAreanTime,
		// Token: 0x04002314 RID: 8980
		Error_Denied_For_Minor_Occupation = -35
	}
}
