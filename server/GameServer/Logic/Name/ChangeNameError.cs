using System;

namespace GameServer.Logic.Name
{
	// Token: 0x0200037E RID: 894
	public enum ChangeNameError
	{
		// Token: 0x04001796 RID: 6038
		Success,
		// Token: 0x04001797 RID: 6039
		InvalidName,
		// Token: 0x04001798 RID: 6040
		DBFailed,
		// Token: 0x04001799 RID: 6041
		NoChangeNameTimes,
		// Token: 0x0400179A RID: 6042
		SelfIsBusy,
		// Token: 0x0400179B RID: 6043
		NameAlreayUsed,
		// Token: 0x0400179C RID: 6044
		NameLengthError,
		// Token: 0x0400179D RID: 6045
		NotContainRole,
		// Token: 0x0400179E RID: 6046
		NeedVerifySecPwd,
		// Token: 0x0400179F RID: 6047
		ZuanShiNotEnough,
		// Token: 0x040017A0 RID: 6048
		ServerDenied,
		// Token: 0x040017A1 RID: 6049
		BackToSelectRole
	}
}
