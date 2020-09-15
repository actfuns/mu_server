using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200033F RID: 831
	public class KingOfBattleScene
	{
		// Token: 0x06000DFC RID: 3580 RVA: 0x000DDECA File Offset: 0x000DC0CA
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

		// Token: 0x040015BA RID: 5562
		public int m_nMapCode = 0;

		// Token: 0x040015BB RID: 5563
		public int FuBenSeqId = 0;

		// Token: 0x040015BC RID: 5564
		public int CopyMapId = 0;

		// Token: 0x040015BD RID: 5565
		public long StartTimeTicks = 0L;

		// Token: 0x040015BE RID: 5566
		public long m_lPrepareTime = 0L;

		// Token: 0x040015BF RID: 5567
		public long m_lBeginTime = 0L;

		// Token: 0x040015C0 RID: 5568
		public long m_lEndTime = 0L;

		// Token: 0x040015C1 RID: 5569
		public long m_lLeaveTime = 0L;

		// Token: 0x040015C2 RID: 5570
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x040015C3 RID: 5571
		public int m_nPlarerCount = 0;

		// Token: 0x040015C4 RID: 5572
		public int SuccessSide = 0;

		// Token: 0x040015C5 RID: 5573
		public bool m_bEndFlag = false;

		// Token: 0x040015C6 RID: 5574
		public bool GuangMuNotify1 = false;

		// Token: 0x040015C7 RID: 5575
		public bool GuangMuNotify2 = false;

		// Token: 0x040015C8 RID: 5576
		public int GameId;

		// Token: 0x040015C9 RID: 5577
		public CopyMap CopyMap;

		// Token: 0x040015CA RID: 5578
		public KingOfBattleSceneInfo SceneInfo;

		// Token: 0x040015CB RID: 5579
		public KingOfBattleStatisticalData GameStatisticalData = new KingOfBattleStatisticalData();

		// Token: 0x040015CC RID: 5580
		public KingOfBattleScoreData ScoreData = new KingOfBattleScoreData();

		// Token: 0x040015CD RID: 5581
		public Dictionary<int, KingOfBattleClientContextData> ClientContextDataDict = new Dictionary<int, KingOfBattleClientContextData>();

		// Token: 0x040015CE RID: 5582
		public KingOfBattleClientContextData ClientContextMVP = new KingOfBattleClientContextData();

		// Token: 0x040015CF RID: 5583
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x040015D0 RID: 5584
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		// Token: 0x040015D1 RID: 5585
		public Dictionary<int, KingOfBattleQiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, KingOfBattleQiZhiConfig>();

		// Token: 0x040015D2 RID: 5586
		public Dictionary<string, KingOfBattleSceneBuff> SceneBuffDict = new Dictionary<string, KingOfBattleSceneBuff>();

		// Token: 0x040015D3 RID: 5587
		public List<int> SceneOpenTeleportList = new List<int>();

		// Token: 0x040015D4 RID: 5588
		public int MapGridWidth = 100;

		// Token: 0x040015D5 RID: 5589
		public int MapGridHeight = 100;
	}
}
