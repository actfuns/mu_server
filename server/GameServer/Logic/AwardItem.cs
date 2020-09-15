using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020001B9 RID: 441
	public class AwardItem
	{
		// Token: 0x040009DB RID: 2523
		public int AwardYuanBao = 0;

		// Token: 0x040009DC RID: 2524
		public List<GoodsData> GoodsDataList = new List<GoodsData>();

		// Token: 0x040009DD RID: 2525
		public int MinAwardCondionValue = 0;

		// Token: 0x040009DE RID: 2526
		public int MinAwardCondionValue2 = 0;

		// Token: 0x040009DF RID: 2527
		public int MinAwardCondionValue3 = 0;
	}
}
