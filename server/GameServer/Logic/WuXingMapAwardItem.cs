using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020007C1 RID: 1985
	public class WuXingMapAwardItem
	{
		// Token: 0x04003FAA RID: 16298
		public int MapCode = 0;

		// Token: 0x04003FAB RID: 16299
		public int Money1 = 0;

		// Token: 0x04003FAC RID: 16300
		public double ExpXiShu = 0.0;

		// Token: 0x04003FAD RID: 16301
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x04003FAE RID: 16302
		public int MinBlessPoint = 0;

		// Token: 0x04003FAF RID: 16303
		public int MaxBlessPoint = 0;
	}
}
