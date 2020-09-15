using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000179 RID: 377
	[ProtoContract]
	public class PrestigeMedalSpecialData
	{
		// Token: 0x0400086C RID: 2156
		[ProtoMember(2)]
		public int SpecialID = 0;

		// Token: 0x0400086D RID: 2157
		[ProtoMember(1)]
		public int MedalID = 0;

		// Token: 0x0400086E RID: 2158
		[ProtoMember(3)]
		public double DoubleAttack = 0.0;

		// Token: 0x0400086F RID: 2159
		[ProtoMember(4)]
		public double DiDouble = 0.0;
	}
}
