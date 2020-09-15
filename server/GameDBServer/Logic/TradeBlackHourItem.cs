using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic
{
	// Token: 0x0200017F RID: 383
	[ProtoContract]
	internal class TradeBlackHourItem
	{
		// Token: 0x040008CD RID: 2253
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x040008CE RID: 2254
		[ProtoMember(2)]
		public string Day;

		// Token: 0x040008CF RID: 2255
		[ProtoMember(3)]
		public int Hour;

		// Token: 0x040008D0 RID: 2256
		[ProtoMember(4)]
		public int MarketTimes;

		// Token: 0x040008D1 RID: 2257
		[ProtoMember(5)]
		public long MarketInPrice;

		// Token: 0x040008D2 RID: 2258
		[ProtoMember(6)]
		public long MarketOutPrice;

		// Token: 0x040008D3 RID: 2259
		[ProtoMember(7)]
		public int TradeTimes;

		// Token: 0x040008D4 RID: 2260
		[ProtoMember(8)]
		public long TradeInPrice;

		// Token: 0x040008D5 RID: 2261
		[ProtoMember(9)]
		public long TradeOutPrice;

		// Token: 0x040008D6 RID: 2262
		[ProtoMember(10)]
		public HashSet<int> TradeRoles;

		// Token: 0x040008D7 RID: 2263
		[ProtoMember(11)]
		public int TradeDistinctRoleCount;
	}
}
