using System;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000332 RID: 818
	public class KingOfBattleStoreConfig
	{
		// Token: 0x0400154C RID: 5452
		public int ID;

		// Token: 0x0400154D RID: 5453
		public GoodsData SaleData = null;

		// Token: 0x0400154E RID: 5454
		public int JiFen;

		// Token: 0x0400154F RID: 5455
		public int SinglePurchase;

		// Token: 0x04001550 RID: 5456
		public int BeginNum;

		// Token: 0x04001551 RID: 5457
		public int EndNum;

		// Token: 0x04001552 RID: 5458
		public int RandNumMinus = 0;

		// Token: 0x04001553 RID: 5459
		public bool RandSkip = false;
	}
}
