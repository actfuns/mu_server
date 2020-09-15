using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020002A1 RID: 673
	public class EraContributionConfig
	{
		// Token: 0x0400108E RID: 4238
		public int ID;

		// Token: 0x0400108F RID: 4239
		public int EraID;

		// Token: 0x04001090 RID: 4240
		public int ProgressID;

		// Token: 0x04001091 RID: 4241
		public int GoodsID;

		// Token: 0x04001092 RID: 4242
		public int Contribution;

		// Token: 0x04001093 RID: 4243
		public HashSet<int> MonsterIDSet = new HashSet<int>();
	}
}
