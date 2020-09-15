using System;
using ProtoBuf;

namespace GameDBServer.Logic.Olympics
{
	// Token: 0x02000158 RID: 344
	[ProtoContract]
	public class OlympicsGuessDataDB
	{
		// Token: 0x04000855 RID: 2133
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000856 RID: 2134
		[ProtoMember(2)]
		public int DayID = 0;

		// Token: 0x04000857 RID: 2135
		[ProtoMember(3)]
		public int A1 = -1;

		// Token: 0x04000858 RID: 2136
		[ProtoMember(4)]
		public int A2 = -1;

		// Token: 0x04000859 RID: 2137
		[ProtoMember(5)]
		public int A3 = -1;

		// Token: 0x0400085A RID: 2138
		[ProtoMember(6)]
		public int Award1 = 0;

		// Token: 0x0400085B RID: 2139
		[ProtoMember(7)]
		public int Award2 = 0;

		// Token: 0x0400085C RID: 2140
		[ProtoMember(8)]
		public int Award3 = 0;
	}
}
