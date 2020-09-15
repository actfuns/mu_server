using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200009C RID: 156
	[ProtoContract]
	public class PortableBagData
	{
		// Token: 0x0400037F RID: 895
		[ProtoMember(1)]
		public int ExtGridNum = 0;

		// Token: 0x04000380 RID: 896
		[ProtoMember(2)]
		public int GoodsUsedGridNum = 0;
	}
}
