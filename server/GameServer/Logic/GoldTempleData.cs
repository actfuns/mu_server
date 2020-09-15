using System;

namespace GameServer.Logic
{
	// Token: 0x02000606 RID: 1542
	public class GoldTempleData
	{
		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06001E6B RID: 7787 RVA: 0x001AD020 File Offset: 0x001AB220
		// (set) Token: 0x06001E6C RID: 7788 RVA: 0x001AD037 File Offset: 0x001AB237
		public int MapID { get; set; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06001E6D RID: 7789 RVA: 0x001AD040 File Offset: 0x001AB240
		// (set) Token: 0x06001E6E RID: 7790 RVA: 0x001AD057 File Offset: 0x001AB257
		public int VIPLevLimit { get; set; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06001E6F RID: 7791 RVA: 0x001AD060 File Offset: 0x001AB260
		// (set) Token: 0x06001E70 RID: 7792 RVA: 0x001AD077 File Offset: 0x001AB277
		public int MinChangeLifeLimit { get; set; }

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06001E71 RID: 7793 RVA: 0x001AD080 File Offset: 0x001AB280
		// (set) Token: 0x06001E72 RID: 7794 RVA: 0x001AD097 File Offset: 0x001AB297
		public int MaxChangeLifeLimit { get; set; }

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06001E73 RID: 7795 RVA: 0x001AD0A0 File Offset: 0x001AB2A0
		// (set) Token: 0x06001E74 RID: 7796 RVA: 0x001AD0B7 File Offset: 0x001AB2B7
		public int MinLevel { get; set; }

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06001E75 RID: 7797 RVA: 0x001AD0C0 File Offset: 0x001AB2C0
		// (set) Token: 0x06001E76 RID: 7798 RVA: 0x001AD0D7 File Offset: 0x001AB2D7
		public int MaxLevel { get; set; }

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06001E77 RID: 7799 RVA: 0x001AD0E0 File Offset: 0x001AB2E0
		// (set) Token: 0x06001E78 RID: 7800 RVA: 0x001AD0F7 File Offset: 0x001AB2F7
		public int EnterNeedDiamond { get; set; }

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06001E79 RID: 7801 RVA: 0x001AD100 File Offset: 0x001AB300
		// (set) Token: 0x06001E7A RID: 7802 RVA: 0x001AD117 File Offset: 0x001AB317
		public int OneMinuteNeedDiamond { get; set; }
	}
}
