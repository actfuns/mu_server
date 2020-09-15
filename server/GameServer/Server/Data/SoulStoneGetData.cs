using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002D5 RID: 725
	[ProtoContract]
	public class SoulStoneGetData
	{
		// Token: 0x040012A7 RID: 4775
		[ProtoMember(1)]
		public int Error;

		// Token: 0x040012A8 RID: 4776
		[ProtoMember(2)]
		public int RequestTimes;

		// Token: 0x040012A9 RID: 4777
		[ProtoMember(3)]
		public int RealDoTimes;

		// Token: 0x040012AA RID: 4778
		[ProtoMember(4)]
		public int NewRandId;

		// Token: 0x040012AB RID: 4779
		[ProtoMember(5)]
		public List<int> Stones;

		// Token: 0x040012AC RID: 4780
		[ProtoMember(6)]
		public List<int> ExtGoods;
	}
}
