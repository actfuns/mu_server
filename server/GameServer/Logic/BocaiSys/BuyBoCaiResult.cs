using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200006E RID: 110
	[ProtoContract]
	public class BuyBoCaiResult
	{
		// Token: 0x04000289 RID: 649
		[ProtoMember(1)]
		public List<BoCaiBuyItem> ItemList;

		// Token: 0x0400028A RID: 650
		[ProtoMember(2)]
		public int Info;

		// Token: 0x0400028B RID: 651
		[ProtoMember(3)]
		public int BocaiType;
	}
}
