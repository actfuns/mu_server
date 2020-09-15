using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000095 RID: 149
	[ProtoContract]
	public class EscapeBattlePropNotify
	{
		// Token: 0x0400038F RID: 911
		[ProtoMember(1)]
		public int Type;

		// Token: 0x04000390 RID: 912
		[ProtoMember(2)]
		public Dictionary<int, double[]> MergeProp;
	}
}
