using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020004C4 RID: 1220
	public class FazhenMapData
	{
		// Token: 0x04002053 RID: 8275
		public int CopyMapID = 0;

		// Token: 0x04002054 RID: 8276
		public int MapCode = 0;

		// Token: 0x04002055 RID: 8277
		public Dictionary<int, SingleFazhenTelegateData> Telegates = new Dictionary<int, SingleFazhenTelegateData>();
	}
}
