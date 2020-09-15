using System;
using System.Windows;

namespace GameServer.Logic
{
	// Token: 0x020005E8 RID: 1512
	public class MapStallItem
	{
		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06001CAA RID: 7338 RVA: 0x001AB4F8 File Offset: 0x001A96F8
		// (set) Token: 0x06001CAB RID: 7339 RVA: 0x001AB50F File Offset: 0x001A970F
		public int MapID { get; set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06001CAC RID: 7340 RVA: 0x001AB518 File Offset: 0x001A9718
		// (set) Token: 0x06001CAD RID: 7341 RVA: 0x001AB52F File Offset: 0x001A972F
		public Point ToPos { get; set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06001CAE RID: 7342 RVA: 0x001AB538 File Offset: 0x001A9738
		// (set) Token: 0x06001CAF RID: 7343 RVA: 0x001AB54F File Offset: 0x001A974F
		public int Radius { get; set; }
	}
}
