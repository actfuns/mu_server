using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003E2 RID: 994
	[ProtoContract]
	public class RebornRankInfoToClient
	{
		// Token: 0x04001A70 RID: 6768
		[ProtoMember(1)]
		public int RankType;

		// Token: 0x04001A71 RID: 6769
		[ProtoMember(2)]
		public List<RebornRankInfo> rankList = new List<RebornRankInfo>();
	}
}
