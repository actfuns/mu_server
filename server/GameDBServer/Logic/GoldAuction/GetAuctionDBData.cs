using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.GoldAuction
{
	
	[ProtoContract]
	public class GetAuctionDBData
	{
		
		public GetAuctionDBData()
		{
			this.Flag = false;
			this.ItemList = new List<GoldAuctionDBItem>();
		}

		
		[ProtoMember(1)]
		public List<GoldAuctionDBItem> ItemList;

		
		[ProtoMember(2)]
		public bool Flag;
	}
}
