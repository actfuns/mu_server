using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	
	public class LivingTimeCondition : ITriggerCondition
	{
		
		
		
		public BossAITriggerTypes TriggerType { get; set; }

		
		public long LivingMinutes = 0L;
	}
}
