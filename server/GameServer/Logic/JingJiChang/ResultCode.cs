using System;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x0200072F RID: 1839
	public class ResultCode
	{
		// Token: 0x04003B5C RID: 15196
		public static readonly int Success = 1;

		// Token: 0x04003B5D RID: 15197
		public static readonly int Illegal = 0;

		// Token: 0x04003B5E RID: 15198
		public static readonly int Combat_Error = -1;

		// Token: 0x04003B5F RID: 15199
		public static readonly int Dead_Error = -2;

		// Token: 0x04003B60 RID: 15200
		public static readonly int FreeNum_Error = -3;

		// Token: 0x04003B61 RID: 15201
		public static readonly int VipNum_Error = -4;

		// Token: 0x04003B62 RID: 15202
		public static readonly int Pay_Error = -5;

		// Token: 0x04003B63 RID: 15203
		public static readonly int Money_Not_Enough_Error = -6;

		// Token: 0x04003B64 RID: 15204
		public static readonly int Map_Error = -7;

		// Token: 0x04003B65 RID: 15205
		public static readonly int FubenSeqId_Error = -8;

		// Token: 0x04003B66 RID: 15206
		public static readonly int Challenge_CD_Error = -9;

		// Token: 0x04003B67 RID: 15207
		public static readonly int BeChallenger_Null_Error = -10;

		// Token: 0x04003B68 RID: 15208
		public static readonly int BeChallenger_Ranking_Change_Error = -11;

		// Token: 0x04003B69 RID: 15209
		public static readonly int BeChallenger_Lock_Error = -12;

		// Token: 0x04003B6A RID: 15210
		public static readonly int RankingReward_CD_Error = -13;

		// Token: 0x04003B6B RID: 15211
		public static readonly int Junxian_Null_Error = -14;

		// Token: 0x04003B6C RID: 15212
		public static readonly int HasJunxianBuff_Error = -15;

		// Token: 0x04003B6D RID: 15213
		public static readonly int ShengWang_Not_Enough_Error = -16;

		// Token: 0x04003B6E RID: 15214
		public static readonly int BagIsFull = -17;

		// Token: 0x04003B6F RID: 15215
		public static readonly int CantChallenger = -18;

		// Token: 0x04003B70 RID: 15216
		public static readonly int GoodsNotEnough = -19;
	}
}
