using System;

namespace GameServer.Core.Executor
{
	
	internal class PeriodicTaskHandleImpl : PeriodicTaskHandle
	{
		
		public PeriodicTaskHandleImpl(TaskWrapper taskWrapper, ScheduleExecutor executor)
		{
			this.taskWrapper = taskWrapper;
			this.executor = executor;
		}

		
		public void cannel()
		{
			if (!this.canneled)
			{
				if (this.executor != null && null != this.taskWrapper)
				{
					this.executor.removeTask(this.taskWrapper);
					this.executor = null;
				}
				this.canneled = true;
			}
		}

		
		public bool isCanneled()
		{
			return this.canneled;
		}

		
		public int getExecuteCount()
		{
			return this.taskWrapper.ExecuteCount;
		}

		
		public long getPeriodic()
		{
			return this.taskWrapper.Periodic;
		}

		
		private TaskWrapper taskWrapper;

		
		private ScheduleExecutor executor;

		
		private bool canneled = false;
	}
}
