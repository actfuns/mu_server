using System;
using System.Collections.Generic;

namespace GameServer.Core.Executor
{
	// Token: 0x0200010A RID: 266
	internal class TaskWrapper : IComparer<TaskWrapper>
	{
		// Token: 0x06000409 RID: 1033 RVA: 0x0003E024 File Offset: 0x0003C224
		public TaskWrapper(ScheduleTask task, long delay, long periodic)
		{
			this.currentTask = task;
			this.startTime = TimeUtil.NOW() + delay;
			this.periodic = periodic;
		}

		// Token: 0x17000013 RID: 19
		
		public ScheduleTask CurrentTask
		{
			get
			{
				return this.currentTask;
			}
		}

		// Token: 0x17000014 RID: 20
		
		public long StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0003E0A4 File Offset: 0x0003C2A4
		public void resetStartTime()
		{
			this.startTime += this.periodic;
		}

		// Token: 0x17000015 RID: 21
		
		public long Periodic
		{
			get
			{
				return this.periodic;
			}
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x0003E0D4 File Offset: 0x0003C2D4
		public void release()
		{
			this.currentTask = null;
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x0003E0DE File Offset: 0x0003C2DE
		public void addExecuteCount()
		{
			this.executeCount++;
		}

		// Token: 0x17000016 RID: 22
		
		public int ExecuteCount
		{
			get
			{
				return this.executeCount;
			}
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x0003E108 File Offset: 0x0003C308
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

		// Token: 0x04000598 RID: 1432
		private ScheduleTask currentTask;

		// Token: 0x04000599 RID: 1433
		private long startTime = -1L;

		// Token: 0x0400059A RID: 1434
		private long periodic = -1L;

		// Token: 0x0400059B RID: 1435
		private int executeCount = 0;

		// Token: 0x0400059C RID: 1436
		public bool canExecute = true;
	}
}
