using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	
	public class BossAIItem
	{
		
		
		
		public int ID { get; set; }

		
		
		
		public int AIID { get; set; }

		
		
		
		public int TriggerNum { get; set; }

		
		
		
		public int TriggerCD { get; set; }

		
		
		
		public int TriggerType { get; set; }

		
		
		
		public ITriggerCondition Condition { get; set; }

		
		
		
		public string Desc { get; set; }
	}
}
