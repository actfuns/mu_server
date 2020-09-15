using System;
using GameServer.Core.Executor;

namespace KF.Remoting.Data
{
	// Token: 0x02000019 RID: 25
	public static class Global
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x0000A39C File Offset: 0x0000859C
		public static DateTime NowTime
		{
			get
			{
				return Global._NowTime;
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000A3B3 File Offset: 0x000085B3
		public static void UpdateNowTime(DateTime nowTime)
		{
			Global._NowTime = nowTime;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000A3BC File Offset: 0x000085BC
		public static int GetRandomNumber(int minV, int maxV)
		{
			int result;
			if (minV == maxV)
			{
				result = minV;
			}
			else if (minV > maxV)
			{
				result = maxV;
			}
			else
			{
				int ret = minV;
				lock (Global.GlobalRand)
				{
					ret = Global.GlobalRand.Next(minV, maxV);
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000A434 File Offset: 0x00008634
		public static int GetOffsetMiniteNow()
		{
			return Global.GetOffsetMinite(TimeUtil.NowDateTime());
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000A450 File Offset: 0x00008650
		public static int GetOffsetMinite(DateTime now)
		{
			double temp = (now - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return (int)(temp / 1000.0 / 60.0);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000A494 File Offset: 0x00008694
		public static long SafeConvertToTicks(string str)
		{
			try
			{
				if (string.IsNullOrEmpty(str))
				{
					return 0L;
				}
				DateTime dt;
				if (!DateTime.TryParse(str, out dt))
				{
					return 0L;
				}
				return dt.Ticks / 10000L;
			}
			catch (Exception)
			{
			}
			return 0L;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000A4F8 File Offset: 0x000086F8
		public static int SafeConvertToInt32(string str)
		{
			int result;
			if (string.IsNullOrEmpty(str))
			{
				result = 0;
			}
			else
			{
				str = str.Trim();
				if (string.IsNullOrEmpty(str))
				{
					result = 0;
				}
				else
				{
					try
					{
						return Convert.ToInt32(str);
					}
					catch (Exception)
					{
					}
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000A558 File Offset: 0x00008758
		public static long SafeConvertToInt64(string str)
		{
			long result;
			if (string.IsNullOrEmpty(str))
			{
				result = 0L;
			}
			else
			{
				str = str.Trim();
				if (string.IsNullOrEmpty(str))
				{
					result = 0L;
				}
				else
				{
					try
					{
						return Convert.ToInt64(str);
					}
					catch (Exception)
					{
					}
					result = 0L;
				}
			}
			return result;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000A5BC File Offset: 0x000087BC
		public static double SafeConvertToDouble(string str)
		{
			double result;
			if (string.IsNullOrEmpty(str))
			{
				result = 0.0;
			}
			else
			{
				str = str.Trim();
				if (string.IsNullOrEmpty(str))
				{
					result = 0.0;
				}
				else
				{
					try
					{
						return Convert.ToDouble(str);
					}
					catch (Exception)
					{
					}
					result = 0.0;
				}
			}
			return result;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000A634 File Offset: 0x00008834
		public static double[] String2DoubleArray(string str)
		{
			double[] result;
			if (string.IsNullOrEmpty(str))
			{
				result = null;
			}
			else
			{
				string[] sa = str.Split(new char[]
				{
					','
				});
				result = Global.StringArray2DoubleArray(sa);
			}
			return result;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000A674 File Offset: 0x00008874
		public static double[] StringArray2DoubleArray(string[] sa)
		{
			double[] da = new double[sa.Length];
			try
			{
				for (int i = 0; i < sa.Length; i++)
				{
					string str = sa[i].Trim();
					str = (string.IsNullOrEmpty(str) ? "0.0" : str);
					da[i] = Convert.ToDouble(str);
				}
			}
			catch (Exception ex)
			{
				string msg = ex.ToString();
			}
			return da;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000A6F0 File Offset: 0x000088F0
		public static int[] String2IntArray(string str, char spliter = ',')
		{
			int[] result;
			if (string.IsNullOrEmpty(str))
			{
				result = null;
			}
			else
			{
				string[] sa = str.Split(new char[]
				{
					spliter
				});
				result = Global.StringArray2IntArray(sa);
			}
			return result;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000A730 File Offset: 0x00008930
		public static string[] String2StringArray(string str, char spliter = '|')
		{
			string[] result;
			if (string.IsNullOrEmpty(str))
			{
				result = null;
			}
			else
			{
				result = str.Split(new char[]
				{
					spliter
				});
			}
			return result;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x0000A768 File Offset: 0x00008968
		public static int[] StringArray2IntArray(string[] sa)
		{
			int[] result;
			if (sa == null)
			{
				result = null;
			}
			else
			{
				result = Global.StringArray2IntArray(sa, 0, sa.Length);
			}
			return result;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000A794 File Offset: 0x00008994
		public static int[] StringArray2IntArray(string[] sa, int start, int count)
		{
			int[] result2;
			if (sa == null)
			{
				result2 = null;
			}
			else if (start < 0 || start >= sa.Length)
			{
				result2 = null;
			}
			else if (count <= 0)
			{
				result2 = null;
			}
			else if (sa.Length - start < count)
			{
				result2 = null;
			}
			else
			{
				int[] result = new int[count];
				for (int i = 0; i < count; i++)
				{
					string str = sa[start + i].Trim();
					str = (string.IsNullOrEmpty(str) ? "0" : str);
					result[i] = Convert.ToInt32(str);
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000A838 File Offset: 0x00008A38
		public static string GetHuoDongKeyString(string fromDate, string toDate)
		{
			return string.Format("{0}_{1}", fromDate, toDate);
		}

		// Token: 0x040000A1 RID: 161
		public static readonly int UninitGameId = -1111;

		// Token: 0x040000A2 RID: 162
		public static bool TestMode;

		// Token: 0x040000A3 RID: 163
		private static DateTime _NowTime = TimeUtil.NowDateTime();

		// Token: 0x040000A4 RID: 164
		private static Random GlobalRand = new Random(Guid.NewGuid().GetHashCode());
	}
}
