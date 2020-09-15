using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000824 RID: 2084
	public class EscapeBattleScene
	{
		// Token: 0x06003AF1 RID: 15089 RVA: 0x00320967 File Offset: 0x0031EB67
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = EscapeBattleGameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
		}

		// Token: 0x04004504 RID: 17668
		public int m_nMapCode = 0;

		// Token: 0x04004505 RID: 17669
		public int FuBenSeqId = 0;

		// Token: 0x04004506 RID: 17670
		public int CopyMapId = 0;

		// Token: 0x04004507 RID: 17671
		public long StartTimeTicks = 0L;

		// Token: 0x04004508 RID: 17672
		public long m_lPrepareTime = 0L;

		// Token: 0x04004509 RID: 17673
		public long m_lBeginTime = 0L;

		// Token: 0x0400450A RID: 17674
		public long m_lFightTime = 0L;

		// Token: 0x0400450B RID: 17675
		public long m_lEndTime = 0L;

		// Token: 0x0400450C RID: 17676
		public long m_lLeaveTime = 0L;

		// Token: 0x0400450D RID: 17677
		public EscapeBattleGameSceneStatuses m_eStatus = EscapeBattleGameSceneStatuses.STATUS_NULL;

		// Token: 0x0400450E RID: 17678
		public int SuccessSide = 0;

		// Token: 0x0400450F RID: 17679
		public bool m_bEndFlag = false;

		// Token: 0x04004510 RID: 17680
		public int GameId;

		// Token: 0x04004511 RID: 17681
		public CopyMap CopyMap;

		// Token: 0x04004512 RID: 17682
		public EscapeBattleMatchConfig SceneInfo;

		// Token: 0x04004513 RID: 17683
		public EscapeBattleFuBenData FuBenData;

		// Token: 0x04004514 RID: 17684
		public Dictionary<int, GameClient> ClientDict = new Dictionary<int, GameClient>();

		// Token: 0x04004515 RID: 17685
		public EscapeBattleSideScore ScoreData = new EscapeBattleSideScore();

		// Token: 0x04004516 RID: 17686
		public Dictionary<int, List<EscapeBattleRoleInfo>> ClientContextDataDict = new Dictionary<int, List<EscapeBattleRoleInfo>>();

		// Token: 0x04004517 RID: 17687
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x04004518 RID: 17688
		public List<EscapeBattleCollection> CollectionConfigList = new List<EscapeBattleCollection>();

		// Token: 0x04004519 RID: 17689
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		// Token: 0x0400451A RID: 17690
		public EscapeBattleStatisticalData GameStatisticalData = new EscapeBattleStatisticalData();

		// Token: 0x0400451B RID: 17691
		public double[][] TopClientCalExtProps = new double[2][];

		// Token: 0x0400451C RID: 17692
		public long AreaDamageTicks;

		// Token: 0x0400451D RID: 17693
		public int SafeAreaRefreshState = -1;

		// Token: 0x0400451E RID: 17694
		public int MapGridWidth = 100;

		// Token: 0x0400451F RID: 17695
		public int MapGridHeight = 100;
	}
}
