using System;

namespace GameServer.Logic
{
	// Token: 0x020005F7 RID: 1527
	public class CopyScoreDataInfo
	{
		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06001D5E RID: 7518 RVA: 0x001ABFC8 File Offset: 0x001AA1C8
		// (set) Token: 0x06001D5F RID: 7519 RVA: 0x001ABFDF File Offset: 0x001AA1DF
		public int CopyMapID { get; set; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06001D60 RID: 7520 RVA: 0x001ABFE8 File Offset: 0x001AA1E8
		// (set) Token: 0x06001D61 RID: 7521 RVA: 0x001ABFFF File Offset: 0x001AA1FF
		public string ScoreName { get; set; }

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06001D62 RID: 7522 RVA: 0x001AC008 File Offset: 0x001AA208
		// (set) Token: 0x06001D63 RID: 7523 RVA: 0x001AC01F File Offset: 0x001AA21F
		public int MinScore { get; set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06001D64 RID: 7524 RVA: 0x001AC028 File Offset: 0x001AA228
		// (set) Token: 0x06001D65 RID: 7525 RVA: 0x001AC03F File Offset: 0x001AA23F
		public int MaxScore { get; set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06001D66 RID: 7526 RVA: 0x001AC048 File Offset: 0x001AA248
		// (set) Token: 0x06001D67 RID: 7527 RVA: 0x001AC05F File Offset: 0x001AA25F
		public double ExpModulus { get; set; }

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06001D68 RID: 7528 RVA: 0x001AC068 File Offset: 0x001AA268
		// (set) Token: 0x06001D69 RID: 7529 RVA: 0x001AC07F File Offset: 0x001AA27F
		public double MoneyModulus { get; set; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06001D6A RID: 7530 RVA: 0x001AC088 File Offset: 0x001AA288
		// (set) Token: 0x06001D6B RID: 7531 RVA: 0x001AC09F File Offset: 0x001AA29F
		public int FallPacketID { get; set; }

		// Token: 0x04002AE3 RID: 10979
		public int AwardType;

		// Token: 0x04002AE4 RID: 10980
		public int MinMoJing;

		// Token: 0x04002AE5 RID: 10981
		public int MaxMoJing;
	}
}
