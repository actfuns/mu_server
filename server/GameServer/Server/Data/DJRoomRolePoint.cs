using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200054E RID: 1358
	[ProtoContract]
	public class DJRoomRolePoint
	{
		// Token: 0x04002476 RID: 9334
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04002477 RID: 9335
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x04002478 RID: 9336
		[ProtoMember(3)]
		public int FightPoint = 0;
	}
}
