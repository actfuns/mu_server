using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000580 RID: 1408
	[ProtoContract]
	public class RebornPortableBagData
	{
		// Token: 0x04002604 RID: 9732
		[ProtoMember(1)]
		public int ExtGridNum = 0;

		// Token: 0x04002605 RID: 9733
		[ProtoMember(2)]
		public int GoodsUsedGridNum = 0;
	}
}
