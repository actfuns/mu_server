using System;

namespace GameServer.Logic.Building
{
	// Token: 0x0200022F RID: 559
	public class BuildingTaskConfigData
	{
		// Token: 0x04000D1A RID: 3354
		public int TaskID = 0;

		// Token: 0x04000D1B RID: 3355
		public int BuildID = 0;

		// Token: 0x04000D1C RID: 3356
		public BuildingQuality quality = BuildingQuality.Null;

		// Token: 0x04000D1D RID: 3357
		public double SumNum = 0.0;

		// Token: 0x04000D1E RID: 3358
		public double ExpNum = 0.0;

		// Token: 0x04000D1F RID: 3359
		public int Time = 0;

		// Token: 0x04000D20 RID: 3360
		public bool RandSkip = false;
	}
}
