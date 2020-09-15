using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200048C RID: 1164
	[ProtoContract]
	public class RoleTianTiData
	{
		// Token: 0x04001ECB RID: 7883
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x04001ECC RID: 7884
		[ProtoMember(2)]
		public int DuanWeiId;

		// Token: 0x04001ECD RID: 7885
		[ProtoMember(3)]
		public int DuanWeiJiFen;

		// Token: 0x04001ECE RID: 7886
		[ProtoMember(4)]
		public int DuanWeiRank;

		// Token: 0x04001ECF RID: 7887
		[ProtoMember(5)]
		public int LianSheng;

		// Token: 0x04001ED0 RID: 7888
		[ProtoMember(6)]
		public int SuccessCount;

		// Token: 0x04001ED1 RID: 7889
		[ProtoMember(7)]
		public int FightCount;

		// Token: 0x04001ED2 RID: 7890
		[ProtoMember(8)]
		public int TodayFightCount;

		// Token: 0x04001ED3 RID: 7891
		[ProtoMember(9)]
		public int MonthDuanWeiRank;

		// Token: 0x04001ED4 RID: 7892
		[ProtoMember(10)]
		public DateTime FetchMonthDuanWeiRankAwardsTime;

		// Token: 0x04001ED5 RID: 7893
		[ProtoMember(11)]
		public int RongYao;

		// Token: 0x04001ED6 RID: 7894
		[ProtoMember(12)]
		public int LastFightDayId;

		// Token: 0x04001ED7 RID: 7895
		[ProtoMember(13)]
		public DateTime RankUpdateTime;

		// Token: 0x04001ED8 RID: 7896
		[ProtoMember(14)]
		public int DayDuanWeiJiFen;
	}
}
