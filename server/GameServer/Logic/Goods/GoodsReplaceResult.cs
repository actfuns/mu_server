using System;
using System.Collections.Generic;

namespace GameServer.Logic.Goods
{
	
	public class GoodsReplaceResult
	{
		
		public int TotalGoodsCnt()
		{
			return this.OriginBindGoods.GoodsCnt + this.OriginUnBindGoods.GoodsCnt + this.TotalBindCnt + this.TotalUnBindCnt;
		}

		
		public GoodsReplaceResult.ReplaceItem OriginBindGoods = new GoodsReplaceResult.ReplaceItem();

		
		public GoodsReplaceResult.ReplaceItem OriginUnBindGoods = new GoodsReplaceResult.ReplaceItem();

		
		public int TotalBindCnt = 0;

		
		public List<GoodsReplaceResult.ReplaceItem> BindList = new List<GoodsReplaceResult.ReplaceItem>();

		
		public int TotalUnBindCnt = 0;

		
		public List<GoodsReplaceResult.ReplaceItem> UnBindList = new List<GoodsReplaceResult.ReplaceItem>();

		
		public class ReplaceItem
		{
			
			public int GoodsID = 0;

			
			public int GoodsCnt = 0;

			
			public bool IsBind = false;
		}
	}
}
