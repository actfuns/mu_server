using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200000F RID: 15
	[ProtoContract]
	public class FuMoCachedTemplate
	{
		// Token: 0x04000065 RID: 101
		[ProtoMember(1)]
		public int Result;

		// Token: 0x04000066 RID: 102
		[ProtoMember(2)]
		public int RoleID;

		// Token: 0x04000067 RID: 103
		[ProtoMember(3)]
		public int DbID;

		// Token: 0x04000068 RID: 104
		[ProtoMember(4)]
		public List<int> AttrTypeValue;
	}
}
