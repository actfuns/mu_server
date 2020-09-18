using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	
	[ProtoContract]
	public class BoCaiShopDBData
	{
		
		[ProtoMember(1)]
		public List<KFBoCaiShopDB> ItemList;

		
		[ProtoMember(2)]
		public bool Flag;
	}
}
