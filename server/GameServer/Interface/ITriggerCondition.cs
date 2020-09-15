using System;
using GameServer.Logic;

namespace GameServer.Interface
{
	// Token: 0x020005B0 RID: 1456
	public interface ITriggerCondition
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06001A5C RID: 6748
		// (set) Token: 0x06001A5D RID: 6749
		BossAITriggerTypes TriggerType { get; set; }
	}
}
