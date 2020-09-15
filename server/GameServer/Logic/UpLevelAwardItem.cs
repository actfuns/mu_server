using System;

namespace GameServer.Logic
{
	// Token: 0x0200071E RID: 1822
	public class UpLevelAwardItem
	{
		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06002B83 RID: 11139 RVA: 0x00268A44 File Offset: 0x00266C44
		// (set) Token: 0x06002B84 RID: 11140 RVA: 0x00268A5B File Offset: 0x00266C5B
		public int ID { get; set; }

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06002B85 RID: 11141 RVA: 0x00268A64 File Offset: 0x00266C64
		// (set) Token: 0x06002B86 RID: 11142 RVA: 0x00268A7B File Offset: 0x00266C7B
		public int MinDay { get; set; }

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06002B87 RID: 11143 RVA: 0x00268A84 File Offset: 0x00266C84
		// (set) Token: 0x06002B88 RID: 11144 RVA: 0x00268A9B File Offset: 0x00266C9B
		public int MaxDay { get; set; }

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06002B89 RID: 11145 RVA: 0x00268AA4 File Offset: 0x00266CA4
		// (set) Token: 0x06002B8A RID: 11146 RVA: 0x00268ABB File Offset: 0x00266CBB
		public int AwardYuanBao { get; set; }
	}
}
