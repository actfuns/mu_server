using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000111 RID: 273
	[ProtoContract]
	public class AchievementRuneSpecialData
	{
		// Token: 0x040005BE RID: 1470
		[ProtoMember(2)]
		public int SpecialID = 0;

		// Token: 0x040005BF RID: 1471
		[ProtoMember(1)]
		public int RuneID = 0;

		// Token: 0x040005C0 RID: 1472
		[ProtoMember(3)]
		public double ZhuoYue = 0.0;

		// Token: 0x040005C1 RID: 1473
		[ProtoMember(4)]
		public double DiKang = 0.0;
	}
}
