using System;
using GameServer.Logic;

namespace Server.Data
{
	// Token: 0x02000443 RID: 1091
	public class ThemeBossScene
	{
		// Token: 0x04001D6F RID: 7535
		public int MapCode;

		// Token: 0x04001D70 RID: 7536
		public ThemeBossConfig BossConfigInfo;

		// Token: 0x04001D71 RID: 7537
		public BattleStates State;

		// Token: 0x04001D72 RID: 7538
		public long StartTick;

		// Token: 0x04001D73 RID: 7539
		public long EndTick;

		// Token: 0x04001D74 RID: 7540
		public int AliveBossNum;

		// Token: 0x04001D75 RID: 7541
		public bool BroadCast4014 = false;
	}
}
