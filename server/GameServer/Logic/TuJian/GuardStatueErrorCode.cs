using System;

namespace GameServer.Logic.TuJian
{
	
	public enum GuardStatueErrorCode
	{
		
		Success,
		
		NotOpen,
		
		ContainNotActiveTuJian,
		
		MoreThanTodayCanRecover,
		
		GuardPointNotEnough,
		
		MaterialNotEnough,
		
		NeedSuitUp,
		
		NeedLevelUp,
		
		SuitIsFull,
		
		LevelIsFull,
		
		ConfigError,
		
		DBFailed
	}
}
