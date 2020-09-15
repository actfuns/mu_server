using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000321 RID: 801
	public class KarenBattleDataEast : KarenBattleDataBase
	{
		// Token: 0x040014B8 RID: 5304
		public Dictionary<int, KarenCenterConfig> KarenCenterConfigDict = new Dictionary<int, KarenCenterConfig>();

		// Token: 0x040014B9 RID: 5305
		public Dictionary<int, KarenBattleQiZhiConfig_East> NPCID2QiZhiConfigDict = new Dictionary<int, KarenBattleQiZhiConfig_East>();
	}
}
