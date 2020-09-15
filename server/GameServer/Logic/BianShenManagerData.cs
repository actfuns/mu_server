using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001F9 RID: 505
	public class BianShenManagerData
	{
		// Token: 0x04000B31 RID: 2865
		public object Mutex = new object();

		// Token: 0x04000B32 RID: 2866
		public bool IsGongNengOpend;

		// Token: 0x04000B33 RID: 2867
		public int FreeNum;

		// Token: 0x04000B34 RID: 2868
		public int TransfigurationBuff;

		// Token: 0x04000B35 RID: 2869
		public int BianShenCDSecs;

		// Token: 0x04000B36 RID: 2870
		public List<List<int>> NeedGoods;

		// Token: 0x04000B37 RID: 2871
		public int[] BianShenFull;

		// Token: 0x04000B38 RID: 2872
		public TemplateLoader<Dictionary<int, FashionEffectInfo>> FashionEffectInfoDict = new TemplateLoader<Dictionary<int, FashionEffectInfo>>();

		// Token: 0x04000B39 RID: 2873
		public TemplateLoader<Dictionary<int, BianShenStarInfo>> BianShenStarDict = new TemplateLoader<Dictionary<int, BianShenStarInfo>>();

		// Token: 0x04000B3A RID: 2874
		public Dictionary<int, List<BianShenStarInfo>> BianShenUpDict = new Dictionary<int, List<BianShenStarInfo>>();
	}
}
