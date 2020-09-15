using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001F4 RID: 500
	public class ArmorManagerData
	{
		// Token: 0x04000B13 RID: 2835
		public object Mutex = new object();

		// Token: 0x04000B14 RID: 2836
		public double[] HudunBaoji;

		// Token: 0x04000B15 RID: 2837
		public TemplateLoader<Dictionary<int, ArmorStarInfo>> ArmorStarDict = new TemplateLoader<Dictionary<int, ArmorStarInfo>>();

		// Token: 0x04000B16 RID: 2838
		public TemplateLoader<Dictionary<int, ArmorUpInfo>> ArmorUpDict = new TemplateLoader<Dictionary<int, ArmorUpInfo>>();
	}
}
