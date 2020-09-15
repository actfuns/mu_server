using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x0200025F RID: 607
	public class CompBattleScene
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0008102F File Offset: 0x0007F22F
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
			this.ScoreData.Clear();
		}

		// Token: 0x04000EEB RID: 3819
		public int m_nMapCode = 0;

		// Token: 0x04000EEC RID: 3820
		public int FuBenSeqId = 0;

		// Token: 0x04000EED RID: 3821
		public int CopyMapId = 0;

		// Token: 0x04000EEE RID: 3822
		public long StartTimeTicks = 0L;

		// Token: 0x04000EEF RID: 3823
		public long m_lPrepareTime = 0L;

		// Token: 0x04000EF0 RID: 3824
		public long m_lBeginTime = 0L;

		// Token: 0x04000EF1 RID: 3825
		public long m_lEndTime = 0L;

		// Token: 0x04000EF2 RID: 3826
		public long m_lLeaveTime = 0L;

		// Token: 0x04000EF3 RID: 3827
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04000EF4 RID: 3828
		public int SuccessSide = 0;

		// Token: 0x04000EF5 RID: 3829
		public bool m_bEndFlag = false;

		// Token: 0x04000EF6 RID: 3830
		public int GameId;

		// Token: 0x04000EF7 RID: 3831
		public CopyMap CopyMap;

		// Token: 0x04000EF8 RID: 3832
		public CompBattleConfig SceneInfo;

		// Token: 0x04000EF9 RID: 3833
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x04000EFA RID: 3834
		public Dictionary<int, CompStrongholdConfig> CompStrongholdConfigDict = new Dictionary<int, CompStrongholdConfig>();

		// Token: 0x04000EFB RID: 3835
		public Dictionary<int, CompBattleClientContextData> ClientContextDataDict = new Dictionary<int, CompBattleClientContextData>();

		// Token: 0x04000EFC RID: 3836
		public List<CompBattleSideScore> ScoreData = new List<CompBattleSideScore>();

		// Token: 0x04000EFD RID: 3837
		public int MapGridWidth = 100;

		// Token: 0x04000EFE RID: 3838
		public int MapGridHeight = 100;
	}
}
