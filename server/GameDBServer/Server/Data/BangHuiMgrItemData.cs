using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200002C RID: 44
	[ProtoContract]
	public class BangHuiMgrItemData
	{
		// Token: 0x040000BE RID: 190
		[ProtoMember(1)]
		public int ZoneID = 0;

		// Token: 0x040000BF RID: 191
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x040000C0 RID: 192
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x040000C1 RID: 193
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x040000C2 RID: 194
		[ProtoMember(5)]
		public int BHZhiwu = 0;

		// Token: 0x040000C3 RID: 195
		[ProtoMember(6)]
		public string ChengHao = "";

		// Token: 0x040000C4 RID: 196
		[ProtoMember(7)]
		public int BangGong = 0;

		// Token: 0x040000C5 RID: 197
		[ProtoMember(8)]
		public int Level = 0;
	}
}
