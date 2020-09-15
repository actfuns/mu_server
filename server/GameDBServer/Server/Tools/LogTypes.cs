using System;

namespace Server.Tools
{
	// Token: 0x02000219 RID: 537
	public enum LogTypes
	{
		// Token: 0x0400122F RID: 4655
		Ignore = -1,
		// Token: 0x04001230 RID: 4656
		Info,
		// Token: 0x04001231 RID: 4657
		Warning,
		// Token: 0x04001232 RID: 4658
		Error,
		// Token: 0x04001233 RID: 4659
		SQL,
		// Token: 0x04001234 RID: 4660
		Exception,
		// Token: 0x04001235 RID: 4661
		Trace,
		// Token: 0x04001236 RID: 4662
		DataCheck = 80,
		// Token: 0x04001237 RID: 4663
		TotalUserMoney = 100,
		// Token: 0x04001238 RID: 4664
		Fatal = 1000
	}
}
