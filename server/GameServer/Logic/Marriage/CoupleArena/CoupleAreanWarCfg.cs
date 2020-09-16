using System;
using System.Collections.Generic;

namespace GameServer.Logic.Marriage.CoupleArena
{
	
	public class CoupleAreanWarCfg
	{
		
		public int Id;

		
		public int MapCode;

		
		public List<CoupleAreanWarCfg.TimePoint> TimePoints;

		
		public int WaitSec;

		
		public int FightSec;

		
		public int ClearSec;

		
		public class TimePoint
		{
			
			public int Weekday;

			
			public long DayStartTicks;

			
			public long DayEndTicks;
		}
	}
}
