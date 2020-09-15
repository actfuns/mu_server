using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	// Token: 0x020001AB RID: 427
	public class HuangDiTeQuanMgr
	{
		// Token: 0x0600090D RID: 2317 RVA: 0x000548EC File Offset: 0x00052AEC
		private static void AddToHuangDiToOtherRoleDict(int cmdID, int otherRoleID)
		{
			string key = string.Format("{0}_{1}", cmdID, otherRoleID);
			HuangDiTeQuanMgr.HuangDiToOtherRoleDict[key] = DateTime.Now.DayOfYear;
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0005492C File Offset: 0x00052B2C
		public static bool FindHuangDiToOtherRoleDict(int cmdID, int otherRoleID)
		{
			string key = string.Format("{0}_{1}", cmdID, otherRoleID);
			bool result;
			lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
			{
				int dayID = -1;
				if (!HuangDiTeQuanMgr.HuangDiToOtherRoleDict.TryGetValue(key, out dayID))
				{
					result = false;
				}
				else
				{
					result = (dayID == DateTime.Now.DayOfYear);
				}
			}
			return result;
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x000549B8 File Offset: 0x00052BB8
		public static void LoadHuangDiTeQuan(DBManager dbMgr)
		{
			HuangDiTeQuanMgr.MyHuangDiTeQuanItem = DBQuery.LoadHuangDiTeQuan(dbMgr);
			if (null == HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
			{
				HuangDiTeQuanMgr.MyHuangDiTeQuanItem = new HuangDiTeQuanItem
				{
					ID = 1,
					ToLaoFangDayID = 0,
					ToLaoFangNum = 0,
					OffLaoFangDayID = 0,
					OffLaoFangNum = 0,
					BanCatDayID = 0,
					BanCatNum = 0,
					LastBanChatTicks = 0L,
					LastSendToLaoFangTicks = 0L,
					LastTakeOffLaoFangTicks = 0L
				};
			}
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x00054A38 File Offset: 0x00052C38
		public static HuangDiTeQuanItem GetHuangDiTeQuanItem()
		{
			return HuangDiTeQuanMgr.MyHuangDiTeQuanItem;
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x00054A50 File Offset: 0x00052C50
		public static void ClearHuangDiTeQuan()
		{
			HuangDiTeQuanMgr.MyHuangDiTeQuanItem = new HuangDiTeQuanItem
			{
				ID = 1,
				ToLaoFangDayID = 0,
				ToLaoFangNum = 0,
				OffLaoFangDayID = 0,
				OffLaoFangNum = 0,
				BanCatDayID = 0,
				BanCatNum = 0,
				LastBanChatTicks = 0L,
				LastSendToLaoFangTicks = 0L,
				LastTakeOffLaoFangTicks = 0L
			};
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x00054AB4 File Offset: 0x00052CB4
		public static bool CanExecuteHuangDiTeQuanNow(int cmdID)
		{
			long nowTicks = DateTime.Now.Ticks;
			if (344 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (nowTicks - HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastSendToLaoFangTicks > 18000000000L)
					{
						return true;
					}
				}
			}
			else if (345 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (nowTicks - HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastTakeOffLaoFangTicks > 18000000000L)
					{
						return true;
					}
				}
			}
			else if (346 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (nowTicks - HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastBanChatTicks > 18000000000L)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x00054C38 File Offset: 0x00052E38
		public static bool AddHuanDiTeQuan(int cmdID, int otherRoleID)
		{
			long nowTicks = DateTime.Now.Ticks;
			int dayID = DateTime.Now.DayOfYear;
			if (344 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangDayID == dayID)
					{
						if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangNum >= 3)
						{
							return false;
						}
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangNum++;
					}
					else
					{
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangDayID = dayID;
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangNum = 1;
					}
					HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastSendToLaoFangTicks = nowTicks;
					HuangDiTeQuanMgr.AddToHuangDiToOtherRoleDict(cmdID, otherRoleID);
					return true;
				}
			}
			if (345 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangDayID == dayID)
					{
						if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangNum >= 3)
						{
							return false;
						}
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangNum++;
					}
					else
					{
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangDayID = dayID;
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangNum = 1;
					}
					HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastTakeOffLaoFangTicks = nowTicks;
					HuangDiTeQuanMgr.AddToHuangDiToOtherRoleDict(cmdID, otherRoleID);
					return true;
				}
			}
			if (346 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatDayID == dayID)
					{
						if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatNum >= 3)
						{
							return false;
						}
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatNum++;
					}
					else
					{
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatDayID = dayID;
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatNum = 1;
					}
					HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastBanChatTicks = nowTicks;
					HuangDiTeQuanMgr.AddToHuangDiToOtherRoleDict(cmdID, otherRoleID);
					return true;
				}
			}
			return false;
		}

		// Token: 0x040009BA RID: 2490
		private static HuangDiTeQuanItem MyHuangDiTeQuanItem = null;

		// Token: 0x040009BB RID: 2491
		private static Dictionary<string, int> HuangDiToOtherRoleDict = new Dictionary<string, int>();
	}
}
