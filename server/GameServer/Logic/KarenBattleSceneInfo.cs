using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200031A RID: 794
	public class KarenBattleSceneInfo
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000CFB RID: 3323 RVA: 0x000CA44C File Offset: 0x000C864C
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs + 120;
			}
		}

		// Token: 0x04001476 RID: 5238
		public int Id;

		// Token: 0x04001477 RID: 5239
		public int MapCode;

		// Token: 0x04001478 RID: 5240
		public int MaxLegions = 4;

		// Token: 0x04001479 RID: 5241
		public int MaxEnterNum = 30;

		// Token: 0x0400147A RID: 5242
		public int EnterCD = 5;

		// Token: 0x0400147B RID: 5243
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		// Token: 0x0400147C RID: 5244
		public List<double> SecondsOfDay = new List<double>();

		// Token: 0x0400147D RID: 5245
		public int WaitingEnterSecs;

		// Token: 0x0400147E RID: 5246
		public int PrepareSecs;

		// Token: 0x0400147F RID: 5247
		public int FightingSecs;

		// Token: 0x04001480 RID: 5248
		public int ClearRolesSecs;

		// Token: 0x04001481 RID: 5249
		public long Exp;

		// Token: 0x04001482 RID: 5250
		public int BandJinBi;

		// Token: 0x04001483 RID: 5251
		public AwardsItemList WinAwardsItemList = new AwardsItemList();

		// Token: 0x04001484 RID: 5252
		public AwardsItemList LoseAwardsItemList = new AwardsItemList();
	}
}
