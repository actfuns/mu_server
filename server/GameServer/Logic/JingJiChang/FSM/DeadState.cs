using System;

namespace GameServer.Logic.JingJiChang.FSM
{
	
	internal class DeadState : IFSMState
	{
		
		public DeadState(Robot owner, FinishStateMachine FSM)
		{
			this.owner = owner;
			this.FSM = FSM;
		}

		
		public void onBegin()
		{
		}

		
		public void onEnd()
		{
		}

		
		public void onUpdate(long now)
		{
		}

		
		public static readonly AIState state = AIState.DEAD;

		
		private Robot owner = null;

		
		private FinishStateMachine FSM = null;
	}
}
