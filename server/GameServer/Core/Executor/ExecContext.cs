using System;
using System.Collections.Generic;
using System.Threading;
using Server.Tools;

namespace GameServer.Core.Executor
{
	// Token: 0x020000CA RID: 202
	internal class ExecContext
	{
		// Token: 0x06000383 RID: 899 RVA: 0x0003C7B0 File Offset: 0x0003A9B0
		public ExecContext()
		{
			this.WorkThread = new Thread(new ThreadStart(this.ThreadProc));
			this.WorkThread.IsBackground = true;
			this.WorkThread.Start();
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0003C80C File Offset: 0x0003AA0C
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

		// Token: 0x06000385 RID: 901 RVA: 0x0003CA1C File Offset: 0x0003AC1C
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

		// Token: 0x040004D8 RID: 1240
		private ReaderWriterLockSlim Mutex = new ReaderWriterLockSlim();

		// Token: 0x040004D9 RID: 1241
		private Thread WorkThread;

		// Token: 0x040004DA RID: 1242
		public List<MyTaskContext> TaskList = new List<MyTaskContext>();

		// Token: 0x040004DB RID: 1243
		private int CurrentIndex;
	}
}
