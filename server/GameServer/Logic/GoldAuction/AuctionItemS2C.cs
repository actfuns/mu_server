using System;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.GoldAuction
{
	
	[ProtoContract]
	public class AuctionItemS2C
	{
		
		public AuctionItemS2C()
		{
			this.BuyRoleId = 0;
			this.Price = 0L;
			this.LastTime = 0L;
			this.AuctionItemKey = "";
			this.Goods = new GoodsData();
			this.MaxPrice = 1L;
			this.MaxPrice = 9999999999999L;
		}

		
		[ProtoMember(1)]
		public int BuyRoleId;

		
		[ProtoMember(2)]
		public long Price;

		
		[ProtoMember(3)]
		public long LastTime;

		
		[ProtoMember(4)]
		public string AuctionItemKey;

		
		[ProtoMember(5)]
		public GoodsData Goods;

		
		[ProtoMember(6)]
		public long MaxPrice;

		
		[ProtoMember(7)]
		public long UnitPrice;
	}
}
