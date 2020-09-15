using System;
using ProtoBuf;

namespace GameServer.Server
{
	// Token: 0x02000897 RID: 2199
	[ProtoContract]
	public class RoleMiniInfo
	{
		// Token: 0x040047A4 RID: 18340
		[ProtoMember(1)]
		public long roleId;

		// Token: 0x040047A5 RID: 18341
		[ProtoMember(2)]
		public int zoneId;

		// Token: 0x040047A6 RID: 18342
		[ProtoMember(3)]
		public string userId;
	}
}
