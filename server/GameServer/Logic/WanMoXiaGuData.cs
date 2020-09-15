using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000835 RID: 2101
	public class WanMoXiaGuData
	{
		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06003B41 RID: 15169 RVA: 0x00325B24 File Offset: 0x00323D24
		public int TotalSecs
		{
			get
			{
				return this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		// Token: 0x04004594 RID: 17812
		public object Mutex = new object();

		// Token: 0x04004595 RID: 17813
		public int MapID = 70300;

		// Token: 0x04004596 RID: 17814
		public int CopyID = 70300;

		// Token: 0x04004597 RID: 17815
		public List<List<int>> AwardList;

		// Token: 0x04004598 RID: 17816
		public int BossMonsterID;

		// Token: 0x04004599 RID: 17817
		public double WanMoXiaGuCall;

		// Token: 0x0400459A RID: 17818
		public int GoodsBinding = 1;

		// Token: 0x0400459B RID: 17819
		public int[] FuBenIds = new int[]
		{
			70300
		};

		// Token: 0x0400459C RID: 17820
		public Dictionary<int, WanMoXiaGuMonsterConfigInfo> MonsterOrderConfigList = new Dictionary<int, WanMoXiaGuMonsterConfigInfo>();

		// Token: 0x0400459D RID: 17821
		public int BeginNum;

		// Token: 0x0400459E RID: 17822
		public int EndNum;

		// Token: 0x0400459F RID: 17823
		public int PrepareSecs = 1;

		// Token: 0x040045A0 RID: 17824
		public int FightingSecs = 900;

		// Token: 0x040045A1 RID: 17825
		public int ClearRolesSecs = 15;
	}
}
