using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	// Token: 0x020006CD RID: 1741
	public class GAreaLua
	{
		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060023BE RID: 9150 RVA: 0x001E81E0 File Offset: 0x001E63E0
		// (set) Token: 0x060023BF RID: 9151 RVA: 0x001E81F7 File Offset: 0x001E63F7
		public int ID { get; set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060023C0 RID: 9152 RVA: 0x001E8200 File Offset: 0x001E6400
		// (set) Token: 0x060023C1 RID: 9153 RVA: 0x001E8217 File Offset: 0x001E6417
		public Point CenterPoint { get; set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060023C2 RID: 9154 RVA: 0x001E8220 File Offset: 0x001E6420
		// (set) Token: 0x060023C3 RID: 9155 RVA: 0x001E8237 File Offset: 0x001E6437
		public int Radius { get; set; }

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060023C4 RID: 9156 RVA: 0x001E8240 File Offset: 0x001E6440
		// (set) Token: 0x060023C5 RID: 9157 RVA: 0x001E8257 File Offset: 0x001E6457
		public string LuaScriptFileName { get; set; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060023C6 RID: 9158 RVA: 0x001E8260 File Offset: 0x001E6460
		// (set) Token: 0x060023C7 RID: 9159 RVA: 0x001E8277 File Offset: 0x001E6477
		public AddtionType AddtionType { get; set; }

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x060023C8 RID: 9160 RVA: 0x001E8280 File Offset: 0x001E6480
		// (set) Token: 0x060023C9 RID: 9161 RVA: 0x001E8297 File Offset: 0x001E6497
		public int TaskId { get; set; }

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060023CA RID: 9162 RVA: 0x001E82A0 File Offset: 0x001E64A0
		// (set) Token: 0x060023CB RID: 9163 RVA: 0x001E82B7 File Offset: 0x001E64B7
		public Dictionary<AreaEventType, List<int>> AreaEventDict { get; set; }
	}
}
