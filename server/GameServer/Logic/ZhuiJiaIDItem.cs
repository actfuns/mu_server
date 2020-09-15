using System;

namespace GameServer.Logic
{
	// Token: 0x020006E2 RID: 1762
	public class ZhuiJiaIDItem
	{
		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06002A13 RID: 10771 RVA: 0x0025A87C File Offset: 0x00258A7C
		// (set) Token: 0x06002A14 RID: 10772 RVA: 0x0025A893 File Offset: 0x00258A93
		public int ID { get; set; }

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06002A15 RID: 10773 RVA: 0x0025A89C File Offset: 0x00258A9C
		// (set) Token: 0x06002A16 RID: 10774 RVA: 0x0025A8B3 File Offset: 0x00258AB3
		public double[] LevelBasePercent { get; set; }

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06002A17 RID: 10775 RVA: 0x0025A8BC File Offset: 0x00258ABC
		// (set) Token: 0x06002A18 RID: 10776 RVA: 0x0025A8D3 File Offset: 0x00258AD3
		public double[] LevelSelfPercent { get; set; }
	}
}
