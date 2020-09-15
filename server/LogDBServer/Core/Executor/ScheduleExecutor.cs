using System;
using System.Collections.Generic;
using System.Threading;

namespace LogDBServer.Core.Executor
{
	// Token: 0x02000007 RID: 7
	public class ScheduleExecutor
	{
		// Token: 0x06000017 RID: 23 RVA: 0x000023BC File Offset: 0x000005BC
		public ScheduleExecutor(int maxThreadNum)
		{
			this.maxThreadNum = maxThreadNum;
			this.threadQueue = new List<Thread>();
			this.workerQueue = new List<Worker>();
			this.TaskQueue = new LinkedList<TaskWrapper>();
			for (int i = 0; i < maxThreadNum; i++)
			{
				Worker worker = new Worker(this);
				Thread thread = new Thread(new ThreadStart(worker.work));
				worker.CurrentThread = thread;
				this.workerQueue.Add(worker);
				this.threadQueue.Add(thread);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002464 File Offset: 0x00000664
		public void start()
		{
			lock (this.threadQueue)
			{
				foreach (Thread thread in this.threadQueue)
				{
					thread.Start();
				}
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000024F8 File Offset: 0x000006F8
		public void stop()
		{
			lock (this.threadQueue)
			{
				foreach (Thread thread in this.threadQueue)
				{
					thread.Abort();
				}
				this.threadQueue.Clear();
			}
			lock (this.workerQueue)
			{
				this.workerQueue.Clear();
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000025DC File Offset: 0x000007DC
		public bool execute(ScheduleTask task)
		{
			TaskWrapper wrapper = new TaskWrapper(task, -1L, -1L);
			this.addTask(wrapper);
			return true;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002604 File Offset: 0x00000804
		public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay)
		{
			return this.scheduleExecute(task, delay, -1L);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002620 File Offset: 0x00000820
		public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay, long periodic)
		{
			TaskWrapper wrapper = new TaskWrapper(task, delay, periodic);
			PeriodicTaskHandle handle = new PeriodicTaskHandleImpl(wrapper, this);
			this.addTask(wrapper);
			return handle;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000264C File Offset: 0x0000084C
		internal TaskWrapper getTask()
		{
			TaskWrapper result;
			lock (this.TaskQueue)
			{
				if (this.TaskQueue.Count == 0)
				{
					result = null;
				}
				else
				{
					TaskWrapper currentTask = this.TaskQueue.First.Value;
					this.TaskQueue.RemoveFirst();
					if (currentTask.canExecute)
					{
						result = currentTask;
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000026E4 File Offset: 0x000008E4
		internal void addTask(TaskWrapper taskWrapper)
		{
			lock (this.TaskQueue)
			{
				this.TaskQueue.AddLast(taskWrapper);
				taskWrapper.canExecute = true;
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002740 File Offset: 0x00000940
		internal void removeTask(TaskWrapper taskWrapper)
		{
			lock (this.TaskQueue)
			{
				this.TaskQueue.Remove(taskWrapper);
				taskWrapper.canExecute = false;
			}
		}

		// Token: 0x0400000B RID: 11
		private List<Worker> workerQueue = null;

		// Token: 0x0400000C RID: 12
		private List<Thread> threadQueue = null;

		// Token: 0x0400000D RID: 13
		private LinkedList<TaskWrapper> TaskQueue = null;

		// Token: 0x0400000E RID: 14
		private int maxThreadNum = 0;
	}
}
