using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200008E RID: 142
	[ProtoContract]
	public class EscapeBattleAwardsData
	{
		// Token: 0x0400035F RID: 863
		[ProtoMember(1)]
		public int Success;

		// Token: 0x04000360 RID: 864
		[ProtoMember(2)]
		public int RankNum;

		// Token: 0x04000361 RID: 865
		[ProtoMember(3)]
		public int AwardID;

		// Token: 0x04000362 RID: 866
		[ProtoMember(4)]
		public int ZhanDuiKillNum;

		// Token: 0x04000363 RID: 867
		[ProtoMember(5)]
		public int ModJiFen;

		// Token: 0x04000364 RID: 868
		[ProtoMember(6)]
		public int WinToDay;
	}
}
