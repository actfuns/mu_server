using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000AA RID: 170
	[ProtoContract]
	public class SaleGoodsData
	{
		// Token: 0x0400047B RID: 1147
		[ProtoMember(1)]
		public int GoodsDbID = 0;

		// Token: 0x0400047C RID: 1148
		[ProtoMember(2)]
		public GoodsData SalingGoodsData = null;

		// Token: 0x0400047D RID: 1149
		[ProtoMember(3)]
		public int RoleID = 0;

		// Token: 0x0400047E RID: 1150
		[ProtoMember(4)]
		public string RoleName = "";

		// Token: 0x0400047F RID: 1151
		[ProtoMember(5)]
		public int RoleLevel = 0;
	}
}
