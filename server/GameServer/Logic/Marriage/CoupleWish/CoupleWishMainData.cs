using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic.Marriage.CoupleWish
{
	// Token: 0x0200036B RID: 875
	[ProtoContract]
	public class CoupleWishMainData : ICompressed
	{
		// Token: 0x04001722 RID: 5922
		[ProtoMember(1)]
		public List<CoupleWishCoupleData> RankList;

		// Token: 0x04001723 RID: 5923
		[ProtoMember(2)]
		public int MyCoupleRank;

		// Token: 0x04001724 RID: 5924
		[ProtoMember(3)]
		public int MyCoupleBeWishNum;

		// Token: 0x04001725 RID: 5925
		[ProtoMember(4)]
		public int CanGetAwardId;

		// Token: 0x04001726 RID: 5926
		[ProtoMember(5)]
		public RoleData4Selector MyCoupleManSelector;

		// Token: 0x04001727 RID: 5927
		[ProtoMember(6)]
		public RoleData4Selector MyCoupleWifeSelector;
	}
}
