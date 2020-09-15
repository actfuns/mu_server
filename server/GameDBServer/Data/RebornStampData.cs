using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Data
{
	// Token: 0x02000068 RID: 104
	[ProtoContract]
	public class RebornStampData
	{
		// Token: 0x04000238 RID: 568
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000239 RID: 569
		[ProtoMember(2)]
		public int ResetNum;

		// Token: 0x0400023A RID: 570
		[ProtoMember(3)]
		public int UsePoint;

		// Token: 0x0400023B RID: 571
		[ProtoMember(4)]
		public List<int> StampInfo;
	}
}
