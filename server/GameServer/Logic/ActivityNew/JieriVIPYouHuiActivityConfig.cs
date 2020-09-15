using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000043 RID: 67
	public class JieriVIPYouHuiActivityConfig
	{
		// Token: 0x04000172 RID: 370
		public int ID = 0;

		// Token: 0x04000173 RID: 371
		public int MinVIPLev = 0;

		// Token: 0x04000174 RID: 372
		public int Price = 0;

		// Token: 0x04000175 RID: 373
		public int SinglePurchase = 0;

		// Token: 0x04000176 RID: 374
		public int FullPurchase = 0;

		// Token: 0x04000177 RID: 375
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		// Token: 0x04000178 RID: 376
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();

		// Token: 0x04000179 RID: 377
		public AwardEffectTimeItem GoodsDataListThr = new AwardEffectTimeItem();
	}
}
