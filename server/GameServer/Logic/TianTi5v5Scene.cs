using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200042D RID: 1069
	public class TianTi5v5Scene
	{
		// Token: 0x06001391 RID: 5009 RVA: 0x00135897 File Offset: 0x00133A97
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

		// Token: 0x04001CC7 RID: 7367
		public int m_nMapCode = 0;

		// Token: 0x04001CC8 RID: 7368
		public int FuBenSeqId = 0;

		// Token: 0x04001CC9 RID: 7369
		public int CopyMapId = 0;

		// Token: 0x04001CCA RID: 7370
		public long m_lPrepareTime = 0L;

		// Token: 0x04001CCB RID: 7371
		public long m_lBeginTime = 0L;

		// Token: 0x04001CCC RID: 7372
		public long m_lEndTime = 0L;

		// Token: 0x04001CCD RID: 7373
		public long m_lLeaveTime = 0L;

		// Token: 0x04001CCE RID: 7374
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04001CCF RID: 7375
		public int m_nPlarerCount = 0;

		// Token: 0x04001CD0 RID: 7376
		public int SuccessSide = 0;

		// Token: 0x04001CD1 RID: 7377
		public bool m_bEndFlag = false;

		// Token: 0x04001CD2 RID: 7378
		public int GameId;

		// Token: 0x04001CD3 RID: 7379
		public CopyMap CopyMap;

		// Token: 0x04001CD4 RID: 7380
		public Dictionary<int, GameClient> ClientDict = new Dictionary<int, GameClient>();

		// Token: 0x04001CD5 RID: 7381
		public Dictionary<int, TianTi5v5RoleMiniData> RoleIdDuanWeiIdDict = new Dictionary<int, TianTi5v5RoleMiniData>();

		// Token: 0x04001CD6 RID: 7382
		public List<Tuple<TianTi5v5ZhanDuiData, int>> ZhanDuiDataDict = new List<Tuple<TianTi5v5ZhanDuiData, int>>();

		// Token: 0x04001CD7 RID: 7383
		public KuaFu5v5FuBenData FuBenData;

		// Token: 0x04001CD8 RID: 7384
		public Dictionary<int, Tuple<int, bool>> RoleSideStateDict = new Dictionary<int, Tuple<int, bool>>();

		// Token: 0x04001CD9 RID: 7385
		public int LastLeaveZhanDuiID = -1;

		// Token: 0x04001CDA RID: 7386
		public KuaFu5v5ScoreInfoData ScoreInfoData = new KuaFu5v5ScoreInfoData();

		// Token: 0x04001CDB RID: 7387
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();
	}
}
