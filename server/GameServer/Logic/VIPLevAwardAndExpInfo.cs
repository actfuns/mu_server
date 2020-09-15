using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000600 RID: 1536
	public class VIPLevAwardAndExpInfo
	{
		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06001E09 RID: 7689 RVA: 0x001ACA30 File Offset: 0x001AAC30
		// (set) Token: 0x06001E0A RID: 7690 RVA: 0x001ACA47 File Offset: 0x001AAC47
		public int VipLev { get; set; }

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06001E0B RID: 7691 RVA: 0x001ACA50 File Offset: 0x001AAC50
		// (set) Token: 0x06001E0C RID: 7692 RVA: 0x001ACA67 File Offset: 0x001AAC67
		public List<GoodsData> AwardList { get; set; }

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06001E0D RID: 7693 RVA: 0x001ACA70 File Offset: 0x001AAC70
		// (set) Token: 0x06001E0E RID: 7694 RVA: 0x001ACA87 File Offset: 0x001AAC87
		public int NeedExp { get; set; }
	}
}
