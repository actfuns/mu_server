using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class WanMoXiaGuAwardsData
	{
		
		[ProtoMember(1)]
		public int Success;

		
		[ProtoMember(2)]
		public int UsedSecs;

		
		[ProtoMember(3)]
		public long Exp;

		
		[ProtoMember(4)]
		public int Money;

		
		[ProtoMember(5)]
		public List<GoodsData> AwardsGoods;
	}
}
