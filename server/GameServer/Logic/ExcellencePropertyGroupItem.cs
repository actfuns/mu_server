using System;

namespace GameServer.Logic
{
	// Token: 0x020006E4 RID: 1764
	public class ExcellencePropertyGroupItem
	{
		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06002A21 RID: 10785 RVA: 0x0025A94C File Offset: 0x00258B4C
		// (set) Token: 0x06002A22 RID: 10786 RVA: 0x0025A963 File Offset: 0x00258B63
		public int ID { get; set; }

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06002A23 RID: 10787 RVA: 0x0025A96C File Offset: 0x00258B6C
		// (set) Token: 0x06002A24 RID: 10788 RVA: 0x0025A983 File Offset: 0x00258B83
		public int[] Max { get; set; }

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06002A25 RID: 10789 RVA: 0x0025A98C File Offset: 0x00258B8C
		// (set) Token: 0x06002A26 RID: 10790 RVA: 0x0025A9A3 File Offset: 0x00258BA3
		public ExcellencePropertyItem[] ExcellencePropertyItems { get; set; }
	}
}
