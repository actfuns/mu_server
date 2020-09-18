using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.ActivityNew
{
	
	public class EverydayActivityConfig
	{
		
		public int ID = 0;

		
		public int Type = 0;

		
		public EveryActGoalData GoalData = new EveryActGoalData();

		
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();

		
		public AwardEffectTimeItem GoodsDataListThr = new AwardEffectTimeItem();

		
		public EveryActPriceData Price = new EveryActPriceData();

		
		public int PurchaseNum = -1;
	}
}
