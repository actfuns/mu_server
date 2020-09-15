using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	// Token: 0x0200027C RID: 636
	[ProtoContract]
	public class CompBattleSelfScore
	{
		// Token: 0x04000FE0 RID: 4064
		[ProtoMember(1)]
		public int RankNum;

		// Token: 0x04000FE1 RID: 4065
		[ProtoMember(2)]
		public int AwardID;

		// Token: 0x04000FE2 RID: 4066
		[ProtoMember(3)]
		public List<KFCompRankInfo> rankInfo2Client = new List<KFCompRankInfo>();
	}
}
