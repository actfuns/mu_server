using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000133 RID: 307
	[ProtoContract]
	public class FluorescentGemData
	{
		// Token: 0x040006BA RID: 1722
		[ProtoMember(1)]
		public Dictionary<int, Dictionary<int, GoodsData>> GemEquipDict = new Dictionary<int, Dictionary<int, GoodsData>>();

		// Token: 0x040006BB RID: 1723
		[ProtoMember(2)]
		public Dictionary<int, GoodsData> GemBagDict = new Dictionary<int, GoodsData>();

		// Token: 0x040006BC RID: 1724
		[ProtoMember(10)]
		public List<GoodsData> GemBagList = new List<GoodsData>();

		// Token: 0x040006BD RID: 1725
		[ProtoMember(11)]
		public List<GoodsData> GemEquipList = new List<GoodsData>();
	}
}
