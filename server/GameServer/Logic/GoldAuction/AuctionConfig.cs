using System;
using System.Collections.Generic;

namespace GameServer.Logic.GoldAuction
{
	
	public class AuctionConfig
	{
		
		public int GetTimeByAuction(int Auction)
		{
			for (int i = 0; i < this.OrderList.Count; i++)
			{
				if (Auction == this.OrderList[i])
				{
					return this.TimeList[i];
				}
			}
			return -1;
		}

		
		public int GetNextAuction(int Auction)
		{
			bool isFind = false;
			foreach (int item in this.OrderList)
			{
				if (Auction == item)
				{
					isFind = true;
				}
				else if (GoldAuctionManager.IsOpenAuction((AuctionOrderEnum)item))
				{
					if (isFind)
					{
						return item;
					}
				}
			}
			return -1;
		}

		
		public int ID;

		
		public string Name;

		
		public List<int> OrderList = new List<int>();

		
		public List<int> TimeList = new List<int>();

		
		public int OriginPrice;

		
		public int UnitPrice;

		
		public int MaxPrice;

		
		public string SuccessTitle;

		
		public string SuccessIntro;

		
		public string FailTitle;

		
		public string FailIntro;
	}
}
