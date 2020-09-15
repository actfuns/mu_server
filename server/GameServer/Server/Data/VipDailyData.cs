using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005A8 RID: 1448
	[ProtoContract]
	public class VipDailyData
	{
		// Token: 0x040028D5 RID: 10453
		[ProtoMember(1)]
		public int PriorityType = 0;

		// Token: 0x040028D6 RID: 10454
		[ProtoMember(2)]
		public int DayID = 0;

		// Token: 0x040028D7 RID: 10455
		[ProtoMember(3)]
		public int UsedTimes = 0;
	}
}
