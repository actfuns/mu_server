using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class ChargeItemData
	{
		
		public int ChargeItemID;

		
		public int ChargeDangID;

		
		public string GoodsStringOne = "";

		
		public List<GoodsData> GoodsDataOne = new List<GoodsData>();

		
		public string GoodsStringTwo = "";

		
		public List<GoodsData> GoodsDataTwo = new List<GoodsData>();

		
		public int SinglePurchase;

		
		public int DayPurchase;

		
		public int ThemePurchase;

		
		public string FromDate;

		
		public string ToDate;
	}
}
