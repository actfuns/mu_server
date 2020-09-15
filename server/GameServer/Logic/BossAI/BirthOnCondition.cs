using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005CD RID: 1485
	public class BirthOnCondition : ITriggerCondition
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06001BA3 RID: 7075 RVA: 0x001A158C File Offset: 0x0019F78C
		// (set) Token: 0x06001BA4 RID: 7076 RVA: 0x001A15A3 File Offset: 0x0019F7A3
		public BossAITriggerTypes TriggerType { get; set; }
	}
}
