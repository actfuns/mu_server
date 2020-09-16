using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class ZhengDuoAwardData
	{
		
		[ProtoMember(1)]
		public int State;

		
		[ProtoMember(2)]
		public int Second;

		
		[ProtoMember(3)]
		public long Exp;

		
		[ProtoMember(4)]
		public int Money;

		
		[ProtoMember(5)]
		public List<GoodsData> GoodsList;
	}
}
