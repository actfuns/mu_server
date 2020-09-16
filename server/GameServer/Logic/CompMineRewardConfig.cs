using System;

namespace GameServer.Logic
{
	
	public class CompMineRewardConfig
	{
		
		public int ID;

		
		public int Rank;

		
		public double RankRate;

		
		public int Grade;

		
		public int Contribution;

		
		public AwardsItemList AwardsItemListOne = new AwardsItemList();

		
		public AwardsItemList AwardsItemListTwo = new AwardsItemList();
	}
}
