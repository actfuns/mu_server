using System;
using ProtoBuf;

namespace GameServer.Logic.Olympics
{
	// Token: 0x0200038B RID: 907
	[ProtoContract]
	public class OlympicsGuessDataDB
	{
		// Token: 0x040017E6 RID: 6118
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040017E7 RID: 6119
		[ProtoMember(2)]
		public int DayID = 0;

		// Token: 0x040017E8 RID: 6120
		[ProtoMember(3)]
		public int A1 = -1;

		// Token: 0x040017E9 RID: 6121
		[ProtoMember(4)]
		public int A2 = -1;

		// Token: 0x040017EA RID: 6122
		[ProtoMember(5)]
		public int A3 = -1;

		// Token: 0x040017EB RID: 6123
		[ProtoMember(6)]
		public int Award1 = 0;

		// Token: 0x040017EC RID: 6124
		[ProtoMember(7)]
		public int Award2 = 0;

		// Token: 0x040017ED RID: 6125
		[ProtoMember(8)]
		public int Award3 = 0;
	}
}
