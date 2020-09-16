using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class GoldCopySceneData
	{
		
		
		
		public List<int[]> m_MonsterPatorlPathList { get; set; }

		
		public Dictionary<int, GoldCopySceneMonster> GoldCopySceneMonsterData = new Dictionary<int, GoldCopySceneMonster>();
	}
}
