using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000D1 RID: 209
	[ProtoContract]
	public class QiangGouBuyItemData
	{
		// Token: 0x040005B0 RID: 1456
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040005B1 RID: 1457
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x040005B2 RID: 1458
		[ProtoMember(3)]
		public int GoodsID = 0;

		// Token: 0x040005B3 RID: 1459
		[ProtoMember(4)]
		public int GoodsNum = 0;

		// Token: 0x040005B4 RID: 1460
		[ProtoMember(5)]
		public int QiangGouID = 0;
	}
}
