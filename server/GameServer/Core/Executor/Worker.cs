using System;
using System.Threading;
using Server.Tools;

namespace GameServer.Core.Executor
{
	// Token: 0x0200010D RID: 269
	internal class Worker
	{
		// Token: 0x0600041B RID: 1051 RVA: 0x0003E219 File Offset: 0x0003C419
		public Worker(ScheduleExecutor executor)
		{
			this.executor = executor;
		}

		// Token: 0x17000017 RID: 23
		
		public Thread CurrentThread
		{
			set
			{
				this.currentThread = value;
			}
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x0003E24C File Offset: 0x0003C44C
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

		// Token: 0x0600041E RID: 1054 RVA: 0x0003E31C File Offset: 0x0003C51C
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

		// Token: 0x040005A0 RID: 1440
		private ScheduleExecutor executor = null;

		// Token: 0x040005A1 RID: 1441
		private Thread currentThread = null;

		// Token: 0x040005A2 RID: 1442
		private static int nThreadCount = 0;

		// Token: 0x040005A3 RID: 1443
		private int nThreadOrder = 0;

		// Token: 0x040005A4 RID: 1444
		private static object _lock = new object();
	}
}
