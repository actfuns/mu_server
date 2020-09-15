using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001E7 RID: 487
	[TemplateMappingOptions(null, "EmblemStar", "ID")]
	public class EmblemStarInfo
	{
		// Token: 0x04000AA7 RID: 2727
		public int ID;

		// Token: 0x04000AA8 RID: 2728
		public int EmblemLevel;

		// Token: 0x04000AA9 RID: 2729
		public int EmblemStar;

		// Token: 0x04000AAA RID: 2730
		public int LifeV;

		// Token: 0x04000AAB RID: 2731
		public int AddAttack;

		// Token: 0x04000AAC RID: 2732
		public int AddDefense;

		// Token: 0x04000AAD RID: 2733
		public int DecreaseInjureValue;

		// Token: 0x04000AAE RID: 2734
		public int StarExp;

		// Token: 0x04000AAF RID: 2735
		public int GoodsExp;

		// Token: 0x04000AB0 RID: 2736
		public int ZuanShiExp;

		// Token: 0x04000AB1 RID: 2737
		public List<int> NeedGoods;

		// Token: 0x04000AB2 RID: 2738
		public int NeedDiamond;

		// Token: 0x04000AB3 RID: 2739
		[TemplateMappingField(Exclude = true)]
		public EmblemUpInfo EmblemUpInfo;

		// Token: 0x04000AB4 RID: 2740
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropValues = new double[177];
	}
}
