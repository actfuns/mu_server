using System;
using GameServer.Core.Executor;
using GameServer.Logic.BocaiSys;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020000A9 RID: 169
	public class NewTimerProc : IManager
	{
		// Token: 0x060002A9 RID: 681 RVA: 0x0002DB54 File Offset: 0x0002BD54
		private NewTimerProc()
		{
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0002DB60 File Offset: 0x0002BD60
		public static NewTimerProc GetInstance()
		{
			return NewTimerProc.instance;
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0002DB78 File Offset: 0x0002BD78
		public bool initialize()
		{
			return true;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0002DB8C File Offset: 0x0002BD8C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0002DBA0 File Offset: 0x0002BDA0
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0002DBB4 File Offset: 0x0002BDB4
		public bool startup()
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("NewTimerProc.TimerProc", new EventHandler(this.Update)), 0, 1000);
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("NewTimerProc.TimerProc", new EventHandler(this.BigDataTimerProc)), 0, 1000);
			return true;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0002DC18 File Offset: 0x0002BE18
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

		// Token: 0x060002B0 RID: 688 RVA: 0x0002DC68 File Offset: 0x0002BE68
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

		// Token: 0x040003F7 RID: 1015
		private static NewTimerProc instance = new NewTimerProc();
	}
}
