using System;

namespace GameServer.Logic.GoldAuction
{
	
	public class GoldAuctionItem : GoldAuctionDBItem
	{
		
		public GoldAuctionItem()
		{
			this.Lock = false;
			this.LifeTime = 0;
		}

		
		public bool Lock;

		
		public int LifeTime;
	}
}
