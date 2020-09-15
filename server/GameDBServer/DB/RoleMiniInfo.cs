using System;
using ProtoBuf;

namespace GameDBServer.DB
{
	// Token: 0x020001E7 RID: 487
	[ProtoContract]
	public class RoleMiniInfo
	{
		// Token: 0x04000C46 RID: 3142
		[ProtoMember(1)]
		public long roleId;

		// Token: 0x04000C47 RID: 3143
		[ProtoMember(2)]
		public int zoneId;

		// Token: 0x04000C48 RID: 3144
		[ProtoMember(3)]
		public string userId;
	}
}
