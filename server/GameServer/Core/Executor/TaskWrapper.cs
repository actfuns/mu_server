using System;
using System.Collections.Generic;

namespace GameServer.Core.Executor
{
	
	internal class TaskWrapper : IComparer<TaskWrapper>
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

		
		public int Compare(TaskWrapper x, TaskWrapper y)
		{
			long ret = x.startTime - y.startTime;
			int result;
			if (ret == 0L)
			{
				result = 0;
			}
			else if (ret > 0L)
			{
				result = 1;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		
		private ScheduleTask currentTask;

		
		private long startTime = -1L;

		
		private long periodic = -1L;

		
		private int executeCount = 0;

		
		public bool canExecute = true;
	}
}
