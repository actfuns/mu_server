using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.UserReturn
{
	// Token: 0x020004B6 RID: 1206
	public class ReturnAwardInfo
	{
		// Token: 0x04002004 RID: 8196
		public int ID = 0;

		// Token: 0x04002005 RID: 8197
		public int Vip = 0;

		// Token: 0x04002006 RID: 8198
		public List<GoodsData> DefaultGoodsList = null;

		// Token: 0x04002007 RID: 8199
		public List<GoodsData> ProGoodsList = null;
	}
}
