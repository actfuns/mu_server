using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000085 RID: 133
	public class EscapeBDuanAwardsConfig
	{
		// Token: 0x0400030D RID: 781
		public int ID;

		// Token: 0x0400030E RID: 782
		public string RankLevelName;

		// Token: 0x0400030F RID: 783
		public int RankValue;

		// Token: 0x04000310 RID: 784
		public List<List<int>> WinRankValue;

		// Token: 0x04000311 RID: 785
		public List<List<int>> LoseRankValue;

		// Token: 0x04000312 RID: 786
		public object FirstWinAwardsItemList;

		// Token: 0x04000313 RID: 787
		public object WinAwardsItemList;

		// Token: 0x04000314 RID: 788
		public object LoseAwardsItemList;
	}
}
