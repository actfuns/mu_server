using System;
using System.Collections.Generic;
using System.Threading;

namespace GameDBServer.Core.Executor
{
	// Token: 0x0200001C RID: 28
	public class ScheduleExecutor
	{
		// Token: 0x0600006B RID: 107 RVA: 0x00004680 File Offset: 0x00002880
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

		// Token: 0x0600006C RID: 108 RVA: 0x00004728 File Offset: 0x00002928
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

		// Token: 0x0600006D RID: 109 RVA: 0x000047BC File Offset: 0x000029BC
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

		// Token: 0x0600006E RID: 110 RVA: 0x000048A0 File Offset: 0x00002AA0
		public bool execute(ScheduleTask task)
		{
			TaskWrapper wrapper = new TaskWrapper(task, -1L, -1L);
			this.addTask(wrapper);
			return true;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000048C8 File Offset: 0x00002AC8
		public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay)
		{
			return this.scheduleExecute(task, delay, -1L);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000048E4 File Offset: 0x00002AE4
		public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay, long periodic)
		{
			TaskWrapper wrapper = new TaskWrapper(task, delay, periodic);
			PeriodicTaskHandle handle = new PeriodicTaskHandleImpl(wrapper, this);
			this.addTask(wrapper);
			return handle;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004910 File Offset: 0x00002B10
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

		// Token: 0x06000072 RID: 114 RVA: 0x000049A8 File Offset: 0x00002BA8
		internal void addTask(TaskWrapper taskWrapper)
		{
			lock (this.TaskQueue)
			{
				this.TaskQueue.AddLast(taskWrapper);
				taskWrapper.canExecute = true;
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004A04 File Offset: 0x00002C04
		internal void removeTask(TaskWrapper taskWrapper)
		{
			lock (this.TaskQueue)
			{
				this.TaskQueue.Remove(taskWrapper);
				taskWrapper.canExecute = false;
			}
		}

		// Token: 0x04000056 RID: 86
		private List<Worker> workerQueue = null;

		// Token: 0x04000057 RID: 87
		private List<Thread> threadQueue = null;

		// Token: 0x04000058 RID: 88
		private LinkedList<TaskWrapper> TaskQueue = null;

		// Token: 0x04000059 RID: 89
		private int maxThreadNum = 0;
	}
}
