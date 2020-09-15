using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000342 RID: 834
	public class KuaFuBossSceneInfo
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000E00 RID: 3584 RVA: 0x000DE004 File Offset: 0x000DC204
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs + 120;
			}
		}

		// Token: 0x040015E3 RID: 5603
		public int Id;

		// Token: 0x040015E4 RID: 5604
		public int MapCode;

		// Token: 0x040015E5 RID: 5605
		public int MinZhuanSheng = 1;

		// Token: 0x040015E6 RID: 5606
		public int MinLevel = 1;

		// Token: 0x040015E7 RID: 5607
		public int MaxZhuanSheng = 1;

		// Token: 0x040015E8 RID: 5608
		public int MaxLevel = 1;

		// Token: 0x040015E9 RID: 5609
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		// Token: 0x040015EA RID: 5610
		public List<double> SecondsOfDay = new List<double>();

		// Token: 0x040015EB RID: 5611
		public int WaitingEnterSecs;

		// Token: 0x040015EC RID: 5612
		public int PrepareSecs;

		// Token: 0x040015ED RID: 5613
		public int FightingSecs;

		// Token: 0x040015EE RID: 5614
		public int ClearRolesSecs;

		// Token: 0x040015EF RID: 5615
		public int SignUpStartSecs;

		// Token: 0x040015F0 RID: 5616
		public int SignUpEndSecs;
	}
}
