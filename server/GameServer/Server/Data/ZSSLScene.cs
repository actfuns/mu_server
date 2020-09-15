using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	// Token: 0x02000442 RID: 1090
	public class ZSSLScene
	{
		// Token: 0x060013F7 RID: 5111 RVA: 0x0013A448 File Offset: 0x00138648
		public void CleanAllInfo()
		{
			this.State = BattleStates.NoBattle;
			this.StartTick = 0L;
			this.EndTick = 0L;
			this.ClearTick = 0L;
			this.StatusEndTime = 0L;
			this.BossDie = false;
			this.AttackLog = new BossAttackLog
			{
				InjureSum = 0L,
				BHInjure = new Dictionary<long, BHAttackLog>(),
				BHAttackRank = new List<BHAttackLog>()
			};
		}

		// Token: 0x04001D66 RID: 7526
		public ZhuanShengMapInfo SceneInfo;

		// Token: 0x04001D67 RID: 7527
		public CopyMap m_CopyMap;

		// Token: 0x04001D68 RID: 7528
		public BattleStates State;

		// Token: 0x04001D69 RID: 7529
		public long StartTick;

		// Token: 0x04001D6A RID: 7530
		public long EndTick;

		// Token: 0x04001D6B RID: 7531
		public long ClearTick;

		// Token: 0x04001D6C RID: 7532
		public long StatusEndTime;

		// Token: 0x04001D6D RID: 7533
		public bool BossDie;

		// Token: 0x04001D6E RID: 7534
		public BossAttackLog AttackLog;
	}
}
