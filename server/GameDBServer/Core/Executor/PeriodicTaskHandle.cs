using System;

namespace GameDBServer.Core.Executor
{
	// Token: 0x02000019 RID: 25
	public interface PeriodicTaskHandle
	{
		// Token: 0x0600005E RID: 94
		void cannel();

		// Token: 0x0600005F RID: 95
		bool isCanneled();

		// Token: 0x06000060 RID: 96
		int getExecuteCount();

		// Token: 0x06000061 RID: 97
		long getPeriodic();
	}
}
