using System;

namespace GameServer.Core.GameEvent
{
	// Token: 0x02000104 RID: 260
	public class GlobalEventSource : EventSource
	{
		// Token: 0x060003F9 RID: 1017 RVA: 0x0003DC73 File Offset: 0x0003BE73
		private GlobalEventSource()
		{
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0003DC80 File Offset: 0x0003BE80
		public static GlobalEventSource getInstance()
		{
			return GlobalEventSource.instance;
		}

		// Token: 0x0400058D RID: 1421
		private static GlobalEventSource instance = new GlobalEventSource();
	}
}
