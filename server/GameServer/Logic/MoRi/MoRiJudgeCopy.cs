using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic.MoRi
{
	// Token: 0x02000379 RID: 889
	internal class MoRiJudgeCopy
	{
		// Token: 0x04001771 RID: 6001
		public CopyMap MyCopyMap;

		// Token: 0x04001772 RID: 6002
		public long GameId;

		// Token: 0x04001773 RID: 6003
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04001774 RID: 6004
		public long DeadlineMs = 0L;

		// Token: 0x04001775 RID: 6005
		public long CurrStateBeginMs = 0L;

		// Token: 0x04001776 RID: 6006
		public int CurrMonsterIdx = -1;

		// Token: 0x04001777 RID: 6007
		public long CurrMonsterBegin = 0L;

		// Token: 0x04001778 RID: 6008
		public List<MoRiMonsterData> MonsterList = new List<MoRiMonsterData>();

		// Token: 0x04001779 RID: 6009
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x0400177A RID: 6010
		public DateTime StartTime;

		// Token: 0x0400177B RID: 6011
		public DateTime EndTime;

		// Token: 0x0400177C RID: 6012
		public int LimitKillCount = 0;

		// Token: 0x0400177D RID: 6013
		public int RoleCount;

		// Token: 0x0400177E RID: 6014
		public bool Passed;
	}
}
