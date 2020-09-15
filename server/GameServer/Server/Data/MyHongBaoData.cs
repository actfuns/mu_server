using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002E7 RID: 743
	[ProtoContract]
	public class MyHongBaoData
	{
		// Token: 0x04001318 RID: 4888
		[ProtoMember(1)]
		public int type;

		// Token: 0x04001319 RID: 4889
		[ProtoMember(2)]
		public List<HongBaoItemData> items;

		// Token: 0x0400131A RID: 4890
		[ProtoMember(3)]
		public long flag;
	}
}
