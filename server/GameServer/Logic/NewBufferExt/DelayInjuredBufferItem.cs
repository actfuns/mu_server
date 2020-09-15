using System;
using GameServer.Core.Executor;
using GameServer.Interface;

namespace GameServer.Logic.NewBufferExt
{
	// Token: 0x02000537 RID: 1335
	public class DelayInjuredBufferItem : IBufferItem
	{
		// Token: 0x04002391 RID: 9105
		public int ObjectID = 0;

		// Token: 0x04002392 RID: 9106
		public int TimeSlotSecs = 0;

		// Token: 0x04002393 RID: 9107
		public int SubLifeV = 0;

		// Token: 0x04002394 RID: 9108
		public long StartSubLifeNoShowTicks = TimeUtil.NOW();
	}
}
