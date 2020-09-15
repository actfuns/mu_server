using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.GoldAuction
{
	// Token: 0x02000139 RID: 313
	[ProtoContract]
	public class GetAuctionDBData
	{
		// Token: 0x06000536 RID: 1334 RVA: 0x0002B89C File Offset: 0x00029A9C
		public GetAuctionDBData()
		{
			this.Flag = false;
			this.ItemList = new List<GoldAuctionDBItem>();
		}

		// Token: 0x040007F2 RID: 2034
		[ProtoMember(1)]
		public List<GoldAuctionDBItem> ItemList;

		// Token: 0x040007F3 RID: 2035
		[ProtoMember(2)]
		public bool Flag;
	}
}
