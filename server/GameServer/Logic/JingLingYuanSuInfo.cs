using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000300 RID: 768
	[TemplateMappingOptions(null, "JingLing", "ID")]
	public class JingLingYuanSuInfo
	{
		// Token: 0x040013D5 RID: 5077
		public int ID;

		// Token: 0x040013D6 RID: 5078
		public int YuanSuType;

		// Token: 0x040013D7 RID: 5079
		public int ShuXingType;

		// Token: 0x040013D8 RID: 5080
		public int QiangHuaLevel;

		// Token: 0x040013D9 RID: 5081
		public int JieXingCurrency;

		// Token: 0x040013DA RID: 5082
		public double Success;

		// Token: 0x040013DB RID: 5083
		public string Attribute;

		// Token: 0x040013DC RID: 5084
		[TemplateMappingField(SpliteChars = "|,")]
		public List<List<int>> NeedGoods;

		// Token: 0x040013DD RID: 5085
		[TemplateMappingField(SpliteChars = "|,")]
		public List<List<int>> Failtofail;

		// Token: 0x040013DE RID: 5086
		[TemplateMappingField(Exclude = true)]
		public double[] ExtProps = new double[177];
	}
}
