using System;

namespace GameServer.Logic
{
	// Token: 0x02000661 RID: 1633
	public enum BattleStates
	{
		// Token: 0x0400311D RID: 12573
		NoBattle,
		// Token: 0x0400311E RID: 12574
		PublishMsg,
		// Token: 0x0400311F RID: 12575
		WaitingFight,
		// Token: 0x04003120 RID: 12576
		StartFight,
		// Token: 0x04003121 RID: 12577
		EndFight,
		// Token: 0x04003122 RID: 12578
		ClearBattle
	}
}
