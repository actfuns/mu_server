using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200032F RID: 815
	public class KingOfBattleSceneInfo
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000DAE RID: 3502 RVA: 0x000D6F48 File Offset: 0x000D5148
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs + 120;
			}
		}

		// Token: 0x0400152B RID: 5419
		public int Id;

		// Token: 0x0400152C RID: 5420
		public int MapCode;

		// Token: 0x0400152D RID: 5421
		public int MinZhuanSheng = 1;

		// Token: 0x0400152E RID: 5422
		public int MinLevel = 1;

		// Token: 0x0400152F RID: 5423
		public int MaxZhuanSheng = 1;

		// Token: 0x04001530 RID: 5424
		public int MaxLevel = 1;

		// Token: 0x04001531 RID: 5425
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		// Token: 0x04001532 RID: 5426
		public List<double> SecondsOfDay = new List<double>();

		// Token: 0x04001533 RID: 5427
		public int WaitingEnterSecs;

		// Token: 0x04001534 RID: 5428
		public int PrepareSecs;

		// Token: 0x04001535 RID: 5429
		public int FightingSecs;

		// Token: 0x04001536 RID: 5430
		public int ClearRolesSecs;

		// Token: 0x04001537 RID: 5431
		public int SignUpStartSecs;

		// Token: 0x04001538 RID: 5432
		public int SignUpEndSecs;

		// Token: 0x04001539 RID: 5433
		public long Exp;

		// Token: 0x0400153A RID: 5434
		public int BandJinBi;

		// Token: 0x0400153B RID: 5435
		public int AwardMinZhuanSheng = 1;

		// Token: 0x0400153C RID: 5436
		public int AwardMinLevel = 1;

		// Token: 0x0400153D RID: 5437
		public int AwardMaxZhuanSheng = 1;

		// Token: 0x0400153E RID: 5438
		public int AwardMaxLevel = 1;

		// Token: 0x0400153F RID: 5439
		public AwardsItemList WinAwardsItemList = new AwardsItemList();

		// Token: 0x04001540 RID: 5440
		public AwardsItemList LoseAwardsItemList = new AwardsItemList();
	}
}
