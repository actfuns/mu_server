using System;

namespace GameServer.Logic.JingJiChang
{
	
	public class ResultCode
	{
		
		public static readonly int Success = 1;

		
		public static readonly int Illegal = 0;

		
		public static readonly int Combat_Error = -1;

		
		public static readonly int Dead_Error = -2;

		
		public static readonly int FreeNum_Error = -3;

		
		public static readonly int VipNum_Error = -4;

		
		public static readonly int Pay_Error = -5;

		
		public static readonly int Money_Not_Enough_Error = -6;

		
		public static readonly int Map_Error = -7;

		
		public static readonly int FubenSeqId_Error = -8;

		
		public static readonly int Challenge_CD_Error = -9;

		
		public static readonly int BeChallenger_Null_Error = -10;

		
		public static readonly int BeChallenger_Ranking_Change_Error = -11;

		
		public static readonly int BeChallenger_Lock_Error = -12;

		
		public static readonly int RankingReward_CD_Error = -13;

		
		public static readonly int Junxian_Null_Error = -14;

		
		public static readonly int HasJunxianBuff_Error = -15;

		
		public static readonly int ShengWang_Not_Enough_Error = -16;

		
		public static readonly int BagIsFull = -17;

		
		public static readonly int CantChallenger = -18;

		
		public static readonly int GoodsNotEnough = -19;
	}
}
