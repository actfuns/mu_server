using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005AB RID: 1451
	[ProtoContract]
	public class YangGongBKDailyJiFenData
	{
		// Token: 0x040028E6 RID: 10470
		[ProtoMember(1)]
		public int DayID = 0;

		// Token: 0x040028E7 RID: 10471
		[ProtoMember(2)]
		public int JiFen = 0;

		// Token: 0x040028E8 RID: 10472
		[ProtoMember(3)]
		public long AwardHistory = 0L;
	}
}
