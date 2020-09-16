using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SoulStoneData
	{
		
		[ProtoMember(1)]
		public List<GoodsData> StonesInBag;

		
		[ProtoMember(2)]
		public List<GoodsData> StonesInUsing;
	}
}
