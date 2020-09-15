using System;

namespace GameDBServer.Logic
{
	// Token: 0x02000006 RID: 6
	public interface IManager
	{
		// Token: 0x0600000E RID: 14
		bool initialize();

		// Token: 0x0600000F RID: 15
		bool startup();

		// Token: 0x06000010 RID: 16
		bool showdown();

		// Token: 0x06000011 RID: 17
		bool destroy();
	}
}
