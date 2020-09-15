using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200058C RID: 1420
	[ProtoContract]
	public class SaleGoodsData : IComparer<SaleGoodsData>
	{
		// Token: 0x06001A10 RID: 6672 RVA: 0x001927AC File Offset: 0x001909AC
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			return x.GoodsDbID - y.GoodsDbID;
		}

		// Token: 0x04002801 RID: 10241
		[ProtoMember(1)]
		public int GoodsDbID = 0;

		// Token: 0x04002802 RID: 10242
		[ProtoMember(2)]
		public GoodsData SalingGoodsData = null;

		// Token: 0x04002803 RID: 10243
		[ProtoMember(3)]
		public int RoleID = 0;

		// Token: 0x04002804 RID: 10244
		[ProtoMember(4)]
		public string RoleName = "";

		// Token: 0x04002805 RID: 10245
		[ProtoMember(5)]
		public int RoleLevel = 0;
	}
}
