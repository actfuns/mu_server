using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000848 RID: 2120
	[ProtoContract]
	public class ZorkBattleTeamInfo
	{
		// Token: 0x04004640 RID: 17984
		[ProtoMember(1)]
		public int TeamID;

		// Token: 0x04004641 RID: 17985
		[ProtoMember(2)]
		public string TeamName;

		// Token: 0x04004642 RID: 17986
		[ProtoMember(3)]
		public int JiFen;

		// Token: 0x04004643 RID: 17987
		[ProtoMember(4)]
		public int BossInjurePct;

		// Token: 0x04004644 RID: 17988
		public long BossInjure;

		// Token: 0x04004645 RID: 17989
		public long BossInjureTicks;
	}
}
