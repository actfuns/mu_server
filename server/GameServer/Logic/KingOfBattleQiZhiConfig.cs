using System;

namespace GameServer.Logic
{
	// Token: 0x02000331 RID: 817
	public class KingOfBattleQiZhiConfig
	{
		// Token: 0x06000DB1 RID: 3505 RVA: 0x000D6FF8 File Offset: 0x000D51F8
		public object Clone()
		{
			return new KingOfBattleQiZhiConfig
			{
				NPCID = this.NPCID,
				PosX = this.PosX,
				PosY = this.PosY,
				QiZhiMonsterID = this.QiZhiMonsterID
			};
		}

		// Token: 0x04001545 RID: 5445
		public int NPCID;

		// Token: 0x04001546 RID: 5446
		public int PosX;

		// Token: 0x04001547 RID: 5447
		public int PosY;

		// Token: 0x04001548 RID: 5448
		public int QiZhiMonsterID;

		// Token: 0x04001549 RID: 5449
		public int BattleWhichSide;

		// Token: 0x0400154A RID: 5450
		public bool Alive;

		// Token: 0x0400154B RID: 5451
		public long DeadTicks;
	}
}
