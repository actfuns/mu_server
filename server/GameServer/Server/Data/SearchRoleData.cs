using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200058E RID: 1422
	[ProtoContract]
	public class SearchRoleData
	{
		// Token: 0x0400280A RID: 10250
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400280B RID: 10251
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x0400280C RID: 10252
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x0400280D RID: 10253
		[ProtoMember(4)]
		public int Level = 0;

		// Token: 0x0400280E RID: 10254
		[ProtoMember(5)]
		public int Occupation = 0;

		// Token: 0x0400280F RID: 10255
		[ProtoMember(6)]
		public int MapCode = 0;

		// Token: 0x04002810 RID: 10256
		[ProtoMember(7)]
		public int PosX = 0;

		// Token: 0x04002811 RID: 10257
		[ProtoMember(8)]
		public int PosY = 0;

		// Token: 0x04002812 RID: 10258
		[ProtoMember(9)]
		public int Faction = 0;

		// Token: 0x04002813 RID: 10259
		[ProtoMember(10)]
		public string BHName = "";

		// Token: 0x04002814 RID: 10260
		[ProtoMember(11)]
		public int CombatForce = 0;

		// Token: 0x04002815 RID: 10261
		[ProtoMember(12)]
		public int ChangeLifeLev = 0;
	}
}
