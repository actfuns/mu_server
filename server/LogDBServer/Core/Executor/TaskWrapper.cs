using System;

namespace LogDBServer.Core.Executor
{
	// Token: 0x02000003 RID: 3
	internal class TaskWrapper
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public TaskWrapper(ScheduleTask task, long delay, long periodic)
		{
			this.currentTask = task;
			this.startTime = TimeUtil.NOW() + delay;
			this.periodic = periodic;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x000020A0 File Offset: 0x000002A0
		public ScheduleTask CurrentTask
		{
			get
			{
				return this.currentTask;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020B8 File Offset: 0x000002B8
		public long StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020D0 File Offset: 0x000002D0
		public void resetStartTime()
		{
			this.startTime += this.periodic;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020E8 File Offset: 0x000002E8
		public long Periodic
		{
			get
			{
				return this.periodic;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002100 File Offset: 0x00000300
		public void release()
		{
			this.currentTask = null;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000210A File Offset: 0x0000030A
		public void addExecuteCount()
		{
			this.executeCount++;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000009 RID: 9 RVA: 0x0000211C File Offset: 0x0000031C
		public int ExecuteCount
		{
			get
			{
				return this.executeCount;
			}
		}

		// Token: 0x04000001 RID: 1
		private ScheduleTask currentTask;

		// Token: 0x04000002 RID: 2
		private long startTime = -1L;

		// Token: 0x04000003 RID: 3
		private long periodic = -1L;

		// Token: 0x04000004 RID: 4
		private int executeCount = 0;

		// Token: 0x04000005 RID: 5
		public bool canExecute = true;
	}
}
