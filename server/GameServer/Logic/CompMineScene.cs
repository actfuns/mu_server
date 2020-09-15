using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x02000275 RID: 629
	public class CompMineScene
	{
		// Token: 0x0600090B RID: 2315 RVA: 0x0008EDDD File Offset: 0x0008CFDD
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
			this.ScoreData = new CompMineSideScore();
		}

		// Token: 0x04000FB6 RID: 4022
		public int m_nMapCode = 0;

		// Token: 0x04000FB7 RID: 4023
		public int FuBenSeqId = 0;

		// Token: 0x04000FB8 RID: 4024
		public int CopyMapId = 0;

		// Token: 0x04000FB9 RID: 4025
		public long StartTimeTicks = 0L;

		// Token: 0x04000FBA RID: 4026
		public long m_lPrepareTime = 0L;

		// Token: 0x04000FBB RID: 4027
		public long m_lBeginTime = 0L;

		// Token: 0x04000FBC RID: 4028
		public long m_lEndTime = 0L;

		// Token: 0x04000FBD RID: 4029
		public long m_lLeaveTime = 0L;

		// Token: 0x04000FBE RID: 4030
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04000FBF RID: 4031
		public bool m_bEndFlag = false;

		// Token: 0x04000FC0 RID: 4032
		public int GameId;

		// Token: 0x04000FC1 RID: 4033
		public CopyMap CopyMap;

		// Token: 0x04000FC2 RID: 4034
		public CompMineConfig SceneInfo;

		// Token: 0x04000FC3 RID: 4035
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		// Token: 0x04000FC4 RID: 4036
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x04000FC5 RID: 4037
		public Dictionary<int, CompMineClientContextData> ClientContextDataDict = new Dictionary<int, CompMineClientContextData>();

		// Token: 0x04000FC6 RID: 4038
		public CompMineSideScore ScoreData = new CompMineSideScore();

		// Token: 0x04000FC7 RID: 4039
		public int MapGridWidth = 100;

		// Token: 0x04000FC8 RID: 4040
		public int MapGridHeight = 100;
	}
}
