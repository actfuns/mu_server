using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000A6 RID: 166
	[ProtoContract]
	public class RoleKuaFuDayLogData
	{
		// Token: 0x04000462 RID: 1122
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000463 RID: 1123
		[ProtoMember(2)]
		public string Day = "2000-1-1";

		// Token: 0x04000464 RID: 1124
		[ProtoMember(3)]
		public int ZoneId = 0;

		// Token: 0x04000465 RID: 1125
		[ProtoMember(4)]
		public int SignupCount = 0;

		// Token: 0x04000466 RID: 1126
		[ProtoMember(5)]
		public int StartGameCount = 0;

		// Token: 0x04000467 RID: 1127
		[ProtoMember(6)]
		public int SuccessCount = 0;

		// Token: 0x04000468 RID: 1128
		[ProtoMember(7)]
		public int FaildCount = 0;

		// Token: 0x04000469 RID: 1129
		[ProtoMember(8)]
		public int GameType = 0;
	}
}
