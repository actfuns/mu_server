using System;

namespace GameServer.Logic.SecondPassword
{
	// Token: 0x02000792 RID: 1938
	public enum SecondPasswordError
	{
		// Token: 0x04003E97 RID: 16023
		SecPwdVerifySuccess,
		// Token: 0x04003E98 RID: 16024
		SecPwdVerifyFailed,
		// Token: 0x04003E99 RID: 16025
		SecPwdIsNotSet,
		// Token: 0x04003E9A RID: 16026
		SecPwdCharInvalid,
		// Token: 0x04003E9B RID: 16027
		SecPwdIsNull,
		// Token: 0x04003E9C RID: 16028
		SecPwdIsTooShort,
		// Token: 0x04003E9D RID: 16029
		SecPwdIsTooLong,
		// Token: 0x04003E9E RID: 16030
		SecPwdSetSuccess,
		// Token: 0x04003E9F RID: 16031
		SecPwdDBFailed,
		// Token: 0x04003EA0 RID: 16032
		SecPwdClearSuccess
	}
}
