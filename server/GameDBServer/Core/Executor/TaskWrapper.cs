using System;

namespace GameDBServer.Core.Executor
{
	// Token: 0x02000018 RID: 24
	internal class TaskWrapper
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00004314 File Offset: 0x00002514
		public TaskWrapper(ScheduleTask task, long delay, long periodic)
		{
			this.currentTask = task;
			this.startTime = TimeUtil.NOW() + delay;
			this.periodic = periodic;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00004364 File Offset: 0x00002564
		public ScheduleTask CurrentTask
		{
			get
			{
				return this.currentTask;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000058 RID: 88 RVA: 0x0000437C File Offset: 0x0000257C
		public long StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004394 File Offset: 0x00002594
		public void resetStartTime()
		{
			this.startTime += this.periodic;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600005A RID: 90 RVA: 0x000043AC File Offset: 0x000025AC
		public long Periodic
		{
			get
			{
				return this.periodic;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000043C4 File Offset: 0x000025C4
		public void release()
		{
			this.currentTask = null;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000043CE File Offset: 0x000025CE
		public void addExecuteCount()
		{
			this.executeCount++;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600005D RID: 93 RVA: 0x000043E0 File Offset: 0x000025E0
		public int ExecuteCount
		{
			get
			{
				return this.executeCount;
			}
		}

		// Token: 0x0400004C RID: 76
		private ScheduleTask currentTask;

		// Token: 0x0400004D RID: 77
		private long startTime = -1L;

		// Token: 0x0400004E RID: 78
		private long periodic = -1L;

		// Token: 0x0400004F RID: 79
		private int executeCount = 0;

		// Token: 0x04000050 RID: 80
		public bool canExecute = true;
	}
}
