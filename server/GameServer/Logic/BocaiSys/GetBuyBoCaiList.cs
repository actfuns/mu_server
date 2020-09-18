using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
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
