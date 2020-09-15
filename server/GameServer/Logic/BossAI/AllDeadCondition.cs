using System;
using System.Collections.Generic;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D2 RID: 1490
	public class AllDeadCondition : ITriggerCondition
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06001BB6 RID: 7094 RVA: 0x001A1694 File Offset: 0x0019F894
		// (set) Token: 0x06001BB7 RID: 7095 RVA: 0x001A16AB File Offset: 0x0019F8AB
		public BossAITriggerTypes TriggerType { get; set; }

		// Token: 0x04002A04 RID: 10756
		public List<int> MonsterIDList = new List<int>();
	}
}
