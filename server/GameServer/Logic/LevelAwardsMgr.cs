using System;

namespace GameServer.Logic
{
	
	public class LevelAwardsMgr
	{
		
		private void ClearAwardsByLevels()
		{
			this.ExpByLevels = null;
			this.RongYuByLevels = null;
		}

		
		public void LoadFromXMlFile(string fullFileName, string rootName, string keyName, int resType = 0)
		{
			this.ClearAwardsByLevels();
			this.systemLevelAwardsXml.LoadFromXMlFile(fullFileName, rootName, keyName, resType);
		}

		
		private long GetExpByLevel(GameClient client, int level)
		{
			long[] expByLevels = this.ExpByLevels;
			if (null == expByLevels)
			{
				SystemXmlItem systemXmlItem = null;
				expByLevels = new long[Data.LevelUpExperienceList.Length];
				for (int i = 1; i < expByLevels.Length; i++)
				{
					if (this.systemLevelAwardsXml.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
					{
						expByLevels[i] = (long)Global.GMax(0, systemXmlItem.GetIntValue("Experience", -1));
					}
				}
				this.ExpByLevels = expByLevels;
			}
			long result;
			if (level <= 0 || level >= this.ExpByLevels.Length)
			{
				result = 0L;
			}
			else
			{
				long addExp = expByLevels[level];
				result = Global.GetExpMultiByZhuanShengExpXiShu(client, addExp);
			}
			return result;
		}

		
		private int GetRongYuByLevel(int level)
		{
			return 0;
		}

		
		private void ProcessAddRoleExperience(GameClient client)
		{
			long exp = this.GetExpByLevel(client, client.ClientData.Level);
			if (exp > 0L)
			{
				GameManager.ClientMgr.ProcessRoleExperience(client, exp, true, false, true, "none");
			}
		}

		
		private void ProcessAddRoleRongYu(GameClient client)
		{
			int rongYu = this.GetRongYuByLevel(client.ClientData.Level);
			if (rongYu > 0)
			{
				GameManager.ClientMgr.ModifyRongYuValue(client, rongYu, true, true);
			}
		}

		
		public void ProcessBangZhanAwards(GameClient client)
		{
			if (client.ClientData.GuanZhanGM <= 0)
			{
				this.ProcessAddRoleExperience(client);
			}
		}

		
		public SystemXmlItems systemLevelAwardsXml = new SystemXmlItems();

		
		private long[] ExpByLevels = null;

		
		private int[] RongYuByLevels = null;
	}
}
