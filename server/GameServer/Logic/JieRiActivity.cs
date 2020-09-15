using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x0200002D RID: 45
	public class JieRiActivity : Activity
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00007814 File Offset: 0x00005A14
		public override bool InActivityTime()
		{
			DateTime startDay = Global.GetJieriStartDay();
			DateTime now = TimeUtil.NowDateTime();
			return now >= startDay && now <= startDay.AddDays((double)Global.GetJieriDaysNum()) && base.InActivityTime();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000785C File Offset: 0x00005A5C
		public override bool InAwardTime()
		{
			DateTime startDay = Global.GetJieriStartDay();
			DateTime now = TimeUtil.NowDateTime();
			return now >= startDay && now <= startDay.AddDays((double)Global.GetJieriDaysNum()) && base.InAwardTime();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000078A4 File Offset: 0x00005AA4
		public new bool CanGiveAward()
		{
			return this.InAwardTime();
		}
	}
}
