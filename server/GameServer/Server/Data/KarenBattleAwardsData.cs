using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000329 RID: 809
	[ProtoContract]
	public class KarenBattleAwardsData
	{
		// Token: 0x040014E3 RID: 5347
		[ProtoMember(1)]
		public int Success;

		// Token: 0x040014E4 RID: 5348
		[ProtoMember(2)]
		public int BindJinBi;

		// Token: 0x040014E5 RID: 5349
		[ProtoMember(3)]
		public long Exp;

		// Token: 0x040014E6 RID: 5350
		[ProtoMember(4)]
		public List<GoodsData> AwardGoodsDataList;
	}
}
