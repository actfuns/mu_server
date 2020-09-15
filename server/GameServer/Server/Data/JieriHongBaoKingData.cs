using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002EC RID: 748
	[ProtoContract]
	public class JieriHongBaoKingData
	{
		// Token: 0x0400133A RID: 4922
		[ProtoMember(1)]
		public List<JieriHongBaoKingItemData> RankList;

		// Token: 0x0400133B RID: 4923
		[ProtoMember(2)]
		public long DataAge;

		// Token: 0x0400133C RID: 4924
		[ProtoMember(3)]
		public int SelfCount;
	}
}
