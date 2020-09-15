using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000433 RID: 1075
	public class TenRetutnAwardsData
	{
		// Token: 0x04001D0E RID: 7438
		public int ID;

		// Token: 0x04001D0F RID: 7439
		public int ChongZhiZhuanShi;

		// Token: 0x04001D10 RID: 7440
		public AwardsItemList GoodsID1 = new AwardsItemList();

		// Token: 0x04001D11 RID: 7441
		public AwardsItemList GoodsID2 = new AwardsItemList();

		// Token: 0x04001D12 RID: 7442
		public string MailUser = "";

		// Token: 0x04001D13 RID: 7443
		public string MailTitle = "";

		// Token: 0x04001D14 RID: 7444
		public string MailContent = "";

		// Token: 0x04001D15 RID: 7445
		public string BeginTimeStr;

		// Token: 0x04001D16 RID: 7446
		public string FinishTimeStr;

		// Token: 0x04001D17 RID: 7447
		public DateTime BeginTime = DateTime.MaxValue;

		// Token: 0x04001D18 RID: 7448
		public DateTime FinishTime = DateTime.MinValue;

		// Token: 0x04001D19 RID: 7449
		public string UserList;

		// Token: 0x04001D1A RID: 7450
		public Dictionary<string, bool> _tenUserIdAwardsDict = new Dictionary<string, bool>();

		// Token: 0x04001D1B RID: 7451
		public bool SystemOpen;
	}
}
