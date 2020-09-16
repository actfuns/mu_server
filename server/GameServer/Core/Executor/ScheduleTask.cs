using System;

namespace GameServer.Core.Executor
{
	// Token: 0x020000C7 RID: 199
	public interface ScheduleTask
	{
		// Token: 0x17000005 RID: 5
		
		TaskInternalLock InternalLock { get; }

		// Token: 0x0600037D RID: 893
		void run();
	}
}
