using System;
using System.Collections.Generic;

namespace GameServer.Logic.GoldAuction
{
	
	public class GlodAuctionIComparer : IComparer<AuctionItemS2C>
	{
		
		public GlodAuctionIComparer(int ordeType, bool isAscend)
		{
			this.IOrdeType = ordeType;
			this.IsAscend = isAscend;
		}

		
		public int Compare(AuctionItemS2C d1, AuctionItemS2C d2)
		{
			int val = this.IsAscend ? -1 : 1;
			long val2 = d1.LastTime;
			long val3 = d2.LastTime;
			if (1 == this.IOrdeType)
			{
				val2 = d1.Price;
				val3 = d2.Price;
			}
			else if (2 == this.IOrdeType)
			{
				val2 = d1.MaxPrice;
				val3 = d2.MaxPrice;
			}
			int result;
			if (val2 < val3)
			{
				result = val;
			}
			else if (val2 > val3)
			{
				result = -val;
			}
			else if (d1.LastTime < d2.LastTime)
			{
				result = val;
			}
			else if (d1.LastTime > d2.LastTime)
			{
				result = -val;
			}
			else if (d1.Price < d2.Price)
			{
				result = val;
			}
			else if (d1.Price > d2.Price)
			{
				result = -val;
			}
			else if (d1.MaxPrice < d2.MaxPrice)
			{
				result = val;
			}
			else if (d1.MaxPrice > d2.MaxPrice)
			{
				result = -val;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		private int IOrdeType;

		
		private bool IsAscend;
	}
}
