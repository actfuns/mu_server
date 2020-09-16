using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LayerRewardData
	{
		
		[ProtoMember(1)]
		public List<SingleLayerRewardData> WanMoTaLayerRewardList = new List<SingleLayerRewardData>();
	}
}
