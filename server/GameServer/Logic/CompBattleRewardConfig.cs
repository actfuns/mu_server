using System;

namespace GameServer.Logic
{
	// Token: 0x0200025B RID: 603
	public class CompBattleRewardConfig
	{
		// Token: 0x04000EC6 RID: 3782
		public int ID;

		// Token: 0x04000EC7 RID: 3783
		public int Rank;

		// Token: 0x04000EC8 RID: 3784
		public double RankRate;

		// Token: 0x04000EC9 RID: 3785
		public int Grade;

		// Token: 0x04000ECA RID: 3786
		public int Contribution;

		// Token: 0x04000ECB RID: 3787
		public AwardsItemList AwardsItemListOne = new AwardsItemList();

		// Token: 0x04000ECC RID: 3788
		public AwardsItemList AwardsItemListTwo = new AwardsItemList();
	}
}
