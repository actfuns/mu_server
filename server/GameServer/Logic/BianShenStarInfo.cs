using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001F6 RID: 502
	[TemplateMappingOptions(null, "TransfigurationLevel", "ID")]
	public class BianShenStarInfo
	{
		// Token: 0x04000B19 RID: 2841
		public int ID;

		// Token: 0x04000B1A RID: 2842
		public List<int> OccupationID;

		// Token: 0x04000B1B RID: 2843
		public int Level;

		// Token: 0x04000B1C RID: 2844
		public string ProPerty;

		// Token: 0x04000B1D RID: 2845
		public int UpExp;

		// Token: 0x04000B1E RID: 2846
		public int GoodsExp;

		// Token: 0x04000B1F RID: 2847
		public double ExpCritRate;

		// Token: 0x04000B20 RID: 2848
		public double ExpCritTimes;

		// Token: 0x04000B21 RID: 2849
		[TemplateMappingField(SpliteChars = "|,")]
		public List<List<int>> NeedGoods;

		// Token: 0x04000B22 RID: 2850
		public List<int> AttackSkill;

		// Token: 0x04000B23 RID: 2851
		public List<int> MagicSkill;

		// Token: 0x04000B24 RID: 2852
		public int Duration;

		// Token: 0x04000B25 RID: 2853
		[TemplateMappingField(Exclude = true)]
		public int NeedDiamond;

		// Token: 0x04000B26 RID: 2854
		[TemplateMappingField(Exclude = true)]
		public int ZuanShiExp;

		// Token: 0x04000B27 RID: 2855
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropValues = new double[177];
	}
}
