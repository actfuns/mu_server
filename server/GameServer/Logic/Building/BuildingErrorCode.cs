using System;

namespace GameServer.Logic.Building
{
	
	public enum BuildingErrorCode
	{
		
		Success,
		
		ErrorAllLevelAwarded,
		
		ErrorAllLevel,
		
		ErrorPayTeamMaxOver,
		
		ErrorNoTaskFinish,
		
		ErrorBQueueNotEnough,
		
		ErrorInBuilding,
		
		ErrorConfig,
		
		ErrorParams,
		
		ZuanShiNotEnough,
		
		DBFailed,
		
		ErrorIsNotOpen,
		
		ErrorBuildNotFind,
		
		ErrorTaskNotFind,
		
		ErrorBagNotEnough,
		
		NengLiangNotEnough
	}
}
