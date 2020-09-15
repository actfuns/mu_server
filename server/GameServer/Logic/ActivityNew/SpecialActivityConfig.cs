using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001CD RID: 461
	public class SpecialActivityConfig
	{
		// Token: 0x04000A2F RID: 2607
		public int ID = 0;

		// Token: 0x04000A30 RID: 2608
		public int GroupID = 0;

		// Token: 0x04000A31 RID: 2609
		public DateTime FromDay;

		// Token: 0x04000A32 RID: 2610
		public DateTime ToDay;

		// Token: 0x04000A33 RID: 2611
		public SpecActLimitData LevLimit = new SpecActLimitData();

		// Token: 0x04000A34 RID: 2612
		public SpecActLimitData VipLimit = new SpecActLimitData();

		// Token: 0x04000A35 RID: 2613
		public SpecActLimitData ChongZhiLimit = new SpecActLimitData();

		// Token: 0x04000A36 RID: 2614
		public SpecActLimitData WingLimit = new SpecActLimitData();

		// Token: 0x04000A37 RID: 2615
		public SpecActLimitData ChengJiuLimit = new SpecActLimitData();

		// Token: 0x04000A38 RID: 2616
		public SpecActLimitData JunXianLimit = new SpecActLimitData();

		// Token: 0x04000A39 RID: 2617
		public SpecActLimitData MerlinLimit = new SpecActLimitData();

		// Token: 0x04000A3A RID: 2618
		public SpecActLimitData ShengWuLimit = new SpecActLimitData();

		// Token: 0x04000A3B RID: 2619
		public SpecActLimitData RingLimit = new SpecActLimitData();

		// Token: 0x04000A3C RID: 2620
		public SpecActLimitData ShouHuShenLimit = new SpecActLimitData();

		// Token: 0x04000A3D RID: 2621
		public int Type = 0;

		// Token: 0x04000A3E RID: 2622
		public SpecActGoalData GoalData = new SpecActGoalData();

		// Token: 0x04000A3F RID: 2623
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		// Token: 0x04000A40 RID: 2624
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();

		// Token: 0x04000A41 RID: 2625
		public AwardEffectTimeItem GoodsDataListThr = new AwardEffectTimeItem();

		// Token: 0x04000A42 RID: 2626
		public SpecActPriceData Price = new SpecActPriceData();

		// Token: 0x04000A43 RID: 2627
		public int PurchaseNum = -1;
	}
}
