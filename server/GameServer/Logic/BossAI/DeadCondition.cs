using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D0 RID: 1488
	public class DeadCondition : ITriggerCondition
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06001BB0 RID: 7088 RVA: 0x001A1644 File Offset: 0x0019F844
		// (set) Token: 0x06001BB1 RID: 7089 RVA: 0x001A165B File Offset: 0x0019F85B
		public BossAITriggerTypes TriggerType { get; set; }
	}
}
