using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.GoldAuction
{
	
	[ProtoContract]
	public class GoldAuctionS2C
	{
		
		public GoldAuctionS2C()
		{
			this.Info = 0;
			this.ItemList = new List<AuctionItemS2C>();
		}

		
		[ProtoMember(1)]
		public List<AuctionItemS2C> ItemList;

		
		[ProtoMember(2)]
		public int Info;

		
		[ProtoMember(3)]
		public int TotalCount;

		
		[ProtoMember(4)]
		public int CurrentPage;
	}
}
