using System;

namespace GameDBServer.Logic.Name
{
	// Token: 0x0200014C RID: 332
	public enum ChangeNameError
	{
		// Token: 0x0400082C RID: 2092
		Success,
		// Token: 0x0400082D RID: 2093
		InvalidName,
		// Token: 0x0400082E RID: 2094
		DBFailed,
		// Token: 0x0400082F RID: 2095
		NoChangeNameTimes,
		// Token: 0x04000830 RID: 2096
		SelfIsBusy,
		// Token: 0x04000831 RID: 2097
		NameAlreayUsed,
		// Token: 0x04000832 RID: 2098
		NameLengthError,
		// Token: 0x04000833 RID: 2099
		NotContainRole,
		// Token: 0x04000834 RID: 2100
		NeedVerifySecPwd,
		// Token: 0x04000835 RID: 2101
		ZuanShiNotEnough,
		// Token: 0x04000836 RID: 2102
		ServerDenied,
		// Token: 0x04000837 RID: 2103
		BackToSelectRole
	}
}
