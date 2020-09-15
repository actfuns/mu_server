using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000070 RID: 112
	[ProtoContract]
	public class JieriGiveKingData
	{
		// Token: 0x0400026E RID: 622
		[ProtoMember(1)]
		public List<JieriGiveKingItemData> RankingList;

		// Token: 0x0400026F RID: 623
		[ProtoMember(2)]
		public JieriGiveKingItemData MyData;
	}
}
