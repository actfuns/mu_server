using System;

namespace GameServer.Logic
{
	// Token: 0x0200020D RID: 525
	public class BHMatchRankAwardConfig
	{
		// Token: 0x04000BBD RID: 3005
		public int ID;

		// Token: 0x04000BBE RID: 3006
		public int BeginNum;

		// Token: 0x04000BBF RID: 3007
		public int EndNum;

		// Token: 0x04000BC0 RID: 3008
		public AwardsItemList AwardsItemList = new AwardsItemList();
	}
}
