using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200075A RID: 1882
	public class MonsterBossManager
	{
		// Token: 0x06002F69 RID: 12137 RVA: 0x002A65A9 File Offset: 0x002A47A9
		public static void AddBoss(Monster monster)
		{
			MonsterBossManager.BossList.Add(monster);
		}

		// Token: 0x06002F6A RID: 12138 RVA: 0x002A65B8 File Offset: 0x002A47B8
		public static Dictionary<int, BossData> GetBossDictData()
		{
			Dictionary<int, BossData> dict = new Dictionary<int, BossData>();
			for (int i = 0; i < MonsterBossManager.BossList.Count; i++)
			{
				Monster monster = MonsterBossManager.BossList[i];
				string nextBirthTime = "";
				if (!monster.Alive)
				{
					nextBirthTime = monster.MonsterZoneNode.GetNextBirthTimePoint();
				}
				BossData bossData = new BossData
				{
					MonsterID = monster.RoleID,
					ExtensionID = monster.MonsterInfo.ExtensionID,
					KillMonsterName = monster.WhoKillMeName,
					KillerOnline = ((GameManager.ClientMgr.FindClient(monster.WhoKillMeID) != null) ? 1 : 0),
					NextTime = nextBirthTime
				};
				dict[bossData.ExtensionID] = bossData;
			}
			return dict;
		}

		// Token: 0x06002F6B RID: 12139 RVA: 0x002A6694 File Offset: 0x002A4894
		public static void OnChangeName(int roleId, string oldName, string newName)
		{
			for (int i = 0; i < MonsterBossManager.BossList.Count; i++)
			{
				Monster j = MonsterBossManager.BossList[i];
				if (j.WhoKillMeID == roleId)
				{
					j.WhoKillMeName = newName;
				}
			}
		}

		// Token: 0x04003CD8 RID: 15576
		private static List<Monster> BossList = new List<Monster>();
	}
}
