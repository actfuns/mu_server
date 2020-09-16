using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SingleLayerRewardData
	{
		
		[ProtoMember(1)]
		public int nLayerOrder;

		
		[ProtoMember(2)]
		public int nExp;

		
		[ProtoMember(3)]
		public int nMoney;

		
		[ProtoMember(4)]
		public int nXinHun;

		
		[ProtoMember(5)]
		public List<GoodsData> sweepAwardGoodsList = null;
	}
}
