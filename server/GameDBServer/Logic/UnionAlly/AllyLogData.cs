using System;
using ProtoBuf;

namespace GameDBServer.Logic.UnionAlly
{
	// Token: 0x02000183 RID: 387
	[ProtoContract]
	public class AllyLogData
	{
		// Token: 0x040008E5 RID: 2277
		[ProtoMember(1)]
		public int UnionID = 0;

		// Token: 0x040008E6 RID: 2278
		[ProtoMember(2)]
		public int UnionZoneID = 0;

		// Token: 0x040008E7 RID: 2279
		[ProtoMember(3)]
		public string UnionName = "";

		// Token: 0x040008E8 RID: 2280
		[ProtoMember(4)]
		public int MyUnionID = 0;

		// Token: 0x040008E9 RID: 2281
		[ProtoMember(5)]
		public DateTime LogTime = DateTime.MinValue;

		// Token: 0x040008EA RID: 2282
		[ProtoMember(6)]
		public int LogState = 0;
	}
}
