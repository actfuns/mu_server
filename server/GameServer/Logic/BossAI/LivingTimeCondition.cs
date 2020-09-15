using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D3 RID: 1491
	public class LivingTimeCondition : ITriggerCondition
	{
		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x001A16C8 File Offset: 0x0019F8C8
		// (set) Token: 0x06001BBA RID: 7098 RVA: 0x001A16DF File Offset: 0x0019F8DF
		public BossAITriggerTypes TriggerType { get; set; }

		// Token: 0x04002A06 RID: 10758
		public long LivingMinutes = 0L;
	}
}
