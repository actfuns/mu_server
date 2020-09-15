using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000033 RID: 51
	[ProtoContract]
	public class BHMatchSupportData
	{
		// Token: 0x04000101 RID: 257
		[ProtoMember(1)]
		public int season;

		// Token: 0x04000102 RID: 258
		[ProtoMember(2)]
		public int round;

		// Token: 0x04000103 RID: 259
		[ProtoMember(3)]
		public int bhid1;

		// Token: 0x04000104 RID: 260
		[ProtoMember(4)]
		public int bhid2;

		// Token: 0x04000105 RID: 261
		[ProtoMember(5)]
		public int guess;

		// Token: 0x04000106 RID: 262
		[ProtoMember(6)]
		public byte isaward;

		// Token: 0x04000107 RID: 263
		[ProtoMember(7)]
		public int rid;
	}
}
