using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200002E RID: 46
	[ProtoContract]
	public class BangHuiItemData
	{
		// Token: 0x040000DD RID: 221
		[ProtoMember(1)]
		public int BHID = 0;

		// Token: 0x040000DE RID: 222
		[ProtoMember(2)]
		public string BHName = "";

		// Token: 0x040000DF RID: 223
		[ProtoMember(3)]
		public int ZoneID = 0;

		// Token: 0x040000E0 RID: 224
		[ProtoMember(4)]
		public int BZRoleID = 0;

		// Token: 0x040000E1 RID: 225
		[ProtoMember(5)]
		public string BZRoleName = "";

		// Token: 0x040000E2 RID: 226
		[ProtoMember(6)]
		public int BZOccupation = 0;

		// Token: 0x040000E3 RID: 227
		[ProtoMember(7)]
		public int TotalNum = 0;

		// Token: 0x040000E4 RID: 228
		[ProtoMember(8)]
		public int TotalLevel = 0;

		// Token: 0x040000E5 RID: 229
		[ProtoMember(9)]
		public int QiLevel = 0;

		// Token: 0x040000E6 RID: 230
		[ProtoMember(10)]
		public int IsVerfiy = 0;

		// Token: 0x040000E7 RID: 231
		[ProtoMember(11)]
		public int TotalCombatForce = 0;
	}
}
