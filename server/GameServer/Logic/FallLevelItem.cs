using System;

namespace GameServer.Logic
{
	// Token: 0x020006E1 RID: 1761
	public class FallLevelItem
	{
		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06002A0C RID: 10764 RVA: 0x0025A814 File Offset: 0x00258A14
		// (set) Token: 0x06002A0D RID: 10765 RVA: 0x0025A82B File Offset: 0x00258A2B
		public int ID { get; set; }

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06002A0E RID: 10766 RVA: 0x0025A834 File Offset: 0x00258A34
		// (set) Token: 0x06002A0F RID: 10767 RVA: 0x0025A84B File Offset: 0x00258A4B
		public double[] LevelBasePercent { get; set; }

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06002A10 RID: 10768 RVA: 0x0025A854 File Offset: 0x00258A54
		// (set) Token: 0x06002A11 RID: 10769 RVA: 0x0025A86B File Offset: 0x00258A6B
		public double[] LevelSelfPercent { get; set; }
	}
}
