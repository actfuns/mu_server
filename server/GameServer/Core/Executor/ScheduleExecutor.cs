using System;
using System.Collections.Generic;
using System.Threading;

namespace GameServer.Core.Executor
{
	// Token: 0x0200010E RID: 270
	public class ScheduleExecutor
	{
		// Token: 0x06000420 RID: 1056 RVA: 0x0003E4D0 File Offset: 0x0003C6D0
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

		// Token: 0x06000421 RID: 1057 RVA: 0x0003E584 File Offset: 0x0003C784
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

		// Token: 0x06000422 RID: 1058 RVA: 0x0003E618 File Offset: 0x0003C818
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

		// Token: 0x06000423 RID: 1059 RVA: 0x0003E6FC File Offset: 0x0003C8FC
		public bool execute(ScheduleTask task)
		{
			TaskWrapper wrapper = new TaskWrapper(task, -1L, -1L);
			this.addTask(wrapper);
			return true;
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0003E724 File Offset: 0x0003C924
		public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay)
		{
			return this.scheduleExecute(task, delay, -1L);
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x0003E740 File Offset: 0x0003C940
		public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay, long periodic)
		{
			TaskWrapper wrapper = new TaskWrapper(task, delay, periodic);
			PeriodicTaskHandle handle = new PeriodicTaskHandleImpl(wrapper, this);
			this.addTask(wrapper);
			return handle;
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x0003E76C File Offset: 0x0003C96C
		internal TaskWrapper GetPreiodictTask(long ticks)
		{
			TaskWrapper result;
			lock (this.PreiodictTaskList)
			{
				if (this.PreiodictTaskList.Count == 0)
				{
					result = null;
				}
				else if (this.PreiodictTaskList[0].StartTime > ticks)
				{
					result = null;
				}
				else
				{
					TaskWrapper taskWrapper = this.PreiodictTaskList[0];
					this.PreiodictTaskList.RemoveAt(0);
					result = taskWrapper;
				}
			}
			return result;
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x0003E810 File Offset: 0x0003CA10
		internal TaskWrapper getTask()
		{
			lock (this.TaskQueue)
			{
				try
				{
					if (this.TaskQueue.Count <= 0)
					{
						return null;
					}
					TaskWrapper currentTask = this.TaskQueue.First.Value;
					this.TaskQueue.RemoveFirst();
					if (currentTask.canExecute)
					{
						return currentTask;
					}
					return null;
				}
				catch (Exception)
				{
				}
			}
			return null;
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x0003E8C0 File Offset: 0x0003CAC0
		internal int GetTaskCount()
		{
			int count;
			lock (this.TaskQueue)
			{
				count = this.TaskQueue.Count;
			}
			return count;
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0003E914 File Offset: 0x0003CB14
		internal void addTask(TaskWrapper taskWrapper)
		{
			if (taskWrapper.Periodic > 0L)
			{
				lock (this.PreiodictTaskList)
				{
					this.PreiodictTaskList.BinaryInsertAsc(taskWrapper, taskWrapper);
				}
			}
			else
			{
				lock (this.TaskQueue)
				{
					this.TaskQueue.AddLast(taskWrapper);
					taskWrapper.canExecute = true;
				}
			}
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x0003E9C8 File Offset: 0x0003CBC8
		internal void removeTask(TaskWrapper taskWrapper)
		{
			lock (this.TaskQueue)
			{
				this.TaskQueue.Remove(taskWrapper);
				taskWrapper.canExecute = false;
			}
		}

		// Token: 0x040005A5 RID: 1445
		private List<Worker> workerQueue = null;

		// Token: 0x040005A6 RID: 1446
		private List<Thread> threadQueue = null;

		// Token: 0x040005A7 RID: 1447
		private LinkedList<TaskWrapper> TaskQueue = null;

		// Token: 0x040005A8 RID: 1448
		private List<TaskWrapper> PreiodictTaskList = new List<TaskWrapper>();

		// Token: 0x040005A9 RID: 1449
		private int maxThreadNum = 0;
	}
}
