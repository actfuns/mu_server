using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x0200043B RID: 1083
	public class BossAttackLog
	{
		// Token: 0x04001D39 RID: 7481
		public long InjureSum;

		// Token: 0x04001D3A RID: 7482
		public Dictionary<long, BHAttackLog> BHInjure;

		// Token: 0x04001D3B RID: 7483
		public List<BHAttackLog> BHAttackRank;
	}
}
