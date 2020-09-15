using System;
using System.Collections.Generic;
using Server.Data;

namespace GameDBServer.Logic.Rank
{
	// Token: 0x0200015F RID: 351
	public class RankData
	{
		// Token: 0x04000866 RID: 2150
		public double QueryFromDBTime;

		// Token: 0x04000867 RID: 2151
		public double MaxRankCount;

		// Token: 0x04000868 RID: 2152
		public List<int> minGateValueList = null;

		// Token: 0x04000869 RID: 2153
		public List<InputKingPaiHangData> RankDataList = null;
	}
}
