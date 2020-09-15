using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200010D RID: 269
	[ProtoContract]
	public class AlchemyData
	{
		// Token: 0x04000745 RID: 1861
		[ProtoMember(1)]
		public int Element = 0;

		// Token: 0x04000746 RID: 1862
		[ProtoMember(2)]
		public Dictionary<int, int> ToDayCost = new Dictionary<int, int>();

		// Token: 0x04000747 RID: 1863
		[ProtoMember(3)]
		public Dictionary<int, int> AlchemyValue = new Dictionary<int, int>();
	}
}
