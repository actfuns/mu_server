using System;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001F7 RID: 503
	[TemplateMappingOptions(null, "TransfigurationFashionEffect", "ID")]
	public class FashionEffectInfo
	{
		// Token: 0x04000B28 RID: 2856
		public int ID;

		// Token: 0x04000B29 RID: 2857
		public string ProPerty;

		// Token: 0x04000B2A RID: 2858
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropValues = new double[177];
	}
}
