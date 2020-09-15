using System;

namespace GameServer.Logic
{
	// Token: 0x02000605 RID: 1541
	public class BossHomeData
	{
		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06001E5A RID: 7770 RVA: 0x001ACF18 File Offset: 0x001AB118
		// (set) Token: 0x06001E5B RID: 7771 RVA: 0x001ACF2F File Offset: 0x001AB12F
		public int MapID { get; set; }

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06001E5C RID: 7772 RVA: 0x001ACF38 File Offset: 0x001AB138
		// (set) Token: 0x06001E5D RID: 7773 RVA: 0x001ACF4F File Offset: 0x001AB14F
		public int VIPLevLimit { get; set; }

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06001E5E RID: 7774 RVA: 0x001ACF58 File Offset: 0x001AB158
		// (set) Token: 0x06001E5F RID: 7775 RVA: 0x001ACF6F File Offset: 0x001AB16F
		public int MinChangeLifeLimit { get; set; }

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06001E60 RID: 7776 RVA: 0x001ACF78 File Offset: 0x001AB178
		// (set) Token: 0x06001E61 RID: 7777 RVA: 0x001ACF8F File Offset: 0x001AB18F
		public int MaxChangeLifeLimit { get; set; }

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06001E62 RID: 7778 RVA: 0x001ACF98 File Offset: 0x001AB198
		// (set) Token: 0x06001E63 RID: 7779 RVA: 0x001ACFAF File Offset: 0x001AB1AF
		public int MinLevel { get; set; }

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06001E64 RID: 7780 RVA: 0x001ACFB8 File Offset: 0x001AB1B8
		// (set) Token: 0x06001E65 RID: 7781 RVA: 0x001ACFCF File Offset: 0x001AB1CF
		public int MaxLevel { get; set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06001E66 RID: 7782 RVA: 0x001ACFD8 File Offset: 0x001AB1D8
		// (set) Token: 0x06001E67 RID: 7783 RVA: 0x001ACFEF File Offset: 0x001AB1EF
		public int EnterNeedDiamond { get; set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06001E68 RID: 7784 RVA: 0x001ACFF8 File Offset: 0x001AB1F8
		// (set) Token: 0x06001E69 RID: 7785 RVA: 0x001AD00F File Offset: 0x001AB20F
		public int OneMinuteNeedDiamond { get; set; }
	}
}
