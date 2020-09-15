using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.UserReturn
{
	// Token: 0x020004B7 RID: 1207
	public class ReturnCheckAwardInfo
	{
		// Token: 0x04002008 RID: 8200
		public int ID = 0;

		// Token: 0x04002009 RID: 8201
		public int LevelMin = 0;

		// Token: 0x0400200A RID: 8202
		public int LevelMax = 0;

		// Token: 0x0400200B RID: 8203
		public int Day = 0;

		// Token: 0x0400200C RID: 8204
		public List<GoodsData> DefaultGoodsList = null;

		// Token: 0x0400200D RID: 8205
		public List<GoodsData> ProGoodsList = null;
	}
}
