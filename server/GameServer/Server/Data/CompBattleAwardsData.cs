using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200027E RID: 638
	[ProtoContract]
	public class CompBattleAwardsData
	{
		// Token: 0x04000FE8 RID: 4072
		[ProtoMember(1)]
		public int RankNum;

		// Token: 0x04000FE9 RID: 4073
		[ProtoMember(2)]
		public int AwardID;

		// Token: 0x04000FEA RID: 4074
		[ProtoMember(3)]
		public int WinNum;
	}
}
