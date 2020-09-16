using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	
	internal class CoupleArenaMatchTimeLimiter
	{
		
		public int GetMatchTimes(int a1, int a2, int b1, int b2)
		{
			long min;
			long max;
			this.GetUnionCouple2(a1, a2, b1, b2, out min, out max);
			Dictionary<long, int> dict = null;
			int times = 0;
			int result;
			if (this.TimesDict.TryGetValue(min, out dict) && dict.TryGetValue(max, out times))
			{
				result = times;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public void AddMatchTimes(int a1, int a2, int b1, int b2, int times = 1)
		{
			long min;
			long max;
			this.GetUnionCouple2(a1, a2, b1, b2, out min, out max);
			Dictionary<long, int> dict = null;
			if (!this.TimesDict.TryGetValue(min, out dict))
			{
				dict = (this.TimesDict[min] = new Dictionary<long, int>());
			}
			if (dict.ContainsKey(max))
			{
				Dictionary<long, int> dictionary;
				long key;
				(dictionary = dict)[key = max] = dictionary[key] + times;
			}
			else
			{
				dict.Add(max, times);
			}
		}

		
		public void Reset()
		{
			this.TimesDict.Clear();
		}

		
		private void GetUnionCouple2(int a1, int a2, int b1, int b2, out long min, out long max)
		{
			long l = this.GetUnionCouple(a1, a2);
			long l2 = this.GetUnionCouple(b1, b2);
			min = Math.Min(l, l2);
			max = Math.Max(l, l2);
		}

		
		private long GetUnionCouple(int a1, int a2)
		{
			int min = Math.Min(a1, a2);
			int max = Math.Max(a1, a2);
			long v = (long)min;
			v <<= 32;
			return v | (long)((ulong)max);
		}

		
		private Dictionary<long, Dictionary<long, int>> TimesDict = new Dictionary<long, Dictionary<long, int>>();
	}
}
