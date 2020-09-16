using System;

namespace LogDBServer.Core.Executor
{
	
	public class TimeUtil
	{
		
		public static long NOW()
		{
			return DateTime.Now.Ticks / 10000L;
		}

		
		public static readonly long MILLISECOND = 1L;

		
		public static readonly long SECOND = 1000L * TimeUtil.MILLISECOND;

		
		public static readonly long MINITE = 60L * TimeUtil.SECOND;

		
		public static readonly long HOUR = 60L * TimeUtil.MINITE;

		
		public static readonly long DAY = 24L * TimeUtil.HOUR;
	}
}
