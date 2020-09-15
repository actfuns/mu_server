using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200021C RID: 540
	public class BangHuiMatchScene
	{
		// Token: 0x06000733 RID: 1843 RVA: 0x0006AD9E File Offset: 0x00068F9E
		public void CleanAllInfo()
		{
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
		}

		// Token: 0x04000C44 RID: 3140
		public int FuBenSeqId = 0;

		// Token: 0x04000C45 RID: 3141
		public long StartTimeTicks = 0L;

		// Token: 0x04000C46 RID: 3142
		public long m_lPrepareTime = 0L;

		// Token: 0x04000C47 RID: 3143
		public long m_lBeginTime = 0L;

		// Token: 0x04000C48 RID: 3144
		public long m_lEndTime = 0L;

		// Token: 0x04000C49 RID: 3145
		public long m_lLeaveTime = 0L;

		// Token: 0x04000C4A RID: 3146
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04000C4B RID: 3147
		public int SuccessSide = 0;

		// Token: 0x04000C4C RID: 3148
		public bool m_bEndFlag = false;

		// Token: 0x04000C4D RID: 3149
		public int GameId;

		// Token: 0x04000C4E RID: 3150
		public Dictionary<int, CopyMap> CopyMapDict = new Dictionary<int, CopyMap>();

		// Token: 0x04000C4F RID: 3151
		public BHMatchConfig SceneInfo;

		// Token: 0x04000C50 RID: 3152
		public BangHuiMatchScoreData ScoreData = new BangHuiMatchScoreData();

		// Token: 0x04000C51 RID: 3153
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x04000C52 RID: 3154
		public Dictionary<int, BHMatchClientContextData> ClientContextDataDict = new Dictionary<int, BHMatchClientContextData>();

		// Token: 0x04000C53 RID: 3155
		public BHMatchClientContextData ClientContextMVP = new BHMatchClientContextData();

		// Token: 0x04000C54 RID: 3156
		public Dictionary<int, BHMatchQiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, BHMatchQiZhiConfig>();

		// Token: 0x04000C55 RID: 3157
		public BangHuiMatchStatisticalData GameStatisticalData = new BangHuiMatchStatisticalData();

		// Token: 0x04000C56 RID: 3158
		public int LT_BHServerID;

		// Token: 0x04000C57 RID: 3159
		public int LT_BattleWhichSide;

		// Token: 0x04000C58 RID: 3160
		public long LT_OwnTicks;

		// Token: 0x04000C59 RID: 3161
		public long LT_OwnTicksDelta;

		// Token: 0x04000C5A RID: 3162
		public int MapGridWidth = 100;

		// Token: 0x04000C5B RID: 3163
		public int MapGridHeight = 100;
	}
}
