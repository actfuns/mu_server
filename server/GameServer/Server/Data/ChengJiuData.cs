using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005AE RID: 1454
	[ProtoContract]
	public class ChengJiuData
	{
		// Token: 0x040028F8 RID: 10488
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040028F9 RID: 10489
		[ProtoMember(2)]
		public long ChengJiuPoints = 0L;

		// Token: 0x040028FA RID: 10490
		[ProtoMember(3)]
		public long TotalKilledMonsterNum = 0L;

		// Token: 0x040028FB RID: 10491
		[ProtoMember(4)]
		public long TotalLoginNum = 0L;

		// Token: 0x040028FC RID: 10492
		[ProtoMember(5)]
		public int ContinueLoginNum = 0;

		// Token: 0x040028FD RID: 10493
		[ProtoMember(6)]
		public List<ushort> ChengJiuFlags = null;

		// Token: 0x040028FE RID: 10494
		[ProtoMember(7)]
		public int NowCompletedChengJiu = 0;

		// Token: 0x040028FF RID: 10495
		[ProtoMember(8)]
		public long TotalKilledBossNum = 0L;

		// Token: 0x04002900 RID: 10496
		[ProtoMember(9)]
		public long CompleteNormalCopyMapCount = 0L;

		// Token: 0x04002901 RID: 10497
		[ProtoMember(10)]
		public long CompleteHardCopyMapCount = 0L;

		// Token: 0x04002902 RID: 10498
		[ProtoMember(11)]
		public long CompleteDifficltCopyMapCount = 0L;

		// Token: 0x04002903 RID: 10499
		[ProtoMember(12)]
		public long GuildChengJiu = 0L;

		// Token: 0x04002904 RID: 10500
		[ProtoMember(13)]
		public long JunXianChengJiu = 0L;
	}
}
