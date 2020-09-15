using System;

namespace GameServer.Logic.CheatGuard
{
	// Token: 0x02000251 RID: 593
	internal class TradeBlackObject
	{
		// Token: 0x04000E46 RID: 3654
		public int RoleId;

		// Token: 0x04000E47 RID: 3655
		public TradeBlackHourItem[] HourItems;

		// Token: 0x04000E48 RID: 3656
		public DateTime LastFlushTime;

		// Token: 0x04000E49 RID: 3657
		public long BanTradeToTicks;

		// Token: 0x04000E4A RID: 3658
		public int ChangeLife;

		// Token: 0x04000E4B RID: 3659
		public int Level;

		// Token: 0x04000E4C RID: 3660
		public int VipLevel;

		// Token: 0x04000E4D RID: 3661
		public int ZoneId;

		// Token: 0x04000E4E RID: 3662
		public string RoleName;
	}
}
