using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D3 RID: 1491
	public class LivingTimeCondition : ITriggerCondition
	{
		// Token: 0x170000BE RID: 190
		
		
		public BossAITriggerTypes TriggerType { get; set; }

		// Token: 0x04002A06 RID: 10758
		public long LivingMinutes = 0L;
	}
}
