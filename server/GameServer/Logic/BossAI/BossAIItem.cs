using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D4 RID: 1492
	public class BossAIItem
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06001BBC RID: 7100 RVA: 0x001A16FC File Offset: 0x0019F8FC
		// (set) Token: 0x06001BBD RID: 7101 RVA: 0x001A1713 File Offset: 0x0019F913
		public int ID { get; set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06001BBE RID: 7102 RVA: 0x001A171C File Offset: 0x0019F91C
		// (set) Token: 0x06001BBF RID: 7103 RVA: 0x001A1733 File Offset: 0x0019F933
		public int AIID { get; set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06001BC0 RID: 7104 RVA: 0x001A173C File Offset: 0x0019F93C
		// (set) Token: 0x06001BC1 RID: 7105 RVA: 0x001A1753 File Offset: 0x0019F953
		public int TriggerNum { get; set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06001BC2 RID: 7106 RVA: 0x001A175C File Offset: 0x0019F95C
		// (set) Token: 0x06001BC3 RID: 7107 RVA: 0x001A1773 File Offset: 0x0019F973
		public int TriggerCD { get; set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06001BC4 RID: 7108 RVA: 0x001A177C File Offset: 0x0019F97C
		// (set) Token: 0x06001BC5 RID: 7109 RVA: 0x001A1793 File Offset: 0x0019F993
		public int TriggerType { get; set; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06001BC6 RID: 7110 RVA: 0x001A179C File Offset: 0x0019F99C
		// (set) Token: 0x06001BC7 RID: 7111 RVA: 0x001A17B3 File Offset: 0x0019F9B3
		public ITriggerCondition Condition { get; set; }

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06001BC8 RID: 7112 RVA: 0x001A17BC File Offset: 0x0019F9BC
		// (set) Token: 0x06001BC9 RID: 7113 RVA: 0x001A17D3 File Offset: 0x0019F9D3
		public string Desc { get; set; }
	}
}
