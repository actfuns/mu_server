using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000562 RID: 1378
	[ProtoContract]
	public class JingJiDetailData
	{
		// Token: 0x04002513 RID: 9491
		[ProtoMember(1)]
		public int state;

		// Token: 0x04002514 RID: 9492
		[ProtoMember(2)]
		public int freeChallengeNum;

		// Token: 0x04002515 RID: 9493
		[ProtoMember(3)]
		public int useFreeChallengeNum;

		// Token: 0x04002516 RID: 9494
		[ProtoMember(4)]
		public int vipChallengeNum;

		// Token: 0x04002517 RID: 9495
		[ProtoMember(5)]
		public int useVipChallengeNum;

		// Token: 0x04002518 RID: 9496
		[ProtoMember(6)]
		public int winCount = 0;

		// Token: 0x04002519 RID: 9497
		[ProtoMember(7)]
		public int ranking = -1;

		// Token: 0x0400251A RID: 9498
		[ProtoMember(8)]
		public long nextRewardTime = 0L;

		// Token: 0x0400251B RID: 9499
		[ProtoMember(9)]
		public long nextChallengeTime = 0L;

		// Token: 0x0400251C RID: 9500
		[ProtoMember(10)]
		public List<PlayerJingJiMiniData> beChallengerData;

		// Token: 0x0400251D RID: 9501
		[ProtoMember(11)]
		public int maxwincount;
	}
}
