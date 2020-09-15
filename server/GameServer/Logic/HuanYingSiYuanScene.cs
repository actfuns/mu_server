using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000504 RID: 1284
	public class HuanYingSiYuanScene
	{
		// Token: 0x060017BA RID: 6074 RVA: 0x00173646 File Offset: 0x00171846
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_nPlarerCount = 0;
			this.m_bEndFlag = false;
		}

		// Token: 0x040021F3 RID: 8691
		public int m_nMapCode = 0;

		// Token: 0x040021F4 RID: 8692
		public int FuBenSeqId = 0;

		// Token: 0x040021F5 RID: 8693
		public int CopyMapId = 0;

		// Token: 0x040021F6 RID: 8694
		public long m_lPrepareTime = 0L;

		// Token: 0x040021F7 RID: 8695
		public long m_lBeginTime = 0L;

		// Token: 0x040021F8 RID: 8696
		public long m_lEndTime = 0L;

		// Token: 0x040021F9 RID: 8697
		public long m_lLeaveTime = 0L;

		// Token: 0x040021FA RID: 8698
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x040021FB RID: 8699
		public int m_nPlarerCount = 0;

		// Token: 0x040021FC RID: 8700
		public Dictionary<int, HuanYingSiYuanShengBeiContextData> ShengBeiContextDict = new Dictionary<int, HuanYingSiYuanShengBeiContextData>();

		// Token: 0x040021FD RID: 8701
		public int SuccessSide = 0;

		// Token: 0x040021FE RID: 8702
		public HuanYingSiYuanScoreInfoData ScoreInfoData = new HuanYingSiYuanScoreInfoData();

		// Token: 0x040021FF RID: 8703
		public bool m_bEndFlag = false;

		// Token: 0x04002200 RID: 8704
		public int GameId;

		// Token: 0x04002201 RID: 8705
		public CopyMap CopyMap;

		// Token: 0x04002202 RID: 8706
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();
	}
}
