using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200009D RID: 157
	[ProtoContract]
	public class RebornPortableBagData
	{
		// Token: 0x04000381 RID: 897
		[ProtoMember(1)]
		public int ExtGridNum = 0;

		// Token: 0x04000382 RID: 898
		[ProtoMember(2)]
		public int GoodsUsedGridNum = 0;
	}
}
