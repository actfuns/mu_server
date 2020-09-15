using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000335 RID: 821
	public class KingOfBattleDynamicMonsterItem
	{
		// Token: 0x0400155C RID: 5468
		public int Id;

		// Token: 0x0400155D RID: 5469
		public int MapCode;

		// Token: 0x0400155E RID: 5470
		public int MonsterID;

		// Token: 0x0400155F RID: 5471
		public int PosX;

		// Token: 0x04001560 RID: 5472
		public int PosY;

		// Token: 0x04001561 RID: 5473
		public int Num;

		// Token: 0x04001562 RID: 5474
		public int Radius;

		// Token: 0x04001563 RID: 5475
		public int DelayBirthMs;

		// Token: 0x04001564 RID: 5476
		public int PursuitRadius;

		// Token: 0x04001565 RID: 5477
		public int MonsterType;

		// Token: 0x04001566 RID: 5478
		public bool RebornBirth;

		// Token: 0x04001567 RID: 5479
		public int RebornID;

		// Token: 0x04001568 RID: 5480
		public int JiFenDamage;

		// Token: 0x04001569 RID: 5481
		public int JiFenKill;

		// Token: 0x0400156A RID: 5482
		public List<KingOfBattleRandomBuff> RandomBuffList = new List<KingOfBattleRandomBuff>();

		// Token: 0x0400156B RID: 5483
		public int BuffTime;
	}
}
