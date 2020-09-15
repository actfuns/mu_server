using System;
using System.Collections.Generic;

namespace GameServer.Logic.AoYunDaTi
{
	// Token: 0x02000205 RID: 517
	public class AoyunData
	{
		// Token: 0x04000B77 RID: 2935
		public object Mutex = new object();

		// Token: 0x04000B78 RID: 2936
		public static readonly int MaxPaiMingRank = 50000;

		// Token: 0x04000B79 RID: 2937
		public int[] ZhuanShengExpCoef;

		// Token: 0x04000B7A RID: 2938
		public int[] GoodsPrice;

		// Token: 0x04000B7B RID: 2939
		public int[] GoodsLimit;

		// Token: 0x04000B7C RID: 2940
		public bool GongNengOpen = false;

		// Token: 0x04000B7D RID: 2941
		public bool DaTiOpen = false;

		// Token: 0x04000B7E RID: 2942
		public Dictionary<int, int> AoyunRoleAnswerDic = new Dictionary<int, int>();

		// Token: 0x04000B7F RID: 2943
		public Dictionary<int, AoyunPaiHangRoleData> AoyunRankRoleDataDic = new Dictionary<int, AoyunPaiHangRoleData>();

		// Token: 0x04000B80 RID: 2944
		public List<AoyunPaiHangRoleData> AoyunRankList = new List<AoyunPaiHangRoleData>();

		// Token: 0x04000B81 RID: 2945
		public Dictionary<int, int> LastRankDic = new Dictionary<int, int>();
	}
}
