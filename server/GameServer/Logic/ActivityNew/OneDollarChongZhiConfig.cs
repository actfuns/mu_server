using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000048 RID: 72
	public class OneDollarChongZhiConfig
	{
		// Token: 0x0400018D RID: 397
		public int ID = 0;

		// Token: 0x0400018E RID: 398
		public DateTime FromDate;

		// Token: 0x0400018F RID: 399
		public DateTime ToDate;

		// Token: 0x04000190 RID: 400
		public string UserListFile = "";

		// Token: 0x04000191 RID: 401
		public HashSet<string> UserListSet = new HashSet<string>();

		// Token: 0x04000192 RID: 402
		public int MinZuanShi = 0;

		// Token: 0x04000193 RID: 403
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		// Token: 0x04000194 RID: 404
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();
	}
}
