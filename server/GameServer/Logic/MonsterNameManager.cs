using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class MonsterNameManager
	{
		
		public static void AddMonsterName(int monsterID, string monsterName)
		{
			lock (MonsterNameManager._MonsterID2NameDict)
			{
				MonsterNameManager._MonsterID2NameDict[monsterID] = monsterName;
			}
		}

		
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

		
		private static Dictionary<int, string> _MonsterID2NameDict = new Dictionary<int, string>(1000);
	}
}
