using System;
using System.Collections.Generic;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x020000A6 RID: 166
	public class GlodAuctionIComparer : IComparer<AuctionItemS2C>
	{
		// Token: 0x0600028B RID: 651 RVA: 0x0002B7AB File Offset: 0x000299AB
		public GlodAuctionIComparer(int ordeType, bool isAscend)
		{
			this.IOrdeType = ordeType;
			this.IsAscend = isAscend;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0002B7C4 File Offset: 0x000299C4
		public int Compare(AuctionItemS2C d1, AuctionItemS2C d2)
		{
			int val = this.IsAscend ? -1 : 1;
			long val2 = d1.LastTime;
			long val3 = d2.LastTime;
			if (1 == this.IOrdeType)
			{
				val2 = d1.Price;
				val3 = d2.Price;
			}
			else if (2 == this.IOrdeType)
			{
				val2 = d1.MaxPrice;
				val3 = d2.MaxPrice;
			}
			int result;
			if (val2 < val3)
			{
				result = val;
			}
			else if (val2 > val3)
			{
				result = -val;
			}
			else if (d1.LastTime < d2.LastTime)
			{
				result = val;
			}
			else if (d1.LastTime > d2.LastTime)
			{
				result = -val;
			}
			else if (d1.Price < d2.Price)
			{
				result = val;
			}
			else if (d1.Price > d2.Price)
			{
				result = -val;
			}
			else if (d1.MaxPrice < d2.MaxPrice)
			{
				result = val;
			}
			else if (d1.MaxPrice > d2.MaxPrice)
			{
				result = -val;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x040003E3 RID: 995
		private int IOrdeType;

		// Token: 0x040003E4 RID: 996
		private bool IsAscend;
	}
}
