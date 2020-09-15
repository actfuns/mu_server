using System;

namespace GameServer.Logic.JingJiChang.FSM
{
	// Token: 0x02000728 RID: 1832
	internal class DeadState : IFSMState
	{
		// Token: 0x06002C8A RID: 11402 RVA: 0x0027C742 File Offset: 0x0027A942
		public DeadState(Robot owner, FinishStateMachine FSM)
		{
			this.owner = owner;
			this.FSM = FSM;
		}

		// Token: 0x06002C8B RID: 11403 RVA: 0x0027C769 File Offset: 0x0027A969
		public void onBegin()
		{
		}

		// Token: 0x06002C8C RID: 11404 RVA: 0x0027C76C File Offset: 0x0027A96C
		public void onEnd()
		{
		}

		// Token: 0x06002C8D RID: 11405 RVA: 0x0027C76F File Offset: 0x0027A96F
		public void onUpdate(long now)
		{
		}

		// Token: 0x04003B1B RID: 15131
		public static readonly AIState state = AIState.DEAD;

		// Token: 0x04003B1C RID: 15132
		private Robot owner = null;

		// Token: 0x04003B1D RID: 15133
		private FinishStateMachine FSM = null;
	}
}
