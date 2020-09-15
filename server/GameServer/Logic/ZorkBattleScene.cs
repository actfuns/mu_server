using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x0200084C RID: 2124
	public class ZorkBattleScene
	{
		// Token: 0x06003BBE RID: 15294 RVA: 0x0032F29B File Offset: 0x0032D49B
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

		// Token: 0x04004651 RID: 18001
		public int m_nMapCode = 0;

		// Token: 0x04004652 RID: 18002
		public int FuBenSeqId = 0;

		// Token: 0x04004653 RID: 18003
		public int CopyMapId = 0;

		// Token: 0x04004654 RID: 18004
		public long StartTimeTicks = 0L;

		// Token: 0x04004655 RID: 18005
		public long m_lPrepareTime = 0L;

		// Token: 0x04004656 RID: 18006
		public long m_lBeginTime = 0L;

		// Token: 0x04004657 RID: 18007
		public long m_lEndTime = 0L;

		// Token: 0x04004658 RID: 18008
		public long m_lLeaveTime = 0L;

		// Token: 0x04004659 RID: 18009
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x0400465A RID: 18010
		public int m_nPlarerCount = 0;

		// Token: 0x0400465B RID: 18011
		public int SuccessSide = 0;

		// Token: 0x0400465C RID: 18012
		public bool m_bEndFlag = false;

		// Token: 0x0400465D RID: 18013
		public int GameId;

		// Token: 0x0400465E RID: 18014
		public CopyMap CopyMap;

		// Token: 0x0400465F RID: 18015
		public ZorkBattleSceneInfo SceneInfo;

		// Token: 0x04004660 RID: 18016
		public ZorkBattleSideScore ScoreData = new ZorkBattleSideScore();

		// Token: 0x04004661 RID: 18017
		public Dictionary<int, List<ZorkBattleRoleInfo>> ClientContextDataDict = new Dictionary<int, List<ZorkBattleRoleInfo>>();

		// Token: 0x04004662 RID: 18018
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x04004663 RID: 18019
		public List<ZorkBattleArmyConfig> ZorkBattleArmyList = new List<ZorkBattleArmyConfig>();

		// Token: 0x04004664 RID: 18020
		public Dictionary<int, List<ZorkBattleMonsterConfig>> ZorkBattleMonsterDict = new Dictionary<int, List<ZorkBattleMonsterConfig>>();

		// Token: 0x04004665 RID: 18021
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		// Token: 0x04004666 RID: 18022
		public Dictionary<string, ZorkBattleSceneBuff> SceneBuffDict = new Dictionary<string, ZorkBattleSceneBuff>();

		// Token: 0x04004667 RID: 18023
		public ZorkBattleStatisticalData GameStatisticalData = new ZorkBattleStatisticalData();

		// Token: 0x04004668 RID: 18024
		public int MapGridWidth = 100;

		// Token: 0x04004669 RID: 18025
		public int MapGridHeight = 100;
	}
}
