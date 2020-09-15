using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Logic;
using Server.Tools;

namespace GameServer.Core.Executor
{
	// Token: 0x020000CB RID: 203
	public class ScheduleExecutor2
	{
		// Token: 0x06000386 RID: 902 RVA: 0x0003CA84 File Offset: 0x0003AC84
		public void start()
		{
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0003CA88 File Offset: 0x0003AC88
		public void stop()
		{
			lock (this)
			{
				foreach (KeyValuePair<ScheduleTask, Timer> t in this.TimerDict)
				{
					t.Value.Dispose();
				}
				this.TimerDict.Clear();
			}
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0003CB28 File Offset: 0x0003AD28
		public void scheduleCancle(ScheduleTask task)
		{
			lock (this)
			{
				Timer timer;
				if (this.TimerDict.TryGetValue(task, out timer))
				{
					timer.Dispose();
					this.TimerDict.Remove(task);
				}
			}
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0003CB94 File Offset: 0x0003AD94
		public void scheduleExecute(ScheduleTask task, int group, int periodic)
		{
			if (periodic < 15 || periodic > 86400000)
			{
				throw new Exception("不正确的调度时间间隔periodic = " + periodic);
			}
			lock (this)
			{
				if (periodic < 250)
				{
					group = 0;
				}
				if (group == 0)
				{
					Timer timer;
					if (!this.TimerDict.TryGetValue(task, out timer))
					{
						timer = new Timer(new TimerCallback(ScheduleExecutor2.OnTimedEvent), task, Global.GetRandomNumber(periodic / 2, periodic * 3 / 2), periodic);
						this.TimerDict.Add(task, timer);
					}
					else
					{
						timer.Change(periodic, periodic);
					}
				}
				else
				{
					ExecContext exec;
					if (!this.ThreadDict.TryGetValue(group, out exec))
					{
						exec = new ExecContext();
						this.ThreadDict[group] = exec;
					}
					exec.Add(task, periodic);
				}
			}
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0003CCBC File Offset: 0x0003AEBC
		private static void OnTimedEvent(object source)
		{
			ScheduleTask task = source as ScheduleTask;
			if (task.InternalLock.TryEnter())
			{
				bool logRunTime = false;
				long nowTicks = TimeUtil.CurrentTicksInexact;
				try
				{
					task.run();
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("{0}执行时异常,{1}", task.ToString(), ex.ToString()), null, true);
				}
				finally
				{
					logRunTime = task.InternalLock.Leave();
				}
				if (logRunTime)
				{
					long finishTicks = TimeUtil.CurrentTicksInexact;
					if (finishTicks - nowTicks > 1000L)
					{
						try
						{
							MonsterTask monsterTask = task as MonsterTask;
							if (null != monsterTask)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("{0} mapCode:{1},subMapCode:{2},执行时间:{3}毫秒", new object[]
								{
									task.ToString(),
									monsterTask.mapCode,
									monsterTask.subMapCode,
									finishTicks - nowTicks
								}), null, true);
							}
							else
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("{0}执行时间:{1}毫秒", task.ToString(), finishTicks - nowTicks), null, true);
							}
						}
						catch
						{
						}
					}
				}
			}
		}

		// Token: 0x040004DC RID: 1244
		public static ScheduleExecutor2 Instance = new ScheduleExecutor2();

		// Token: 0x040004DD RID: 1245
		private Dictionary<ScheduleTask, Timer> TimerDict = new Dictionary<ScheduleTask, Timer>();

		// Token: 0x040004DE RID: 1246
		private Dictionary<int, ExecContext> ThreadDict = new Dictionary<int, ExecContext>();
	}
}
