using System;
using GameServer.Logic;

namespace Server.Data
{
	// Token: 0x020003E5 RID: 997
	public class RebornBossAwardConfig
	{
		// Token: 0x04001A83 RID: 6787
		public int ID;

		// Token: 0x04001A84 RID: 6788
		public int MonstersID;

		// Token: 0x04001A85 RID: 6789
		public int BeginNum;

		// Token: 0x04001A86 RID: 6790
		public int EndNum;

		// Token: 0x04001A87 RID: 6791
		public AwardsItemList AwardsItemListOne = new AwardsItemList();

		// Token: 0x04001A88 RID: 6792
		public AwardsItemList AwardsItemListTwo = new AwardsItemList();

		// Token: 0x04001A89 RID: 6793
		public int AwardType;
	}
}
