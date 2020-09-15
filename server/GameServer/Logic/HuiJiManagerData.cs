using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001EA RID: 490
	public class HuiJiManagerData
	{
		// Token: 0x04000ACE RID: 2766
		public object Mutex = new object();

		// Token: 0x04000ACF RID: 2767
		public bool IsGongNengOpend;

		// Token: 0x04000AD0 RID: 2768
		public int[] EmblemFull;

		// Token: 0x04000AD1 RID: 2769
		public double[] EmblemShengXing;

		// Token: 0x04000AD2 RID: 2770
		public TemplateLoader<Dictionary<int, EmblemStarInfo>> EmblemStarDict = new TemplateLoader<Dictionary<int, EmblemStarInfo>>();

		// Token: 0x04000AD3 RID: 2771
		public TemplateLoader<Dictionary<int, EmblemUpInfo>> EmblemUpDict = new TemplateLoader<Dictionary<int, EmblemUpInfo>>();
	}
}
