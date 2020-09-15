using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020007BF RID: 1983
	public class WuXingMapItem
	{
		// Token: 0x04003FA2 RID: 16290
		public int GlobalID = 0;

		// Token: 0x04003FA3 RID: 16291
		public List<int> OtherNPCIDList = new List<int>();

		// Token: 0x04003FA4 RID: 16292
		public List<int> GoToMapCodeList = new List<int>();

		// Token: 0x04003FA5 RID: 16293
		public int DayID = -1;
	}
}
