using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.FluorescentGem
{
	// Token: 0x020002CD RID: 717
	internal class SoulStoneRandConfig
	{
		// Token: 0x04001277 RID: 4727
		public int RandId;

		// Token: 0x04001278 RID: 4728
		public int NeedLangHunFenMo;

		// Token: 0x04001279 RID: 4729
		public double SuccessRate;

		// Token: 0x0400127A RID: 4730
		public List<int> SuccessTo = new List<int>();

		// Token: 0x0400127B RID: 4731
		public List<int> FailTo = new List<int>();

		// Token: 0x0400127C RID: 4732
		public List<SoulStoneRandInfo> RandStoneList = new List<SoulStoneRandInfo>();

		// Token: 0x0400127D RID: 4733
		public int RandMinNumber;

		// Token: 0x0400127E RID: 4734
		public int RandMaxNumber;

		// Token: 0x0400127F RID: 4735
		public Dictionary<ESoulStoneExtCostType, int> AddedNeedDict = new Dictionary<ESoulStoneExtCostType, int>();

		// Token: 0x04001280 RID: 4736
		public double AddedRate;

		// Token: 0x04001281 RID: 4737
		public GoodsData AddedGoods;

		// Token: 0x04001282 RID: 4738
		public Dictionary<ESoulStoneExtCostType, int> ReduceNeedDict = new Dictionary<ESoulStoneExtCostType, int>();

		// Token: 0x04001283 RID: 4739
		public double ReduceRate;

		// Token: 0x04001284 RID: 4740
		public int ReduceValue;

		// Token: 0x04001285 RID: 4741
		public Dictionary<ESoulStoneExtCostType, int> UpSucRateNeedDict = new Dictionary<ESoulStoneExtCostType, int>();

		// Token: 0x04001286 RID: 4742
		public double UpSucRateTo;

		// Token: 0x04001287 RID: 4743
		public Dictionary<ESoulStoneExtCostType, int> FailHoldNeedDict = new Dictionary<ESoulStoneExtCostType, int>();

		// Token: 0x04001288 RID: 4744
		public List<int> FailToIfHold = new List<int>();
	}
}
