using System;

namespace GameServer.Logic
{
	// Token: 0x020005EC RID: 1516
	public class RoleSpeedItem
	{
		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06001CD2 RID: 7378 RVA: 0x001AB758 File Offset: 0x001A9958
		// (set) Token: 0x06001CD3 RID: 7379 RVA: 0x001AB76F File Offset: 0x001A996F
		public int MapCode { get; set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06001CD4 RID: 7380 RVA: 0x001AB778 File Offset: 0x001A9978
		// (set) Token: 0x06001CD5 RID: 7381 RVA: 0x001AB78F File Offset: 0x001A998F
		public int X { get; set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06001CD6 RID: 7382 RVA: 0x001AB798 File Offset: 0x001A9998
		// (set) Token: 0x06001CD7 RID: 7383 RVA: 0x001AB7AF File Offset: 0x001A99AF
		public int Y { get; set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06001CD8 RID: 7384 RVA: 0x001AB7B8 File Offset: 0x001A99B8
		// (set) Token: 0x06001CD9 RID: 7385 RVA: 0x001AB7CF File Offset: 0x001A99CF
		public double OverflowSpeed { get; set; }
	}
}
