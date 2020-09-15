using System;

namespace LogDBServer.Logic
{
	// Token: 0x02000013 RID: 19
	public interface IManager
	{
		// Token: 0x06000047 RID: 71
		bool initialize();

		// Token: 0x06000048 RID: 72
		bool startup();

		// Token: 0x06000049 RID: 73
		bool showdown();

		// Token: 0x0600004A RID: 74
		bool destroy();
	}
}
