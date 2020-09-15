using System;
using System.Collections.Generic;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x0200042A RID: 1066
	public class ZhengDuoScene
	{
		// Token: 0x06001366 RID: 4966 RVA: 0x0013248C File Offset: 0x0013068C
		public void Start()
		{
			this.m_lPrepareTime = this.StartTimeTicks;
			this.m_lBeginTime = this.m_lPrepareTime + (long)(this.SceneInfo.SecondWait * 1000);
			this.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
			this.StateTimeData.GameType = 17;
			this.StateTimeData.State = (int)this.m_eStatus;
			this.StateTimeData.EndTicks = this.m_lBeginTime;
		}

		// Token: 0x06001367 RID: 4967 RVA: 0x001324FB File Offset: 0x001306FB
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
		}

		// Token: 0x04001C9A RID: 7322
		public int m_nMapCode = 0;

		// Token: 0x04001C9B RID: 7323
		public int FuBenSeqId = 0;

		// Token: 0x04001C9C RID: 7324
		public int CopyMapId = 0;

		// Token: 0x04001C9D RID: 7325
		public long StartTimeTicks = 0L;

		// Token: 0x04001C9E RID: 7326
		public long m_lPrepareTime = 0L;

		// Token: 0x04001C9F RID: 7327
		public long m_lBeginTime = 0L;

		// Token: 0x04001CA0 RID: 7328
		public long m_lEndTime = 0L;

		// Token: 0x04001CA1 RID: 7329
		public long m_lLeaveTime = 0L;

		// Token: 0x04001CA2 RID: 7330
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04001CA3 RID: 7331
		public int SuccessSide = 0;

		// Token: 0x04001CA4 RID: 7332
		public int KillUsedTicks = 1800000;

		// Token: 0x04001CA5 RID: 7333
		public int KillerId;

		// Token: 0x04001CA6 RID: 7334
		public bool m_bEndFlag = false;

		// Token: 0x04001CA7 RID: 7335
		public bool PreliminarisesMode = true;

		// Token: 0x04001CA8 RID: 7336
		public int GameId;

		// Token: 0x04001CA9 RID: 7337
		public CopyMap CopyMap;

		// Token: 0x04001CAA RID: 7338
		public ZhengDuoSceneInfo SceneInfo;

		// Token: 0x04001CAB RID: 7339
		public ZhengDuoRankData[] RankDatas = new ZhengDuoRankData[2];

		// Token: 0x04001CAC RID: 7340
		public ZhengDuoScoreData[] ScoreData = new ZhengDuoScoreData[4];

		// Token: 0x04001CAD RID: 7341
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x04001CAE RID: 7342
		public int IsMonsterFlag = 0;

		// Token: 0x04001CAF RID: 7343
		public long CreateMonsterTime = 0L;

		// Token: 0x04001CB0 RID: 7344
		public Dictionary<int, ZhengDuoScoreData> ClientContextDataDict = new Dictionary<int, ZhengDuoScoreData>();

		// Token: 0x04001CB1 RID: 7345
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		// Token: 0x04001CB2 RID: 7346
		public int MapGridWidth = 100;

		// Token: 0x04001CB3 RID: 7347
		public int MapGridHeight = 100;

		// Token: 0x04001CB4 RID: 7348
		public int Age;
	}
}
