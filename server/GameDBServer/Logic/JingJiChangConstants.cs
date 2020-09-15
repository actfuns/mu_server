using System;

namespace GameDBServer.Logic
{
	// Token: 0x02000141 RID: 321
	public class JingJiChangConstants
	{
		// Token: 0x04000810 RID: 2064
		public static readonly int Current_Data_Version = 0;

		// Token: 0x04000811 RID: 2065
		public static readonly int ChallengeInfoType_Challenge_Win = 0;

		// Token: 0x04000812 RID: 2066
		public static readonly int ChallengeInfoType_Challenge_Failed = 1;

		// Token: 0x04000813 RID: 2067
		public static readonly int ChallengeInfoType_Be_Challenge_Win = 2;

		// Token: 0x04000814 RID: 2068
		public static readonly int ChallengeInfoType_Be_Challenge_Failed = 3;

		// Token: 0x04000815 RID: 2069
		public static readonly int ChallengeInfoType_LianSheng = 4;

		// Token: 0x04000816 RID: 2070
		public static readonly int ChallengeInfo_PageShowNum = 20;

		// Token: 0x04000817 RID: 2071
		public static readonly int ChallengeInfo_Max_PageIndex = (JingJiChangConstants.ChallengeInfo_Max_Num % JingJiChangConstants.ChallengeInfo_PageShowNum == 0) ? (JingJiChangConstants.ChallengeInfo_Max_Num / JingJiChangConstants.ChallengeInfo_PageShowNum) : (JingJiChangConstants.ChallengeInfo_Max_Num / JingJiChangConstants.ChallengeInfo_PageShowNum + 1);

		// Token: 0x04000818 RID: 2072
		public static readonly int ChallengeInfo_Max_Num = 50;

		// Token: 0x04000819 RID: 2073
		public static readonly int RankingList_Max_Num = 5000;

		// Token: 0x0400081A RID: 2074
		public static readonly int RankingList_PageShowNum = 100;

		// Token: 0x0400081B RID: 2075
		public static readonly int RankingList_Max_PageIndex = (JingJiChangConstants.RankingList_Max_Num % JingJiChangConstants.RankingList_PageShowNum == 0) ? (JingJiChangConstants.RankingList_Max_Num / JingJiChangConstants.RankingList_PageShowNum) : (JingJiChangConstants.RankingList_Max_Num % JingJiChangConstants.RankingList_PageShowNum + 1);
	}
}
