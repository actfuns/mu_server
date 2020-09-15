using System;

namespace GameDBServer.Core.Executor
{
	// Token: 0x0200001A RID: 26
	internal class PeriodicTaskHandleImpl : PeriodicTaskHandle
	{
		// Token: 0x06000062 RID: 98 RVA: 0x000043F8 File Offset: 0x000025F8
		public PeriodicTaskHandleImpl(TaskWrapper taskWrapper, ScheduleExecutor executor)
		{
			this.taskWrapper = taskWrapper;
			this.executor = executor;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004418 File Offset: 0x00002618
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

		// Token: 0x06000064 RID: 100 RVA: 0x00004470 File Offset: 0x00002670
		public bool isCanneled()
		{
			return this.canneled;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004488 File Offset: 0x00002688
		public int getExecuteCount()
		{
			return this.taskWrapper.ExecuteCount;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000044A8 File Offset: 0x000026A8
		public long getPeriodic()
		{
			return this.taskWrapper.Periodic;
		}

		// Token: 0x04000051 RID: 81
		private TaskWrapper taskWrapper;

		// Token: 0x04000052 RID: 82
		private ScheduleExecutor executor;

		// Token: 0x04000053 RID: 83
		private bool canneled = false;
	}
}
