using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000154 RID: 340
	[ProtoContract]
	public class KingRolePutData
	{
		// Token: 0x04000784 RID: 1924
		[ProtoMember(1)]
		public int KingType;

		// Token: 0x04000785 RID: 1925
		[ProtoMember(2)]
		public RoleDataEx RoleDataEx;
	}
}
