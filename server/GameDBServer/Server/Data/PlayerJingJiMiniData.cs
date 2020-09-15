using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000A1 RID: 161
	[ProtoContract]
	public class PlayerJingJiMiniData
	{
		// Token: 0x0400038E RID: 910
		[ProtoMember(1)]
		public int roleId;

		// Token: 0x0400038F RID: 911
		[ProtoMember(2)]
		public string roleName;

		// Token: 0x04000390 RID: 912
		[ProtoMember(3)]
		public int occupationId;

		// Token: 0x04000391 RID: 913
		[ProtoMember(4)]
		public int combatForce;

		// Token: 0x04000392 RID: 914
		[ProtoMember(5)]
		public int ranking;

		// Token: 0x04000393 RID: 915
		[ProtoMember(6)]
		public int sex;
	}
}
