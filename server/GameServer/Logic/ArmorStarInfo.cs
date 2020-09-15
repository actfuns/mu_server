using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001F1 RID: 497
	[TemplateMappingOptions(null, "JingLing", "ID")]
	public class ArmorStarInfo
	{
		// Token: 0x04000AF1 RID: 2801
		public int ID;

		// Token: 0x04000AF2 RID: 2802
		public int ArmorupStage;

		// Token: 0x04000AF3 RID: 2803
		public int StarLevel;

		// Token: 0x04000AF4 RID: 2804
		public int ArmorUp;

		// Token: 0x04000AF5 RID: 2805
		public int AddAttack;

		// Token: 0x04000AF6 RID: 2806
		public int AddDefense;

		// Token: 0x04000AF7 RID: 2807
		public int ShenmingUP;

		// Token: 0x04000AF8 RID: 2808
		public int StarExp;

		// Token: 0x04000AF9 RID: 2809
		public int GoodsExp;

		// Token: 0x04000AFA RID: 2810
		public int ZuanShiExp;

		// Token: 0x04000AFB RID: 2811
		public List<int> NeedGoods;

		// Token: 0x04000AFC RID: 2812
		public int NeedDiamond;

		// Token: 0x04000AFD RID: 2813
		[TemplateMappingField(Exclude = true)]
		public ArmorUpInfo ArmorUpInfo;

		// Token: 0x04000AFE RID: 2814
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropValues = new double[177];
	}
}
