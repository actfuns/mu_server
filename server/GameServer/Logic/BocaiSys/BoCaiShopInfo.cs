using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200007C RID: 124
	[ProtoContract]
	public class BoCaiShopInfo
	{
		// Token: 0x040002D6 RID: 726
		[ProtoMember(1)]
		public List<SelfBuyInfo> ItemList;

		// Token: 0x040002D7 RID: 727
		[ProtoMember(2)]
		public int Info;
	}
}
