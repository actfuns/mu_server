using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class ExperienceCopyMapDataInfo
	{
		
		
		
		public int CopyMapID { get; set; }

		
		
		
		public int MapCodeID { get; set; }

		
		
		
		public Dictionary<int, List<int>> MonsterIDList { get; set; }

		
		
		
		public Dictionary<int, List<int>> MonsterNumList { get; set; }

		
		
		
		public int posX { get; set; }

		
		
		
		public int posZ { get; set; }

		
		
		
		public int Radius { get; set; }

		
		
		
		public int MonsterSum { get; set; }

		
		
		
		public int[] CreateNextWaveMonsterCondition { get; set; }
	}
}
