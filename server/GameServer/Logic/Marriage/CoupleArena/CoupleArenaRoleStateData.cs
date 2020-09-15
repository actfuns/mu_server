using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000360 RID: 864
	[ProtoContract]
	public class CoupleArenaRoleStateData
	{
		// Token: 0x040016E3 RID: 5859
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x040016E4 RID: 5860
		[ProtoMember(2)]
		public int MatchState;
	}
}
