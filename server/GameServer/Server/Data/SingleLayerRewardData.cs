using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200056A RID: 1386
	[ProtoContract]
	public class SingleLayerRewardData
	{
		// Token: 0x04002558 RID: 9560
		[ProtoMember(1)]
		public int nLayerOrder;

		// Token: 0x04002559 RID: 9561
		[ProtoMember(2)]
		public int nExp;

		// Token: 0x0400255A RID: 9562
		[ProtoMember(3)]
		public int nMoney;

		// Token: 0x0400255B RID: 9563
		[ProtoMember(4)]
		public int nXinHun;

		// Token: 0x0400255C RID: 9564
		[ProtoMember(5)]
		public List<GoodsData> sweepAwardGoodsList = null;
	}
}
