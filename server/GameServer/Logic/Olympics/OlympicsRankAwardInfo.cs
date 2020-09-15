using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.Olympics
{
	// Token: 0x02000396 RID: 918
	public class OlympicsRankAwardInfo
	{
		// Token: 0x04001828 RID: 6184
		public int ID = 0;

		// Token: 0x04001829 RID: 6185
		public string Content = "";

		// Token: 0x0400182A RID: 6186
		public int RankBegin = 0;

		// Token: 0x0400182B RID: 6187
		public int RankEnd = 0;

		// Token: 0x0400182C RID: 6188
		public List<GoodsData> DefaultGoodsList = null;
	}
}
