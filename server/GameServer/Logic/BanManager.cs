using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class BanManager
	{
		
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

		
		private static Dictionary<string, int> _RoleNameDict = new Dictionary<string, int>();

		
		private static Dictionary<string, long> _RoleNameTicksDict = new Dictionary<string, long>();

		
		private static object m_HourBanDictMutex = new object();

		
		private static Dictionary<int, Dictionary<string, int>> m_HourBanDict = new Dictionary<int, Dictionary<string, int>>();

		
		private static int m_UpdateHour = Global.GetOffsetHour(TimeUtil.NowDateTime());

		
		public enum BanReason
		{
			
			UseSpeedSoftware = 1,
			
			RobotTask,
			
			TradeException
		}
	}
}
