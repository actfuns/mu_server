using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200056B RID: 1387
	[ProtoContract]
	public class LayerRewardData
	{
		// Token: 0x0400255D RID: 9565
		[ProtoMember(1)]
		public List<SingleLayerRewardData> WanMoTaLayerRewardList = new List<SingleLayerRewardData>();
	}
}
