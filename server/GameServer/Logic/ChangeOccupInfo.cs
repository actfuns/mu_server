using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020005F2 RID: 1522
	public class ChangeOccupInfo
	{
		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06001CFD RID: 7421 RVA: 0x001AB9E0 File Offset: 0x001A9BE0
		// (set) Token: 0x06001CFE RID: 7422 RVA: 0x001AB9F7 File Offset: 0x001A9BF7
		public int OccupationID { get; set; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06001CFF RID: 7423 RVA: 0x001ABA00 File Offset: 0x001A9C00
		// (set) Token: 0x06001D00 RID: 7424 RVA: 0x001ABA17 File Offset: 0x001A9C17
		public int NeedLevel { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06001D01 RID: 7425 RVA: 0x001ABA20 File Offset: 0x001A9C20
		// (set) Token: 0x06001D02 RID: 7426 RVA: 0x001ABA37 File Offset: 0x001A9C37
		public int NeedMoney { get; set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06001D03 RID: 7427 RVA: 0x001ABA40 File Offset: 0x001A9C40
		// (set) Token: 0x06001D04 RID: 7428 RVA: 0x001ABA57 File Offset: 0x001A9C57
		public List<GoodsData> NeedGoodsDataList { get; set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06001D05 RID: 7429 RVA: 0x001ABA60 File Offset: 0x001A9C60
		// (set) Token: 0x06001D06 RID: 7430 RVA: 0x001ABA77 File Offset: 0x001A9C77
		public List<GoodsData> AwardGoodsDataList { get; set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06001D07 RID: 7431 RVA: 0x001ABA80 File Offset: 0x001A9C80
		// (set) Token: 0x06001D08 RID: 7432 RVA: 0x001ABA97 File Offset: 0x001A9C97
		public int AwardPropPoint { get; set; }
	}
}
