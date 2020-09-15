using System;
using System.Collections.Generic;

namespace GameServer.Logic.Building
{
	// Token: 0x0200022E RID: 558
	public class BuildingConfigData
	{
		// Token: 0x04000D16 RID: 3350
		public int BuildID = 0;

		// Token: 0x04000D17 RID: 3351
		public int MaxLevel = 0;

		// Token: 0x04000D18 RID: 3352
		public List<BuildingRandomData> FreeRandomList = new List<BuildingRandomData>();

		// Token: 0x04000D19 RID: 3353
		public List<BuildingRandomData> RandomList = new List<BuildingRandomData>();
	}
}
