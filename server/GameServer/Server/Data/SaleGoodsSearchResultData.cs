using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200017A RID: 378
	[ProtoContract]
	public class SaleGoodsSearchResultData
	{
		// Token: 0x04000870 RID: 2160
		[ProtoMember(1)]
		public int Type;

		// Token: 0x04000871 RID: 2161
		[ProtoMember(2)]
		public int ID;

		// Token: 0x04000872 RID: 2162
		[ProtoMember(3)]
		public int MoneyFlags;

		// Token: 0x04000873 RID: 2163
		[ProtoMember(4)]
		public int ColorFlags;

		// Token: 0x04000874 RID: 2164
		[ProtoMember(5)]
		public int OrderBy;

		// Token: 0x04000875 RID: 2165
		[ProtoMember(6)]
		public int StartIndex = 0;

		// Token: 0x04000876 RID: 2166
		[ProtoMember(7)]
		public int TotalCount = 0;

		// Token: 0x04000877 RID: 2167
		[ProtoMember(8)]
		public List<SaleGoodsData> saleGoodsDataList;
	}
}
