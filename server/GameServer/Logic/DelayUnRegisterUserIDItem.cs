using System;

namespace GameServer.Logic
{
	// Token: 0x020007E5 RID: 2021
	public class DelayUnRegisterUserIDItem
	{
		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06003931 RID: 14641 RVA: 0x00309BAC File Offset: 0x00307DAC
		// (set) Token: 0x06003932 RID: 14642 RVA: 0x00309BC3 File Offset: 0x00307DC3
		public string UserID { get; set; }

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x06003933 RID: 14643 RVA: 0x00309BCC File Offset: 0x00307DCC
		// (set) Token: 0x06003934 RID: 14644 RVA: 0x00309BE3 File Offset: 0x00307DE3
		public long StartTicks { get; set; }

		// Token: 0x04004325 RID: 17189
		public int ServerId;
	}
}
