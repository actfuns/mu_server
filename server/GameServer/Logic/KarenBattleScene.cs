using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200032C RID: 812
	public class KarenBattleScene
	{
		// Token: 0x06000D75 RID: 3445 RVA: 0x000D1B25 File Offset: 0x000CFD25
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

		// Token: 0x040014F0 RID: 5360
		public int m_nMapCode = 0;

		// Token: 0x040014F1 RID: 5361
		public int FuBenSeqId = 0;

		// Token: 0x040014F2 RID: 5362
		public int CopyMapId = 0;

		// Token: 0x040014F3 RID: 5363
		public long StartTimeTicks = 0L;

		// Token: 0x040014F4 RID: 5364
		public long m_lPrepareTime = 0L;

		// Token: 0x040014F5 RID: 5365
		public long m_lBeginTime = 0L;

		// Token: 0x040014F6 RID: 5366
		public long m_lEndTime = 0L;

		// Token: 0x040014F7 RID: 5367
		public long m_lLeaveTime = 0L;

		// Token: 0x040014F8 RID: 5368
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x040014F9 RID: 5369
		public int m_nPlarerCount = 0;

		// Token: 0x040014FA RID: 5370
		public int SuccessSide = 0;

		// Token: 0x040014FB RID: 5371
		public bool m_bEndFlag = false;

		// Token: 0x040014FC RID: 5372
		public bool GuangMuNotify1 = false;

		// Token: 0x040014FD RID: 5373
		public bool GuangMuNotify2 = false;

		// Token: 0x040014FE RID: 5374
		public int GameId;

		// Token: 0x040014FF RID: 5375
		public CopyMap CopyMap;

		// Token: 0x04001500 RID: 5376
		public KarenBattleSceneInfo SceneInfo;

		// Token: 0x04001501 RID: 5377
		public List<KarenBattleScoreData> ScoreData = new List<KarenBattleScoreData>();

		// Token: 0x04001502 RID: 5378
		public Dictionary<int, KarenBattleClientContextData> ClientContextDataDict = new Dictionary<int, KarenBattleClientContextData>();

		// Token: 0x04001503 RID: 5379
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x04001504 RID: 5380
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		// Token: 0x04001505 RID: 5381
		public Dictionary<int, KarenBattleQiZhiConfig_West> NPCID2QiZhiConfigDict = new Dictionary<int, KarenBattleQiZhiConfig_West>();

		// Token: 0x04001506 RID: 5382
		public Dictionary<int, KarenCenterConfig> KarenCenterConfigDict = new Dictionary<int, KarenCenterConfig>();

		// Token: 0x04001507 RID: 5383
		public Dictionary<string, KarenBattleSceneBuff> SceneBuffDict = new Dictionary<string, KarenBattleSceneBuff>();

		// Token: 0x04001508 RID: 5384
		public int MapGridWidth = 100;

		// Token: 0x04001509 RID: 5385
		public int MapGridHeight = 100;
	}
}
