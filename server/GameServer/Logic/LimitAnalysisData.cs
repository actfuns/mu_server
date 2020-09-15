using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000241 RID: 577
	public class LimitAnalysisData
	{
		// Token: 0x04000DC6 RID: 3526
		public DateTime Timestamp = TimeUtil.NowDateTime();

		// Token: 0x04000DC7 RID: 3527
		public Dictionary<string, int> dict = new Dictionary<string, int>();
	}
}
