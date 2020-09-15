using System;

namespace GameDBServer.Core.GameEvent
{
	// Token: 0x02000024 RID: 36
	public class GlobalEventSource : EventSource
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00004CF0 File Offset: 0x00002EF0
		private GlobalEventSource()
		{
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00004CFC File Offset: 0x00002EFC
		public static GlobalEventSource getInstance()
		{
			return GlobalEventSource.instance;
		}

		// Token: 0x04000065 RID: 101
		private static GlobalEventSource instance = new GlobalEventSource();
	}
}
