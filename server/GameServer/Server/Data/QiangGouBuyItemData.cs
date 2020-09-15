using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000582 RID: 1410
	[ProtoContract]
	public class QiangGouBuyItemData
	{
		// Token: 0x04002609 RID: 9737
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400260A RID: 9738
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x0400260B RID: 9739
		[ProtoMember(3)]
		public int GoodsID = 0;

		// Token: 0x0400260C RID: 9740
		[ProtoMember(4)]
		public int GoodsNum = 0;

		// Token: 0x0400260D RID: 9741
		[ProtoMember(5)]
		public int QiangGouID = 0;
	}
}
