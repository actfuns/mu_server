using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class JieRiActivity : Activity
	{
		
		public override bool InActivityTime()
		{
			DateTime startDay = Global.GetJieriStartDay();
			DateTime now = TimeUtil.NowDateTime();
			return now >= startDay && now <= startDay.AddDays((double)Global.GetJieriDaysNum()) && base.InActivityTime();
		}

		
		public override bool InAwardTime()
		{
			DateTime startDay = Global.GetJieriStartDay();
			DateTime now = TimeUtil.NowDateTime();
			return now >= startDay && now <= startDay.AddDays((double)Global.GetJieriDaysNum()) && base.InAwardTime();
		}

		
		public new bool CanGiveAward()
		{
			return this.InAwardTime();
		}
	}
}
