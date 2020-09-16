using System;

namespace LogDBServer.Core.Executor
{
	
	public interface PeriodicTaskHandle
	{
		
		void cannel();

		
		bool isCanneled();

		
		int getExecuteCount();

		
		long getPeriodic();
	}
}
