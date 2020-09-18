using System;
using System.Threading;
using Server.Tools;

namespace GameServer.Core.Executor
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

		
		private TaskWrapper getCanExecuteTask(long ticks)
		{
			TaskWrapper taskWrapper = this.executor.GetPreiodictTask(ticks);
			TaskWrapper result;
			if (null != taskWrapper)
			{
				result = taskWrapper;
			}
			else
			{
				int getNum = 0;
				int nMaxProcCount = 200;
				int nTaskCount = this.executor.GetTaskCount();
				if (nTaskCount == 0)
				{
					result = null;
				}
				else
				{
					if (nTaskCount < nMaxProcCount)
					{
						nMaxProcCount = nTaskCount;
					}
					while (null != (taskWrapper = this.executor.getTask()))
					{
						if (ticks >= taskWrapper.StartTime)
						{
							return taskWrapper;
						}
						if (taskWrapper.canExecute)
						{
							this.executor.addTask(taskWrapper);
						}
						getNum++;
						if (getNum >= nMaxProcCount)
						{
							break;
						}
					}
					result = null;
				}
			}
			return result;
		}

		
		public void work()
		{
			lock (Worker._lock)
			{
				Worker.nThreadCount++;
				this.nThreadOrder = Worker.nThreadCount;
			}
			TaskWrapper taskWrapper = null;
			int lastTickCount = int.MinValue;
			while (!Program.NeedExitServer)
			{
				int tickCount = Environment.TickCount;
				if (tickCount <= lastTickCount + 5)
				{
					if (lastTickCount <= 0 || tickCount >= 0)
					{
						Thread.Sleep(5);
						continue;
					}
				}
				lastTickCount = tickCount;
				long ticks = TimeUtil.NOW();
				for (;;)
				{
					try
					{
						taskWrapper = this.getCanExecuteTask(ticks);
						if (taskWrapper == null || null == taskWrapper.CurrentTask)
						{
							break;
						}
						if (taskWrapper.canExecute)
						{
							try
							{
								taskWrapper.CurrentTask.run();
							}
							catch (Exception ex)
							{
								DataHelper.WriteFormatExceptionLog(ex, "异步调度任务执行异常", false, false);
							}
						}
						if (taskWrapper.Periodic > 0L && taskWrapper.canExecute)
						{
							taskWrapper.resetStartTime();
							this.executor.addTask(taskWrapper);
							taskWrapper.addExecuteCount();
						}
					}
					catch (Exception)
					{
					}
				}
			}
			SysConOut.WriteLine(string.Format("ScheduleTask Worker{0}退出...", this.nThreadOrder));
		}

		
		private ScheduleExecutor executor = null;

		
		private Thread currentThread = null;

		
		private static int nThreadCount = 0;

		
		private int nThreadOrder = 0;

		
		private static object _lock = new object();
	}
}
