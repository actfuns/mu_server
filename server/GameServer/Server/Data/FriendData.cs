using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000553 RID: 1363
	[ProtoContract]
	public class FriendData
	{
		// Token: 0x04002492 RID: 9362
		[ProtoMember(1)]
		public int DbID;

		// Token: 0x04002493 RID: 9363
		[ProtoMember(2)]
		public int OtherRoleID;

		// Token: 0x04002494 RID: 9364
		[ProtoMember(3)]
		public string OtherRoleName;

		// Token: 0x04002495 RID: 9365
		[ProtoMember(4)]
		public int OtherLevel;

		// Token: 0x04002496 RID: 9366
		[ProtoMember(5)]
		public int Occupation;

		// Token: 0x04002497 RID: 9367
		[ProtoMember(6)]
		public int OnlineState;

		// Token: 0x04002498 RID: 9368
		[ProtoMember(7)]
		public string Position;

		// Token: 0x04002499 RID: 9369
		[ProtoMember(8)]
		public int FriendType;

		// Token: 0x0400249A RID: 9370
		[ProtoMember(9)]
		public int FriendChangeLifeLev;

		// Token: 0x0400249B RID: 9371
		[ProtoMember(10)]
		public int FriendCombatForce;

		// Token: 0x0400249C RID: 9372
		[ProtoMember(11)]
		public int SpouseId;

		// Token: 0x0400249D RID: 9373
		[ProtoMember(12)]
		public int YaoSaiBossState;

		// Token: 0x0400249E RID: 9374
		[ProtoMember(13)]
		public int YaoSaiJianYuState;

		// Token: 0x0400249F RID: 9375
		[ProtoMember(14)]
		public int ZhanDuiID;
	}
}
