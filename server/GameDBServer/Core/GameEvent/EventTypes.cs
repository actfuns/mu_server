using System;

namespace GameDBServer.Core.GameEvent
{
	
	public enum EventTypes
	{
		
		PlayerLogin,
		
		PlayerLogout,
		
		PlayerOnline,
		
		GameRunning,
		
		BeforeProcessMsg
	}
}
