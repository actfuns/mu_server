using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000AC RID: 172
	[ProtoContract]
	public class SearchRoleData
	{
		// Token: 0x04000484 RID: 1156
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000485 RID: 1157
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x04000486 RID: 1158
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x04000487 RID: 1159
		[ProtoMember(4)]
		public int Level = 0;

		// Token: 0x04000488 RID: 1160
		[ProtoMember(5)]
		public int Occupation = 0;

		// Token: 0x04000489 RID: 1161
		[ProtoMember(6)]
		public int MapCode = 0;

		// Token: 0x0400048A RID: 1162
		[ProtoMember(7)]
		public int PosX = 0;

		// Token: 0x0400048B RID: 1163
		[ProtoMember(8)]
		public int PosY = 0;

		// Token: 0x0400048C RID: 1164
		[ProtoMember(9)]
		public int Faction = 0;

		// Token: 0x0400048D RID: 1165
		[ProtoMember(10)]
		public string BHName = "";
	}
}
