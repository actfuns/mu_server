using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000356 RID: 854
	internal class CoupleArenaCopyScene
	{
		// Token: 0x04001688 RID: 5768
		public int FuBenSeq;

		// Token: 0x04001689 RID: 5769
		public int GameId;

		// Token: 0x0400168A RID: 5770
		public int MapCode;

		// Token: 0x0400168B RID: 5771
		public CopyMap CopyMap;

		// Token: 0x0400168C RID: 5772
		public long m_lPrepareTime = 0L;

		// Token: 0x0400168D RID: 5773
		public long m_lBeginTime = 0L;

		// Token: 0x0400168E RID: 5774
		public long m_lEndTime = 0L;

		// Token: 0x0400168F RID: 5775
		public long m_lLeaveTime = 0L;

		// Token: 0x04001690 RID: 5776
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04001691 RID: 5777
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x04001692 RID: 5778
		public int WinSide = 0;

		// Token: 0x04001693 RID: 5779
		public long m_lPrevUpdateTime = 0L;

		// Token: 0x04001694 RID: 5780
		public long m_lCurrUpdateTime = 0L;

		// Token: 0x04001695 RID: 5781
		public Dictionary<int, int> EnterRoleSide = new Dictionary<int, int>();

		// Token: 0x04001696 RID: 5782
		public bool IsYongQiMonsterExist = false;

		// Token: 0x04001697 RID: 5783
		public int YongQiBuff_Role;

		// Token: 0x04001698 RID: 5784
		public bool IsZhenAiMonsterExist = false;

		// Token: 0x04001699 RID: 5785
		public int ZhenAiBuff_Role;

		// Token: 0x0400169A RID: 5786
		public long ZhenAiBuff_StartMs;
	}
}
