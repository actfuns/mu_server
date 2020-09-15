using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006F0 RID: 1776
	public class BigAwardItem
	{
		// Token: 0x040039EB RID: 14827
		public long StartTicks = 0L;

		// Token: 0x040039EC RID: 14828
		public long EndTicks = 0L;

		// Token: 0x040039ED RID: 14829
		public Dictionary<int, int> NeedJiFenDict = new Dictionary<int, int>();

		// Token: 0x040039EE RID: 14830
		public Dictionary<int, List<GoodsData>> GoodsDataListDict = new Dictionary<int, List<GoodsData>>();
	}
}
