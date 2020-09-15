using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020004E4 RID: 1252
	public class FashionBagData
	{
		// Token: 0x0400212C RID: 8492
		public int ID;

		// Token: 0x0400212D RID: 8493
		public int GoodsID;

		// Token: 0x0400212E RID: 8494
		public int ForgeLev;

		// Token: 0x0400212F RID: 8495
		public int NeedGoodsID;

		// Token: 0x04002130 RID: 8496
		public int NeedGoodsCount;

		// Token: 0x04002131 RID: 8497
		public int LimitTime;

		// Token: 0x04002132 RID: 8498
		public double[] ExtProps = new double[177];

		// Token: 0x04002133 RID: 8499
		public int FasionTab;

		// Token: 0x04002134 RID: 8500
		public List<int> AttackSkill;

		// Token: 0x04002135 RID: 8501
		public List<int> MagicSkill;

		// Token: 0x04002136 RID: 8502
		public int BianShenEffect;

		// Token: 0x04002137 RID: 8503
		public int BianShenDuration;
	}
}
