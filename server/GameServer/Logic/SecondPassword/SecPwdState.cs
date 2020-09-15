using System;
using GameServer.Core.Executor;

namespace GameServer.Logic.SecondPassword
{
	// Token: 0x02000793 RID: 1939
	public class SecPwdState
	{
		// Token: 0x04003EA1 RID: 16033
		public string SecPwd = "";

		// Token: 0x04003EA2 RID: 16034
		public DateTime AuthDeadTime = TimeUtil.NowDateTime();

		// Token: 0x04003EA3 RID: 16035
		public bool NeedVerify = false;
	}
}
