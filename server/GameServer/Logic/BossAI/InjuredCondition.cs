using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005CF RID: 1487
	public class InjuredCondition : ITriggerCondition
	{
		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06001BAD RID: 7085 RVA: 0x001A161C File Offset: 0x0019F81C
		// (set) Token: 0x06001BAE RID: 7086 RVA: 0x001A1633 File Offset: 0x0019F833
		public BossAITriggerTypes TriggerType { get; set; }
	}
}
