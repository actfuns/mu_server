using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200057F RID: 1407
	[ProtoContract]
	public class PortableBagData
	{
		// Token: 0x04002602 RID: 9730
		[ProtoMember(1)]
		public int ExtGridNum = 0;

		// Token: 0x04002603 RID: 9731
		[ProtoMember(2)]
		public int GoodsUsedGridNum = 0;
	}
}
