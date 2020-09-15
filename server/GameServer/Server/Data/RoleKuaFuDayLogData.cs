using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000176 RID: 374
	[ProtoContract]
	public class RoleKuaFuDayLogData
	{
		// Token: 0x04000850 RID: 2128
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000851 RID: 2129
		[ProtoMember(2)]
		public string Day = "2000-1-1";

		// Token: 0x04000852 RID: 2130
		[ProtoMember(3)]
		public int ZoneId = 0;

		// Token: 0x04000853 RID: 2131
		[ProtoMember(4)]
		public int SignupCount = 0;

		// Token: 0x04000854 RID: 2132
		[ProtoMember(5)]
		public int StartGameCount = 0;

		// Token: 0x04000855 RID: 2133
		[ProtoMember(6)]
		public int SuccessCount = 0;

		// Token: 0x04000856 RID: 2134
		[ProtoMember(7)]
		public int FaildCount = 0;

		// Token: 0x04000857 RID: 2135
		[ProtoMember(8)]
		public int GameType = 0;
	}
}
