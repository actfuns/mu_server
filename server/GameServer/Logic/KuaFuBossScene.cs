using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000347 RID: 839
	public class KuaFuBossScene
	{
		// Token: 0x06000E25 RID: 3621 RVA: 0x000E096D File Offset: 0x000DEB6D
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

		// Token: 0x0400160B RID: 5643
		public int m_nMapCode = 0;

		// Token: 0x0400160C RID: 5644
		public int FuBenSeqId = 0;

		// Token: 0x0400160D RID: 5645
		public int CopyMapId = 0;

		// Token: 0x0400160E RID: 5646
		public long StartTimeTicks = 0L;

		// Token: 0x0400160F RID: 5647
		public long m_lPrepareTime = 0L;

		// Token: 0x04001610 RID: 5648
		public long m_lBeginTime = 0L;

		// Token: 0x04001611 RID: 5649
		public long m_lEndTime = 0L;

		// Token: 0x04001612 RID: 5650
		public long m_lLeaveTime = 0L;

		// Token: 0x04001613 RID: 5651
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04001614 RID: 5652
		public int m_nPlarerCount = 0;

		// Token: 0x04001615 RID: 5653
		public int ElapsedSeconds;

		// Token: 0x04001616 RID: 5654
		public bool m_bEndFlag = false;

		// Token: 0x04001617 RID: 5655
		public int GameId;

		// Token: 0x04001618 RID: 5656
		public CopyMap CopyMap;

		// Token: 0x04001619 RID: 5657
		public KuaFuBossSceneInfo SceneInfo;

		// Token: 0x0400161A RID: 5658
		public KuaFuBossStatisticalData GameStatisticalData = new KuaFuBossStatisticalData();

		// Token: 0x0400161B RID: 5659
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x0400161C RID: 5660
		public long NextNotifySceneStateDataTicks = 0L;

		// Token: 0x0400161D RID: 5661
		public KuaFuBossSceneStateData SceneStateData = new KuaFuBossSceneStateData();

		// Token: 0x0400161E RID: 5662
		public List<BattleDynamicMonsterItem> DynMonsterList = null;

		// Token: 0x0400161F RID: 5663
		public HashSet<int> DynMonsterSet = new HashSet<int>();

		// Token: 0x04001620 RID: 5664
		public HashSet<int> KilledMonsterSet = new HashSet<int>();

		// Token: 0x04001621 RID: 5665
		public int MapGridWidth = 100;

		// Token: 0x04001622 RID: 5666
		public int MapGridHeight = 100;
	}
}
