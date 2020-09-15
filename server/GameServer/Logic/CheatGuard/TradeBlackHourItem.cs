using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace GameServer.Logic.CheatGuard
{
	// Token: 0x02000250 RID: 592
	[ProtoContract]
	internal class TradeBlackHourItem
	{
		// Token: 0x0600084A RID: 2122 RVA: 0x0007F608 File Offset: 0x0007D808
		public TradeBlackHourItem SimpleClone()
		{
			return new TradeBlackHourItem
			{
				RoleId = this.RoleId,
				Day = this.Day,
				Hour = this.Hour,
				MarketTimes = this.MarketTimes,
				MarketInPrice = this.MarketInPrice,
				MarketOutPrice = this.MarketOutPrice,
				TradeTimes = this.TradeTimes,
				TradeInPrice = this.TradeInPrice,
				TradeOutPrice = this.TradeOutPrice,
				TradeDistinctRoleCount = ((this.TradeRoles != null) ? this.TradeRoles.Count<int>() : 0)
			};
		}

		// Token: 0x04000E3B RID: 3643
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x04000E3C RID: 3644
		[ProtoMember(2)]
		public string Day;

		// Token: 0x04000E3D RID: 3645
		[ProtoMember(3)]
		public int Hour;

		// Token: 0x04000E3E RID: 3646
		[ProtoMember(4)]
		public int MarketTimes;

		// Token: 0x04000E3F RID: 3647
		[ProtoMember(5)]
		public long MarketInPrice;

		// Token: 0x04000E40 RID: 3648
		[ProtoMember(6)]
		public long MarketOutPrice;

		// Token: 0x04000E41 RID: 3649
		[ProtoMember(7)]
		public int TradeTimes;

		// Token: 0x04000E42 RID: 3650
		[ProtoMember(8)]
		public long TradeInPrice;

		// Token: 0x04000E43 RID: 3651
		[ProtoMember(9)]
		public long TradeOutPrice;

		// Token: 0x04000E44 RID: 3652
		[ProtoMember(10)]
		public HashSet<int> TradeRoles;

		// Token: 0x04000E45 RID: 3653
		[ProtoMember(11)]
		public int TradeDistinctRoleCount;
	}
}
