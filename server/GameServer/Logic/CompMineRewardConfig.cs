using System;

namespace GameServer.Logic
{
	// Token: 0x02000272 RID: 626
	public class CompMineRewardConfig
	{
		// Token: 0x04000F94 RID: 3988
		public int ID;

		// Token: 0x04000F95 RID: 3989
		public int Rank;

		// Token: 0x04000F96 RID: 3990
		public double RankRate;

		// Token: 0x04000F97 RID: 3991
		public int Grade;

		// Token: 0x04000F98 RID: 3992
		public int Contribution;

		// Token: 0x04000F99 RID: 3993
		public AwardsItemList AwardsItemListOne = new AwardsItemList();

		// Token: 0x04000F9A RID: 3994
		public AwardsItemList AwardsItemListTwo = new AwardsItemList();
	}
}
