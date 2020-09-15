using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001F2 RID: 498
	[TemplateMappingOptions(null, "JingLing", "ID")]
	public class ArmorUpInfo
	{
		// Token: 0x04000AFF RID: 2815
		public int ID;

		// Token: 0x04000B00 RID: 2816
		public int ArmorClass;

		// Token: 0x04000B01 RID: 2817
		public int LuckyOne;

		// Token: 0x04000B02 RID: 2818
		public int LuckyTwo;

		// Token: 0x04000B03 RID: 2819
		public double LuckyTwoRate;

		// Token: 0x04000B04 RID: 2820
		public double Damageabsorption;

		// Token: 0x04000B05 RID: 2821
		public double Armorrecovery;

		// Token: 0x04000B06 RID: 2822
		[TemplateMappingField(Exclude = true)]
		public int ArmorUp;

		// Token: 0x04000B07 RID: 2823
		[TemplateMappingField(Exclude = true)]
		public int AddAttack;

		// Token: 0x04000B08 RID: 2824
		[TemplateMappingField(Exclude = true)]
		public int AddDefense;

		// Token: 0x04000B09 RID: 2825
		[TemplateMappingField(Exclude = true)]
		public int ShenmingUP;

		// Token: 0x04000B0A RID: 2826
		public List<int> NeedGoods;

		// Token: 0x04000B0B RID: 2827
		public int NeedDiamond;

		// Token: 0x04000B0C RID: 2828
		[TemplateMappingField(Exclude = true)]
		public int MaxStarLevel;
	}
}
