using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200057D RID: 1405
	[ProtoContract]
	public class PlayerJingJiRankingData
	{
		// Token: 0x040025FC RID: 9724
		[ProtoMember(1)]
		public int roleId;

		// Token: 0x040025FD RID: 9725
		[ProtoMember(2)]
		public string roleName;

		// Token: 0x040025FE RID: 9726
		[ProtoMember(3)]
		public int combatForce;

		// Token: 0x040025FF RID: 9727
		[ProtoMember(4)]
		public int ranking;
	}
}
