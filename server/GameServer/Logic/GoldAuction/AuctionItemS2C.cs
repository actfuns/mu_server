using System;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x020000A0 RID: 160
	[ProtoContract]
	public class AuctionItemS2C
	{
		// Token: 0x06000278 RID: 632 RVA: 0x0002AAA4 File Offset: 0x00028CA4
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

		// Token: 0x040003C5 RID: 965
		[ProtoMember(1)]
		public int BuyRoleId;

		// Token: 0x040003C6 RID: 966
		[ProtoMember(2)]
		public long Price;

		// Token: 0x040003C7 RID: 967
		[ProtoMember(3)]
		public long LastTime;

		// Token: 0x040003C8 RID: 968
		[ProtoMember(4)]
		public string AuctionItemKey;

		// Token: 0x040003C9 RID: 969
		[ProtoMember(5)]
		public GoodsData Goods;

		// Token: 0x040003CA RID: 970
		[ProtoMember(6)]
		public long MaxPrice;

		// Token: 0x040003CB RID: 971
		[ProtoMember(7)]
		public long UnitPrice;
	}
}
