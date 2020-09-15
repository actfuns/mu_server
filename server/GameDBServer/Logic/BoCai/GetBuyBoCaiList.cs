using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	// Token: 0x0200011E RID: 286
	[ProtoContract]
	public class GetBuyBoCaiList
	{
		// Token: 0x0400079B RID: 1947
		[ProtoMember(1)]
		public List<BuyBoCai2SDB> ItemList;

		// Token: 0x0400079C RID: 1948
		[ProtoMember(2)]
		public bool Flag;
	}
}
