using System;

namespace GameServer.Logic.ActivityNew
{
	
	public enum JieriGiveErrorCode
	{
		
		Success,
		
		ActivityNotOpen,
		
		NotAwardTime,
		
		GoodsIDError,
		
		GoodsNotEnough,
		
		ReceiverNotExist,
		
		ReceiverCannotSelf,
		
		DBFailed,
		
		ConfigError,
		
		NoBagSpace,
		
		NotMeetAwardCond
	}
}
