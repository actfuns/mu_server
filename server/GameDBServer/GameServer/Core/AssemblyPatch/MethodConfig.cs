using System;
using GameDBServer.Core.GameEvent;

namespace GameServer.Core.AssemblyPatch
{
	// Token: 0x02000004 RID: 4
	internal class MethodConfig
	{
		// Token: 0x04000006 RID: 6
		public string assemblyName;

		// Token: 0x04000007 RID: 7
		public EventTypes eventType;

		// Token: 0x04000008 RID: 8
		public string fullClassName;

		// Token: 0x04000009 RID: 9
		public string methodName;

		// Token: 0x0400000A RID: 10
		public string[] methodParams;

		// Token: 0x0400000B RID: 11
		public int cmdID;
	}
}
