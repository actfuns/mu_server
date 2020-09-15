using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000011 RID: 17
	[ProtoContract]
	public class NotifyMsgWithGoodsData
	{
		// Token: 0x0400006E RID: 110
		[ProtoMember(1)]
		public int index = 0;

		// Token: 0x0400006F RID: 111
		[ProtoMember(2)]
		public int type = 0;

		// Token: 0x04000070 RID: 112
		[ProtoMember(3)]
		public List<GoodsData> goodsDataList = null;

		// Token: 0x04000071 RID: 113
		[ProtoMember(4)]
		public string param1 = "";

		// Token: 0x04000072 RID: 114
		[ProtoMember(5)]
		public Dictionary<int, List<GoodsData>> goodsDic = null;
	}
}
