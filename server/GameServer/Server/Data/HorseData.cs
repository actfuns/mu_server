using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200055B RID: 1371
	[ProtoContract]
	public class HorseData
	{
		// Token: 0x040024E1 RID: 9441
		[ProtoMember(1)]
		public int DbID = 0;

		// Token: 0x040024E2 RID: 9442
		[ProtoMember(2)]
		public int HorseID = 0;

		// Token: 0x040024E3 RID: 9443
		[ProtoMember(3)]
		public int BodyID = 0;

		// Token: 0x040024E4 RID: 9444
		[ProtoMember(4)]
		public string PropsNum = "";

		// Token: 0x040024E5 RID: 9445
		[ProtoMember(5)]
		public string PropsVal = "";

		// Token: 0x040024E6 RID: 9446
		[ProtoMember(6)]
		public long AddDateTime = 0L;

		// Token: 0x040024E7 RID: 9447
		[ProtoMember(7)]
		public int JinJieFailedNum = 0;

		// Token: 0x040024E8 RID: 9448
		[ProtoMember(8)]
		public long JinJieTempTime = 0L;

		// Token: 0x040024E9 RID: 9449
		[ProtoMember(9)]
		public int JinJieTempNum = 0;

		// Token: 0x040024EA RID: 9450
		[ProtoMember(10)]
		public int JinJieFailedDayID = 0;
	}
}
