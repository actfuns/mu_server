using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x0200009C RID: 156
	[ProtoContract]
	public class GetAuctionDBData
	{
		// Token: 0x06000273 RID: 627 RVA: 0x0002A8FC File Offset: 0x00028AFC
		public GetAuctionDBData()
		{
			this.Flag = false;
			this.ItemList = new List<GoldAuctionDBItem>();
		}

		// Token: 0x040003AE RID: 942
		[ProtoMember(1)]
		public List<GoldAuctionDBItem> ItemList;

		// Token: 0x040003AF RID: 943
		[ProtoMember(2)]
		public bool Flag;
	}
}
