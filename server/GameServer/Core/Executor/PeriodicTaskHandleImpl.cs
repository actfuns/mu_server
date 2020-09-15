using System;

namespace GameServer.Core.Executor
{
	// Token: 0x0200010C RID: 268
	internal class PeriodicTaskHandleImpl : PeriodicTaskHandle
	{
		// Token: 0x06000416 RID: 1046 RVA: 0x0003E14C File Offset: 0x0003C34C
		public PeriodicTaskHandleImpl(TaskWrapper taskWrapper, ScheduleExecutor executor)
		{
			this.taskWrapper = taskWrapper;
			this.executor = executor;
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x0003E16C File Offset: 0x0003C36C
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

		// Token: 0x06000418 RID: 1048 RVA: 0x0003E1C4 File Offset: 0x0003C3C4
		public bool isCanneled()
		{
			return this.canneled;
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x0003E1DC File Offset: 0x0003C3DC
		public int getExecuteCount()
		{
			return this.taskWrapper.ExecuteCount;
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x0003E1FC File Offset: 0x0003C3FC
		public long getPeriodic()
		{
			return this.taskWrapper.Periodic;
		}

		// Token: 0x0400059D RID: 1437
		private TaskWrapper taskWrapper;

		// Token: 0x0400059E RID: 1438
		private ScheduleExecutor executor;

		// Token: 0x0400059F RID: 1439
		private bool canneled = false;
	}
}
