using System;
using GameServer.Core.Executor;

namespace KF.Remoting.Data
{
	
	public static class Global
	{
		
		
		public static DateTime NowTime
		{
			get
			{
				return Global._NowTime;
			}
		}

		
		public static void UpdateNowTime(DateTime nowTime)
		{
			Global._NowTime = nowTime;
		}

		
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

		
		public static int GetOffsetMiniteNow()
		{
			return Global.GetOffsetMinite(TimeUtil.NowDateTime());
		}

		
		public static int GetOffsetMinite(DateTime now)
		{
			double temp = (now - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return (int)(temp / 1000.0 / 60.0);
		}

		
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

		
		public static string GetHuoDongKeyString(string fromDate, string toDate)
		{
			return string.Format("{0}_{1}", fromDate, toDate);
		}

		
		public static readonly int UninitGameId = -1111;

		
		public static bool TestMode;

		
		private static DateTime _NowTime = TimeUtil.NowDateTime();

		
		private static Random GlobalRand = new Random(Guid.NewGuid().GetHashCode());
	}
}
