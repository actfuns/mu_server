using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace GameDBServer.Core
{
    
    public class TimeUtil
    {
        
        
        public static long CurrentTicksInexact
        {
            get
            {
                return TimeUtil.CurrentTicks;
            }
        }

        
        static TimeUtil()
        {
            TimeUtil.UnixStartTicks = TimeUtil.UnixStartDateTime.Ticks / 10000L;
            TimeUtil.Before1970Ticks = TimeUtil.UnixStartTicks;
            TimeUtil.Init();
            TimeUtil.TryInitMMTimer();
        }

        
        public static void TryInitMMTimer()
        {
        }

        
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

        
        public static long TimeStamp()
        {
            return Convert.ToInt64((TimeUtil._Now - new DateTime(1970, 1, 1)).TotalSeconds);
        }

        
        public static long NowRealTime()
        {
            return DateTime.Now.Ticks / 10000L;
        }

        
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

        
        public static long UTCTicks()
        {
            return TimeUtil.NowDateTime().ToUniversalTime().Ticks / 1000L;
        }

        
        public static DateTime UTCTime()
        {
            return TimeUtil.NowDateTime().ToUniversalTime();
        }

        
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

        
        public static string DataTimeToString(DateTime now, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return now.ToString(format);
        }

        
        public static long ConvertDateTimeInt(DateTime time)
        {
            return (time.Ticks - TimeUtil.UnixStartDateTime.Ticks) / 10000000L;
        }

        
        public static DateTime ConvertIntDateTime(double d)
        {
            return TimeUtil.UnixStartDateTime.AddMilliseconds(d);
        }

        
        public static int GetOffsetDays(DateTime startTime)
        {
            return (int)(TimeUtil._Now - startTime).TotalDays;
        }

        
        public static DateTime GetWeekStartTime(DateTime now)
        {
            int days = (int)((int)(DayOfWeek.Saturday + (int)now.DayOfWeek) % 7);
            return now.Date.AddDays((double)(-(double)days));
        }

        
        public static DateTime GetWeekStartTimeNow()
        {
            DateTime now = TimeUtil.NowDateTime();
            int days = (int)((int)(DayOfWeek.Saturday + (int)now.DayOfWeek) % 7);
            return now.Date.AddDays((double)(-(double)days));
        }

        
        public static int GetWeekStartDayIdNow()
        {
            DateTime now = TimeUtil.NowDateTime();
            int days = (int)((int)(DayOfWeek.Saturday + (int)now.DayOfWeek) % 7);
            return TimeUtil.GetOffsetDay(now.Date.AddDays((double)(-(double)days)));
        }

        
        public static TimeSpan GetTimeOfWeek(DateTime now)
        {
            int days = (int)((int)(DayOfWeek.Saturday + (int)now.DayOfWeek) % 7);
            return new TimeSpan(days, now.Hour, now.Minute, now.Second);
        }

        
        public static TimeSpan GetTimeOfWeekNow()
        {
            DateTime now = TimeUtil.NowDateTime();
            int days = (int)((int)(DayOfWeek.Saturday + (int)now.DayOfWeek) % 7);
            return new TimeSpan(days, now.Hour, now.Minute, now.Second);
        }

        
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

        
        public static TimeSpan GetTimeOfWeek2(DateTime now)
        {
            int days = (int)now.DayOfWeek;
            if (days == 0)
            {
                days = 7;
            }
            return new TimeSpan(days, now.Hour, now.Minute, now.Second);
        }

        
        public static int GetOffsetMonth(DateTime now)
        {
            return now.Year * 100 + now.Month;
        }

        
        [DllImport("kernel32.dll")]
        private static extern long GetTickCount64();

        
        public static long NowTickCount64()
        {
            return TimeUtil.GetTickCount64() + TimeUtil.ServerStartTicks;
        }

        
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(ref long x);

        
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(ref long x);

        
        
        public static long CounterPerSecs
        {
            get
            {
                return TimeUtil._CounterPerSecs;
            }
        }

        
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

        
        public static long NowEx()
        {
            return TimeUtil.CurrentTicks;
        }

        
        public static double TimeMS(long time, int round = 2)
        {
            return (double)time;
        }

        
        private static long TimeDiffEx(long timeEnd, long timeStart)
        {
            long counter = timeEnd - timeStart;
            long count;
            long secs = Math.DivRem(counter, TimeUtil._CounterPerSecs, out count);
            return secs * 10000000L + count * 10000000L / TimeUtil._CounterPerSecs;
        }

        
        public static long TimeDiff(long timeEnd, long timeStart = 0L)
        {
            return timeEnd - timeStart;
        }

        
        [DllImport("winmm.dll")]
        public static extern uint timeGetTime();

        
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

        
        public static bool HasTimeDrift()
        {
            return TimeUtil.NOW() - TimeUtil.TimeDriftTicks < 180000L;
        }

        
        public static int MakeYear(DateTime time)
        {
            return time.Year;
        }

        
        public static int MakeYearMonth(DateTime time)
        {
            return TimeUtil.MakeYear(time) * 100 + time.Month;
        }

        
        public static int MakeYearMonthDay(DateTime time)
        {
            return TimeUtil.MakeYearMonth(time) * 100 + time.Day;
        }

        
        public static int MakeFirstWeekday(DateTime time)
        {
            time = time.AddDays((double)((time.DayOfWeek == DayOfWeek.Sunday) ? -6 : (int)(DayOfWeek.Monday - time.DayOfWeek)));
            return TimeUtil.MakeYearMonthDay(time);
        }

        
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

        
        public static double GetOffsetSecond(DateTime date)
        {
            return (date - TimeUtil.StartDate).TotalSeconds;
        }

        
        public static int GetOffsetDay(DateTime now)
        {
            return (int)(now - TimeUtil.StartDate).TotalDays;
        }

        
        public static int GetOffsetDay2(DateTime now)
        {
            return now.Year * 10000 + now.Month * 100 + now.Day;
        }

        
        public static DateTime GetRealDate2(int day)
        {
            return new DateTime(day / 10000, day % 10000 / 100, day % 100);
        }

        
        public static int GetOffsetDayNow()
        {
            return TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
        }

        
        public static DateTime GetRealDate(int day)
        {
            return TimeUtil.StartDate.AddDays((double)day);
        }

        
        public static long UnixSecondsToTicks(int secs)
        {
            return TimeUtil.UnixStartTicks + (long)secs * 1000L;
        }

        
        public static long UnixSecondsToTicks(string secs)
        {
            int intSecs = Convert.ToInt32(secs);
            return TimeUtil.UnixSecondsToTicks(intSecs);
        }

        
        public static int UnixSecondsNow()
        {
            long ticks = TimeUtil.NOW();
            return TimeUtil.SysTicksToUnixSeconds(ticks);
        }

        
        public static int SysTicksToUnixSeconds(long ticks)
        {
            long secs = (ticks - TimeUtil.UnixStartTicks) / 1000L;
            return (int)secs;
        }

        
        public const int MILLISECOND = 1;

        
        public const int SECOND = 1000;

        
        public const int MINITE = 60000;

        
        public const int HOUR = 3600000;

        
        public const int DAY = 86400000;

        
        public const int DAYFLAGS = 100000000;

        
        public const long BackFreezeTicks = -3000000000L;

        
        public static long Before1970Ticks;

        
        public static long ServerStartTicks;

        
        private static int CurrentTickCount = 0;

        
        private static long CurrentTicks = DateTime.Now.Ticks / 10000L;

        
        private static DateTime _Now = DateTime.Now;

        
        private static volatile int CorrectTimeSecs = 0;

        
        private static bool UpdateByTimer = false;

        
        private static long CorrectNetTicks = 0L;

        
        private static long MaxTimeDriftTicks = 500L;

        
        public static string _CurrentDataTimeString = "2011-01-01 00:00:00";

        
        private static long _CurrentDataTimeStringTicks = 0L;

        
        private static DateTime BasicDateTime;

        
        private static long BasicPerfromanceCounter;

        
        private static long CorrectPerfromanceCounterTicks;

        
        private static ManualResetEvent waitStartEvent = new ManualResetEvent(false);

        
        private static long _StartCounter = 0L;

        
        private static long _CounterPerSecs = 0L;

        
        private static bool _EnabelPerformaceCounter = false;

        
        private static long _StartTicks = 0L;

        
        private static long TimeDriftTicks;

        
        private static long LastAnchorTicks;

        
        private static long LastAnchorCounter;

        
        private static Random Rnd = new Random();

        
        private static DateTime StartDate = new DateTime(2011, 11, 11);

        
        private static long UnixStartTicks;

        
        private static DateTime UnixStartDateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
    }
}
