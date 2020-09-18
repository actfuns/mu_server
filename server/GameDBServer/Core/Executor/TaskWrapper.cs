using System;

namespace GameDBServer.Core.Executor
{
	
	internal class TaskWrapper
	{
		
		public TaskWrapper(ScheduleTask task, long delay, long periodic)
		{
			this.currentTask = task;
			this.startTime = TimeUtil.NOW() + delay;
			this.periodic = periodic;
		}

		
		
		public ScheduleTask CurrentTask
		{
			get
			{
				return this.currentTask;
			}
		}

		
		
		public long StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		
		public void resetStartTime()
		{
			this.startTime += this.periodic;
		}

		
		
		public long Periodic
		{
			get
			{
				return this.periodic;
			}
		}

		
		public void release()
		{
			this.currentTask = null;
		}

		
		public void addExecuteCount()
		{
			this.executeCount++;
		}

		
		
		public int ExecuteCount
		{
			get
			{
				return this.executeCount;
			}
		}

		
		private ScheduleTask currentTask;

		
		private long startTime = -1L;

		
		private long periodic = -1L;

		
		private int executeCount = 0;

		
		public bool canExecute = true;
	}
}
