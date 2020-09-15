using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002E5 RID: 741
	[ProtoContract]
	public class HongBaoRankData
	{
		// Token: 0x04001311 RID: 4881
		[ProtoMember(1)]
		public int type;

		// Token: 0x04001312 RID: 4882
		[ProtoMember(2)]
		public List<HongBaoRankItemData> items;

		// Token: 0x04001313 RID: 4883
		[ProtoMember(3)]
		public long flag;
	}
}
