using System;
using GameServer.Core.GameEvent;

namespace GameServer.Core.AssemblyPatch
{
	// Token: 0x020000C2 RID: 194
	internal class MethodConfig
	{
		// Token: 0x040004C1 RID: 1217
		public string assemblyName;

		// Token: 0x040004C2 RID: 1218
		public EventTypes eventType;

		// Token: 0x040004C3 RID: 1219
		public string fullClassName;

		// Token: 0x040004C4 RID: 1220
		public string methodName;

		// Token: 0x040004C5 RID: 1221
		public string[] methodParams;

		// Token: 0x040004C6 RID: 1222
		public int cmdID;
	}
}
