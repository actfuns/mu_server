using System;

namespace GameServer.Logic
{
	
	public class BangZhanAwardsMgr
	{
		
		public static void ClearAwardsByLevels()
		{
			BangZhanAwardsMgr.ExpByLevels = null;
			BangZhanAwardsMgr.RongYuByLevels = null;
		}

		
		private static long GetExpByLevel(int level)
		{
			long[] expByLevels = BangZhanAwardsMgr.ExpByLevels;
			if (null == expByLevels)
			{
				SystemXmlItem systemXmlItem = null;
				expByLevels = new long[Data.LevelUpExperienceList.Length - 1];
				for (int i = 1; i < expByLevels.Length; i++)
				{
					if (GameManager.systemBangZhanAwardsMgr.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
					{
						expByLevels[i] = (long)Global.GMax(0, systemXmlItem.GetIntValue("Experience", -1));
					}
				}
				BangZhanAwardsMgr.ExpByLevels = expByLevels;
			}
			long result;
			if (level <= 0 || level >= BangZhanAwardsMgr.ExpByLevels.Length)
			{
				result = 0L;
			}
			else
			{
				result = expByLevels[level];
			}
			return result;
		}

		
		private static int GetRongYuByLevel(int level)
		{
			int[] rongYuByLevels = BangZhanAwardsMgr.RongYuByLevels;
			if (null == rongYuByLevels)
			{
				SystemXmlItem systemXmlItem = null;
				rongYuByLevels = new int[Data.LevelUpExperienceList.Length - 1];
				for (int i = 1; i < rongYuByLevels.Length; i++)
				{
					if (GameManager.systemBangZhanAwardsMgr.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
					{
						rongYuByLevels[i] = Global.GMax(0, systemXmlItem.GetIntValue("RongYu", -1));
					}
				}
				BangZhanAwardsMgr.RongYuByLevels = rongYuByLevels;
			}
			int result;
			if (level <= 0 || level >= BangZhanAwardsMgr.RongYuByLevels.Length)
			{
				result = 0;
			}
			else
			{
				result = rongYuByLevels[level];
			}
			return result;
		}

		
		private static void ProcessAddRoleExperience(GameClient client)
		{
			long exp = BangZhanAwardsMgr.GetExpByLevel(client.ClientData.Level);
			if (exp > 0L)
			{
				GameManager.ClientMgr.ProcessRoleExperience(client, exp, true, false, false, "none");
			}
		}

		
		private static void ProcessAddRoleRongYu(GameClient client)
		{
			int rongYu = BangZhanAwardsMgr.GetRongYuByLevel(client.ClientData.Level);
			if (rongYu > 0)
			{
				GameManager.ClientMgr.ModifyRongYuValue(client, rongYu, true, true);
			}
		}

		
		public static void ProcessBangZhanAwards(GameClient client)
		{
		}

		
		private static long[] ExpByLevels = null;

		
		private static int[] RongYuByLevels = null;
	}
}
