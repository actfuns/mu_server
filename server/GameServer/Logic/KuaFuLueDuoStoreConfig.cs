using System;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000220 RID: 544
	public class KuaFuLueDuoStoreConfig
	{
		// Token: 0x04000C83 RID: 3203
		public int ID;

		// Token: 0x04000C84 RID: 3204
		public int Type;

		// Token: 0x04000C85 RID: 3205
		public GoodsData SaleData = null;

		// Token: 0x04000C86 RID: 3206
		public int ZuanShi;

		// Token: 0x04000C87 RID: 3207
		public int JueXingNum;

		// Token: 0x04000C88 RID: 3208
		public int SinglePurchase;

		// Token: 0x04000C89 RID: 3209
		public int BeginNum;

		// Token: 0x04000C8A RID: 3210
		public int EndNum;

		// Token: 0x04000C8B RID: 3211
		public int RandNumMinus = 0;

		// Token: 0x04000C8C RID: 3212
		public bool RandSkip = false;
	}
}
