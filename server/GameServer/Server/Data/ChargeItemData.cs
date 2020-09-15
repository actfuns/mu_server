using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x0200012D RID: 301
	public class ChargeItemData
	{
		// Token: 0x0400067F RID: 1663
		public int ChargeItemID;

		// Token: 0x04000680 RID: 1664
		public int ChargeDangID;

		// Token: 0x04000681 RID: 1665
		public string GoodsStringOne = "";

		// Token: 0x04000682 RID: 1666
		public List<GoodsData> GoodsDataOne = new List<GoodsData>();

		// Token: 0x04000683 RID: 1667
		public string GoodsStringTwo = "";

		// Token: 0x04000684 RID: 1668
		public List<GoodsData> GoodsDataTwo = new List<GoodsData>();

		// Token: 0x04000685 RID: 1669
		public int SinglePurchase;

		// Token: 0x04000686 RID: 1670
		public int DayPurchase;

		// Token: 0x04000687 RID: 1671
		public int ThemePurchase;

		// Token: 0x04000688 RID: 1672
		public string FromDate;

		// Token: 0x04000689 RID: 1673
		public string ToDate;
	}
}
