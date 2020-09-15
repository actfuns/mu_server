using System;

namespace LogDBServer.Core.GameEvent
{
	// Token: 0x0200000C RID: 12
	public class GlobalEventSource : EventSource
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002A18 File Offset: 0x00000C18
		private GlobalEventSource()
		{
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002A24 File Offset: 0x00000C24
		public static GlobalEventSource getInstance()
		{
			return GlobalEventSource.instance;
		}

		// Token: 0x04000019 RID: 25
		private static GlobalEventSource instance = new GlobalEventSource();
	}
}
