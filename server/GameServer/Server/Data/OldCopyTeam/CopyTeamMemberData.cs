using System;
using ProtoBuf;

namespace Server.Data.OldCopyTeam
{
	// Token: 0x02000547 RID: 1351
	[ProtoContract]
	public class CopyTeamMemberData
	{
		// Token: 0x04002430 RID: 9264
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04002431 RID: 9265
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x04002432 RID: 9266
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x04002433 RID: 9267
		[ProtoMember(4)]
		public int Level = 0;

		// Token: 0x04002434 RID: 9268
		[ProtoMember(5)]
		public int Occupation = 0;

		// Token: 0x04002435 RID: 9269
		[ProtoMember(6)]
		public int RolePic = 0;

		// Token: 0x04002436 RID: 9270
		[ProtoMember(7)]
		public int MapCode = 0;

		// Token: 0x04002437 RID: 9271
		[ProtoMember(8)]
		public int OnlineState = 0;

		// Token: 0x04002438 RID: 9272
		[ProtoMember(9)]
		public int MaxLifeV = 0;

		// Token: 0x04002439 RID: 9273
		[ProtoMember(10)]
		public int CurrentLifeV = 0;

		// Token: 0x0400243A RID: 9274
		[ProtoMember(11)]
		public int MaxMagicV = 0;

		// Token: 0x0400243B RID: 9275
		[ProtoMember(12)]
		public int CurrentMagicV = 0;

		// Token: 0x0400243C RID: 9276
		[ProtoMember(13)]
		public int PosX = 0;

		// Token: 0x0400243D RID: 9277
		[ProtoMember(14)]
		public int PosY = 0;

		// Token: 0x0400243E RID: 9278
		[ProtoMember(15)]
		public int CombatForce = 0;

		// Token: 0x0400243F RID: 9279
		[ProtoMember(16)]
		public int ChangeLifeLev = 0;

		// Token: 0x04002440 RID: 9280
		[ProtoMember(17)]
		public bool IsReady = false;
	}
}
