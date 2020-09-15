using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020005DC RID: 1500
	public class BuChangItem
	{
		// Token: 0x04002A25 RID: 10789
		public int MinLevel = 0;

		// Token: 0x04002A26 RID: 10790
		public int MinZhuanSheng = 0;

		// Token: 0x04002A27 RID: 10791
		public int MaxLevel = 0;

		// Token: 0x04002A28 RID: 10792
		public int MaxZhuanSheng = 0;

		// Token: 0x04002A29 RID: 10793
		public int MoJing = 0;

		// Token: 0x04002A2A RID: 10794
		public long Experience = 0L;

		// Token: 0x04002A2B RID: 10795
		public List<GoodsData> GoodsDataList = new List<GoodsData>();
	}
}
