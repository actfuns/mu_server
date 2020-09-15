using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020004F1 RID: 1265
	public class XiLianShuXing
	{
		// Token: 0x04002186 RID: 8582
		public int ID;

		// Token: 0x04002187 RID: 8583
		public string Name;

		// Token: 0x04002188 RID: 8584
		public int NeedJinBi;

		// Token: 0x04002189 RID: 8585
		public int NeedZuanShi;

		// Token: 0x0400218A RID: 8586
		public List<int> NeedGoodsIDs = new List<int>();

		// Token: 0x0400218B RID: 8587
		public List<int> NeedGoodsCounts = new List<int>();

		// Token: 0x0400218C RID: 8588
		public Dictionary<int, List<long>> PromoteJinBiRange = new Dictionary<int, List<long>>();

		// Token: 0x0400218D RID: 8589
		public Dictionary<int, List<long>> PromoteZuanShiRange = new Dictionary<int, List<long>>();

		// Token: 0x0400218E RID: 8590
		public Dictionary<int, int> PromotePropLimit = new Dictionary<int, int>();

		// Token: 0x0400218F RID: 8591
		public Dictionary<int, int> PromoteRangeMin = new Dictionary<int, int>();

		// Token: 0x04002190 RID: 8592
		public Dictionary<int, int> PromoteRangeMax = new Dictionary<int, int>();
	}
}
