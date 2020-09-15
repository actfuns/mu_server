using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	// Token: 0x02000441 RID: 1089
	public class ZhuanShengRunData
	{
		// Token: 0x04001D5F RID: 7519
		public const int MaxAttackRankNum = 20;

		// Token: 0x04001D60 RID: 7520
		public const int MaxAttackRankNumClient = 5;

		// Token: 0x04001D61 RID: 7521
		public object Mutex = new object();

		// Token: 0x04001D62 RID: 7522
		public Dictionary<int, ZhuanShengMapInfo> ZhuanShengMapDict = new Dictionary<int, ZhuanShengMapInfo>();

		// Token: 0x04001D63 RID: 7523
		public Dictionary<int, List<ShiLianReward>> ShiLianRewardDict = new Dictionary<int, List<ShiLianReward>>();

		// Token: 0x04001D64 RID: 7524
		public List<int> BroadGoodsIDList = new List<int>();

		// Token: 0x04001D65 RID: 7525
		public Activity ThemeZSActivity = new Activity();
	}
}
