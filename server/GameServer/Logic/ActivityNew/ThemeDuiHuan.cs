using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000445 RID: 1093
	public class ThemeDuiHuan
	{
		// Token: 0x04001D81 RID: 7553
		public int id;

		// Token: 0x04001D82 RID: 7554
		public int DayMaxTimes = 0;

		// Token: 0x04001D83 RID: 7555
		public List<GoodsData> NeedGoodsList = null;

		// Token: 0x04001D84 RID: 7556
		public AwardItem MyAwardItem = new AwardItem();
	}
}
