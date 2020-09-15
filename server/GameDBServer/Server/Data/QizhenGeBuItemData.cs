using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200009F RID: 159
	[ProtoContract]
	public class QizhenGeBuItemData
	{
		// Token: 0x04000386 RID: 902
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000387 RID: 903
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x04000388 RID: 904
		[ProtoMember(3)]
		public int GoodsID = 0;

		// Token: 0x04000389 RID: 905
		[ProtoMember(4)]
		public int GoodsNum = 0;
	}
}
