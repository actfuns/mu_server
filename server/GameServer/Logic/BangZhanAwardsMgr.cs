using System;

namespace GameServer.Logic
{
	// Token: 0x020005C2 RID: 1474
	public class BangZhanAwardsMgr
	{
		// Token: 0x06001ABF RID: 6847 RVA: 0x0019864A File Offset: 0x0019684A
		public static void ClearAwardsByLevels()
		{
			BangZhanAwardsMgr.ExpByLevels = null;
			BangZhanAwardsMgr.RongYuByLevels = null;
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x0019865C File Offset: 0x0019685C
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

		// Token: 0x06001AC1 RID: 6849 RVA: 0x00198704 File Offset: 0x00196904
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

		// Token: 0x06001AC2 RID: 6850 RVA: 0x001987AC File Offset: 0x001969AC
		private static void ProcessAddRoleExperience(GameClient client)
		{
			long exp = BangZhanAwardsMgr.GetExpByLevel(client.ClientData.Level);
			if (exp > 0L)
			{
				GameManager.ClientMgr.ProcessRoleExperience(client, exp, true, false, false, "none");
			}
		}

		// Token: 0x06001AC3 RID: 6851 RVA: 0x001987EC File Offset: 0x001969EC
		private static void ProcessAddRoleRongYu(GameClient client)
		{
			int rongYu = BangZhanAwardsMgr.GetRongYuByLevel(client.ClientData.Level);
			if (rongYu > 0)
			{
				GameManager.ClientMgr.ModifyRongYuValue(client, rongYu, true, true);
			}
		}

		// Token: 0x06001AC4 RID: 6852 RVA: 0x00198824 File Offset: 0x00196A24
		public static void ProcessBangZhanAwards(GameClient client)
		{
		}

		// Token: 0x04002976 RID: 10614
		private static long[] ExpByLevels = null;

		// Token: 0x04002977 RID: 10615
		private static int[] RongYuByLevels = null;
	}
}
