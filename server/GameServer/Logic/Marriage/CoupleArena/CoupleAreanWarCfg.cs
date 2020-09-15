using System;
using System.Collections.Generic;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000359 RID: 857
	public class CoupleAreanWarCfg
	{
		// Token: 0x040016A7 RID: 5799
		public int Id;

		// Token: 0x040016A8 RID: 5800
		public int MapCode;

		// Token: 0x040016A9 RID: 5801
		public List<CoupleAreanWarCfg.TimePoint> TimePoints;

		// Token: 0x040016AA RID: 5802
		public int WaitSec;

		// Token: 0x040016AB RID: 5803
		public int FightSec;

		// Token: 0x040016AC RID: 5804
		public int ClearSec;

		// Token: 0x0200035A RID: 858
		public class TimePoint
		{
			// Token: 0x040016AD RID: 5805
			public int Weekday;

			// Token: 0x040016AE RID: 5806
			public long DayStartTicks;

			// Token: 0x040016AF RID: 5807
			public long DayEndTicks;
		}
	}
}
