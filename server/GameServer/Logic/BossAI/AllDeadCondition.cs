using System;
using System.Collections.Generic;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D2 RID: 1490
	public class AllDeadCondition : ITriggerCondition
	{
		// Token: 0x170000BD RID: 189
		
		
		public BossAITriggerTypes TriggerType { get; set; }

		// Token: 0x04002A04 RID: 10756
		public List<int> MonsterIDList = new List<int>();
	}
}
