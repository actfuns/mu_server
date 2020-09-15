using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200081E RID: 2078
	public class YongZheZhanChangScene
	{
		// Token: 0x06003ABD RID: 15037 RVA: 0x0031E619 File Offset: 0x0031C819
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

		// Token: 0x040044CC RID: 17612
		public int m_nMapCode = 0;

		// Token: 0x040044CD RID: 17613
		public int FuBenSeqId = 0;

		// Token: 0x040044CE RID: 17614
		public int CopyMapId = 0;

		// Token: 0x040044CF RID: 17615
		public long StartTimeTicks = 0L;

		// Token: 0x040044D0 RID: 17616
		public long m_lPrepareTime = 0L;

		// Token: 0x040044D1 RID: 17617
		public long m_lBeginTime = 0L;

		// Token: 0x040044D2 RID: 17618
		public long m_lEndTime = 0L;

		// Token: 0x040044D3 RID: 17619
		public long m_lLeaveTime = 0L;

		// Token: 0x040044D4 RID: 17620
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x040044D5 RID: 17621
		public int m_nPlarerCount = 0;

		// Token: 0x040044D6 RID: 17622
		public int SuccessSide = 0;

		// Token: 0x040044D7 RID: 17623
		public bool m_bEndFlag = false;

		// Token: 0x040044D8 RID: 17624
		public int GameId;

		// Token: 0x040044D9 RID: 17625
		public CopyMap CopyMap;

		// Token: 0x040044DA RID: 17626
		public YongZheZhanChangSceneInfo SceneInfo;

		// Token: 0x040044DB RID: 17627
		public YongZheZhanChangStatisticalData GameStatisticalData = new YongZheZhanChangStatisticalData();

		// Token: 0x040044DC RID: 17628
		public YongZheZhanChangScoreData ScoreData = new YongZheZhanChangScoreData();

		// Token: 0x040044DD RID: 17629
		public Dictionary<int, YongZheZhanChangClientContextData> ClientContextDataDict = new Dictionary<int, YongZheZhanChangClientContextData>();

		// Token: 0x040044DE RID: 17630
		public YongZheZhanChangClientContextData ClientContextMVP = new YongZheZhanChangClientContextData();

		// Token: 0x040044DF RID: 17631
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x040044E0 RID: 17632
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		// Token: 0x040044E1 RID: 17633
		public int MapGridWidth = 100;

		// Token: 0x040044E2 RID: 17634
		public int MapGridHeight = 100;
	}
}
