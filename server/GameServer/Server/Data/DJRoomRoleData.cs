using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200054D RID: 1357
	[ProtoContract]
	public class DJRoomRoleData
	{
		// Token: 0x04002470 RID: 9328
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04002471 RID: 9329
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x04002472 RID: 9330
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x04002473 RID: 9331
		[ProtoMember(4)]
		public int DJPoint = 0;

		// Token: 0x04002474 RID: 9332
		[ProtoMember(5)]
		public int DJTotal = 0;

		// Token: 0x04002475 RID: 9333
		[ProtoMember(6)]
		public int DJWincnt = 0;
	}
}
