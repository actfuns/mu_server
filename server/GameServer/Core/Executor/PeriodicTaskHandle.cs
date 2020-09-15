using System;

namespace GameServer.Core.Executor
{
	// Token: 0x0200010B RID: 267
	public interface PeriodicTaskHandle
	{
		// Token: 0x06000412 RID: 1042
		void cannel();

		// Token: 0x06000413 RID: 1043
		bool isCanneled();

		// Token: 0x06000414 RID: 1044
		int getExecuteCount();

		// Token: 0x06000415 RID: 1045
		long getPeriodic();
	}
}
