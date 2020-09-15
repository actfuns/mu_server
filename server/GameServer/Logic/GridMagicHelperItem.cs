using System;

namespace GameServer.Logic
{
	// Token: 0x0200074D RID: 1869
	public class GridMagicHelperItem
	{
		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06002F06 RID: 12038 RVA: 0x002A1604 File Offset: 0x0029F804
		// (set) Token: 0x06002F07 RID: 12039 RVA: 0x002A161B File Offset: 0x0029F81B
		public MagicActionIDs MagicActionID { get; set; }

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06002F08 RID: 12040 RVA: 0x002A1624 File Offset: 0x0029F824
		// (set) Token: 0x06002F09 RID: 12041 RVA: 0x002A163B File Offset: 0x0029F83B
		public double[] MagicActionParams { get; set; }

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06002F0A RID: 12042 RVA: 0x002A1644 File Offset: 0x0029F844
		// (set) Token: 0x06002F0B RID: 12043 RVA: 0x002A165B File Offset: 0x0029F85B
		public long StartedTicks { get; set; }

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06002F0C RID: 12044 RVA: 0x002A1664 File Offset: 0x0029F864
		// (set) Token: 0x06002F0D RID: 12045 RVA: 0x002A167B File Offset: 0x0029F87B
		public long LastTicks { get; set; }

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06002F0E RID: 12046 RVA: 0x002A1684 File Offset: 0x0029F884
		// (set) Token: 0x06002F0F RID: 12047 RVA: 0x002A169B File Offset: 0x0029F89B
		public int ExecutedNum { get; set; }

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06002F10 RID: 12048 RVA: 0x002A16A4 File Offset: 0x0029F8A4
		// (set) Token: 0x06002F11 RID: 12049 RVA: 0x002A16BB File Offset: 0x0029F8BB
		public int MapCode { get; set; }

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06002F12 RID: 12050 RVA: 0x002A16C4 File Offset: 0x0029F8C4
		// (set) Token: 0x06002F13 RID: 12051 RVA: 0x002A16DB File Offset: 0x0029F8DB
		public int DecoID { get; set; }
	}
}
