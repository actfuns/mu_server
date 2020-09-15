using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006F1 RID: 1777
	public class SongLiItem
	{
		// Token: 0x040039EF RID: 14831
		public long StartTicks = 0L;

		// Token: 0x040039F0 RID: 14832
		public long EndTicks = 0L;

		// Token: 0x040039F1 RID: 14833
		public int IsNeedCode = 0;

		// Token: 0x040039F2 RID: 14834
		public Dictionary<int, List<GoodsData>> SongGoodsDataDict = new Dictionary<int, List<GoodsData>>();
	}
}
