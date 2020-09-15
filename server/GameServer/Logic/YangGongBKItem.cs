using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020007EB RID: 2027
	public class YangGongBKItem
	{
		// Token: 0x04004336 RID: 17206
		public List<FallGoodsItem> FallGoodsItemList = null;

		// Token: 0x04004337 RID: 17207
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x04004338 RID: 17208
		public List<GoodsData> TempGoodsDataList = null;

		// Token: 0x04004339 RID: 17209
		public int FreeRefreshNum = 0;

		// Token: 0x0400433A RID: 17210
		public int ClickBKNum = 0;

		// Token: 0x0400433B RID: 17211
		public Dictionary<int, bool> PickUpDict = new Dictionary<int, bool>();

		// Token: 0x0400433C RID: 17212
		public bool IsBaoWuBinding = false;
	}
}
