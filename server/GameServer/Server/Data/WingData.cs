using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005AA RID: 1450
	[ProtoContract]
	public class WingData
	{
		// Token: 0x040028DD RID: 10461
		[ProtoMember(1)]
		public int DbID = 0;

		// Token: 0x040028DE RID: 10462
		[ProtoMember(2)]
		public int WingID = 0;

		// Token: 0x040028DF RID: 10463
		[ProtoMember(3)]
		public int ForgeLevel = 0;

		// Token: 0x040028E0 RID: 10464
		[ProtoMember(4)]
		public long AddDateTime = 0L;

		// Token: 0x040028E1 RID: 10465
		[ProtoMember(5)]
		public int JinJieFailedNum = 0;

		// Token: 0x040028E2 RID: 10466
		[ProtoMember(6)]
		public int Using = 0;

		// Token: 0x040028E3 RID: 10467
		[ProtoMember(7)]
		public int StarExp = 0;

		// Token: 0x040028E4 RID: 10468
		[ProtoMember(8)]
		public int ZhuLingNum = 0;

		// Token: 0x040028E5 RID: 10469
		[ProtoMember(9)]
		public int ZhuHunNum = 0;
	}
}
