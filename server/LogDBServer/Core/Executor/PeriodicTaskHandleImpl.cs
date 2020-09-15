using System;

namespace LogDBServer.Core.Executor
{
	// Token: 0x02000005 RID: 5
	internal class PeriodicTaskHandleImpl : PeriodicTaskHandle
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00002134 File Offset: 0x00000334
		public PeriodicTaskHandleImpl(TaskWrapper taskWrapper, ScheduleExecutor executor)
		{
			this.taskWrapper = taskWrapper;
			this.executor = executor;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002154 File Offset: 0x00000354
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

		// Token: 0x06000010 RID: 16 RVA: 0x000021AC File Offset: 0x000003AC
		public bool isCanneled()
		{
			return this.canneled;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000021C4 File Offset: 0x000003C4
		public int getExecuteCount()
		{
			return this.taskWrapper.ExecuteCount;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000021E4 File Offset: 0x000003E4
		public long getPeriodic()
		{
			return this.taskWrapper.Periodic;
		}

		// Token: 0x04000006 RID: 6
		private TaskWrapper taskWrapper;

		// Token: 0x04000007 RID: 7
		private ScheduleExecutor executor;

		// Token: 0x04000008 RID: 8
		private bool canneled = false;
	}
}
