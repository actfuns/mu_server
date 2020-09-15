using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005A9 RID: 1449
	[ProtoContract]
	public class WangChengMapInfoData
	{
		// Token: 0x040028D8 RID: 10456
		[ProtoMember(1)]
		public long FightingEndTime = 0L;

		// Token: 0x040028D9 RID: 10457
		[ProtoMember(2)]
		public int FightingState = 0;

		// Token: 0x040028DA RID: 10458
		[ProtoMember(3)]
		public string NextBattleTime = "";

		// Token: 0x040028DB RID: 10459
		[ProtoMember(4)]
		public string WangZuBHName = "";

		// Token: 0x040028DC RID: 10460
		[ProtoMember(5)]
		public int WangZuBHid = -1;
	}
}
