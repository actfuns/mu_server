using System;

namespace Server.Data
{
	// Token: 0x02000142 RID: 322
	public enum GiftCodeResultType
	{
		// Token: 0x04000738 RID: 1848
		Default,
		// Token: 0x04000739 RID: 1849
		Success,
		// Token: 0x0400073A RID: 1850
		EnoUserOrRole = -1,
		// Token: 0x0400073B RID: 1851
		EAware = -2,
		// Token: 0x0400073C RID: 1852
		Fail = -3,
		// Token: 0x0400073D RID: 1853
		Exception = -4
	}
}
