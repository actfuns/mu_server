using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000079 RID: 121
	[ProtoContract]
	public class KingRolePutData
	{
		// Token: 0x04000298 RID: 664
		[ProtoMember(1)]
		public int KingType;

		// Token: 0x04000299 RID: 665
		[ProtoMember(2)]
		public RoleDataEx RoleDataEx;
	}
}
