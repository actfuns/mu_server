using System;

namespace GameServer.Logic
{
	// Token: 0x020003C9 RID: 969
	public class LangHunLingYuEveryDayAwardsItem
	{
		// Token: 0x04001952 RID: 6482
		public int ID;

		// Token: 0x04001953 RID: 6483
		public int ZhiWu;

		// Token: 0x04001954 RID: 6484
		public int DayZhanGong;

		// Token: 0x04001955 RID: 6485
		public long DayExp;

		// Token: 0x04001956 RID: 6486
		public AwardsItemList DayGoods = new AwardsItemList();
	}
}
