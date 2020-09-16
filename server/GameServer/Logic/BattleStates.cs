using System;

namespace GameServer.Logic
{
	
	public enum BattleStates
	{
		
		NoBattle,
		
		PublishMsg,
		
		WaitingFight,
		
		StartFight,
		
		EndFight,
		
		ClearBattle
	}
}
