using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200005B RID: 91
	[ProtoContract]
	public class AlchemyData
	{
		// Token: 0x040001F4 RID: 500
		[ProtoMember(1)]
		public int Element = 0;

		// Token: 0x040001F5 RID: 501
		[ProtoMember(2)]
		public Dictionary<int, int> ToDayCost = new Dictionary<int, int>();

		// Token: 0x040001F6 RID: 502
		[ProtoMember(3)]
		public Dictionary<int, int> AlchemyValue = new Dictionary<int, int>();
	}
}
