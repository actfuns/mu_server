using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace GameDBServer.Core
{
	// Token: 0x02000026 RID: 38
	public class TimeUtil
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00004E94 File Offset: 0x00003094
		public static long CurrentTicksInexact
		{
			get
			{
				return TimeUtil.CurrentTicks;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004EAC File Offset: 0x000030AC
		static TimeUtil()
		{
			TimeUtil.UnixStartTicks = TimeUtil.UnixStartDateTime.Ticks / 10000L;
			TimeUtil.Before1970Ticks = TimeUtil.UnixStartTicks;
			TimeUtil.Init();
			TimeUtil.TryInitMMTimer();
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004FA3 File Offset: 0x000031A3
		public static void TryInitMMTimer()
		{
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004FA8 File Offset: 0x000031A8
		public static void WaitStart()
		{
			if (TimeUtil.waitStartEvent.WaitOne(5000))
			{
				long c = TimeUtil.CurrentTicks;
				for (int i = 0; i < 1000; i++)
				{
					while (c == Thread.VolatileRead(ref TimeUtil.CurrentTicks))
					{
						Thread.Yield();
					}
					c = TimeUtil.CurrentTicks;
				}
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000500C File Offset: 0x0000320C
		public static DateTime SetTime(string timeStr)
		{
			DateTime dt;
			DateTime result;
			if (DateTime.TryParse(timeStr, out dt))
			{
				TimeUtil.CorrectTimeSecs = (int)(dt - DateTime.Now).TotalSeconds;
				TimeUtil.CurrentTickCount = 0;
				result = dt;
			}
			else
			{
				result = TimeUtil._Now;
			}
			return result;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005058 File Offset: 0x00003258
		public static bool AsyncNetTicks(long nowTicks, long netTicks)
		{
			long subTicks = netTicks - nowTicks;
			bool result;
			if (Math.Abs(subTicks - TimeUtil.CorrectNetTicks) > TimeUtil.MaxTimeDriftTicks)
			{
				TimeUtil.TimeDriftTicks = nowTicks;
				TimeUtil.CorrectNetTicks = nowTicks;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000509C File Offset: 0x0000329C
		public static long NOW()
		{
			long result;
			if (!TimeUtil.UpdateByTimer)
			{
				int tickCount = Environment.TickCount;
				if (tickCount != TimeUtil.CurrentTickCount)
				{
					DateTime now = DateTime.Now;
					long ticks = now.Ticks - TimeUtil._Now.Ticks;
					if (ticks < 0L && ticks > -3000000000L)
					{
						return TimeUtil.CurrentTicks;
					}
					if (TimeUtil.CorrectTimeSecs != 0)
					{
						now = now.AddSeconds((double)TimeUtil.CorrectTimeSecs);
					}
					TimeUtil._Now = now;
					TimeUtil.CurrentTickCount = tickCount;
					TimeUtil.CurrentTicks = now.Ticks / 10000L;
				}
				result = TimeUtil.CurrentTicks;
			}
			else
			{
				result = Thread.VolatileRead(ref TimeUtil.CurrentTicks);
			}
			return result;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000516C File Offset: 0x0000336C
		public static long TimeStamp()
		{
			return Convert.ToInt64((TimeUtil._Now - new DateTime(1970, 1, 1)).TotalSeconds);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000051A4 File Offset: 0x000033A4
		public static long NowRealTime()
		{
			return DateTime.Now.Ticks / 10000L;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000051CC File Offset: 0x000033CC
		public static DateTime NowDateTime()
		{
			if (!TimeUtil.UpdateByTimer)
			{
				int tickCount = Environment.TickCount;
				if (tickCount != TimeUtil.CurrentTickCount)
				{
					DateTime now = DateTime.Now;
					long ticks = now.Ticks - TimeUtil._Now.Ticks;
					if (ticks < 0L && ticks > -3000000000L)
					{
						return TimeUtil._Now;
					}
					if (TimeUtil.CorrectTimeSecs != 0)
					{
						now = now.AddSeconds((double)TimeUtil.CorrectTimeSecs);
					}
					TimeUtil._Now = now;
					TimeUtil.CurrentTickCount = tickCount;
					TimeUtil.CurrentTicks = now.Ticks / 10000L;
				}
			}
			return TimeUtil._Now;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005290 File Offset: 0x00003490
		public static long UTCTicks()
		{
			return TimeUtil.NowDateTime().ToUniversalTime().Ticks / 1000L;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000052C0 File Offset: 0x000034C0
		public static DateTime UTCTime()
		{
			return TimeUtil.NowDateTime().ToUniversalTime();
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000052E0 File Offset: 0x000034E0
		public static string NowDataTimeString(string format = "yyyy-MM-dd HH:mm:ss")
		{
			int tickCount = TimeUtil.CurrentTickCount;
			DateTime now = TimeUtil.NowDateTime();
			if (now.Ticks != TimeUtil._CurrentDataTimeStringTicks)
			{
				TimeUtil._CurrentDataTimeStringTicks = now.Ticks;
				TimeUtil._CurrentDataTimeString = now.ToString(format);
			}
			return TimeUtil._CurrentDataTimeString;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005330 File Offset: 0x00003530
		public static string DataTimeToString(DateTime now, string format = "yyyy-MM-dd HH:mm:ss")
		{
			return now.ToString(format);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000534C File Offset: 0x0000354C
		public static long ConvertDateTimeInt(DateTime time)
		{
			return (time.Ticks - TimeUtil.UnixStartDateTime.Ticks) / 10000000L;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000537C File Offset: 0x0000357C
		public static DateTime ConvertIntDateTime(double d)
		{
			return TimeUtil.UnixStartDateTime.AddMilliseconds(d);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000539C File Offset: 0x0000359C
		public static int GetOffsetDays(DateTime startTime)
		{
			return (int)(TimeUtil._Now - startTime).TotalDays;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000053C4 File Offset: 0x000035C4
		public static DateTime GetWeekStartTime(DateTime now)
		{
			int days = (int)((DayOfWeek.Saturday + (int)now.DayOfWeek) % (DayOfWeek)7);
			return now.Date.AddDays((double)(-(double)days));
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000053F4 File Offset: 0x000035F4
		public static DateTime GetWeekStartTimeNow()
		{
			DateTime now = TimeUtil.NowDateTime();
			int days = (int)((DayOfWeek.Saturday + (int)now.DayOfWeek) % (DayOfWeek)7);
			return now.Date.AddDays((double)(-(double)days));
		}

		// Token: 0x0600009D RID: 157 RVA: 0x0000542C File Offset: 0x0000362C
		public static int GetWeekStartDayIdNow()
		{
			DateTime now = TimeUtil.NowDateTime();
			int days = (int)((DayOfWeek.Saturday + (int)now.DayOfWeek) % (DayOfWeek)7);
			return TimeUtil.GetOffsetDay(now.Date.AddDays((double)(-(double)days)));
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005468 File Offset: 0x00003668
		public static TimeSpan GetTimeOfWeek(DateTime now)
		{
			int days = (int)((DayOfWeek.Saturday + (int)now.DayOfWeek) % (DayOfWeek)7);
			return new TimeSpan(days, now.Hour, now.Minute, now.Second);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000054A4 File Offset: 0x000036A4
		public static TimeSpan GetTimeOfWeekNow()
		{
			DateTime now = TimeUtil.NowDateTime();
			int days = (int)((DayOfWeek.Saturday + (int)now.DayOfWeek) % (DayOfWeek)7);
			return new TimeSpan(days, now.Hour, now.Minute, now.Second);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000054E4 File Offset: 0x000036E4
		public static TimeSpan GetTimeOfWeekNow2()
		{
			DateTime now = TimeUtil.NowDateTime();
			int days = (int)now.DayOfWeek;
			if (days == 0)
			{
				days = 7;
			}
			return new TimeSpan(days, now.Hour, now.Minute, now.Second);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005530 File Offset: 0x00003730
		public static TimeSpan GetTimeOfWeek2(DateTime now)
		{
			int days = (int)now.DayOfWeek;
			if (days == 0)
			{
				days = 7;
			}
			return new TimeSpan(days, now.Hour, now.Minute, now.Second);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00005574 File Offset: 0x00003774
		public static int GetOffsetMonth(DateTime now)
		{
			return now.Year * 100 + now.Month;
		}

		// Token: 0x060000A3 RID: 163
		[DllImport("kernel32.dll")]
		private static extern long GetTickCount64();

		// Token: 0x060000A4 RID: 164 RVA: 0x00005598 File Offset: 0x00003798
		public static long NowTickCount64()
		{
			return TimeUtil.GetTickCount64() + TimeUtil.ServerStartTicks;
		}

		// Token: 0x060000A5 RID: 165
		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceCounter(ref long x);

		// Token: 0x060000A6 RID: 166
		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(ref long x);

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x000055B8 File Offset: 0x000037B8
		public static long CounterPerSecs
		{
			get
			{
				return TimeUtil._CounterPerSecs;
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000055D0 File Offset: 0x000037D0
		public static long Init()
		{
			TimeUtil._EnabelPerformaceCounter = TimeUtil.QueryPerformanceFrequency(ref TimeUtil._CounterPerSecs);
			TimeUtil.QueryPerformanceCounter(ref TimeUtil._StartCounter);
			TimeUtil._EnabelPerformaceCounter = (TimeUtil._EnabelPerformaceCounter && TimeUtil._CounterPerSecs > 0L && TimeUtil._StartCounter > 0L);
			TimeUtil.BasicDateTime = DateTime.Now;
			TimeUtil.BasicPerfromanceCounter = TimeUtil._StartCounter;
			TimeUtil._StartTicks = TimeUtil.BasicDateTime.Ticks;
			TimeUtil.ServerStartTicks = TimeUtil._StartTicks / 10000L - TimeUtil.GetTickCount64();
			return TimeUtil._StartTicks;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005660 File Offset: 0x00003860
		public static long NowEx()
		{
			return TimeUtil.CurrentTicks;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005678 File Offset: 0x00003878
		public static double TimeMS(long time, int round = 2)
		{
			return (double)time;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000568C File Offset: 0x0000388C
		private static long TimeDiffEx(long timeEnd, long timeStart)
		{
			long counter = timeEnd - timeStart;
			long count;
			long secs = Math.DivRem(counter, TimeUtil._CounterPerSecs, out count);
			return secs * 10000000L + count * 10000000L / TimeUtil._CounterPerSecs;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000056C8 File Offset: 0x000038C8
		public static long TimeDiff(long timeEnd, long timeStart = 0L)
		{
			return timeEnd - timeStart;
		}

		// Token: 0x060000AD RID: 173
		[DllImport("winmm.dll")]
		public static extern uint timeGetTime();

		// Token: 0x060000AE RID: 174 RVA: 0x000056E0 File Offset: 0x000038E0
		public static void RecordTimeAnchor()
		{
			if (TimeUtil._EnabelPerformaceCounter)
			{
				long ticks = TimeUtil.NOW();
				long timeDiff0 = ticks - TimeUtil.LastAnchorTicks;
				long counter = 0L;
				TimeUtil.QueryPerformanceCounter(ref counter);
				long count;
				long secs = Math.DivRem(counter - TimeUtil.LastAnchorCounter, TimeUtil._CounterPerSecs, out count);
				long timeDiff = secs * 1000L + count * 1000L / TimeUtil._CounterPerSecs;
				if (Math.Abs(timeDiff0 - timeDiff) >= timeDiff / 10L)
				{
					TimeUtil.TimeDriftTicks = ticks;
				}
				TimeUtil.LastAnchorTicks = ticks;
				TimeUtil.LastAnchorCounter = counter;
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005774 File Offset: 0x00003974
		public static bool HasTimeDrift()
		{
			return TimeUtil.NOW() - TimeUtil.TimeDriftTicks < 180000L;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000057A8 File Offset: 0x000039A8
		public static int MakeYear(DateTime time)
		{
			return time.Year;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000057C4 File Offset: 0x000039C4
		public static int MakeYearMonth(DateTime time)
		{
			return TimeUtil.MakeYear(time) * 100 + time.Month;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000057E8 File Offset: 0x000039E8
		public static int MakeYearMonthDay(DateTime time)
		{
			return TimeUtil.MakeYearMonth(time) * 100 + time.Day;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000580C File Offset: 0x00003A0C
		public static int MakeFirstWeekday(DateTime time)
		{
			time = time.AddDays((double)((time.DayOfWeek == DayOfWeek.Sunday) ? ((DayOfWeek)(-6)) : (DayOfWeek.Monday - time.DayOfWeek)));
			return TimeUtil.MakeYearMonthDay(time);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00005848 File Offset: 0x00003A48
		public static int GetWeekDay1To7(DateTime time)
		{
			int result;
			switch (time.DayOfWeek)
			{
			case DayOfWeek.Sunday:
				result = 7;
				break;
			case DayOfWeek.Monday:
				result = 1;
				break;
			case DayOfWeek.Tuesday:
				result = 2;
				break;
			case DayOfWeek.Wednesday:
				result = 3;
				break;
			case DayOfWeek.Thursday:
				result = 4;
				break;
			case DayOfWeek.Friday:
				result = 5;
				break;
			case DayOfWeek.Saturday:
				result = 6;
				break;
			default:
				throw new Exception("unbelievable");
			}
			return result;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000058AC File Offset: 0x00003AAC
		public static bool RandomDispatchTime(DateTime ctime, DateTime stime, int minSecs = 180, int randomSecs = 60, int times = 10)
		{
			long secs = (long)Math.Abs((stime - ctime).TotalSeconds);
			bool result;
			if (secs < (long)minSecs)
			{
				result = false;
			}
			else
			{
				if (secs >= (long)(randomSecs + minSecs))
				{
					times = times / 2 + 1;
				}
				int rnd = TimeUtil.Rnd.Next(randomSecs + 1);
				int limit = randomSecs / times;
				result = (rnd <= limit);
			}
			return result;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00005928 File Offset: 0x00003B28
		public static long AgeByNow(ref long age)
		{
			long a = TimeUtil.NOW();
			long result;
			if (age < a)
			{
				result = (age = a);
			}
			else
			{
				result = (age += 1L);
			}
			return result;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00005964 File Offset: 0x00003B64
		public static long AgeByNow(long age)
		{
			long a = TimeUtil.NOW();
			long result;
			if (age < a)
			{
				result = a;
			}
			else
			{
				result = age + 1L;
			}
			return result;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00005994 File Offset: 0x00003B94
		public static int AgeByUnixTime(int age)
		{
			int a = TimeUtil.UnixSecondsNow();
			int result;
			if (age < a)
			{
				result = a;
			}
			else
			{
				result = age + 1;
			}
			return result;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000059C0 File Offset: 0x00003BC0
		public static double GetOffsetSecond(DateTime date)
		{
			return (date - TimeUtil.StartDate).TotalSeconds;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000059E8 File Offset: 0x00003BE8
		public static int GetOffsetDay(DateTime now)
		{
			return (int)(now - TimeUtil.StartDate).TotalDays;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005A10 File Offset: 0x00003C10
		public static int GetOffsetDay2(DateTime now)
		{
			return now.Year * 10000 + now.Month * 100 + now.Day;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005A44 File Offset: 0x00003C44
		public static DateTime GetRealDate2(int day)
		{
			return new DateTime(day / 10000, day % 10000 / 100, day % 100);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005A70 File Offset: 0x00003C70
		public static int GetOffsetDayNow()
		{
			return TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00005A8C File Offset: 0x00003C8C
		public static DateTime GetRealDate(int day)
		{
			return TimeUtil.StartDate.AddDays((double)day);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00005AAC File Offset: 0x00003CAC
		public static long UnixSecondsToTicks(int secs)
		{
			return TimeUtil.UnixStartTicks + (long)secs * 1000L;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00005AD0 File Offset: 0x00003CD0
		public static long UnixSecondsToTicks(string secs)
		{
			int intSecs = Convert.ToInt32(secs);
			return TimeUtil.UnixSecondsToTicks(intSecs);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00005AF0 File Offset: 0x00003CF0
		public static int UnixSecondsNow()
		{
			long ticks = TimeUtil.NOW();
			return TimeUtil.SysTicksToUnixSeconds(ticks);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00005B10 File Offset: 0x00003D10
		public static int SysTicksToUnixSeconds(long ticks)
		{
			long secs = (ticks - TimeUtil.UnixStartTicks) / 1000L;
			return (int)secs;
		}

		// Token: 0x0400006C RID: 108
		public const int MILLISECOND = 1;

		// Token: 0x0400006D RID: 109
		public const int SECOND = 1000;

		// Token: 0x0400006E RID: 110
		public const int MINITE = 60000;

		// Token: 0x0400006F RID: 111
		public const int HOUR = 3600000;

		// Token: 0x04000070 RID: 112
		public const int DAY = 86400000;

		// Token: 0x04000071 RID: 113
		public const int DAYFLAGS = 100000000;

		// Token: 0x04000072 RID: 114
		public const long BackFreezeTicks = -3000000000L;

		// Token: 0x04000073 RID: 115
		public static long Before1970Ticks;

		// Token: 0x04000074 RID: 116
		public static long ServerStartTicks;

		// Token: 0x04000075 RID: 117
		private static int CurrentTickCount = 0;

		// Token: 0x04000076 RID: 118
		private static long CurrentTicks = DateTime.Now.Ticks / 10000L;

		// Token: 0x04000077 RID: 119
		private static DateTime _Now = DateTime.Now;

		// Token: 0x04000078 RID: 120
		private static volatile int CorrectTimeSecs = 0;

		// Token: 0x04000079 RID: 121
		private static bool UpdateByTimer = false;

		// Token: 0x0400007A RID: 122
		private static long CorrectNetTicks = 0L;

		// Token: 0x0400007B RID: 123
		private static long MaxTimeDriftTicks = 500L;

		// Token: 0x0400007C RID: 124
		public static string _CurrentDataTimeString = "2011-01-01 00:00:00";

		// Token: 0x0400007D RID: 125
		private static long _CurrentDataTimeStringTicks = 0L;

		// Token: 0x0400007E RID: 126
		private static DateTime BasicDateTime;

		// Token: 0x0400007F RID: 127
		private static long BasicPerfromanceCounter;

		// Token: 0x04000080 RID: 128
		private static long CorrectPerfromanceCounterTicks;

		// Token: 0x04000081 RID: 129
		private static ManualResetEvent waitStartEvent = new ManualResetEvent(false);

		// Token: 0x04000082 RID: 130
		private static long _StartCounter = 0L;

		// Token: 0x04000083 RID: 131
		private static long _CounterPerSecs = 0L;

		// Token: 0x04000084 RID: 132
		private static bool _EnabelPerformaceCounter = false;

		// Token: 0x04000085 RID: 133
		private static long _StartTicks = 0L;

		// Token: 0x04000086 RID: 134
		private static long TimeDriftTicks;

		// Token: 0x04000087 RID: 135
		private static long LastAnchorTicks;

		// Token: 0x04000088 RID: 136
		private static long LastAnchorCounter;

		// Token: 0x04000089 RID: 137
		private static Random Rnd = new Random();

		// Token: 0x0400008A RID: 138
		private static DateTime StartDate = new DateTime(2011, 11, 11);

		// Token: 0x0400008B RID: 139
		private static long UnixStartTicks;

		// Token: 0x0400008C RID: 140
		private static DateTime UnixStartDateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
	}
}
