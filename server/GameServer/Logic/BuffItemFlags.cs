using System;

namespace GameServer.Logic
{
	// Token: 0x02000062 RID: 98
	public struct BuffItemFlags
	{
		// Token: 0x0400022C RID: 556
		public const int UpdateByTime = 1;

		// Token: 0x0400022D RID: 557
		public const int UpdateByVip = 2;

		// Token: 0x0400022E RID: 558
		public const int UpdateByMapCode = 4;

		// Token: 0x0400022F RID: 559
		public const int NotifyUpdateProps = 268435456;

		// Token: 0x04000230 RID: 560
		public const int CustomMessage = 1073741824;

		// Token: 0x04000231 RID: 561
		public bool isUpdateByTime;

		// Token: 0x04000232 RID: 562
		public bool isUpdateByVip;

		// Token: 0x04000233 RID: 563
		public bool isUpdateByMapCode;
	}
}
