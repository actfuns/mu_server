using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000C1 RID: 193
	[ProtoContract]
	public class RoleTianTiData
	{
		// Token: 0x04000531 RID: 1329
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x04000532 RID: 1330
		[ProtoMember(2)]
		public int DuanWeiId;

		// Token: 0x04000533 RID: 1331
		[ProtoMember(3)]
		public int DuanWeiJiFen;

		// Token: 0x04000534 RID: 1332
		[ProtoMember(4)]
		public int DuanWeiRank;

		// Token: 0x04000535 RID: 1333
		[ProtoMember(5)]
		public int LianSheng;

		// Token: 0x04000536 RID: 1334
		[ProtoMember(6)]
		public int SuccessCount;

		// Token: 0x04000537 RID: 1335
		[ProtoMember(7)]
		public int FightCount;

		// Token: 0x04000538 RID: 1336
		[ProtoMember(8)]
		public int TodayFightCount;

		// Token: 0x04000539 RID: 1337
		[ProtoMember(9)]
		public int MonthDuanWeiRank;

		// Token: 0x0400053A RID: 1338
		[ProtoMember(10)]
		public DateTime FetchMonthDuanWeiRankAwardsTime;

		// Token: 0x0400053B RID: 1339
		[ProtoMember(11)]
		public int RongYao;

		// Token: 0x0400053C RID: 1340
		[ProtoMember(12)]
		public int LastFightDayId;

		// Token: 0x0400053D RID: 1341
		[ProtoMember(13)]
		public DateTime RankUpdateTime;
	}
}
