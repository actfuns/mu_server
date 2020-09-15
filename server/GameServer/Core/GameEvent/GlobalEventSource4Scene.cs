using System;

namespace GameServer.Core.GameEvent
{
	// Token: 0x02000106 RID: 262
	public class GlobalEventSource4Scene : SceneEventSource
	{
		// Token: 0x06000401 RID: 1025 RVA: 0x0003DF60 File Offset: 0x0003C160
		private GlobalEventSource4Scene()
		{
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0003DF6C File Offset: 0x0003C16C
		public static GlobalEventSource4Scene getInstance()
		{
			return GlobalEventSource4Scene.instance;
		}

		// Token: 0x0400058F RID: 1423
		private static GlobalEventSource4Scene instance = new GlobalEventSource4Scene();
	}
}
