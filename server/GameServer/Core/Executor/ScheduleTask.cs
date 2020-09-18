using System;

namespace GameServer.Core.Executor
{
	
	public interface ScheduleTask
	{
		
		
		TaskInternalLock InternalLock { get; }

		
		void run();
	}
}
