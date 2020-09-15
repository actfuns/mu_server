using System;

namespace GameServer.Logic.KuaFuIPStatistics
{
	// Token: 0x02000349 RID: 841
	internal enum UserParamType
	{
		// Token: 0x0400162D RID: 5677
		Begin,
		// Token: 0x0400162E RID: 5678
		LoginFailByDataCheckCnt = 0,
		// Token: 0x0400162F RID: 5679
		LoginFailByBanCnt,
		// Token: 0x04001630 RID: 5680
		LoginFailByLoginTimeOutCnt,
		// Token: 0x04001631 RID: 5681
		InitGameFailByBanCnt,
		// Token: 0x04001632 RID: 5682
		createRoleFailByCnt,
		// Token: 0x04001633 RID: 5683
		createRoleFailByBan,
		// Token: 0x04001634 RID: 5684
		Max
	}
}
