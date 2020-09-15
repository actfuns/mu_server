using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	// Token: 0x0200027B RID: 635
	[ProtoContract]
	public class CompMineSelfScore
	{
		// Token: 0x04000FDD RID: 4061
		[ProtoMember(1)]
		public int RankNum;

		// Token: 0x04000FDE RID: 4062
		[ProtoMember(2)]
		public int AwardID;

		// Token: 0x04000FDF RID: 4063
		[ProtoMember(3)]
		public List<KFCompRankInfo> rankInfo2Client = new List<KFCompRankInfo>();
	}
}
