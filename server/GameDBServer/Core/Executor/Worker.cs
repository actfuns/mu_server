using System;
using System.Threading;
using Server.Tools;

namespace GameDBServer.Core.Executor
{
	// Token: 0x0200001B RID: 27
	internal class Worker
	{
		// Token: 0x06000067 RID: 103 RVA: 0x000044C5 File Offset: 0x000026C5
		public Worker(ScheduleExecutor executor)
		{
			this.executor = executor;
		}

		// Token: 0x17000009 RID: 9
		// (set) Token: 0x06000068 RID: 104 RVA: 0x000044E5 File Offset: 0x000026E5
		public Thread CurrentThread
		{
			set
			{
				this.currentThread = value;
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000044F0 File Offset: 0x000026F0
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

		// Token: 0x0600006A RID: 106 RVA: 0x0000456C File Offset: 0x0000276C
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
									LogManager.WriteLog(LogTypes.Error, string.Format("异步调度任务执行错误: {0}", ex), null, true);
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

		// Token: 0x04000054 RID: 84
		private ScheduleExecutor executor = null;

		// Token: 0x04000055 RID: 85
		private Thread currentThread = null;
	}
}
