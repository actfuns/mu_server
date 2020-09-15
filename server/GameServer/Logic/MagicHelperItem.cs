using System;

namespace GameServer.Logic
{
	// Token: 0x020007D7 RID: 2007
	public class MagicHelperItem
	{
		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x0600389D RID: 14493 RVA: 0x00303F3C File Offset: 0x0030213C
		// (set) Token: 0x0600389E RID: 14494 RVA: 0x00303F53 File Offset: 0x00302153
		public MagicActionIDs MagicActionID { get; set; }

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x0600389F RID: 14495 RVA: 0x00303F5C File Offset: 0x0030215C
		// (set) Token: 0x060038A0 RID: 14496 RVA: 0x00303F73 File Offset: 0x00302173
		public double[] MagicActionParams { get; set; }

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x060038A1 RID: 14497 RVA: 0x00303F7C File Offset: 0x0030217C
		// (set) Token: 0x060038A2 RID: 14498 RVA: 0x00303F93 File Offset: 0x00302193
		public long StartedTicks { get; set; }

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x060038A3 RID: 14499 RVA: 0x00303F9C File Offset: 0x0030219C
		// (set) Token: 0x060038A4 RID: 14500 RVA: 0x00303FB3 File Offset: 0x003021B3
		public long LastTicks { get; set; }

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x060038A5 RID: 14501 RVA: 0x00303FBC File Offset: 0x003021BC
		// (set) Token: 0x060038A6 RID: 14502 RVA: 0x00303FD3 File Offset: 0x003021D3
		public int ExecutedNum { get; set; }

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x060038A7 RID: 14503 RVA: 0x00303FDC File Offset: 0x003021DC
		// (set) Token: 0x060038A8 RID: 14504 RVA: 0x00303FF3 File Offset: 0x003021F3
		public int ObjectID { get; set; }
	}
}
