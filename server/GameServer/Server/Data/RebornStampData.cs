using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000015 RID: 21
	[ProtoContract]
	public class RebornStampData
	{
		// Token: 0x0400007F RID: 127
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000080 RID: 128
		[ProtoMember(2)]
		public int ResetNum;

		// Token: 0x04000081 RID: 129
		[ProtoMember(3)]
		public int UsePoint;

		// Token: 0x04000082 RID: 130
		[ProtoMember(4)]
		public List<int> StampInfo;
	}
}
