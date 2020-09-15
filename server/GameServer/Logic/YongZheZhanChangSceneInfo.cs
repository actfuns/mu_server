using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000813 RID: 2067
	public class YongZheZhanChangSceneInfo
	{
		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x06003A86 RID: 14982 RVA: 0x00319E70 File Offset: 0x00318070
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs + 120;
			}
		}

		// Token: 0x04004469 RID: 17513
		public int Id;

		// Token: 0x0400446A RID: 17514
		public int MapCode;

		// Token: 0x0400446B RID: 17515
		public int MinZhuanSheng = 1;

		// Token: 0x0400446C RID: 17516
		public int MinLevel = 1;

		// Token: 0x0400446D RID: 17517
		public int MaxZhuanSheng = 1;

		// Token: 0x0400446E RID: 17518
		public int MaxLevel = 1;

		// Token: 0x0400446F RID: 17519
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		// Token: 0x04004470 RID: 17520
		public List<double> SecondsOfDay = new List<double>();

		// Token: 0x04004471 RID: 17521
		public int WaitingEnterSecs;

		// Token: 0x04004472 RID: 17522
		public int PrepareSecs;

		// Token: 0x04004473 RID: 17523
		public int FightingSecs;

		// Token: 0x04004474 RID: 17524
		public int ClearRolesSecs;

		// Token: 0x04004475 RID: 17525
		public int SignUpStartSecs;

		// Token: 0x04004476 RID: 17526
		public int SignUpEndSecs;

		// Token: 0x04004477 RID: 17527
		public long Exp;

		// Token: 0x04004478 RID: 17528
		public int BandJinBi;

		// Token: 0x04004479 RID: 17529
		public AwardsItemList WinAwardsItemList = new AwardsItemList();

		// Token: 0x0400447A RID: 17530
		public AwardsItemList LoseAwardsItemList = new AwardsItemList();
	}
}
