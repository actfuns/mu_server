using System;

namespace GameServer.Logic.ActivityNew
{
	
	public enum JieriLianXuChargeErrorCode
	{
		
		Success,
		
		InvalidParam,
		
		ActivityNotOpen,
		
		NotAwardTime,
		
		DBFailed,
		
		ConfigError,
		
		NoBagSpace,
		
		NotMeetAwardCond
	}
}
