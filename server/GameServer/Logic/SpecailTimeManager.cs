using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class SpecailTimeManager
	{
		
		private static DateTimeRange[] GetTimeLimitsByID(int timeLimitsID)
		{
			DateTimeRange[] dateTimeRangeArray = null;
			lock (SpecailTimeManager._TimeLimitsDict)
			{
				if (SpecailTimeManager._TimeLimitsDict.TryGetValue(timeLimitsID, out dateTimeRangeArray))
				{
					return dateTimeRangeArray;
				}
			}
			SystemXmlItem systemSpecialTimeItem = null;
			DateTimeRange[] result;
			if (!GameManager.systemSpecialTimeMgr.SystemXmlItemDict.TryGetValue(timeLimitsID, out systemSpecialTimeItem))
			{
				result = null;
			}
			else
			{
				string timeLimits = systemSpecialTimeItem.GetStringValue("TimeLimits");
				if (string.IsNullOrEmpty(timeLimits))
				{
					result = null;
				}
				else
				{
					dateTimeRangeArray = Global.ParseDateTimeRangeStr(timeLimits);
					lock (SpecailTimeManager._TimeLimitsDict)
					{
						SpecailTimeManager._TimeLimitsDict[timeLimitsID] = dateTimeRangeArray;
					}
					result = dateTimeRangeArray;
				}
			}
			return result;
		}

		
		public static int ResetSpecialTimeLimits()
		{
			int ret = GameManager.systemSpecialTimeMgr.ReloadLoadFromXMlFile();
			lock (SpecailTimeManager._TimeLimitsDict)
			{
				SpecailTimeManager._TimeLimitsDict.Clear();
			}
			return ret;
		}

		
		public static void ProcessDoulbeExperience()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			if (dateTime.Ticks - SpecailTimeManager.JugeDoulbeExperienceTicks >= 50000000L)
			{
				SpecailTimeManager.JugeDoulbeExperienceTicks = dateTime.Ticks;
				SpecailTimeManager.IsDoulbeExperienceAndLingli = SpecailTimeManager.InDoubleExperienceAndLingLiTimeRange(dateTime);
				SpecailTimeManager.IsDoulbeKaoHuo = SpecailTimeManager.InDoubleKaoHuoTimeRange(dateTime);
			}
		}

		
		private static bool InDoubleExperienceAndLingLiTimeRange(DateTime dateTime)
		{
			DateTimeRange[] dateTimeRangeArray = SpecailTimeManager.GetTimeLimitsByID(1);
			bool result;
			if (null == dateTimeRangeArray)
			{
				result = false;
			}
			else
			{
				int endMinute = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, dateTimeRangeArray, out endMinute, true);
			}
			return result;
		}

		
		public static bool JugeIsDoulbeExperienceAndLingli()
		{
			return SpecailTimeManager.IsDoulbeExperienceAndLingli;
		}

		
		private static bool InDoubleKaoHuoTimeRange(DateTime dateTime)
		{
			DateTimeRange[] dateTimeRangeArray = SpecailTimeManager.GetTimeLimitsByID(2);
			bool result;
			if (null == dateTimeRangeArray)
			{
				result = false;
			}
			else
			{
				int endMinute = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, dateTimeRangeArray, out endMinute, true);
			}
			return result;
		}

		
		public static bool JugeIsDoulbeKaoHuo()
		{
			return SpecailTimeManager.IsDoulbeKaoHuo;
		}

		
		public static bool InSpercailTime(int spercailid)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			DateTimeRange[] dateTimeRangeArray = SpecailTimeManager.GetTimeLimitsByID(spercailid);
			bool result;
			if (null == dateTimeRangeArray)
			{
				result = false;
			}
			else
			{
				int endMinute = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, dateTimeRangeArray, out endMinute, true);
			}
			return result;
		}

		
		private static Dictionary<int, DateTimeRange[]> _TimeLimitsDict = new Dictionary<int, DateTimeRange[]>();

		
		private static long JugeDoulbeExperienceTicks = 0L;

		
		private static bool IsDoulbeExperienceAndLingli = false;

		
		private static bool IsDoulbeKaoHuo = false;
	}
}
