using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	
	[ProtoContract]
	public class GetBuyBoCaiList
	{
		
		[ProtoMember(1)]
		public List<BuyBoCai2SDB> ItemList;

		
		[ProtoMember(2)]
		public bool Flag;
	}
}
