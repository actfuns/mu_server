using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020003CA RID: 970
	public class CityLevelInfo
	{
		// Token: 0x04001957 RID: 6487
		public int ID;

		// Token: 0x04001958 RID: 6488
		public int CityLevel;

		// Token: 0x04001959 RID: 6489
		public int CityNum;

		// Token: 0x0400195A RID: 6490
		public int MaxNum;

		// Token: 0x0400195B RID: 6491
		public List<TimeSpan> BaoMingTime = new List<TimeSpan>();

		// Token: 0x0400195C RID: 6492
		public List<int> AttackWeekDay;

		// Token: 0x0400195D RID: 6493
		public List<TimeSpan> AttackTime = new List<TimeSpan>();

		// Token: 0x0400195E RID: 6494
		public AwardsItemList Award = new AwardsItemList();

		// Token: 0x0400195F RID: 6495
		public AwardsItemList DayAward = new AwardsItemList();

		// Token: 0x04001960 RID: 6496
		public int ZhanMengZiJin;
	}
}
