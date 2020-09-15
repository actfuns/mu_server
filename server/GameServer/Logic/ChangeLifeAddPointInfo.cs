using System;

namespace GameServer.Logic
{
	// Token: 0x020005F4 RID: 1524
	public class ChangeLifeAddPointInfo
	{
		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06001D0F RID: 7439 RVA: 0x001ABAF0 File Offset: 0x001A9CF0
		// (set) Token: 0x06001D10 RID: 7440 RVA: 0x001ABB07 File Offset: 0x001A9D07
		public int ChangeLevel { get; set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06001D11 RID: 7441 RVA: 0x001ABB10 File Offset: 0x001A9D10
		// (set) Token: 0x06001D12 RID: 7442 RVA: 0x001ABB27 File Offset: 0x001A9D27
		public int AddPoint { get; set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06001D13 RID: 7443 RVA: 0x001ABB30 File Offset: 0x001A9D30
		// (set) Token: 0x06001D14 RID: 7444 RVA: 0x001ABB47 File Offset: 0x001A9D47
		public int nStrLimit { get; set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06001D15 RID: 7445 RVA: 0x001ABB50 File Offset: 0x001A9D50
		// (set) Token: 0x06001D16 RID: 7446 RVA: 0x001ABB67 File Offset: 0x001A9D67
		public int nDexLimit { get; set; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06001D17 RID: 7447 RVA: 0x001ABB70 File Offset: 0x001A9D70
		// (set) Token: 0x06001D18 RID: 7448 RVA: 0x001ABB87 File Offset: 0x001A9D87
		public int nIntLimit { get; set; }

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06001D19 RID: 7449 RVA: 0x001ABB90 File Offset: 0x001A9D90
		// (set) Token: 0x06001D1A RID: 7450 RVA: 0x001ABBA7 File Offset: 0x001A9DA7
		public int nConLimit { get; set; }
	}
}
