using System;
using ProtoBuf;

namespace GameServer.Logic.UnionPalace
{
	// Token: 0x0200049F RID: 1183
	[ProtoContract]
	public class UnionPalaceData
	{
		// Token: 0x04001F5F RID: 8031
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04001F60 RID: 8032
		[ProtoMember(2)]
		public int StatueID = 0;

		// Token: 0x04001F61 RID: 8033
		[ProtoMember(3)]
		public int StatueLevel = 0;

		// Token: 0x04001F62 RID: 8034
		[ProtoMember(4)]
		public int LifeAdd = 0;

		// Token: 0x04001F63 RID: 8035
		[ProtoMember(5)]
		public int AttackAdd = 0;

		// Token: 0x04001F64 RID: 8036
		[ProtoMember(6)]
		public int DefenseAdd = 0;

		// Token: 0x04001F65 RID: 8037
		[ProtoMember(7)]
		public int AttackInjureAdd = 0;

		// Token: 0x04001F66 RID: 8038
		[ProtoMember(8)]
		public int ZhanGongNeed = 0;

		// Token: 0x04001F67 RID: 8039
		[ProtoMember(9)]
		public int BurstType = 0;

		// Token: 0x04001F68 RID: 8040
		[ProtoMember(10)]
		public int ResultType = 0;

		// Token: 0x04001F69 RID: 8041
		[ProtoMember(11)]
		public int ZhanGongLeft = 0;

		// Token: 0x04001F6A RID: 8042
		[ProtoMember(12)]
		public int UnionLevel = 0;

		// Token: 0x04001F6B RID: 8043
		[ProtoMember(13)]
		public int StatueType = 0;
	}
}
