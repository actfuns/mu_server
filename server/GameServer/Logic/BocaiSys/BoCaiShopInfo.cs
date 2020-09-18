using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	
	[ProtoContract]
	public class BoCaiShopInfo
	{
		
		[ProtoMember(1)]
		public List<SelfBuyInfo> ItemList;

		
		[ProtoMember(2)]
		public int Info;
	}
}
