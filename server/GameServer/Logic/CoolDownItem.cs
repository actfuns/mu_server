using System;

namespace GameServer.Logic
{
	// Token: 0x020005ED RID: 1517
	public class CoolDownItem
	{
		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06001CDB RID: 7387 RVA: 0x001AB7E0 File Offset: 0x001A99E0
		// (set) Token: 0x06001CDC RID: 7388 RVA: 0x001AB7F7 File Offset: 0x001A99F7
		public int ID { get; set; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06001CDD RID: 7389 RVA: 0x001AB800 File Offset: 0x001A9A00
		// (set) Token: 0x06001CDE RID: 7390 RVA: 0x001AB817 File Offset: 0x001A9A17
		public long StartTicks { get; set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06001CDF RID: 7391 RVA: 0x001AB820 File Offset: 0x001A9A20
		// (set) Token: 0x06001CE0 RID: 7392 RVA: 0x001AB837 File Offset: 0x001A9A37
		public long CDTicks { get; set; }
	}
}
