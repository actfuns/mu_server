using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class KarenBattleDataEast : KarenBattleDataBase
	{
		
		public Dictionary<int, KarenCenterConfig> KarenCenterConfigDict = new Dictionary<int, KarenCenterConfig>();

		
		public Dictionary<int, KarenBattleQiZhiConfig_East> NPCID2QiZhiConfigDict = new Dictionary<int, KarenBattleQiZhiConfig_East>();
	}
}
