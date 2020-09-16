using System;
using GameServer.Core.Executor;
using GameServer.Logic.BocaiSys;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class NewTimerProc : IManager
	{
		
		private NewTimerProc()
		{
		}

		
		public static NewTimerProc GetInstance()
		{
			return NewTimerProc.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool startup()
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("NewTimerProc.TimerProc", new EventHandler(this.Update)), 0, 1000);
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("NewTimerProc.TimerProc", new EventHandler(this.BigDataTimerProc)), 0, 1000);
			return true;
		}

		
		private void Update(object sender, EventArgs e)
		{
			try
			{
				BoCaiShopManager.GetInstance().Update();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		
		private void BigDataTimerProc(object sender, EventArgs e)
		{
			try
			{
				BoCaiCaiDaXiao.GetInstance().BigTimeUpData(false);
				BoCaiCaiShuZi.GetInstance().BigTimeUpData(false);
				BoCaiBuy2DBList.getInstance().BigTimeUpData();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		
		private static NewTimerProc instance = new NewTimerProc();
	}
}
