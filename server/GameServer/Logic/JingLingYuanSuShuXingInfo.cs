using System;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000301 RID: 769
	[TemplateMappingOptions(null, "Rose", "ID")]
	public class JingLingYuanSuShuXingInfo
	{
		// Token: 0x040013DF RID: 5087
		public int ID;

		// Token: 0x040013E0 RID: 5088
		public int Tipe;

		// Token: 0x040013E1 RID: 5089
		public int Level;

		// Token: 0x040013E2 RID: 5090
		public string AcetiveElement;

		// Token: 0x040013E3 RID: 5091
		[TemplateMappingField(Exclude = true)]
		public double[] ExtProps = new double[177];
	}
}
