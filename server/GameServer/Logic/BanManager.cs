using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x020005C3 RID: 1475
	public class BanManager
	{
		// Token: 0x06001AC7 RID: 6855 RVA: 0x00198840 File Offset: 0x00196A40
		public static void BanRoleName(string roleName, int banMinutes, int reason = 1)
		{
			lock (BanManager._RoleNameDict)
			{
				BanManager._RoleNameDict[roleName] = reason;
			}
			lock (BanManager._RoleNameTicksDict)
			{
				if (banMinutes > 0)
				{
					BanManager._RoleNameTicksDict[roleName] = TimeUtil.NOW() + (long)(banMinutes * 60 * 1000);
				}
				else
				{
					BanManager._RoleNameTicksDict[roleName] = 0L;
				}
			}
		}

		// Token: 0x06001AC8 RID: 6856 RVA: 0x00198904 File Offset: 0x00196B04
		public static int IsBanRoleName(string roleName, out int leftSecs)
		{
			leftSecs = 0;
			int reason = 0;
			lock (BanManager._RoleNameDict)
			{
				if (!BanManager._RoleNameDict.TryGetValue(roleName, out reason))
				{
					reason = 0;
				}
			}
			if (reason > 0)
			{
				lock (BanManager._RoleNameTicksDict)
				{
					long timeout = 0L;
					if (!BanManager._RoleNameTicksDict.TryGetValue(roleName, out timeout))
					{
						reason = 0;
					}
					else
					{
						long nowTicks = TimeUtil.NOW();
						if (nowTicks >= timeout)
						{
							reason = 0;
						}
						else
						{
							leftSecs = (int)((timeout - nowTicks) / 1000L);
						}
					}
				}
			}
			return reason;
		}

		// Token: 0x06001AC9 RID: 6857 RVA: 0x001989FC File Offset: 0x00196BFC
		public static void BanUserID2Memory(string userID)
		{
			int nCurrHour = Global.GetOffsetHour(TimeUtil.NowDateTime());
			lock (BanManager.m_HourBanDictMutex)
			{
				if (BanManager.m_HourBanDict.ContainsKey(nCurrHour))
				{
					if (!BanManager.m_HourBanDict[nCurrHour].ContainsKey(userID))
					{
						BanManager.m_HourBanDict[nCurrHour][userID] = 1;
					}
				}
				else
				{
					Dictionary<string, int> tmpDict = new Dictionary<string, int>();
					tmpDict[userID] = 1;
					BanManager.m_HourBanDict[nCurrHour] = tmpDict;
				}
			}
		}

		// Token: 0x06001ACA RID: 6858 RVA: 0x00198AB0 File Offset: 0x00196CB0
		public static void ClearBanMemory(int nHour)
		{
			int nCurrHour = Global.GetOffsetHour(TimeUtil.NowDateTime());
			int nMinHour = nCurrHour - nHour;
			lock (BanManager.m_HourBanDictMutex)
			{
				List<int> tempList = new List<int>();
				foreach (KeyValuePair<int, Dictionary<string, int>> item in BanManager.m_HourBanDict)
				{
					if (item.Key <= nCurrHour && item.Key >= nMinHour)
					{
						tempList.Add(item.Key);
					}
				}
				foreach (int item2 in tempList)
				{
					BanManager.m_HourBanDict.Remove(item2);
				}
			}
		}

		// Token: 0x06001ACB RID: 6859 RVA: 0x00198BD0 File Offset: 0x00196DD0
		public static void CheckBanMemory()
		{
			int nCurrHour = Global.GetOffsetHour(TimeUtil.NowDateTime());
			if (BanManager.m_UpdateHour != nCurrHour)
			{
				int maxBanHour = GameManager.PlatConfigMgr.GetGameConfigItemInt("fileban_hour", 24);
				int nMinHour = nCurrHour - maxBanHour;
				lock (BanManager.m_HourBanDictMutex)
				{
					List<int> tempList = new List<int>();
					foreach (KeyValuePair<int, Dictionary<string, int>> item in BanManager.m_HourBanDict)
					{
						if (item.Key < nMinHour)
						{
							tempList.Add(item.Key);
						}
					}
					foreach (int item2 in tempList)
					{
						BanManager.m_HourBanDict.Remove(item2);
					}
				}
				BanManager.m_UpdateHour = nCurrHour;
			}
		}

		// Token: 0x06001ACC RID: 6860 RVA: 0x00198D14 File Offset: 0x00196F14
		public static bool IsBanInMemory(string userID)
		{
			lock (BanManager.m_HourBanDictMutex)
			{
				foreach (KeyValuePair<int, Dictionary<string, int>> item in BanManager.m_HourBanDict)
				{
					if (item.Value.ContainsKey(userID))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001ACD RID: 6861 RVA: 0x00198DC0 File Offset: 0x00196FC0
		public static bool UnBanUserID2Memory(string userID)
		{
			lock (BanManager.m_HourBanDictMutex)
			{
				foreach (KeyValuePair<int, Dictionary<string, int>> item in BanManager.m_HourBanDict)
				{
					item.Value.Remove(userID);
				}
			}
			return false;
		}

		// Token: 0x04002978 RID: 10616
		private static Dictionary<string, int> _RoleNameDict = new Dictionary<string, int>();

		// Token: 0x04002979 RID: 10617
		private static Dictionary<string, long> _RoleNameTicksDict = new Dictionary<string, long>();

		// Token: 0x0400297A RID: 10618
		private static object m_HourBanDictMutex = new object();

		// Token: 0x0400297B RID: 10619
		private static Dictionary<int, Dictionary<string, int>> m_HourBanDict = new Dictionary<int, Dictionary<string, int>>();

		// Token: 0x0400297C RID: 10620
		private static int m_UpdateHour = Global.GetOffsetHour(TimeUtil.NowDateTime());

		// Token: 0x020005C4 RID: 1476
		public enum BanReason
		{
			// Token: 0x0400297E RID: 10622
			UseSpeedSoftware = 1,
			// Token: 0x0400297F RID: 10623
			RobotTask,
			// Token: 0x04002980 RID: 10624
			TradeException
		}
	}
}
