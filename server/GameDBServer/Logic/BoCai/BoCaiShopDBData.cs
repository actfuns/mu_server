using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	// Token: 0x0200011B RID: 283
	[ProtoContract]
	public class BoCaiShopDBData
	{
		// Token: 0x04000786 RID: 1926
		[ProtoMember(1)]
		public List<BoCaiShopDB> ItemList;

		// Token: 0x04000787 RID: 1927
		[ProtoMember(2)]
		public bool Flag;
	}
}
