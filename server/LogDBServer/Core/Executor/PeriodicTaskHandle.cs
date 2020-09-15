using System;

namespace LogDBServer.Core.Executor
{
	// Token: 0x02000004 RID: 4
	public interface PeriodicTaskHandle
	{
		// Token: 0x0600000A RID: 10
		void cannel();

		// Token: 0x0600000B RID: 11
		bool isCanneled();

		// Token: 0x0600000C RID: 12
		int getExecuteCount();

		// Token: 0x0600000D RID: 13
		long getPeriodic();
	}
}
