using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000838 RID: 2104
	[ProtoContract]
	public class WanMoXiaGuAwardsData
	{
		// Token: 0x040045AD RID: 17837
		[ProtoMember(1)]
		public int Success;

		// Token: 0x040045AE RID: 17838
		[ProtoMember(2)]
		public int UsedSecs;

		// Token: 0x040045AF RID: 17839
		[ProtoMember(3)]
		public long Exp;

		// Token: 0x040045B0 RID: 17840
		[ProtoMember(4)]
		public int Money;

		// Token: 0x040045B1 RID: 17841
		[ProtoMember(5)]
		public List<GoodsData> AwardsGoods;
	}
}
