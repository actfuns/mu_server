using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200015A RID: 346
	[ProtoContract]
	public class LuoLanChengZhanRoleCountData
	{
		// Token: 0x040007A9 RID: 1961
		[ProtoMember(1)]
		public int BHID;

		// Token: 0x040007AA RID: 1962
		[ProtoMember(2)]
		public int RoleCount;
	}
}
