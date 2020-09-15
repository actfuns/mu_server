using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005A6 RID: 1446
	[ProtoContract]
	public class TeamMemberData
	{
		// Token: 0x040028C3 RID: 10435
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040028C4 RID: 10436
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x040028C5 RID: 10437
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x040028C6 RID: 10438
		[ProtoMember(4)]
		public int Level = 0;

		// Token: 0x040028C7 RID: 10439
		[ProtoMember(5)]
		public int Occupation = 0;

		// Token: 0x040028C8 RID: 10440
		[ProtoMember(6)]
		public int RolePic = 0;

		// Token: 0x040028C9 RID: 10441
		[ProtoMember(7)]
		public int MapCode = 0;

		// Token: 0x040028CA RID: 10442
		[ProtoMember(8)]
		public int OnlineState = 0;

		// Token: 0x040028CB RID: 10443
		[ProtoMember(9)]
		public int MaxLifeV = 0;

		// Token: 0x040028CC RID: 10444
		[ProtoMember(10)]
		public int CurrentLifeV = 0;

		// Token: 0x040028CD RID: 10445
		[ProtoMember(11)]
		public int MaxMagicV = 0;

		// Token: 0x040028CE RID: 10446
		[ProtoMember(12)]
		public int CurrentMagicV = 0;

		// Token: 0x040028CF RID: 10447
		[ProtoMember(13)]
		public int PosX = 0;

		// Token: 0x040028D0 RID: 10448
		[ProtoMember(14)]
		public int PosY = 0;

		// Token: 0x040028D1 RID: 10449
		[ProtoMember(15)]
		public int CombatForce = 0;

		// Token: 0x040028D2 RID: 10450
		[ProtoMember(16)]
		public int ChangeLifeLev = 0;
	}
}
