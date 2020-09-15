using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020005FE RID: 1534
	public class TotalLoginDataInfo
	{
		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06001DE3 RID: 7651 RVA: 0x001AC7E0 File Offset: 0x001AA9E0
		// (set) Token: 0x06001DE4 RID: 7652 RVA: 0x001AC7F7 File Offset: 0x001AA9F7
		public int TotalLoginDays { get; set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06001DE5 RID: 7653 RVA: 0x001AC800 File Offset: 0x001AAA00
		// (set) Token: 0x06001DE6 RID: 7654 RVA: 0x001AC817 File Offset: 0x001AAA17
		public List<GoodsData> NormalAward { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06001DE7 RID: 7655 RVA: 0x001AC820 File Offset: 0x001AAA20
		// (set) Token: 0x06001DE8 RID: 7656 RVA: 0x001AC837 File Offset: 0x001AAA37
		public List<GoodsData> Award0 { get; set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06001DE9 RID: 7657 RVA: 0x001AC840 File Offset: 0x001AAA40
		// (set) Token: 0x06001DEA RID: 7658 RVA: 0x001AC857 File Offset: 0x001AAA57
		public List<GoodsData> Award1 { get; set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06001DEB RID: 7659 RVA: 0x001AC860 File Offset: 0x001AAA60
		// (set) Token: 0x06001DEC RID: 7660 RVA: 0x001AC877 File Offset: 0x001AAA77
		public List<GoodsData> Award2 { get; set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06001DED RID: 7661 RVA: 0x001AC880 File Offset: 0x001AAA80
		// (set) Token: 0x06001DEE RID: 7662 RVA: 0x001AC897 File Offset: 0x001AAA97
		public List<GoodsData> Award3 { get; set; }

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06001DEF RID: 7663 RVA: 0x001AC8A0 File Offset: 0x001AAAA0
		// (set) Token: 0x06001DF0 RID: 7664 RVA: 0x001AC8B7 File Offset: 0x001AAAB7
		public List<GoodsData> Award5 { get; set; }
	}
}
