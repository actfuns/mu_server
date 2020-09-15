using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.GoldAuction
{
	// Token: 0x0200013A RID: 314
	[ProtoContract]
	public class GoldAuctionDBItem
	{
		// Token: 0x06000537 RID: 1335 RVA: 0x0002B8BC File Offset: 0x00029ABC
		public GoldAuctionDBItem()
		{
			this.AuctionType = 0;
			this.OldAuctionType = 0;
			this.AuctionSource = 0;
			this.StrGoods = "";
			this.BossLife = 0L;
			this.KillBossRoleID = 0;
			this.UpDBWay = 0;
			this.AuctionTime = "";
			this.ProductionTime = "";
			this.BuyerData = new AuctionRoleData();
			this.RoleList = new List<AuctionRoleData>();
		}

		// Token: 0x040007F4 RID: 2036
		[ProtoMember(1)]
		public string AuctionTime;

		// Token: 0x040007F5 RID: 2037
		[ProtoMember(2)]
		public int AuctionType;

		// Token: 0x040007F6 RID: 2038
		[ProtoMember(3)]
		public int AuctionSource;

		// Token: 0x040007F7 RID: 2039
		[ProtoMember(4)]
		public string ProductionTime;

		// Token: 0x040007F8 RID: 2040
		[ProtoMember(5)]
		public string StrGoods;

		// Token: 0x040007F9 RID: 2041
		[ProtoMember(6)]
		public List<AuctionRoleData> RoleList;

		// Token: 0x040007FA RID: 2042
		[ProtoMember(7)]
		public long BossLife;

		// Token: 0x040007FB RID: 2043
		[ProtoMember(8)]
		public int KillBossRoleID;

		// Token: 0x040007FC RID: 2044
		[ProtoMember(9)]
		public int UpDBWay;

		// Token: 0x040007FD RID: 2045
		[ProtoMember(10)]
		public int OldAuctionType;

		// Token: 0x040007FE RID: 2046
		[ProtoMember(11)]
		public AuctionRoleData BuyerData;
	}
}
