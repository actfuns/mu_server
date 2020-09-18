using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	
	[ProtoContract]
	public class BuyBoCaiResult
	{
		
		[ProtoMember(1)]
		public List<BoCaiBuyItem> ItemList;

		
		[ProtoMember(2)]
		public int Info;

		
		[ProtoMember(3)]
		public int BocaiType;
	}
}
