using System;
using System.Threading;
using Server.Tools;

namespace GameDBServer.Core.Executor
{
	
	internal class Worker
	{
		
		public Worker(ScheduleExecutor executor)
		{
			this.executor = executor;
		}

		
		
		public Thread CurrentThread
		{
			set
			{
				this.currentThread = value;
			}
		}

		
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

		
		private ScheduleExecutor executor = null;

		
		private Thread currentThread = null;
	}
}
