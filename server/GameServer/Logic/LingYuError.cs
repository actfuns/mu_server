using System;

namespace GameServer.Logic
{
	// Token: 0x02000512 RID: 1298
	internal enum LingYuError
	{
		// Token: 0x0400226F RID: 8815
		Success,
		// Token: 0x04002270 RID: 8816
		NotOpen,
		// Token: 0x04002271 RID: 8817
		LevelFull,
		// Token: 0x04002272 RID: 8818
		NeedLevelUp,
		// Token: 0x04002273 RID: 8819
		NeedSuitUp,
		// Token: 0x04002274 RID: 8820
		SuitFull,
		// Token: 0x04002275 RID: 8821
		LevelUpMaterialNotEnough,
		// Token: 0x04002276 RID: 8822
		LevelUpJinBiNotEnough,
		// Token: 0x04002277 RID: 8823
		SuitUpMaterialNotEnough,
		// Token: 0x04002278 RID: 8824
		SuitUpJinBiNotEnough,
		// Token: 0x04002279 RID: 8825
		ErrorConfig,
		// Token: 0x0400227A RID: 8826
		ErrorParams,
		// Token: 0x0400227B RID: 8827
		ZuanShiNotEnough,
		// Token: 0x0400227C RID: 8828
		DBSERVERERROR
	}
}
