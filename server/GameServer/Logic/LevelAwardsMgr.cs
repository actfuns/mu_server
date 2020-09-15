using System;

namespace GameServer.Logic
{
	// Token: 0x020004C1 RID: 1217
	public class LevelAwardsMgr
	{
		// Token: 0x0600167D RID: 5757 RVA: 0x0015FC58 File Offset: 0x0015DE58
		private void ClearAwardsByLevels()
		{
			this.ExpByLevels = null;
			this.RongYuByLevels = null;
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x0015FC69 File Offset: 0x0015DE69
		public void LoadFromXMlFile(string fullFileName, string rootName, string keyName, int resType = 0)
		{
			this.ClearAwardsByLevels();
			this.systemLevelAwardsXml.LoadFromXMlFile(fullFileName, rootName, keyName, resType);
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x0015FC84 File Offset: 0x0015DE84
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

		// Token: 0x06001680 RID: 5760 RVA: 0x0015FD3C File Offset: 0x0015DF3C
		private int GetRongYuByLevel(int level)
		{
			return 0;
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x0015FD50 File Offset: 0x0015DF50
		private void ProcessAddRoleExperience(GameClient client)
		{
			long exp = this.GetExpByLevel(client, client.ClientData.Level);
			if (exp > 0L)
			{
				GameManager.ClientMgr.ProcessRoleExperience(client, exp, true, false, true, "none");
			}
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x0015FD94 File Offset: 0x0015DF94
		private void ProcessAddRoleRongYu(GameClient client)
		{
			int rongYu = this.GetRongYuByLevel(client.ClientData.Level);
			if (rongYu > 0)
			{
				GameManager.ClientMgr.ModifyRongYuValue(client, rongYu, true, true);
			}
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x0015FDD0 File Offset: 0x0015DFD0
		public void ProcessBangZhanAwards(GameClient client)
		{
			if (client.ClientData.GuanZhanGM <= 0)
			{
				this.ProcessAddRoleExperience(client);
			}
		}

		// Token: 0x04002047 RID: 8263
		public SystemXmlItems systemLevelAwardsXml = new SystemXmlItems();

		// Token: 0x04002048 RID: 8264
		private long[] ExpByLevels = null;

		// Token: 0x04002049 RID: 8265
		private int[] RongYuByLevels = null;
	}
}
