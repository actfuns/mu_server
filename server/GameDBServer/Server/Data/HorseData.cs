using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000063 RID: 99
	[ProtoContract]
	public class HorseData
	{
		// Token: 0x04000221 RID: 545
		[ProtoMember(1)]
		public int DbID = 0;

		// Token: 0x04000222 RID: 546
		[ProtoMember(2)]
		public int HorseID = 0;

		// Token: 0x04000223 RID: 547
		[ProtoMember(3)]
		public int BodyID = 0;

		// Token: 0x04000224 RID: 548
		[ProtoMember(4)]
		public string PropsNum = "";

		// Token: 0x04000225 RID: 549
		[ProtoMember(5)]
		public string PropsVal = "";

		// Token: 0x04000226 RID: 550
		[ProtoMember(6)]
		public long AddDateTime = 0L;

		// Token: 0x04000227 RID: 551
		[ProtoMember(7)]
		public int JinJieFailedNum = 0;

		// Token: 0x04000228 RID: 552
		[ProtoMember(8)]
		public long JinJieTempTime = 0L;

		// Token: 0x04000229 RID: 553
		[ProtoMember(9)]
		public int JinJieTempNum = 0;

		// Token: 0x0400022A RID: 554
		[ProtoMember(10)]
		public int JinJieFailedDayID = 0;
	}
}
