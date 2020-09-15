using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000054 RID: 84
	[ProtoContract]
	public class FriendData
	{
		// Token: 0x040001BB RID: 443
		[ProtoMember(1)]
		public int DbID;

		// Token: 0x040001BC RID: 444
		[ProtoMember(2)]
		public int OtherRoleID;

		// Token: 0x040001BD RID: 445
		[ProtoMember(3)]
		public string OtherRoleName;

		// Token: 0x040001BE RID: 446
		[ProtoMember(4)]
		public int OtherLevel;

		// Token: 0x040001BF RID: 447
		[ProtoMember(5)]
		public int Occupation;

		// Token: 0x040001C0 RID: 448
		[ProtoMember(6)]
		public int OnlineState;

		// Token: 0x040001C1 RID: 449
		[ProtoMember(7)]
		public string Position;

		// Token: 0x040001C2 RID: 450
		[ProtoMember(8)]
		public int FriendType;

		// Token: 0x040001C3 RID: 451
		[ProtoMember(9)]
		public int FriendChangeLifeLev;

		// Token: 0x040001C4 RID: 452
		[ProtoMember(10)]
		public int FriendCombatForce;

		// Token: 0x040001C5 RID: 453
		[ProtoMember(11)]
		public int SpouseId;

		// Token: 0x040001C6 RID: 454
		[ProtoMember(12)]
		public int YaoSaiBossState;

		// Token: 0x040001C7 RID: 455
		[ProtoMember(13)]
		public int YaoSaiJianYuState;

		// Token: 0x040001C8 RID: 456
		[ProtoMember(14)]
		public int ZhanDuiID;
	}
}
