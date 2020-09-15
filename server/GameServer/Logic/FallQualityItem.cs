using System;

namespace GameServer.Logic
{
	// Token: 0x020006E0 RID: 1760
	public class FallQualityItem
	{
		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06002A05 RID: 10757 RVA: 0x0025A7AC File Offset: 0x002589AC
		// (set) Token: 0x06002A06 RID: 10758 RVA: 0x0025A7C3 File Offset: 0x002589C3
		public int ID { get; set; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06002A07 RID: 10759 RVA: 0x0025A7CC File Offset: 0x002589CC
		// (set) Token: 0x06002A08 RID: 10760 RVA: 0x0025A7E3 File Offset: 0x002589E3
		public double[] QualityBasePercent { get; set; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06002A09 RID: 10761 RVA: 0x0025A7EC File Offset: 0x002589EC
		// (set) Token: 0x06002A0A RID: 10762 RVA: 0x0025A803 File Offset: 0x00258A03
		public double[] QualitySelfPercent { get; set; }
	}
}
