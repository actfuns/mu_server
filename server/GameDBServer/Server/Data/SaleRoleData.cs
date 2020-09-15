using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000AB RID: 171
	[ProtoContract]
	public class SaleRoleData
	{
		// Token: 0x04000480 RID: 1152
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000481 RID: 1153
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x04000482 RID: 1154
		[ProtoMember(3)]
		public int RoleLevel = 0;

		// Token: 0x04000483 RID: 1155
		[ProtoMember(4)]
		public int SaleGoodsNum = 0;
	}
}
