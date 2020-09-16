using System;

namespace GameServer.Logic.JingJiChang
{
	
	public enum JingJiFuBenState
	{
		
		INITIALIZED,
		
		WAITING_CHANGEMAP_FINISH,
		
		START_CD,
		
		STARTED,
		
		STOP_CD,
		
		STOP_TIMEOUT_CD,
		
		STOPED,
		
		DESTROYED
	}
}
