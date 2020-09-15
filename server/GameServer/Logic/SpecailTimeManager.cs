using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000799 RID: 1945
	public class SpecailTimeManager
	{
		// Token: 0x060032BA RID: 12986 RVA: 0x002CFBD0 File Offset: 0x002CDDD0
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

		// Token: 0x060032BB RID: 12987 RVA: 0x002CFCD0 File Offset: 0x002CDED0
		public static int ResetSpecialTimeLimits()
		{
			int ret = GameManager.systemSpecialTimeMgr.ReloadLoadFromXMlFile();
			lock (SpecailTimeManager._TimeLimitsDict)
			{
				SpecailTimeManager._TimeLimitsDict.Clear();
			}
			return ret;
		}

		// Token: 0x060032BC RID: 12988 RVA: 0x002CFD34 File Offset: 0x002CDF34
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

		// Token: 0x060032BD RID: 12989 RVA: 0x002CFD8C File Offset: 0x002CDF8C
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

		// Token: 0x060032BE RID: 12990 RVA: 0x002CFDCC File Offset: 0x002CDFCC
		public static bool JugeIsDoulbeExperienceAndLingli()
		{
			return SpecailTimeManager.IsDoulbeExperienceAndLingli;
		}

		// Token: 0x060032BF RID: 12991 RVA: 0x002CFDE4 File Offset: 0x002CDFE4
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

		// Token: 0x060032C0 RID: 12992 RVA: 0x002CFE24 File Offset: 0x002CE024
		public static bool JugeIsDoulbeKaoHuo()
		{
			return SpecailTimeManager.IsDoulbeKaoHuo;
		}

		// Token: 0x060032C1 RID: 12993 RVA: 0x002CFE3C File Offset: 0x002CE03C
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

		// Token: 0x04003EBE RID: 16062
		private static Dictionary<int, DateTimeRange[]> _TimeLimitsDict = new Dictionary<int, DateTimeRange[]>();

		// Token: 0x04003EBF RID: 16063
		private static long JugeDoulbeExperienceTicks = 0L;

		// Token: 0x04003EC0 RID: 16064
		private static bool IsDoulbeExperienceAndLingli = false;

		// Token: 0x04003EC1 RID: 16065
		private static bool IsDoulbeKaoHuo = false;
	}
}
