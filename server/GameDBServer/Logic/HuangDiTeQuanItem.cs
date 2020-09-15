using System;

namespace GameDBServer.Logic
{
	// Token: 0x020001AA RID: 426
	public class HuangDiTeQuanItem
	{
		// Token: 0x040009B0 RID: 2480
		public int ID = 0;

		// Token: 0x040009B1 RID: 2481
		public int ToLaoFangDayID = 0;

		// Token: 0x040009B2 RID: 2482
		public int ToLaoFangNum = 0;

		// Token: 0x040009B3 RID: 2483
		public int OffLaoFangDayID = 0;

		// Token: 0x040009B4 RID: 2484
		public int OffLaoFangNum = 0;

		// Token: 0x040009B5 RID: 2485
		public int BanCatDayID = 0;

		// Token: 0x040009B6 RID: 2486
		public int BanCatNum = 0;

		// Token: 0x040009B7 RID: 2487
		public long LastBanChatTicks = 0L;

		// Token: 0x040009B8 RID: 2488
		public long LastSendToLaoFangTicks = 0L;

		// Token: 0x040009B9 RID: 2489
		public long LastTakeOffLaoFangTicks = 0L;
	}
}
