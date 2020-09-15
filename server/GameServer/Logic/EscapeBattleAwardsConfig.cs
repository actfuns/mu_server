using System;

namespace GameServer.Logic
{
	// Token: 0x02000087 RID: 135
	public class EscapeBattleAwardsConfig
	{
		// Token: 0x0400031A RID: 794
		public int ID;

		// Token: 0x0400031B RID: 795
		public int MinRank;

		// Token: 0x0400031C RID: 796
		public int MaxRank;

		// Token: 0x0400031D RID: 797
		public AwardsItemList Award = new AwardsItemList();
	}
}
