using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000110 RID: 272
	[ProtoContract]
	public class AchievementRuneData
	{
		// Token: 0x040005B3 RID: 1459
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040005B4 RID: 1460
		[ProtoMember(2)]
		public int RuneID = 0;

		// Token: 0x040005B5 RID: 1461
		[ProtoMember(3)]
		public int LifeAdd = 0;

		// Token: 0x040005B6 RID: 1462
		[ProtoMember(4)]
		public int AttackAdd = 0;

		// Token: 0x040005B7 RID: 1463
		[ProtoMember(5)]
		public int DefenseAdd = 0;

		// Token: 0x040005B8 RID: 1464
		[ProtoMember(6)]
		public int DodgeAdd = 0;

		// Token: 0x040005B9 RID: 1465
		[ProtoMember(7)]
		public int Achievement = 0;

		// Token: 0x040005BA RID: 1466
		[ProtoMember(8)]
		public int Diamond = 0;

		// Token: 0x040005BB RID: 1467
		[ProtoMember(9)]
		public int BurstType = 0;

		// Token: 0x040005BC RID: 1468
		[ProtoMember(10)]
		public int UpResultType = 0;

		// Token: 0x040005BD RID: 1469
		[ProtoMember(11)]
		public int AchievementLeft = 0;
	}
}
