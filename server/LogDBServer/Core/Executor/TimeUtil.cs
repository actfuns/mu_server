using System;

namespace LogDBServer.Core.Executor
{
	// Token: 0x02000008 RID: 8
	public class TimeUtil
	{
		// Token: 0x06000020 RID: 32 RVA: 0x0000279C File Offset: 0x0000099C
		public static long NOW()
		{
			return DateTime.Now.Ticks / 10000L;
		}

		// Token: 0x0400000F RID: 15
		public static readonly long MILLISECOND = 1L;

		// Token: 0x04000010 RID: 16
		public static readonly long SECOND = 1000L * TimeUtil.MILLISECOND;

		// Token: 0x04000011 RID: 17
		public static readonly long MINITE = 60L * TimeUtil.SECOND;

		// Token: 0x04000012 RID: 18
		public static readonly long HOUR = 60L * TimeUtil.MINITE;

		// Token: 0x04000013 RID: 19
		public static readonly long DAY = 24L * TimeUtil.HOUR;
	}
}
