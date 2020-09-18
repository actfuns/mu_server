using System;
using System.Collections.Generic;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	
	public class AllDeadCondition : ITriggerCondition
	{
		
		
		
		public BossAITriggerTypes TriggerType { get; set; }

		
		public List<int> MonsterIDList = new List<int>();
	}
}
