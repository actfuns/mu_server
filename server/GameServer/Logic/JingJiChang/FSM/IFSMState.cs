using System;

namespace GameServer.Logic.JingJiChang.FSM
{
	
	internal interface IFSMState
	{
		
		void onBegin();

		
		void onEnd();

		
		void onUpdate(long now);
	}
}
