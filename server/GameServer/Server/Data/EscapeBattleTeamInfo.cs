using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000092 RID: 146
	[ProtoContract]
	public class EscapeBattleTeamInfo
	{
		// Token: 0x04000380 RID: 896
		[ProtoMember(1)]
		public int TeamID;

		// Token: 0x04000381 RID: 897
		[ProtoMember(2)]
		public string TeamName;

		// Token: 0x04000382 RID: 898
		[ProtoMember(3)]
		public int LifeSeed;

		// Token: 0x04000383 RID: 899
		public int ZhanDuiKillNum;

		// Token: 0x04000384 RID: 900
		public int RankNum;
	}
}
