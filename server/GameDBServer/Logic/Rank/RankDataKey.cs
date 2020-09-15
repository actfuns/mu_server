using System;
using System.Collections.Generic;

namespace GameDBServer.Logic.Rank
{
	// Token: 0x02000161 RID: 353
	public class RankDataKey
	{
		// Token: 0x060005FE RID: 1534 RVA: 0x000358FC File Offset: 0x00033AFC
		public RankDataKey()
		{
			this.rankType = RankType.UnKnown;
			this.StartDate = "2011-11-11 11:11:11";
			this.EndDate = "2011-11-11 11:11:11";
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00035954 File Offset: 0x00033B54
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

		// Token: 0x06000600 RID: 1536 RVA: 0x000359E4 File Offset: 0x00033BE4
		public RankDataKey(RankType type, DateTime fromDate, DateTime toDate, List<int> minValueList)
		{
			this.rankType = type;
			this.StartDate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
			this.EndDate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
			this.minGateValueList = minValueList;
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00035A54 File Offset: 0x00033C54
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

		// Token: 0x06000602 RID: 1538 RVA: 0x00035B20 File Offset: 0x00033D20
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

		// Token: 0x0400086E RID: 2158
		public RankType rankType = RankType.UnKnown;

		// Token: 0x0400086F RID: 2159
		public string StartDate = "2011-11-11 11:11:11";

		// Token: 0x04000870 RID: 2160
		public string EndDate = "2011-11-11 11:11:11";

		// Token: 0x04000871 RID: 2161
		private List<int> minGateValueList = null;

		// Token: 0x04000872 RID: 2162
		private static char SplitChar = '_';
	}
}
