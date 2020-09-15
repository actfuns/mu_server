using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000072 RID: 114
	[ProtoContract]
	public class JieriRecvKingData
	{
		// Token: 0x04000276 RID: 630
		[ProtoMember(1)]
		public List<JieriRecvKingItemData> RankingList;

		// Token: 0x04000277 RID: 631
		[ProtoMember(2)]
		public JieriRecvKingItemData MyData;
	}
}
