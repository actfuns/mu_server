using System;
using System.Windows;

namespace GameServer.Logic
{
	// Token: 0x020006CA RID: 1738
	public class GSafeRegion
	{
		// Token: 0x17000243 RID: 579
		// (get) Token: 0x060023B7 RID: 9143 RVA: 0x001E8178 File Offset: 0x001E6378
		// (set) Token: 0x060023B8 RID: 9144 RVA: 0x001E818F File Offset: 0x001E638F
		public int ID { get; set; }

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060023B9 RID: 9145 RVA: 0x001E8198 File Offset: 0x001E6398
		// (set) Token: 0x060023BA RID: 9146 RVA: 0x001E81AF File Offset: 0x001E63AF
		public Point CenterPoint { get; set; }

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060023BB RID: 9147 RVA: 0x001E81B8 File Offset: 0x001E63B8
		// (set) Token: 0x060023BC RID: 9148 RVA: 0x001E81CF File Offset: 0x001E63CF
		public int Radius { get; set; }
	}
}
