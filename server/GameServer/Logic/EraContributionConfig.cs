using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class EraContributionConfig
	{
		
		public int ID;

		
		public int EraID;

		
		public int ProgressID;

		
		public int GoodsID;

		
		public int Contribution;

		
		public HashSet<int> MonsterIDSet = new HashSet<int>();
	}
}
