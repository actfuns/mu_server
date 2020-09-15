using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200007B RID: 123
	[ProtoContract]
	public class BoCaiShopDBData
	{
		// Token: 0x040002D4 RID: 724
		[ProtoMember(1)]
		public List<KFBoCaiShopDB> ItemList;

		// Token: 0x040002D5 RID: 725
		[ProtoMember(2)]
		public bool Flag;
	}
}
