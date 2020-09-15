using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200007B RID: 123
	[ProtoContract]
	public class LayerRewardData
	{
		// Token: 0x0400029F RID: 671
		[ProtoMember(1)]
		public List<SingleLayerRewardData> WanMoTaLayerRewardList = null;
	}
}
