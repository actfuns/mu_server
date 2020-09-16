using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005CE RID: 1486
	public class BloodChangedCondition : ITriggerCondition
	{
		// Token: 0x170000B7 RID: 183
		
		
		public BossAITriggerTypes TriggerType { get; set; }

		// Token: 0x170000B8 RID: 184
		
		
		public double MinLifePercent { get; set; }

		// Token: 0x170000B9 RID: 185
		
		
		public double MaxLifePercent { get; set; }
	}
}
