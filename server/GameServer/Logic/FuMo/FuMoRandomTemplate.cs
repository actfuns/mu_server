using System;
using System.Collections.Generic;

namespace GameServer.Logic.FuMo
{
	// Token: 0x020002B9 RID: 697
	public class FuMoRandomTemplate
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000ADF RID: 2783 RVA: 0x000AB6A8 File Offset: 0x000A98A8
		// (set) Token: 0x06000AE0 RID: 2784 RVA: 0x000AB6BF File Offset: 0x000A98BF
		public int ID { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x000AB6C8 File Offset: 0x000A98C8
		// (set) Token: 0x06000AE2 RID: 2786 RVA: 0x000AB6DF File Offset: 0x000A98DF
		public string Name { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000AE3 RID: 2787 RVA: 0x000AB6E8 File Offset: 0x000A98E8
		// (set) Token: 0x06000AE4 RID: 2788 RVA: 0x000AB6FF File Offset: 0x000A98FF
		public string Type { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x000AB708 File Offset: 0x000A9908
		// (set) Token: 0x06000AE6 RID: 2790 RVA: 0x000AB71F File Offset: 0x000A991F
		public Dictionary<double, double> Parameter { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x000AB728 File Offset: 0x000A9928
		// (set) Token: 0x06000AE8 RID: 2792 RVA: 0x000AB73F File Offset: 0x000A993F
		public int BeginNum { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x000AB748 File Offset: 0x000A9948
		// (set) Token: 0x06000AEA RID: 2794 RVA: 0x000AB75F File Offset: 0x000A995F
		public int EndNum { get; set; }
	}
}
