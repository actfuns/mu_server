using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200054F RID: 1359
	[ProtoContract]
	public class DJRoomRolesData
	{
		// Token: 0x04002479 RID: 9337
		[ProtoMember(1)]
		public int RoomID = 0;

		// Token: 0x0400247A RID: 9338
		[ProtoMember(2)]
		public List<DJRoomRoleData> Team1;

		// Token: 0x0400247B RID: 9339
		[ProtoMember(3)]
		public List<DJRoomRoleData> Team2;

		// Token: 0x0400247C RID: 9340
		[ProtoMember(4)]
		public Dictionary<int, int> TeamStates;

		// Token: 0x0400247D RID: 9341
		[ProtoMember(5)]
		public int Locked;

		// Token: 0x0400247E RID: 9342
		[ProtoMember(6)]
		public int Removed;

		// Token: 0x0400247F RID: 9343
		[ProtoMember(7)]
		public List<DJRoomRoleData> ViewRoles;

		// Token: 0x04002480 RID: 9344
		[ProtoMember(8)]
		public Dictionary<int, int> RoleStates;
	}
}
