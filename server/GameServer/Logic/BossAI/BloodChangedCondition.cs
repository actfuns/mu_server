using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	
	public class BloodChangedCondition : ITriggerCondition
	{
		
		
		
		public BossAITriggerTypes TriggerType { get; set; }

		
		
		
		public double MinLifePercent { get; set; }

		
		
		
		public double MaxLifePercent { get; set; }
	}
}
