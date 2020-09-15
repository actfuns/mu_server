using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000171 RID: 369
	[ProtoContract]
	public class PaiHangItemData
	{
		// Token: 0x0400083A RID: 2106
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x0400083B RID: 2107
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x0400083C RID: 2108
		[ProtoMember(3)]
		public int Val1;

		// Token: 0x0400083D RID: 2109
		[ProtoMember(4)]
		public int Val2;

		// Token: 0x0400083E RID: 2110
		[ProtoMember(5)]
		public int Val3;

		// Token: 0x0400083F RID: 2111
		[ProtoMember(6)]
		public string uid;
	}
}
