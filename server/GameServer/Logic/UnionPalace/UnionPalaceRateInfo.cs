using System;
using System.Collections.Generic;

namespace GameServer.Logic.UnionPalace
{
	// Token: 0x0200049E RID: 1182
	public class UnionPalaceRateInfo
	{
		// Token: 0x04001F5C RID: 8028
		public int StatueLevel = 0;

		// Token: 0x04001F5D RID: 8029
		public List<int> RateList = new List<int>();

		// Token: 0x04001F5E RID: 8030
		public Dictionary<int, List<int>> AddNumList = new Dictionary<int, List<int>>();
	}
}
