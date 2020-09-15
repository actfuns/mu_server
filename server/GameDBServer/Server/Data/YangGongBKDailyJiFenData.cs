using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000CF RID: 207
	[ProtoContract]
	public class YangGongBKDailyJiFenData
	{
		// Token: 0x040005A5 RID: 1445
		[ProtoMember(1)]
		public int DayID = 0;

		// Token: 0x040005A6 RID: 1446
		[ProtoMember(2)]
		public int JiFen = 0;

		// Token: 0x040005A7 RID: 1447
		[ProtoMember(3)]
		public long AwardHistory = 0L;
	}
}
