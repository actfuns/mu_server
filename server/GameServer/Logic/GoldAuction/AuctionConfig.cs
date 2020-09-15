using System;
using System.Collections.Generic;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x020000A4 RID: 164
	public class AuctionConfig
	{
		// Token: 0x0600027C RID: 636 RVA: 0x0002AB40 File Offset: 0x00028D40
		public int GetTimeByAuction(int Auction)
		{
			for (int i = 0; i < this.OrderList.Count; i++)
			{
				if (Auction == this.OrderList[i])
				{
					return this.TimeList[i];
				}
			}
			return -1;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0002AB98 File Offset: 0x00028D98
		public int GetNextAuction(int Auction)
		{
			bool isFind = false;
			foreach (int item in this.OrderList)
			{
				if (Auction == item)
				{
					isFind = true;
				}
				else if (GoldAuctionManager.IsOpenAuction((AuctionOrderEnum)item))
				{
					if (isFind)
					{
						return item;
					}
				}
			}
			return -1;
		}

		// Token: 0x040003D5 RID: 981
		public int ID;

		// Token: 0x040003D6 RID: 982
		public string Name;

		// Token: 0x040003D7 RID: 983
		public List<int> OrderList = new List<int>();

		// Token: 0x040003D8 RID: 984
		public List<int> TimeList = new List<int>();

		// Token: 0x040003D9 RID: 985
		public int OriginPrice;

		// Token: 0x040003DA RID: 986
		public int UnitPrice;

		// Token: 0x040003DB RID: 987
		public int MaxPrice;

		// Token: 0x040003DC RID: 988
		public string SuccessTitle;

		// Token: 0x040003DD RID: 989
		public string SuccessIntro;

		// Token: 0x040003DE RID: 990
		public string FailTitle;

		// Token: 0x040003DF RID: 991
		public string FailIntro;
	}
}
