using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000218 RID: 536
	[ProtoContract]
	public class BHMatchSupportData
	{
		// Token: 0x04000C2A RID: 3114
		[ProtoMember(1)]
		public int season;

		// Token: 0x04000C2B RID: 3115
		[ProtoMember(2)]
		public int round;

		// Token: 0x04000C2C RID: 3116
		[ProtoMember(3)]
		public int bhid1;

		// Token: 0x04000C2D RID: 3117
		[ProtoMember(4)]
		public int bhid2;

		// Token: 0x04000C2E RID: 3118
		[ProtoMember(5)]
		public int guess;

		// Token: 0x04000C2F RID: 3119
		[ProtoMember(6)]
		public byte isaward;

		// Token: 0x04000C30 RID: 3120
		[ProtoMember(7)]
		public int rid;
	}
}
