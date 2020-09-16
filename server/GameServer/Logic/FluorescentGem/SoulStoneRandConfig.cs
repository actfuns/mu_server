using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.FluorescentGem
{
	
	internal class SoulStoneRandConfig
	{
		
		public int RandId;

		
		public int NeedLangHunFenMo;

		
		public double SuccessRate;

		
		public List<int> SuccessTo = new List<int>();

		
		public List<int> FailTo = new List<int>();

		
		public List<SoulStoneRandInfo> RandStoneList = new List<SoulStoneRandInfo>();

		
		public int RandMinNumber;

		
		public int RandMaxNumber;

		
		public Dictionary<ESoulStoneExtCostType, int> AddedNeedDict = new Dictionary<ESoulStoneExtCostType, int>();

		
		public double AddedRate;

		
		public GoodsData AddedGoods;

		
		public Dictionary<ESoulStoneExtCostType, int> ReduceNeedDict = new Dictionary<ESoulStoneExtCostType, int>();

		
		public double ReduceRate;

		
		public int ReduceValue;

		
		public Dictionary<ESoulStoneExtCostType, int> UpSucRateNeedDict = new Dictionary<ESoulStoneExtCostType, int>();

		
		public double UpSucRateTo;

		
		public Dictionary<ESoulStoneExtCostType, int> FailHoldNeedDict = new Dictionary<ESoulStoneExtCostType, int>();

		
		public List<int> FailToIfHold = new List<int>();
	}
}
