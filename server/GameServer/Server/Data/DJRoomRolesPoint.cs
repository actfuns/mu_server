using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000550 RID: 1360
	[ProtoContract]
	public class DJRoomRolesPoint
	{
		// Token: 0x04002481 RID: 9345
		[ProtoMember(1)]
		public int RoomID = 0;

		// Token: 0x04002482 RID: 9346
		[ProtoMember(2)]
		public string RoomName = "";

		// Token: 0x04002483 RID: 9347
		[ProtoMember(3)]
		public List<DJRoomRolePoint> RolePoints;
	}
}
