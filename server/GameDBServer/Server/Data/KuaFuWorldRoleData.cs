using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000DC RID: 220
	[ProtoContract]
	public class KuaFuWorldRoleData
	{
		// Token: 0x04000611 RID: 1553
		[ProtoMember(1)]
		public int LocalRoleID;

		// Token: 0x04000612 RID: 1554
		[ProtoMember(2)]
		public int TempRoleID;

		// Token: 0x04000613 RID: 1555
		[ProtoMember(3)]
		public string WorldRoleID;
	}
}
