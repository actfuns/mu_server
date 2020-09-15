using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x020007F8 RID: 2040
	public class YaoSaiBossRunTimeData
	{
		// Token: 0x04004399 RID: 17305
		public object Mutex = new object();

		// Token: 0x0400439A RID: 17306
		public Dictionary<int, YaoSaiBossData> RoleBossCacheDict = new Dictionary<int, YaoSaiBossData>();

		// Token: 0x0400439B RID: 17307
		public Dictionary<int, Dictionary<int, List<YaoSaiBossFightLog>>> BossZhanDouLogDict = new Dictionary<int, Dictionary<int, List<YaoSaiBossFightLog>>>();
	}
}
