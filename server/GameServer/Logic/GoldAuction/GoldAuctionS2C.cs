using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x0200009F RID: 159
	[ProtoContract]
	public class GoldAuctionS2C
	{
		// Token: 0x06000277 RID: 631 RVA: 0x0002AA86 File Offset: 0x00028C86
		public GoldAuctionS2C()
		{
			this.Info = 0;
			this.ItemList = new List<AuctionItemS2C>();
		}

		// Token: 0x040003C1 RID: 961
		[ProtoMember(1)]
		public List<AuctionItemS2C> ItemList;

		// Token: 0x040003C2 RID: 962
		[ProtoMember(2)]
		public int Info;

		// Token: 0x040003C3 RID: 963
		[ProtoMember(3)]
		public int TotalCount;

		// Token: 0x040003C4 RID: 964
		[ProtoMember(4)]
		public int CurrentPage;
	}
}
