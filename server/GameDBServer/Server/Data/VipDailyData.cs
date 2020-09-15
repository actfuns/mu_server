using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000CE RID: 206
	[ProtoContract]
	public class VipDailyData
	{
		// Token: 0x040005A2 RID: 1442
		[ProtoMember(1)]
		public int PriorityType = 0;

		// Token: 0x040005A3 RID: 1443
		[ProtoMember(2)]
		public int DayID = 0;

		// Token: 0x040005A4 RID: 1444
		[ProtoMember(3)]
		public int UsedTimes = 0;
	}
}
