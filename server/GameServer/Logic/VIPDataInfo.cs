using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020005FF RID: 1535
	public class VIPDataInfo
	{
		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06001DF2 RID: 7666 RVA: 0x001AC8C8 File Offset: 0x001AAAC8
		// (set) Token: 0x06001DF3 RID: 7667 RVA: 0x001AC8DF File Offset: 0x001AAADF
		public int AwardID { get; set; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06001DF4 RID: 7668 RVA: 0x001AC8E8 File Offset: 0x001AAAE8
		// (set) Token: 0x06001DF5 RID: 7669 RVA: 0x001AC8FF File Offset: 0x001AAAFF
		public int VIPlev { get; set; }

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06001DF6 RID: 7670 RVA: 0x001AC908 File Offset: 0x001AAB08
		// (set) Token: 0x06001DF7 RID: 7671 RVA: 0x001AC91F File Offset: 0x001AAB1F
		public int DailyMaxUseTimes { get; set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x001AC928 File Offset: 0x001AAB28
		// (set) Token: 0x06001DF9 RID: 7673 RVA: 0x001AC93F File Offset: 0x001AAB3F
		public List<GoodsData> AwardGoods { get; set; }

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06001DFA RID: 7674 RVA: 0x001AC948 File Offset: 0x001AAB48
		// (set) Token: 0x06001DFB RID: 7675 RVA: 0x001AC95F File Offset: 0x001AAB5F
		public int ZuanShi { get; set; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06001DFC RID: 7676 RVA: 0x001AC968 File Offset: 0x001AAB68
		// (set) Token: 0x06001DFD RID: 7677 RVA: 0x001AC97F File Offset: 0x001AAB7F
		public int BindZuanShi { get; set; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06001DFE RID: 7678 RVA: 0x001AC988 File Offset: 0x001AAB88
		// (set) Token: 0x06001DFF RID: 7679 RVA: 0x001AC99F File Offset: 0x001AAB9F
		public int JinBi { get; set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06001E00 RID: 7680 RVA: 0x001AC9A8 File Offset: 0x001AABA8
		// (set) Token: 0x06001E01 RID: 7681 RVA: 0x001AC9BF File Offset: 0x001AABBF
		public int BindJinBi { get; set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06001E02 RID: 7682 RVA: 0x001AC9C8 File Offset: 0x001AABC8
		// (set) Token: 0x06001E03 RID: 7683 RVA: 0x001AC9DF File Offset: 0x001AABDF
		public int[] BufferGoods { get; set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06001E04 RID: 7684 RVA: 0x001AC9E8 File Offset: 0x001AABE8
		// (set) Token: 0x06001E05 RID: 7685 RVA: 0x001AC9FF File Offset: 0x001AABFF
		public int XiHongMing { get; set; }

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06001E06 RID: 7686 RVA: 0x001ACA08 File Offset: 0x001AAC08
		// (set) Token: 0x06001E07 RID: 7687 RVA: 0x001ACA1F File Offset: 0x001AAC1F
		public int XiuLi { get; set; }
	}
}
