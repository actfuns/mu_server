using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic.JingJiChang.FSM
{
	// Token: 0x0200072A RID: 1834
	public class FinishStateMachine
	{
		// Token: 0x06002C95 RID: 11413 RVA: 0x0027C8DC File Offset: 0x0027AADC
		public FinishStateMachine(GameClient player, Robot owner)
		{
			this.owner = owner;
			IFSMState attackState = new AttackState(player, owner, this);
			IFSMState deadState = new DeadState(owner, this);
			IFSMState returnState = new ReturnState(owner, this);
			IFSMState normalState = new NormalState(owner, this);
			this.states.Add(AIState.ATTACK, attackState);
			this.states.Add(AIState.DEAD, deadState);
			this.states.Add(AIState.RETURN, returnState);
			this.states.Add(AIState.NORMAL, normalState);
			this.currentState = normalState;
		}

		// Token: 0x06002C96 RID: 11414 RVA: 0x0027C974 File Offset: 0x0027AB74
		public void onUpdate()
		{
			long now = TimeUtil.NOW();
			this.currentState.onUpdate(now);
		}

		// Token: 0x06002C97 RID: 11415 RVA: 0x0027C998 File Offset: 0x0027AB98
		public void switchState(AIState state)
		{
			IFSMState fsmState = null;
			if (this.states.TryGetValue(state, out fsmState))
			{
				if (fsmState != this.currentState)
				{
					this.currentState.onEnd();
					this.currentState = fsmState;
					fsmState.onBegin();
				}
			}
		}

		// Token: 0x04003B21 RID: 15137
		private Dictionary<AIState, IFSMState> states = new Dictionary<AIState, IFSMState>();

		// Token: 0x04003B22 RID: 15138
		private Robot owner = null;

		// Token: 0x04003B23 RID: 15139
		private IFSMState currentState = null;
	}
}
