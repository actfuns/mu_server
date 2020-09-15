using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x0200003C RID: 60
	public class EverydayActivityConfig
	{
		// Token: 0x0400013E RID: 318
		public int ID = 0;

		// Token: 0x0400013F RID: 319
		public int Type = 0;

		// Token: 0x04000140 RID: 320
		public EveryActGoalData GoalData = new EveryActGoalData();

		// Token: 0x04000141 RID: 321
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		// Token: 0x04000142 RID: 322
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();

		// Token: 0x04000143 RID: 323
		public AwardEffectTimeItem GoodsDataListThr = new AwardEffectTimeItem();

		// Token: 0x04000144 RID: 324
		public EveryActPriceData Price = new EveryActPriceData();

		// Token: 0x04000145 RID: 325
		public int PurchaseNum = -1;
	}
}
