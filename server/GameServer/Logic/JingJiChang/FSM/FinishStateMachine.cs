using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic.JingJiChang.FSM
{
	
	public class FinishStateMachine
	{
		
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

		
		public void onUpdate()
		{
			long now = TimeUtil.NOW();
			this.currentState.onUpdate(now);
		}

		
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

		
		private Dictionary<AIState, IFSMState> states = new Dictionary<AIState, IFSMState>();

		
		private Robot owner = null;

		
		private IFSMState currentState = null;
	}
}
