using System;

namespace GameServer.Logic
{
	// Token: 0x0200004E RID: 78
	public interface IManager
	{
		// Token: 0x060000EB RID: 235
		bool initialize();

		// Token: 0x060000EC RID: 236
		bool startup();

		// Token: 0x060000ED RID: 237
		bool showdown();

		// Token: 0x060000EE RID: 238
		bool destroy();
	}
}
