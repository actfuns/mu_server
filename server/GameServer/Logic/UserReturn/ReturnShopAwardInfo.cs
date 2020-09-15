using System;
using Server.Data;

namespace GameServer.Logic.UserReturn
{
	// Token: 0x020004B8 RID: 1208
	public class ReturnShopAwardInfo
	{
		// Token: 0x0400200E RID: 8206
		public int ID = 0;

		// Token: 0x0400200F RID: 8207
		public GoodsData Goods = null;

		// Token: 0x04002010 RID: 8208
		public int OldPrice = 0;

		// Token: 0x04002011 RID: 8209
		public int NewPrice = 0;

		// Token: 0x04002012 RID: 8210
		public int LimitCount = 0;
	}
}
