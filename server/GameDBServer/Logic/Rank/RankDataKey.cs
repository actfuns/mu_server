using System;
using System.Collections.Generic;

namespace GameDBServer.Logic.Rank
{
	
	public class RankDataKey
	{
		
		public RankDataKey()
		{
			this.rankType = RankType.UnKnown;
			this.StartDate = "2011-11-11 11:11:11";
			this.EndDate = "2011-11-11 11:11:11";
		}

		
		public RankDataKey(RankType type, string fromDate, string toDate, List<int> minValueList)
		{
			this.rankType = type;
			DateTime tmpTime;
			if (DateTime.TryParse(fromDate, out tmpTime))
			{
				this.StartDate = tmpTime.ToString("yyyy-MM-dd HH:mm:ss");
			}
			if (DateTime.TryParse(toDate, out tmpTime))
			{
				this.EndDate = tmpTime.ToString("yyyy-MM-dd HH:mm:ss");
			}
			this.minGateValueList = minValueList;
		}

		
		public RankDataKey(RankType type, DateTime fromDate, DateTime toDate, List<int> minValueList)
		{
			this.rankType = type;
			this.StartDate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
			this.EndDate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
			this.minGateValueList = minValueList;
		}

		
		public string GetKey()
		{
			string[] array = new string[5];
			string[] array2 = array;
			int num = 0;
			int num2 = (int)this.rankType;
			array2[num] = num2.ToString();
			array[1] = "_";
			array[2] = this.StartDate;
			array[3] = "_";
			array[4] = this.EndDate;
			string keyStr = string.Concat(array);
			if (null != this.minGateValueList)
			{
				foreach (int item in this.minGateValueList)
				{
					keyStr += "_";
					keyStr += item.ToString();
				}
			}
			return keyStr;
		}

		
		public static RankDataKey GetKeyFromStr(string key)
		{
			string[] fields = key.Split(new char[]
			{
				RankDataKey.SplitChar
			});
			RankDataKey result;
			if (fields == null || fields.Length < 3)
			{
				result = null;
			}
			else
			{
				RankDataKey rankDataKey = new RankDataKey();
				rankDataKey.rankType = (RankType)Global.SafeConvertToInt32(fields[0], 10);
				rankDataKey.StartDate = fields[1];
				rankDataKey.EndDate = fields[2];
				for (int i = 3; i < fields.Length; i++)
				{
					if (null == rankDataKey.minGateValueList)
					{
						rankDataKey.minGateValueList = new List<int>();
					}
					rankDataKey.minGateValueList.Add(int.Parse(fields[i]));
				}
				result = rankDataKey;
			}
			return result;
		}

		
		public RankType rankType = RankType.UnKnown;

		
		public string StartDate = "2011-11-11 11:11:11";

		
		public string EndDate = "2011-11-11 11:11:11";

		
		private List<int> minGateValueList = null;

		
		private static char SplitChar = '_';
	}
}
