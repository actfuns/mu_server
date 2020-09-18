using System;
using System.Collections.Generic;
using System.Threading;
using Server.Tools;

namespace GameServer.Core.Executor
{
	
	internal class ExecContext
	{
		
		public ExecContext()
		{
			this.WorkThread = new Thread(new ThreadStart(this.ThreadProc));
			this.WorkThread.IsBackground = true;
			this.WorkThread.Start();
		}

		
		private void ThreadProc()
		{
			Stack<ScheduleTask> stack = new Stack<ScheduleTask>();
			for (;;)
			{
				long nowTicks = TimeUtil.NOW();
				try
				{
					stack.Clear();
					lock (this.Mutex)
					{
						for (int i = 0; i < this.TaskList.Count; i++)
						{
							if (nowTicks >= this.TaskList[i].NextTicks)
							{
								this.TaskList[i].NextTicks = nowTicks + this.TaskList[i].Periodic;
								ScheduleTask task = this.TaskList[i].Task;
								stack.Push(task);
							}
						}
					}
					foreach (ScheduleTask task in stack)
					{
                        try
						{
							task.run();
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("{0}执行时异常,{1}", task.ToString(), ex.ToString()), null, true);
						}
						long finishTicks = TimeUtil.CurrentTicksInexact;
						if (finishTicks - nowTicks > 1000L)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("{0}执行时间:{1}毫秒", task.ToString(), finishTicks - nowTicks), null, true);
						}
					}
				}
				catch
				{
				}
				int sleepMs = Math.Max(0, (int)Math.Min(TimeUtil.NOW() + 250L - nowTicks, 250L));
				Thread.Sleep(sleepMs);
			}
		}

		
		public void Add(ScheduleTask task, int periodic)
		{
			lock (this.Mutex)
			{
				this.TaskList.Add(new MyTaskContext
				{
					Periodic = (long)periodic,
					Task = task
				});
			}
		}

		
		private ReaderWriterLockSlim Mutex = new ReaderWriterLockSlim();

		
		private Thread WorkThread;

		
		public List<MyTaskContext> TaskList = new List<MyTaskContext>();

		
		private int CurrentIndex;
	}
}
