using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000F4 RID: 244
	[ProtoContract]
	public class HongBaoRankData
	{
		// Token: 0x040006E8 RID: 1768
		[ProtoMember(1)]
		public int type;

		// Token: 0x040006E9 RID: 1769
		[ProtoMember(2)]
		public List<HongBaoRankItemData> items;

		// Token: 0x040006EA RID: 1770
		[ProtoMember(3)]
		public long flag;
	}
}
