using System;
using System.Collections.Generic;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	
	public class BoCaiHelper
	{
		
		public static bool CopyHistoryData(List<KFBoCaoHistoryData> sData, out List<KFBoCaoHistoryData> rData)
		{
			rData = new List<KFBoCaoHistoryData>();
			try
			{
				foreach (KFBoCaoHistoryData item in sData)
				{
					KFBoCaoHistoryData data = new KFBoCaoHistoryData();
					if (GlobalNew.Copy<KFBoCaoHistoryData>(item, ref data))
					{
						rData.Add(data);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		
		public static string ListInt2String(List<int> iList)
		{
			string str = "";
			try
			{
				foreach (int item in iList)
				{
					if (string.IsNullOrEmpty(str))
					{
						str = string.Format("{0}", item);
					}
					else
					{
						str = string.Format("{0},{1}", str, item);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return str;
		}

		
		public static void String2ListInt(string str, out List<int> iList)
		{
			iList = new List<int>();
			try
			{
				string[] temp = str.Split(new char[]
				{
					','
				});
				foreach (string item in temp)
				{
					iList.Add(Convert.ToInt32(item));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		
		public static int String2Int(string str)
		{
			int num = 0;
			try
			{
				string[] temp = str.Split(new char[]
				{
					','
				});
				foreach (string item in temp)
				{
					num += Convert.ToInt32(item);
				}
				return num;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return -1;
		}

		
		public static bool IsSameData(List<int> d1, List<int> d2)
		{
			try
			{
				List<int> temp = new List<int>();
				temp.AddRange(d2);
				foreach (int item in d1)
				{
					int index = temp.IndexOf(item);
					if (index < 0)
					{
						return false;
					}
					temp.RemoveAt(index);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}
	}
}
