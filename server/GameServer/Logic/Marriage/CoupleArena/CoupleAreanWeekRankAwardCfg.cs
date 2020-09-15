using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x0200035C RID: 860
	public class CoupleAreanWeekRankAwardCfg
	{
		// Token: 0x040016B9 RID: 5817
		public int Id;

		// Token: 0x040016BA RID: 5818
		public string Name;

		// Token: 0x040016BB RID: 5819
		public int StartRank;

		// Token: 0x040016BC RID: 5820
		public int EndRank;

		// Token: 0x040016BD RID: 5821
		public List<GoodsData> AwardGoods;
	}
}
