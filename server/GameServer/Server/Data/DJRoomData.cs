using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200054C RID: 1356
	[ProtoContract]
	public class DJRoomData
	{
		// Token: 0x04002466 RID: 9318
		[ProtoMember(1)]
		public int RoomID = 0;

		// Token: 0x04002467 RID: 9319
		[ProtoMember(2)]
		public int CreateRoleID = 0;

		// Token: 0x04002468 RID: 9320
		[ProtoMember(3)]
		public string CreateRoleName = "";

		// Token: 0x04002469 RID: 9321
		[ProtoMember(4)]
		public string RoomName = "";

		// Token: 0x0400246A RID: 9322
		[ProtoMember(5)]
		public int VSMode = 0;

		// Token: 0x0400246B RID: 9323
		[ProtoMember(6)]
		public int PKState = 0;

		// Token: 0x0400246C RID: 9324
		[ProtoMember(7)]
		public int PKRoleNum = 0;

		// Token: 0x0400246D RID: 9325
		[ProtoMember(8)]
		public int ViewRoleNum = 0;

		// Token: 0x0400246E RID: 9326
		[ProtoMember(9)]
		public long StartFightTicks = 0L;

		// Token: 0x0400246F RID: 9327
		[ProtoMember(10)]
		public int DJFightState = 0;
	}
}
