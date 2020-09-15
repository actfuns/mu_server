using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200075F RID: 1887
	public class MonsterNameManager
	{
		// Token: 0x06002FE1 RID: 12257 RVA: 0x002AE75C File Offset: 0x002AC95C
		public static void AddMonsterName(int monsterID, string monsterName)
		{
			lock (MonsterNameManager._MonsterID2NameDict)
			{
				MonsterNameManager._MonsterID2NameDict[monsterID] = monsterName;
			}
		}

		// Token: 0x06002FE2 RID: 12258 RVA: 0x002AE7B0 File Offset: 0x002AC9B0
		public static string GetMonsterName(int monsterID)
		{
			string monsterName = null;
			lock (MonsterNameManager._MonsterID2NameDict)
			{
				if (MonsterNameManager._MonsterID2NameDict.TryGetValue(monsterID, out monsterName))
				{
					return monsterName;
				}
			}
			return "";
		}

		// Token: 0x04003CE6 RID: 15590
		private static Dictionary<int, string> _MonsterID2NameDict = new Dictionary<int, string>(1000);
	}
}
