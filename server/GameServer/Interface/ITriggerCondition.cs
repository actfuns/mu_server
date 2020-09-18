using System;
using GameServer.Logic;

namespace GameServer.Interface
{
	
	public interface ITriggerCondition
	{
		
		
		
		BossAITriggerTypes TriggerType { get; set; }
	}
}
