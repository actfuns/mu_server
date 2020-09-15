using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005CE RID: 1486
	public class BloodChangedCondition : ITriggerCondition
	{
		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06001BA6 RID: 7078 RVA: 0x001A15B4 File Offset: 0x0019F7B4
		// (set) Token: 0x06001BA7 RID: 7079 RVA: 0x001A15CB File Offset: 0x0019F7CB
		public BossAITriggerTypes TriggerType { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06001BA8 RID: 7080 RVA: 0x001A15D4 File Offset: 0x0019F7D4
		// (set) Token: 0x06001BA9 RID: 7081 RVA: 0x001A15EB File Offset: 0x0019F7EB
		public double MinLifePercent { get; set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06001BAA RID: 7082 RVA: 0x001A15F4 File Offset: 0x0019F7F4
		// (set) Token: 0x06001BAB RID: 7083 RVA: 0x001A160B File Offset: 0x0019F80B
		public double MaxLifePercent { get; set; }
	}
}
