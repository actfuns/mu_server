using System;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x020000A1 RID: 161
	public class GoldAuctionItem : GoldAuctionDBItem
	{
		// Token: 0x06000279 RID: 633 RVA: 0x0002AAFE File Offset: 0x00028CFE
		public GoldAuctionItem()
		{
			this.Lock = false;
			this.LifeTime = 0;
		}

		// Token: 0x040003CC RID: 972
		public bool Lock;

		// Token: 0x040003CD RID: 973
		public int LifeTime;
	}
}
