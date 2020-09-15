using System;
using System.Collections.Generic;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x02000079 RID: 121
	public class BoCaiHelper
	{
		// Token: 0x060001C1 RID: 449 RVA: 0x0001E2E4 File Offset: 0x0001C4E4
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

		// Token: 0x060001C2 RID: 450 RVA: 0x0001E398 File Offset: 0x0001C598
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

		// Token: 0x060001C3 RID: 451 RVA: 0x0001E460 File Offset: 0x0001C660
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

		// Token: 0x060001C4 RID: 452 RVA: 0x0001E4F4 File Offset: 0x0001C6F4
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

		// Token: 0x060001C5 RID: 453 RVA: 0x0001E588 File Offset: 0x0001C788
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
