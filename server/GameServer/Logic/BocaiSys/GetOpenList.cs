using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200006D RID: 109
	[ProtoContract]
	public class GetOpenList
	{
		// Token: 0x04000286 RID: 646
		[ProtoMember(1)]
		public List<OpenLottery> ItemList;

		// Token: 0x04000287 RID: 647
		[ProtoMember(2)]
		public bool Flag;

		// Token: 0x04000288 RID: 648
		[ProtoMember(3)]
		public long MaxDataPeriods;
	}
}
