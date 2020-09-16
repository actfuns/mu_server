using System;

namespace GameServer.Logic
{
	
	public enum MarryResult
	{
		
		Success,
		
		SelfMarried,
		
		SelfLevelNotEnough,
		
		SelfBusy,
		
		TargetMarried,
		
		TargetBusy,
		
		TargetLevelNotEnough,
		
		TargetOffline,
		
		InvalidSex,
		
		ApplyCD,
		
		ApplyTimeout,
		
		MoneyNotEnough,
		
		NotMarried,
		
		AutoReject,
		
		NotOpen,
		
		TargetNotOpen,
		
		DeniedByCoupleAreanTime,
		
		Error_Denied_For_Minor_Occupation = -35
	}
}
