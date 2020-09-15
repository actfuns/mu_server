using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000151 RID: 337
	[ProtoContract]
	public class JieriRecvKingData
	{
		// Token: 0x0400077E RID: 1918
		[ProtoMember(1)]
		public List<JieriRecvKingItemData> RankingList;

		// Token: 0x0400077F RID: 1919
		[ProtoMember(2)]
		public JieriRecvKingItemData MyData;
	}
}
