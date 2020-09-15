using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.UserReturn
{
	// Token: 0x020004B1 RID: 1201
	public class RecallAwardInfo
	{
		// Token: 0x04001FDC RID: 8156
		public int ID = 0;

		// Token: 0x04001FDD RID: 8157
		public int Level = 0;

		// Token: 0x04001FDE RID: 8158
		public int Vip = 0;

		// Token: 0x04001FDF RID: 8159
		public int Count = 0;

		// Token: 0x04001FE0 RID: 8160
		public List<GoodsData> DefaultGoodsList = null;

		// Token: 0x04001FE1 RID: 8161
		public List<GoodsData> ProGoodsList = null;
	}
}
