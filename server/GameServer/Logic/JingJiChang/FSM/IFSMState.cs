using System;

namespace GameServer.Logic.JingJiChang.FSM
{
	// Token: 0x02000725 RID: 1829
	internal interface IFSMState
	{
		// Token: 0x06002C74 RID: 11380
		void onBegin();

		// Token: 0x06002C75 RID: 11381
		void onEnd();

		// Token: 0x06002C76 RID: 11382
		void onUpdate(long now);
	}
}
