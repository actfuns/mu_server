using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000224 RID: 548
	[ProtoContract]
	public class BuildingData
	{
		// Token: 0x04000CD2 RID: 3282
		[ProtoMember(1)]
		public int BuildId = 0;

		// Token: 0x04000CD3 RID: 3283
		[ProtoMember(2)]
		public int BuildLev = 1;

		// Token: 0x04000CD4 RID: 3284
		[ProtoMember(3)]
		public int BuildExp = 0;

		// Token: 0x04000CD5 RID: 3285
		[ProtoMember(4)]
		public string BuildTime = null;

		// Token: 0x04000CD6 RID: 3286
		[ProtoMember(5)]
		public int TaskID_1 = 0;

		// Token: 0x04000CD7 RID: 3287
		[ProtoMember(6)]
		public int TaskID_2 = 0;

		// Token: 0x04000CD8 RID: 3288
		[ProtoMember(7)]
		public int TaskID_3 = 0;

		// Token: 0x04000CD9 RID: 3289
		[ProtoMember(8)]
		public int TaskID_4 = 0;
	}
}
