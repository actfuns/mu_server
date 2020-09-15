using System;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000089 RID: 137
	public class EscapeBattleCollection
	{
		// Token: 0x0600020B RID: 523 RVA: 0x000217D8 File Offset: 0x0001F9D8
		public EscapeBattleCollection Clone()
		{
			return base.MemberwiseClone() as EscapeBattleCollection;
		}

		// Token: 0x04000321 RID: 801
		public int ID;

		// Token: 0x04000322 RID: 802
		public int MapCodeID;

		// Token: 0x04000323 RID: 803
		public EscapeBCollectType cType;

		// Token: 0x04000324 RID: 804
		public EscapeBattleGameSceneStatuses eState;

		// Token: 0x04000325 RID: 805
		public int RefreshRegion;

		// Token: 0x04000326 RID: 806
		public int RefreshTime;

		// Token: 0x04000327 RID: 807
		public int RefreshMonsterId;

		// Token: 0x04000328 RID: 808
		public int RefreshMonsterNum;

		// Token: 0x04000329 RID: 809
		public int CollectTime;

		// Token: 0x0400032A RID: 810
		public int CollectGodNum;

		// Token: 0x0400032B RID: 811
		public int CollectLiveTime;

		// Token: 0x0400032C RID: 812
		public int IsDeath;
	}
}
