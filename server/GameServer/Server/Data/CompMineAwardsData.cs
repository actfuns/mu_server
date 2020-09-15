using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000278 RID: 632
	[ProtoContract]
	public class CompMineAwardsData
	{
		// Token: 0x04000FD1 RID: 4049
		[ProtoMember(1)]
		public int RankNum;

		// Token: 0x04000FD2 RID: 4050
		[ProtoMember(2)]
		public int AwardID;

		// Token: 0x04000FD3 RID: 4051
		[ProtoMember(3)]
		public int RankMine;
	}
}
