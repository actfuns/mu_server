using System;
using GameServer.Interface;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D4 RID: 1492
	public class BossAIItem
	{
		// Token: 0x170000BF RID: 191
		
		
		public int ID { get; set; }

		// Token: 0x170000C0 RID: 192
		
		
		public int AIID { get; set; }

		// Token: 0x170000C1 RID: 193
		
		
		public int TriggerNum { get; set; }

		// Token: 0x170000C2 RID: 194
		
		
		public int TriggerCD { get; set; }

		// Token: 0x170000C3 RID: 195
		
		
		public int TriggerType { get; set; }

		// Token: 0x170000C4 RID: 196
		
		
		public ITriggerCondition Condition { get; set; }

		// Token: 0x170000C5 RID: 197
		
		
		public string Desc { get; set; }
	}
}
