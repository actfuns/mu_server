using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020004C8 RID: 1224
	public class SystemFazhenMapData
	{
		// Token: 0x0400205F RID: 8287
		public int MapCode = 0;

		// Token: 0x04002060 RID: 8288
		public List<int> listGateID = new List<int>();

		// Token: 0x04002061 RID: 8289
		public List<int> listDestMapCode = new List<int>();

		// Token: 0x04002062 RID: 8290
		public int SpecialDestMapCode = 0;

		// Token: 0x04002063 RID: 8291
		public int SpecialDestX = 0;

		// Token: 0x04002064 RID: 8292
		public int SpecialDestY = 0;
	}
}
