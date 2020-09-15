using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D1 RID: 1489
	public class AttackedCondition : ITriggerCondition
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06001BB3 RID: 7091 RVA: 0x001A166C File Offset: 0x0019F86C
		// (set) Token: 0x06001BB4 RID: 7092 RVA: 0x001A1683 File Offset: 0x0019F883
		public BossAITriggerTypes TriggerType { get; set; }
	}
}
