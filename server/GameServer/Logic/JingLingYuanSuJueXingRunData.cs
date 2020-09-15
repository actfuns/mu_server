using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000302 RID: 770
	public class JingLingYuanSuJueXingRunData
	{
		// Token: 0x040013E4 RID: 5092
		public object Mutex = new object();

		// Token: 0x040013E5 RID: 5093
		public TemplateLoader<Dictionary<int, JingLingYuanSuInfo>> YuanSuInfoDict = new TemplateLoader<Dictionary<int, JingLingYuanSuInfo>>();

		// Token: 0x040013E6 RID: 5094
		public TemplateLoader<Dictionary<int, JingLingYuanSuShuXingInfo>> ShuXingInfoDict = new TemplateLoader<Dictionary<int, JingLingYuanSuShuXingInfo>>();
	}
}
