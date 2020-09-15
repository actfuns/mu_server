using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000F6 RID: 246
	[ProtoContract]
	public class MyHongBaoData
	{
		// Token: 0x040006EF RID: 1775
		[ProtoMember(1)]
		public int type;

		// Token: 0x040006F0 RID: 1776
		[ProtoMember(2)]
		public List<HongBaoItemData> items;

		// Token: 0x040006F1 RID: 1777
		[ProtoMember(3)]
		public long flag;
	}
}
