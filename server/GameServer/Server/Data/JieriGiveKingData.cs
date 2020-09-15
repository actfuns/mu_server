using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200014F RID: 335
	[ProtoContract]
	public class JieriGiveKingData
	{
		// Token: 0x04000776 RID: 1910
		[ProtoMember(1)]
		public List<JieriGiveKingItemData> RankingList;

		// Token: 0x04000777 RID: 1911
		[ProtoMember(2)]
		public JieriGiveKingItemData MyData;
	}
}
