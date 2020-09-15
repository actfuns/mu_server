using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000FB RID: 251
	[ProtoContract]
	public class JieriHongBaoKingData
	{
		// Token: 0x0400070F RID: 1807
		[ProtoMember(1)]
		public List<JieriHongBaoKingItemData> RankList;

		// Token: 0x04000710 RID: 1808
		[ProtoMember(2)]
		public long DataAge;
	}
}
