using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200006C RID: 108
	[ProtoContract]
	public class GetBuyBoCaiList
	{
		// Token: 0x04000284 RID: 644
		[ProtoMember(1)]
		public List<BuyBoCai2SDB> ItemList;

		// Token: 0x04000285 RID: 645
		[ProtoMember(2)]
		public bool Flag;
	}
}
