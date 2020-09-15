using System;

namespace GameServer.Logic
{
	// Token: 0x020006E5 RID: 1765
	public class FallBornIndexItem
	{
		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06002A28 RID: 10792 RVA: 0x0025A9B4 File Offset: 0x00258BB4
		// (set) Token: 0x06002A29 RID: 10793 RVA: 0x0025A9CB File Offset: 0x00258BCB
		public int ID { get; set; }

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06002A2A RID: 10794 RVA: 0x0025A9D4 File Offset: 0x00258BD4
		// (set) Token: 0x06002A2B RID: 10795 RVA: 0x0025A9EB File Offset: 0x00258BEB
		public double[] LevelBasePercent { get; set; }

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06002A2C RID: 10796 RVA: 0x0025A9F4 File Offset: 0x00258BF4
		// (set) Token: 0x06002A2D RID: 10797 RVA: 0x0025AA0B File Offset: 0x00258C0B
		public double[] LevelSelfPercent { get; set; }
	}
}
