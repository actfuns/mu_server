using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000178 RID: 376
	[ProtoContract]
	public class PrestigeMedalData
	{
		// Token: 0x04000861 RID: 2145
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000862 RID: 2146
		[ProtoMember(2)]
		public int MedalID = 0;

		// Token: 0x04000863 RID: 2147
		[ProtoMember(3)]
		public int LifeAdd = 0;

		// Token: 0x04000864 RID: 2148
		[ProtoMember(4)]
		public int AttackAdd = 0;

		// Token: 0x04000865 RID: 2149
		[ProtoMember(5)]
		public int DefenseAdd = 0;

		// Token: 0x04000866 RID: 2150
		[ProtoMember(6)]
		public int HitAdd = 0;

		// Token: 0x04000867 RID: 2151
		[ProtoMember(7)]
		public int Prestige = 0;

		// Token: 0x04000868 RID: 2152
		[ProtoMember(8)]
		public int Diamond = 0;

		// Token: 0x04000869 RID: 2153
		[ProtoMember(9)]
		public int BurstType = 0;

		// Token: 0x0400086A RID: 2154
		[ProtoMember(10)]
		public int UpResultType = 0;

		// Token: 0x0400086B RID: 2155
		[ProtoMember(11)]
		public int PrestigeLeft = 0;
	}
}
