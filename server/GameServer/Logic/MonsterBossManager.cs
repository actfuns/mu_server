using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class MonsterBossManager
	{
		
		public static void AddBoss(Monster monster)
		{
			MonsterBossManager.BossList.Add(monster);
		}

		
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

		
		private static List<Monster> BossList = new List<Monster>();
	}
}
