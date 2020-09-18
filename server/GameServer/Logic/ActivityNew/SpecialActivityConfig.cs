using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.ActivityNew
{
	
	public class SpecialActivityConfig
	{
		
		public int ID = 0;

		
		public int GroupID = 0;

		
		public DateTime FromDay;

		
		public DateTime ToDay;

		
		public SpecActLimitData LevLimit = new SpecActLimitData();

		
		public SpecActLimitData VipLimit = new SpecActLimitData();

		
		public SpecActLimitData ChongZhiLimit = new SpecActLimitData();

		
		public SpecActLimitData WingLimit = new SpecActLimitData();

		
		public SpecActLimitData ChengJiuLimit = new SpecActLimitData();

		
		public SpecActLimitData JunXianLimit = new SpecActLimitData();

		
		public SpecActLimitData MerlinLimit = new SpecActLimitData();

		
		public SpecActLimitData ShengWuLimit = new SpecActLimitData();

		
		public SpecActLimitData RingLimit = new SpecActLimitData();

		
		public SpecActLimitData ShouHuShenLimit = new SpecActLimitData();

		
		public int Type = 0;

		
		public SpecActGoalData GoalData = new SpecActGoalData();

		
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();

		
		public AwardEffectTimeItem GoodsDataListThr = new AwardEffectTimeItem();

		
		public SpecActPriceData Price = new SpecActPriceData();

		
		public int PurchaseNum = -1;
	}
}
