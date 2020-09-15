using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent
{
	// Token: 0x02000107 RID: 263
	public class TimerEventObject : EventObject
	{
		// Token: 0x06000404 RID: 1028 RVA: 0x0003DF8F File Offset: 0x0003C18F
		public TimerEventObject() : base(57)
		{
		}

		// Token: 0x04000590 RID: 1424
		public long LastRunTicks;

		// Token: 0x04000591 RID: 1425
		public long DeltaTicks;

		// Token: 0x04000592 RID: 1426
		public long NowTicks;

		// Token: 0x04000593 RID: 1427
		public GameClient Client;
	}
}
