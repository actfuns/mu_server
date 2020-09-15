using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200004E RID: 78
	[ProtoContract]
	public class FluorescentGemData
	{
		// Token: 0x04000195 RID: 405
		[ProtoMember(1)]
		public Dictionary<int, Dictionary<int, GoodsData>> GemEquipDict = new Dictionary<int, Dictionary<int, GoodsData>>();

		// Token: 0x04000196 RID: 406
		[ProtoMember(2)]
		public Dictionary<int, GoodsData> GemBagDict = new Dictionary<int, GoodsData>();

		// Token: 0x04000197 RID: 407
		[ProtoMember(10)]
		public List<GoodsData> GemBagList = new List<GoodsData>();

		// Token: 0x04000198 RID: 408
		[ProtoMember(11)]
		public List<GoodsData> GemEquipList = new List<GoodsData>();
	}
}
