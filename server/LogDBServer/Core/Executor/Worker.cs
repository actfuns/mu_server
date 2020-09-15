using System;
using System.Threading;
using Server.Tools;

namespace LogDBServer.Core.Executor
{
	// Token: 0x02000006 RID: 6
	internal class Worker
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002201 File Offset: 0x00000401
		public Worker(ScheduleExecutor executor)
		{
			this.executor = executor;
		}

		// Token: 0x17000005 RID: 5
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00002221 File Offset: 0x00000421
		public Thread CurrentThread
		{
			set
			{
				this.currentThread = value;
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000222C File Offset: 0x0000042C
		private TaskWrapper getCanExecuteTask()
		{
			int getNum = 0;
			TaskWrapper TaskWrapper;
			while (null != (TaskWrapper = this.executor.getTask()))
			{
				if (TimeUtil.NOW() >= TaskWrapper.StartTime)
				{
					return TaskWrapper;
				}
				if (TaskWrapper.canExecute)
				{
					this.executor.addTask(TaskWrapper);
				}
				getNum++;
				if (getNum >= 1000)
				{
					break;
				}
			}
			return null;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000022A8 File Offset: 0x000004A8
		public void work()
		{
			TaskWrapper TaskWrapper = null;
			for (;;)
			{
				TaskWrapper = this.getCanExecuteTask();
				if (null == TaskWrapper)
				{
					try
					{
						Thread.Sleep(5);
					}
					catch (ThreadInterruptedException)
					{
					}
				}
				else
				{
					try
					{
						if (TaskWrapper != null && null != TaskWrapper.CurrentTask)
						{
							if (TaskWrapper.canExecute)
							{
								try
								{
									TaskWrapper.CurrentTask.run();
								}
								catch (Exception ex)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("异步调度任务执行错误: {0}", ex));
								}
							}
							if (TaskWrapper.Periodic > 0L && TaskWrapper.canExecute)
							{
								TaskWrapper.resetStartTime();
								this.executor.addTask(TaskWrapper);
								TaskWrapper.addExecuteCount();
							}
						}
					}
					catch (Exception ex)
					{
						DataHelper.WriteFormatExceptionLog(ex, "异步调度任务执行异常", false, false);
					}
				}
			}
		}

		// Token: 0x04000009 RID: 9
		private ScheduleExecutor executor = null;

		// Token: 0x0400000A RID: 10
		private Thread currentThread = null;
	}
}
