using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000825 RID: 2085
	public class ZhanDuiZhengBaScene
	{
		// Token: 0x06003AF3 RID: 15091 RVA: 0x00320A76 File Offset: 0x0031EC76
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
		}

		// Token: 0x04004520 RID: 17696
		public int m_nMapCode = 0;

		// Token: 0x04004521 RID: 17697
		public int FuBenSeqId = 0;

		// Token: 0x04004522 RID: 17698
		public int CopyMapId = 0;

		// Token: 0x04004523 RID: 17699
		public long m_lPrepareTime = 0L;

		// Token: 0x04004524 RID: 17700
		public long m_lBeginTime = 0L;

		// Token: 0x04004525 RID: 17701
		public long m_lEndTime = 0L;

		// Token: 0x04004526 RID: 17702
		public long m_lLeaveTime = 0L;

		// Token: 0x04004527 RID: 17703
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04004528 RID: 17704
		public int SuccessSide = 0;

		// Token: 0x04004529 RID: 17705
		public bool m_bEndFlag = false;

		// Token: 0x0400452A RID: 17706
		public ZhanDuiZhengBaMatchConfig SceneConfig;

		// Token: 0x0400452B RID: 17707
		public int GameId;

		// Token: 0x0400452C RID: 17708
		public CopyMap CopyMap;

		// Token: 0x0400452D RID: 17709
		public Dictionary<int, GameClient> ClientDict = new Dictionary<int, GameClient>();

		// Token: 0x0400452E RID: 17710
		public Dictionary<int, TianTi5v5RoleMiniData> RoleIdDuanWeiIdDict = new Dictionary<int, TianTi5v5RoleMiniData>();

		// Token: 0x0400452F RID: 17711
		public List<Tuple<TianTi5v5ZhanDuiData, int>> ZhanDuiDataDict = new List<Tuple<TianTi5v5ZhanDuiData, int>>();

		// Token: 0x04004530 RID: 17712
		public ZhanDuiZhengBaFuBenData FuBenData;

		// Token: 0x04004531 RID: 17713
		public Dictionary<int, Tuple<int, bool>> RoleSideStateDict = new Dictionary<int, Tuple<int, bool>>();

		// Token: 0x04004532 RID: 17714
		public int LastLeaveZhanDuiID = -1;

		// Token: 0x04004533 RID: 17715
		public ZhanDuiZhengBaScoreInfoData ScoreInfoData = new ZhanDuiZhengBaScoreInfoData();

		// Token: 0x04004534 RID: 17716
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();
	}
}
