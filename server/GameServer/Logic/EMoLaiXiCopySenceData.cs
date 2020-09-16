using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class EMoLaiXiCopySenceData
	{
		
		public List<EMoLaiXiCopySenceMonster> EMoLaiXiCopySenceMonsterData = new List<EMoLaiXiCopySenceMonster>();

		
		public Dictionary<int, List<int[]>> m_MonsterPatorlPathLists = new Dictionary<int, List<int[]>>();

		
		public int TotalWave;

		
		public int FaildEscapeMonsterNum;
	}
}
