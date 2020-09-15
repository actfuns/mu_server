using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200057C RID: 1404
	[ProtoContract]
	public class PlayerJingJiMiniData
	{
		// Token: 0x040025F6 RID: 9718
		[ProtoMember(1)]
		public int roleId;

		// Token: 0x040025F7 RID: 9719
		[ProtoMember(2)]
		public string roleName;

		// Token: 0x040025F8 RID: 9720
		[ProtoMember(3)]
		public int occupationId;

		// Token: 0x040025F9 RID: 9721
		[ProtoMember(4)]
		public int combatForce;

		// Token: 0x040025FA RID: 9722
		[ProtoMember(5)]
		public int ranking;

		// Token: 0x040025FB RID: 9723
		[ProtoMember(6)]
		public int sex;
	}
}
