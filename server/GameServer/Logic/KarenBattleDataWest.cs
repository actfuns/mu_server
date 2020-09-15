using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000320 RID: 800
	public class KarenBattleDataWest : KarenBattleDataBase
	{
		// Token: 0x040014B6 RID: 5302
		public Dictionary<int, int> KarenBattleDamage = new Dictionary<int, int>();

		// Token: 0x040014B7 RID: 5303
		public Dictionary<int, KarenBattleQiZhiConfig_West> NPCID2QiZhiConfigDict = new Dictionary<int, KarenBattleQiZhiConfig_West>();
	}
}
