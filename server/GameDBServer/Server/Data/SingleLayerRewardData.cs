using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200007A RID: 122
	[ProtoContract]
	public class SingleLayerRewardData
	{
		// Token: 0x0400029A RID: 666
		[ProtoMember(1)]
		public int nLayerOrder;

		// Token: 0x0400029B RID: 667
		[ProtoMember(2)]
		public int nExp;

		// Token: 0x0400029C RID: 668
		[ProtoMember(3)]
		public int nMoney;

		// Token: 0x0400029D RID: 669
		[ProtoMember(4)]
		public int nXinHun;

		// Token: 0x0400029E RID: 670
		[ProtoMember(5)]
		public List<GoodsData> sweepAwardGoodsList = null;
	}
}
