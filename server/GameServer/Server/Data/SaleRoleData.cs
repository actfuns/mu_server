using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200058D RID: 1421
	[ProtoContract]
	public class SaleRoleData
	{
		// Token: 0x04002806 RID: 10246
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04002807 RID: 10247
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x04002808 RID: 10248
		[ProtoMember(3)]
		public int RoleLevel = 0;

		// Token: 0x04002809 RID: 10249
		[ProtoMember(4)]
		public int SaleGoodsNum = 0;
	}
}
