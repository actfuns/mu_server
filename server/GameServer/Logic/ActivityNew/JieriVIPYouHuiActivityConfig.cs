using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.ActivityNew
{
	
	public class JieriVIPYouHuiActivityConfig
	{
		
		public int ID = 0;

		
		public int MinVIPLev = 0;

		
		public int Price = 0;

		
		public int SinglePurchase = 0;

		
		public int FullPurchase = 0;

		
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();

		
		public AwardEffectTimeItem GoodsDataListThr = new AwardEffectTimeItem();
	}
}
