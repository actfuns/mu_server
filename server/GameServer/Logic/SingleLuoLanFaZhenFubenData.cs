using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020004C7 RID: 1223
	public class SingleLuoLanFaZhenFubenData
	{
		// Token: 0x0400205A RID: 8282
		public int FubenID = 0;

		// Token: 0x0400205B RID: 8283
		public int FubenSeqID = 0;

		// Token: 0x0400205C RID: 8284
		public int FinalBossLeftNum = 1;

		// Token: 0x0400205D RID: 8285
		public int SpecailBossLeftNum = 1;

		// Token: 0x0400205E RID: 8286
		public Dictionary<int, FazhenMapData> MapDatas = new Dictionary<int, FazhenMapData>();
	}
}
