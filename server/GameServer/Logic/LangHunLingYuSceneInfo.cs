using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020003CB RID: 971
	public class LangHunLingYuSceneInfo
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060010EC RID: 4332 RVA: 0x001077CC File Offset: 0x001059CC
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs + 1200;
			}
		}

		// Token: 0x04001961 RID: 6497
		public int Id;

		// Token: 0x04001962 RID: 6498
		public int MapCode;

		// Token: 0x04001963 RID: 6499
		public int MapCode_LongTa;

		// Token: 0x04001964 RID: 6500
		public int MinZhuanSheng = 1;

		// Token: 0x04001965 RID: 6501
		public int MinLevel = 1;

		// Token: 0x04001966 RID: 6502
		public int MaxZhuanSheng = 1;

		// Token: 0x04001967 RID: 6503
		public int MaxLevel = 1;

		// Token: 0x04001968 RID: 6504
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		// Token: 0x04001969 RID: 6505
		public List<double> SecondsOfDay = new List<double>();

		// Token: 0x0400196A RID: 6506
		public int WaitingEnterSecs;

		// Token: 0x0400196B RID: 6507
		public int PrepareSecs;

		// Token: 0x0400196C RID: 6508
		public int FightingSecs;

		// Token: 0x0400196D RID: 6509
		public int ClearRolesSecs;

		// Token: 0x0400196E RID: 6510
		public int SignUpStartSecs;

		// Token: 0x0400196F RID: 6511
		public int SignUpEndSecs;

		// Token: 0x04001970 RID: 6512
		public AwardsItemList WinAwardsItemList = new AwardsItemList();
	}
}
