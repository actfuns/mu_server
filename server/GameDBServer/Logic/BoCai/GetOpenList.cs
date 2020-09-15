using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	// Token: 0x0200011F RID: 287
	[ProtoContract]
	public class GetOpenList
	{
		// Token: 0x0400079D RID: 1949
		[ProtoMember(1)]
		public List<OpenLottery> ItemList;

		// Token: 0x0400079E RID: 1950
		[ProtoMember(2)]
		public bool Flag;

		// Token: 0x0400079F RID: 1951
		[ProtoMember(3)]
		public long MaxDataPeriods;
	}
}
