using System;
using System.Collections.Generic;

namespace GameServer.Logic.Goods
{
	// Token: 0x020004E9 RID: 1257
	public class GoodsReplaceResult
	{
		// Token: 0x0600176D RID: 5997 RVA: 0x0016F6F0 File Offset: 0x0016D8F0
		public int TotalGoodsCnt()
		{
			return this.OriginBindGoods.GoodsCnt + this.OriginUnBindGoods.GoodsCnt + this.TotalBindCnt + this.TotalUnBindCnt;
		}

		// Token: 0x04002148 RID: 8520
		public GoodsReplaceResult.ReplaceItem OriginBindGoods = new GoodsReplaceResult.ReplaceItem();

		// Token: 0x04002149 RID: 8521
		public GoodsReplaceResult.ReplaceItem OriginUnBindGoods = new GoodsReplaceResult.ReplaceItem();

		// Token: 0x0400214A RID: 8522
		public int TotalBindCnt = 0;

		// Token: 0x0400214B RID: 8523
		public List<GoodsReplaceResult.ReplaceItem> BindList = new List<GoodsReplaceResult.ReplaceItem>();

		// Token: 0x0400214C RID: 8524
		public int TotalUnBindCnt = 0;

		// Token: 0x0400214D RID: 8525
		public List<GoodsReplaceResult.ReplaceItem> UnBindList = new List<GoodsReplaceResult.ReplaceItem>();

		// Token: 0x020004EA RID: 1258
		public class ReplaceItem
		{
			// Token: 0x0400214E RID: 8526
			public int GoodsID = 0;

			// Token: 0x0400214F RID: 8527
			public int GoodsCnt = 0;

			// Token: 0x04002150 RID: 8528
			public bool IsBind = false;
		}
	}
}
