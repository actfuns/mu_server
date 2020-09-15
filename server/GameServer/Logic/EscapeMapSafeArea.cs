using System;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200008A RID: 138
	public class EscapeMapSafeArea
	{
		// Token: 0x0400032D RID: 813
		public int ID;

		// Token: 0x0400032E RID: 814
		public EscapeBattleGameSceneStatuses eState;

		// Token: 0x0400032F RID: 815
		public int TimeStage;

		// Token: 0x04000330 RID: 816
		public int[] StartSafePoint;

		// Token: 0x04000331 RID: 817
		public int SafeRadius;

		// Token: 0x04000332 RID: 818
		public int GodFireHitTime;

		// Token: 0x04000333 RID: 819
		public double GodFireHitPercent;

		// Token: 0x04000334 RID: 820
		public int GodFireHitHp;
	}
}
