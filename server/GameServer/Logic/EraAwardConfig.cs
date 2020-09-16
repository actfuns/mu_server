using System;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class EraAwardConfig : EraAwardConfigBase
	{
		
		public string EraName;

		
		public int EraRanking;

		
		public int Progress;

		
		public int Contribution;

		
		public AwardsItemList LeaderReward = new AwardsItemList();

		
		public AwardsItemList MasterReward = new AwardsItemList();

		
		public AwardsItemList Reward = new AwardsItemList();
	}
}
