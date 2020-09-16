using System;

namespace GameServer.Logic
{
	
	public class EscapeBattleAwardsConfig
	{
		
		public int ID;

		
		public int MinRank;

		
		public int MaxRank;

		
		public AwardsItemList Award = new AwardsItemList();
	}
}
