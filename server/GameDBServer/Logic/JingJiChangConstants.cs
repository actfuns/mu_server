using System;

namespace GameDBServer.Logic
{
	
	public class JingJiChangConstants
	{
		
		public static readonly int Current_Data_Version = 0;

		
		public static readonly int ChallengeInfoType_Challenge_Win = 0;

		
		public static readonly int ChallengeInfoType_Challenge_Failed = 1;

		
		public static readonly int ChallengeInfoType_Be_Challenge_Win = 2;

		
		public static readonly int ChallengeInfoType_Be_Challenge_Failed = 3;

		
		public static readonly int ChallengeInfoType_LianSheng = 4;

		
		public static readonly int ChallengeInfo_PageShowNum = 20;

		
		public static readonly int ChallengeInfo_Max_PageIndex = (JingJiChangConstants.ChallengeInfo_Max_Num % JingJiChangConstants.ChallengeInfo_PageShowNum == 0) ? (JingJiChangConstants.ChallengeInfo_Max_Num / JingJiChangConstants.ChallengeInfo_PageShowNum) : (JingJiChangConstants.ChallengeInfo_Max_Num / JingJiChangConstants.ChallengeInfo_PageShowNum + 1);

		
		public static readonly int ChallengeInfo_Max_Num = 50;

		
		public static readonly int RankingList_Max_Num = 5000;

		
		public static readonly int RankingList_PageShowNum = 100;

		
		public static readonly int RankingList_Max_PageIndex = (JingJiChangConstants.RankingList_Max_Num % JingJiChangConstants.RankingList_PageShowNum == 0) ? (JingJiChangConstants.RankingList_Max_Num / JingJiChangConstants.RankingList_PageShowNum) : (JingJiChangConstants.RankingList_Max_Num % JingJiChangConstants.RankingList_PageShowNum + 1);
	}
}
