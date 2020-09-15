using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000492 RID: 1170
	public class TianTiScene
	{
		// Token: 0x0600153E RID: 5438 RVA: 0x0014CAC4 File Offset: 0x0014ACC4
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

		// Token: 0x04001EF7 RID: 7927
		public int m_nMapCode = 0;

		// Token: 0x04001EF8 RID: 7928
		public int FuBenSeqId = 0;

		// Token: 0x04001EF9 RID: 7929
		public int CopyMapId = 0;

		// Token: 0x04001EFA RID: 7930
		public long m_lPrepareTime = 0L;

		// Token: 0x04001EFB RID: 7931
		public long m_lBeginTime = 0L;

		// Token: 0x04001EFC RID: 7932
		public long m_lEndTime = 0L;

		// Token: 0x04001EFD RID: 7933
		public long m_lLeaveTime = 0L;

		// Token: 0x04001EFE RID: 7934
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04001EFF RID: 7935
		public int m_nPlarerCount = 0;

		// Token: 0x04001F00 RID: 7936
		public int SuccessSide = 0;

		// Token: 0x04001F01 RID: 7937
		public bool m_bEndFlag = false;

		// Token: 0x04001F02 RID: 7938
		public int GameId;

		// Token: 0x04001F03 RID: 7939
		public CopyMap CopyMap;

		// Token: 0x04001F04 RID: 7940
		public Dictionary<int, TianTiRoleMiniData> RoleIdDuanWeiIdDict = new Dictionary<int, TianTiRoleMiniData>();

		// Token: 0x04001F05 RID: 7941
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();
	}
}
