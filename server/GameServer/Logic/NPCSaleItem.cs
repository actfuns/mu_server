using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200076E RID: 1902
	public class NPCSaleItem
	{
		// Token: 0x04003D6F RID: 15727
		public Dictionary<int, bool> SaleTypesDict = new Dictionary<int, bool>();

		// Token: 0x04003D70 RID: 15728
		public int Money1Price = 0;

		// Token: 0x04003D71 RID: 15729
		public int YinLiangPrice = 0;

		// Token: 0x04003D72 RID: 15730
		public int TianDiJingYuanPrice = 0;

		// Token: 0x04003D73 RID: 15731
		public int LieShaZhiPrice = 0;

		// Token: 0x04003D74 RID: 15732
		public int JiFenPrice = 0;

		// Token: 0x04003D75 RID: 15733
		public int JunGongPrice = 0;

		// Token: 0x04003D76 RID: 15734
		public int ZhanHunPrice = 0;

		// Token: 0x04003D77 RID: 15735
		public int Forge_level;

		// Token: 0x04003D78 RID: 15736
		public int Lucky;

		// Token: 0x04003D79 RID: 15737
		public int ExcellenceInfo;

		// Token: 0x04003D7A RID: 15738
		public int AppendPropLev;
	}
}
