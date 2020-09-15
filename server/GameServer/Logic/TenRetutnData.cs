using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000434 RID: 1076
	public class TenRetutnData
	{
		// Token: 0x04001D1C RID: 7452
		public object Mutex = new object();

		// Token: 0x04001D1D RID: 7453
		public Dictionary<int, TenRetutnAwardsData> _tenAwardDic = new Dictionary<int, TenRetutnAwardsData>();

		// Token: 0x04001D1E RID: 7454
		public bool SystemOpen;
	}
}
