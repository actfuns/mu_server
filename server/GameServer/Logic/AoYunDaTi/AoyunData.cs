using System;
using System.Collections.Generic;

namespace GameServer.Logic.AoYunDaTi
{
	
	public class AoyunData
	{
		
		public object Mutex = new object();

		
		public static readonly int MaxPaiMingRank = 50000;

		
		public int[] ZhuanShengExpCoef;

		
		public int[] GoodsPrice;

		
		public int[] GoodsLimit;

		
		public bool GongNengOpen = false;

		
		public bool DaTiOpen = false;

		
		public Dictionary<int, int> AoyunRoleAnswerDic = new Dictionary<int, int>();

		
		public Dictionary<int, AoyunPaiHangRoleData> AoyunRankRoleDataDic = new Dictionary<int, AoyunPaiHangRoleData>();

		
		public List<AoyunPaiHangRoleData> AoyunRankList = new List<AoyunPaiHangRoleData>();

		
		public Dictionary<int, int> LastRankDic = new Dictionary<int, int>();
	}
}
