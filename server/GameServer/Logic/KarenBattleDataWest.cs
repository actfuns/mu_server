using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class KarenBattleDataWest : KarenBattleDataBase
	{
		
		public Dictionary<int, int> KarenBattleDamage = new Dictionary<int, int>();

		
		public Dictionary<int, KarenBattleQiZhiConfig_West> NPCID2QiZhiConfigDict = new Dictionary<int, KarenBattleQiZhiConfig_West>();
	}
}
