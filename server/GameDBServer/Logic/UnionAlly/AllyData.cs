using System;
using ProtoBuf;

namespace GameDBServer.Logic.UnionAlly
{
	// Token: 0x02000182 RID: 386
	[ProtoContract]
	public class AllyData
	{
		// Token: 0x040008DB RID: 2267
		[ProtoMember(1)]
		public int UnionID = 0;

		// Token: 0x040008DC RID: 2268
		[ProtoMember(2)]
		public int UnionZoneID = 0;

		// Token: 0x040008DD RID: 2269
		[ProtoMember(3)]
		public string UnionName = "";

		// Token: 0x040008DE RID: 2270
		[ProtoMember(4)]
		public int UnionLevel = 0;

		// Token: 0x040008DF RID: 2271
		[ProtoMember(5)]
		public int UnionNum = 0;

		// Token: 0x040008E0 RID: 2272
		[ProtoMember(6)]
		public int LeaderID = 0;

		// Token: 0x040008E1 RID: 2273
		[ProtoMember(7)]
		public int LeaderZoneID = 0;

		// Token: 0x040008E2 RID: 2274
		[ProtoMember(8)]
		public string LeaderName = "";

		// Token: 0x040008E3 RID: 2275
		[ProtoMember(9)]
		public DateTime LogTime = DateTime.MinValue;

		// Token: 0x040008E4 RID: 2276
		[ProtoMember(10)]
		public int LogState = 0;
	}
}
