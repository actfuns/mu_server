using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	
	[ProtoContract]
	public class BoCaiShopDBData
	{
		
		[ProtoMember(1)]
		public List<BoCaiShopDB> ItemList;

		
		[ProtoMember(2)]
		public bool Flag;
	}
}
