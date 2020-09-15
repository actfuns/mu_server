using System;

namespace GameServer.Logic
{
	// Token: 0x020005EB RID: 1515
	public class MapTeleport
	{
		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06001CC1 RID: 7361 RVA: 0x001AB650 File Offset: 0x001A9850
		// (set) Token: 0x06001CC2 RID: 7362 RVA: 0x001AB667 File Offset: 0x001A9867
		public int Code { get; set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06001CC3 RID: 7363 RVA: 0x001AB670 File Offset: 0x001A9870
		// (set) Token: 0x06001CC4 RID: 7364 RVA: 0x001AB687 File Offset: 0x001A9887
		public int MapID { get; set; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06001CC5 RID: 7365 RVA: 0x001AB690 File Offset: 0x001A9890
		// (set) Token: 0x06001CC6 RID: 7366 RVA: 0x001AB6A7 File Offset: 0x001A98A7
		public int X { get; set; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06001CC7 RID: 7367 RVA: 0x001AB6B0 File Offset: 0x001A98B0
		// (set) Token: 0x06001CC8 RID: 7368 RVA: 0x001AB6C7 File Offset: 0x001A98C7
		public int Y { get; set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06001CC9 RID: 7369 RVA: 0x001AB6D0 File Offset: 0x001A98D0
		// (set) Token: 0x06001CCA RID: 7370 RVA: 0x001AB6E7 File Offset: 0x001A98E7
		public int Radius { get; set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06001CCB RID: 7371 RVA: 0x001AB6F0 File Offset: 0x001A98F0
		// (set) Token: 0x06001CCC RID: 7372 RVA: 0x001AB707 File Offset: 0x001A9907
		public int ToMapID { get; set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06001CCD RID: 7373 RVA: 0x001AB710 File Offset: 0x001A9910
		// (set) Token: 0x06001CCE RID: 7374 RVA: 0x001AB727 File Offset: 0x001A9927
		public int ToX { get; set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06001CCF RID: 7375 RVA: 0x001AB730 File Offset: 0x001A9930
		// (set) Token: 0x06001CD0 RID: 7376 RVA: 0x001AB747 File Offset: 0x001A9947
		public int ToY { get; set; }
	}
}
