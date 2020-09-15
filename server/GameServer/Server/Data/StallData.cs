using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005A2 RID: 1442
	[ProtoContract]
	public class StallData
	{
		// Token: 0x04002893 RID: 10387
		[ProtoMember(1)]
		public int StallID;

		// Token: 0x04002894 RID: 10388
		[ProtoMember(2)]
		public int RoleID;

		// Token: 0x04002895 RID: 10389
		[ProtoMember(3)]
		public string StallName;

		// Token: 0x04002896 RID: 10390
		[ProtoMember(4)]
		public string StallMessage;

		// Token: 0x04002897 RID: 10391
		[ProtoMember(5)]
		public List<GoodsData> GoodsList;

		// Token: 0x04002898 RID: 10392
		[ProtoMember(6)]
		public Dictionary<int, int> GoodsPriceDict;

		// Token: 0x04002899 RID: 10393
		[ProtoMember(7)]
		public long AddDateTime;

		// Token: 0x0400289A RID: 10394
		[ProtoMember(8)]
		public int Start;
	}
}
