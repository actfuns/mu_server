using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	// Token: 0x0200043E RID: 1086
	public class MoYuRunData
	{
		// Token: 0x04001D43 RID: 7491
		public object Mutex = new object();

		// Token: 0x04001D44 RID: 7492
		public Dictionary<int, MoYuMonsterInfo> MonsterXmlDict = new Dictionary<int, MoYuMonsterInfo>();

		// Token: 0x04001D45 RID: 7493
		public Dictionary<int, BossAttackLog> BossAttackLogDict = new Dictionary<int, BossAttackLog>();

		// Token: 0x04001D46 RID: 7494
		public List<int> BroadGoodsIDList = new List<int>();

		// Token: 0x04001D47 RID: 7495
		public List<int> MapCodeList = new List<int>();

		// Token: 0x04001D48 RID: 7496
		public Activity ThemeMoYuActivity = new Activity();

		// Token: 0x04001D49 RID: 7497
		public DateTime LastBirthTimePoint = DateTime.MinValue;
	}
}
