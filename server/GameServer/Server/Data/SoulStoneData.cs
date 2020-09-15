using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002D4 RID: 724
	[ProtoContract]
	public class SoulStoneData
	{
		// Token: 0x040012A5 RID: 4773
		[ProtoMember(1)]
		public List<GoodsData> StonesInBag;

		// Token: 0x040012A6 RID: 4774
		[ProtoMember(2)]
		public List<GoodsData> StonesInUsing;
	}
}
