using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006F3 RID: 1779
	public class UpLevelItem
	{
		// Token: 0x040039F8 RID: 14840
		public int ID = 0;

		// Token: 0x040039F9 RID: 14841
		public int ToLevel = 0;

		// Token: 0x040039FA RID: 14842
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x040039FB RID: 14843
		public int BindYuanBao = 0;

		// Token: 0x040039FC RID: 14844
		public int BindMoney = 0;

		// Token: 0x040039FD RID: 14845
		public int MoJing = 0;

		// Token: 0x040039FE RID: 14846
		public int Occupation = -1;
	}
}
