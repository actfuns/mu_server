using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001E8 RID: 488
	[TemplateMappingOptions(null, "EmblemUp", "ID")]
	public class EmblemUpInfo
	{
		// Token: 0x04000AB5 RID: 2741
		public int ID;

		// Token: 0x04000AB6 RID: 2742
		public int EmblemLevel;

		// Token: 0x04000AB7 RID: 2743
		public int LuckyOne;

		// Token: 0x04000AB8 RID: 2744
		public int LuckyTwo;

		// Token: 0x04000AB9 RID: 2745
		public double LuckyTwoRate;

		// Token: 0x04000ABA RID: 2746
		public int DurationTime;

		// Token: 0x04000ABB RID: 2747
		public int CDTime;

		// Token: 0x04000ABC RID: 2748
		public double SubAttackInjurePercent;

		// Token: 0x04000ABD RID: 2749
		public double SPAttackInjurePercent;

		// Token: 0x04000ABE RID: 2750
		public double AttackInjurePercent;

		// Token: 0x04000ABF RID: 2751
		public double ElementAttackInjurePercent;

		// Token: 0x04000AC0 RID: 2752
		public int LifeV;

		// Token: 0x04000AC1 RID: 2753
		public int AddAttack;

		// Token: 0x04000AC2 RID: 2754
		public int AddDefense;

		// Token: 0x04000AC3 RID: 2755
		public int DecreaseInjureValue;

		// Token: 0x04000AC4 RID: 2756
		public List<int> NeedGoods;

		// Token: 0x04000AC5 RID: 2757
		public int NeedDiamond;

		// Token: 0x04000AC6 RID: 2758
		[TemplateMappingField(Exclude = true)]
		public int MaxStarLevel;

		// Token: 0x04000AC7 RID: 2759
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropTempValues = new double[177];
	}
}
