using System;

namespace GameServer.Logic
{
	
	public enum EMagicTargetType
	{
		
		EMTT_Self = 1,
		
		EMTT_SelfAndTeam,
		
		EMTT_Enemy,
		
		EMTT_SelfOrTarget
	}
}
